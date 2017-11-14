using System;

namespace AntMessageSimulator
{
    public class SpecificTrainerDataMessage : Message
    {
        public short InstantPower;
        public byte TargetPowerLimits;
        public byte TrainerFEState;

        public SpecificTrainerDataMessage(Message message) : base(message)
        {
            if (message.MessageId != SPECIFIC_TRAINER_DATA_PAGE)
                throw new ApplicationException("Not a valid Specific Trainer data message.");

            InstantPower = GetInstantPower(message);
            TargetPowerLimits = GetTargetPowerLimits(message);
            TrainerFEState = GetTrainerFEState(message);
        }

        private short GetInstantPower(Message message)
        {
            // Uses 1.5 bytes
            return (short)(((message.Bytes[6 + MESSAGE_HEADER_LENGTH] & 0x0F) << 8) | 
                message.Bytes[5 + MESSAGE_HEADER_LENGTH]);
        }

        private byte GetTargetPowerLimits(Message message)
        {
            /* 0 - Trainer Operating at target power, 1 - Speed is too low, 2 - speed is too high, 3 - target power limit reached */
            return (byte)(message.Bytes[7 + MESSAGE_HEADER_LENGTH] & 0x03);
        }

        private byte GetTrainerFEState(Message message)
        {
            /* 1 = Off, 2 = READY, 3 = IN_USE, 4 = Finished (Paused) */
            return (byte)((message.Bytes[7 + MESSAGE_HEADER_LENGTH] & 0x70) >> 4);
        }
    }
}
