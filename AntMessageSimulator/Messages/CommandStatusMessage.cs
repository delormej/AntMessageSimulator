using System;

namespace AntMessageSimulator.Messages.Fec
{
    public class CommandStatusMessage : Message
    {
        public new const byte Page = 0x47;

        public byte LastCommand { get; private set; }
        public byte Status { get; private set; }

        public CommandStatusMessage(Message message) : base(message)
        {
            if (message.MessageId != Page)
                throw new ApplicationException("Not a valid Command Status data message.");

            LastCommand = GetLastCommand(message);
            Status = GetStatus(message);
            //Grade = TrackResistanceMessage.DecodeGrade(message),    // hack: this is only good if lastcommand = 0x33.
            //CoEff = message.Bytes[7 + 4]
        }

        private byte GetLastCommand(Message message)
        {
            return message.Bytes[1 + MESSAGE_HEADER_LENGTH];
        }

        private byte GetStatus(Message message)
        {
            return message.Bytes[3 + MESSAGE_HEADER_LENGTH];
        }
    }
}
