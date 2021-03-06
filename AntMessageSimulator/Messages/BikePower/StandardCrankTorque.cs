using System;
using System.Collections.Generic;

namespace AntMessageSimulator.Messages.BikePower
{
    public class StandardCrankTorque : Message
    {
        public new const byte Page = 0x12;

        public byte EventCount { get; private set; }
        public byte CrankTicks { get; private set; }
        public byte InstantCadence { get; private set; }
        public ushort Period { get; private set; } // 1/2048s
        public ushort AccumulatedTorque { get; private set; }
        public double CalculatedPower { get; private set; }

        public StandardCrankTorque(Message message) : base(message)
        {
            EventCount = message.Bytes[5];
            CrankTicks = message.Bytes[6];
            InstantCadence = message.Bytes[7];
            Period = (ushort)(message.Bytes[9] << 8 | message.Bytes[8]);
            AccumulatedTorque = (ushort)(message.Bytes[11] << 8 | message.Bytes[10]); 
        }

        public override bool OnBeforeAddToCollection(in List<Message> messages)
        {
            var lastMessage = messages.FindLast(FindLastStandardCrankTorque) as StandardCrankTorque;
            if (lastMessage == null)
                return true;

            if (EventCountDiff(lastMessage) > 0)
            {
                try
                {
                    InstantCadence = CalculateCadence(lastMessage);
                    CalculatedPower = CalculatePower(lastMessage);

                    return true;
                }
                catch (Exception) {}
            }

            return false;
        }

        private bool FindLastStandardCrankTorque(Message message)
        {
            //
            // TODO: figure out how to pass context for averaging a certain # of seconds, instead of just using last message.
            //

            StandardCrankTorque torqueMessage = message as StandardCrankTorque;
            return (torqueMessage != null);
        }

        private byte CalculateCadence(StandardCrankTorque lastMessage)
        {
            double cadence = 60 * (EventCountDiff(lastMessage)) /
                ( PeriodDiff(lastMessage) / 2048D );
            
            return (byte)cadence;
        }

        private double CalculatePower(StandardCrankTorque lastMessage)
        {
            double result = AverageAngularVelocity(lastMessage) * AverageTorque(lastMessage);
            return result;
        }

        private double AverageAngularVelocity(StandardCrankTorque lastMessage)
        {
            //
            // TODO: this doesn't respect rollover math.
            //
            double result = ( 2D * Math.PI * (EventCountDiff(lastMessage)) ) / 
                ( PeriodDiff(lastMessage) / 2048D );

            return result;
        }

        private double AverageTorque(StandardCrankTorque lastMessage)
        {
            double result = AccumulatedTorqueDiff(lastMessage) / 
                (32D * EventCountDiff(lastMessage));

            return result;
        }

        private double AveragePower(double averageTorque, double angularVelocity)
        {
            return averageTorque * angularVelocity;

            // OR Power = 128 * Math.PI * ( (AccumulatedTorque - last.AccumulatedTorque) / (Period - last.Period) )
        }

        private byte EventCountDiff(StandardCrankTorque lastMessage)
        {
            int value; 
            if (lastMessage.EventCount > this.EventCount) // rollover
                value = 1 + (byte.MaxValue ^ lastMessage.EventCount) + this.EventCount;
            else
                value = this.EventCount - lastMessage.EventCount;
            return (byte)value;
        }

        private ushort PeriodDiff(StandardCrankTorque lastMessage)
        {
           int value; 
            if (lastMessage.Period > this.Period) // rollover
                value = (ushort.MaxValue ^ lastMessage.Period) + this.Period;
            else
                value = this.Period - lastMessage.Period;
            return (ushort)value;
        }

        private ushort AccumulatedTorqueDiff(StandardCrankTorque lastMessage)
        {
           int value; 
            if (lastMessage.AccumulatedTorque > this.AccumulatedTorque) // rollover
                value = (ushort.MaxValue ^ lastMessage.AccumulatedTorque) + this.AccumulatedTorque;
            else
                value = this.AccumulatedTorque - lastMessage.AccumulatedTorque;
            return (ushort)value;            
        }
    }
}
