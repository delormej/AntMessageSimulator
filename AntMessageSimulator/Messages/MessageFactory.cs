using System;

namespace AntMessageSimulator
{
    public class MessageFactory
    {
        /// <summary>
        /// Parses a single line from an ANT device log and represents as a Message object.
        /// </summary>
        public static Message MessageFromLine(string line)
        {
            Message message = new Message(line);
            switch (message.MessageId)
            {
                case 0xF1:
                    return new IrtExtraInfoMessage(message);
                case 0x19:
                    return new SpecificTrainerDataMessage(message);
                case 0x10:
                    return new GeneralFEDataMessage(message);
                case 0x33:
                    return new TrackResistanceMessage(message);
                case 0x47:
                    return new CommandStatusMessage(message);
                case 0x51:
                    return new ProductMessage(message);
                default:
                    return message;
            }
        }
    }
}
