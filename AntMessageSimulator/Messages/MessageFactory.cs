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
                var s = t.Namespace;
                yield return new KeyValuePair<byte, Type>(page, t);
            }
        }

        /// <summary>
        /// Parses a single line from an ANT device log and represents as a Message object.
        /// </summary>
        public static Message MessageFromLine(string line)
        {
            Message message = new Message(line);
            if (messageTypes.ContainsKey(message.MessageId))
            {
                Type t = messageTypes[message.MessageId];
                return Activator.CreateInstance(t, message) as Message;
            }
            else
            {
                return message;
            }
        }
    }
}
