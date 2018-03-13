using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    public class MessageFactory
    {
        private static IDictionary<byte, Type> messageTypes;

        static MessageFactory()
        {
            messageTypes = new Dictionary<byte, Type>();
            foreach (var kv in ReflectMessageTypes())
                messageTypes.Add(kv);
        }

        private static IEnumerable<KeyValuePair<byte, Type>> ReflectMessageTypes()
        {
            // ASSERT that Page is a static member of base class Message.
            if (Message.Page != 0)
                throw new ApplicationException("Message must have a static member Page.");

            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.BaseType == typeof(Message));
            foreach (Type t in types)
            {
                var pageField = t.GetField("Page", BindingFlags.Public | BindingFlags.Static);
                if (pageField == null)
                    continue;
                byte page = (byte)pageField.GetValue(null);
                yield return new KeyValuePair<byte, Type>(page, t);
            }
        }

        /// <summary>
        /// Parses a single line from an ANT device log and represents as a Message object.
        /// </summary>
        public static Message MessageFromLine(string line)
        {
            Message message = new Message(line);
            Type t = messageTypes[message.MessageId];
            
            switch (message.MessageId)
            {
                case IrtExtraInfoMessage.Page:
                    //return new IrtExtraInfoMessage(message);
                    return Activator.CreateInstance(t, message) as Message;
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
