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
        public ushort CalculatedPower { get; private set; }

        public StandardCrankTorque(Message message) : base(message)
        {
            EventCount = message.Bytes[5];
            CrankTicks = message.Bytes[6];
            InstantCadence = message.Bytes[7];
            Period = (ushort)(message.Bytes[9] << 8 | message.Bytes[8]);
            AccumulatedTorque = (ushort)(message.Bytes[11] << 8 | message.Bytes[10]); 
        }

        public override void OnAddedToCollection(in List<Message> messages)
        {
            var lastMessage = messages.FindLast(FindLastStandardCrankTorque) as StandardCrankTorque;

            if (lastMessage != null)
            {
                try
                {
                    InstantCadence = CalculateCadence(lastMessage);
                    CalculatedPower = CalculatePower(lastMessage);
                }
                catch (Exception)
                { }
               
            }
        }

        private bool FindLastStandardCrankTorque(Message message)
        {
            //
            // TODO: figure out how to pass context for averaging a certain # of seconds, instead of just using last message.
            //

            StandardCrankTorque torqueMessage = message as StandardCrankTorque;
            if (torqueMessage == null)
                return false;
            // Only return true if there has been at least 1 event count difference.
            return (EventCountDiff(torqueMessage) > 0);
        }

        private byte CalculateCadence(StandardCrankTorque lastMessage)
        {
            //
            // TODO: this doesn't respect rollover math.
            //
            int cadence = 60 * (EventCountDiff(lastMessage)) /
                ( PeriodDiff(lastMessage) / 2048 );

            return (byte)cadence;
        }

        private ushort CalculatePower(StandardCrankTorque lastMessage)
        {
            double result = AverageAngularVelocity(lastMessage) * AverageAngularVelocity(lastMessage);
            return (ushort)result;
        }

        private double AverageAngularVelocity(StandardCrankTorque lastMessage)
        {
            //
            // TODO: this doesn't respect rollover math.
            //
            double result = ( 2 * Math.PI * (EventCountDiff(lastMessage)) ) / 
                ( PeriodDiff(lastMessage) / 2048 );

            return result;
        }

        private double AverageTorque(StandardCrankTorque lastMessage)
        {
            double result = AccumulatedTorqueDiff(lastMessage) / 
                (32 * EventCountDiff(lastMessage));

            return result;
        }

        private double AveragePower(double averageTorque, double angularVelocity)
        {
            return averageTorque * angularVelocity;

            // OR Power = 128 * Math.PI * ( (AccumulatedTorque - last.AccumulatedTorque) / (Period - last.Period) )
        }

        private byte EventCountDiff(StandardCrankTorque lastMessage)
        {
            byte value; 
            if (lastMessage.EventCount > this.EventCount) // rollover
                value = byte.MaxValue ^ lastMessage.EventCount + this.EventCount;
            else
                value = this.EventCount - lastMessage.EventCount;
            return value;
        }

        private ushort PeriodDiff(StandardCrankTorque lastMessage)
        {
           byte value; 
            if (lastMessage.Period > this.Period) // rollover
                value = ushort.MaxValue ^ lastMessage.Period + this.Period;
            else
                value = this.Period - lastMessage.Period;
            return value;
        }

        private ushort AccumulatedTorqueDiff(StandardCrankTorque lastMessage)
        {
           byte value; 
            if (lastMessage.AccumulatedTorque > this.AccumulatedTorque) // rollover
                value = ushort.MaxValue ^ lastMessage.AccumulatedTorque + this.AccumulatedTorque;
            else
                value = this.AccumulatedTorque - lastMessage.AccumulatedTorque;
            return value;            
        }
    }
}
