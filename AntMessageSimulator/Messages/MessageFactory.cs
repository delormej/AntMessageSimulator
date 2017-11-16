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
                case Message.IRT_EXTRAINFO_PAGE:
                    return new IrtExtraInfoMessage(message);
                case Message.SPECIFIC_TRAINER_DATA_PAGE:
                    return new SpecificTrainerDataMessage(message);
                case Message.GENERAL_FEDATA_PAGE:
                    return new GeneralFEDataMessage(message);
                case Message.TRACK_RESISTANCE_PAGE:
                    return new TrackResistanceMessage(message);
                case Message.COMMAND_STATUS_PAGE:
                    return new CommandStatusMessage(message);
                case Message.GENERAL_SETTINGS_PAGE:
                    return new GeneralSettingsMessage(message);
                case Message.PRODUCT_PAGE:
                    return new ProductMessage(message);
                default:
                    return message;
            }
        }
    }
}
