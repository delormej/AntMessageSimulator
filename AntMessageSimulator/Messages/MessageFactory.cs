using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace AntMessageSimulator
{
    public class MessageFactory
    {
        private static IDictionary<string, IDictionary<byte, Type>> namespaces;

        static MessageFactory()
        {
            namespaces = new Dictionary<string, IDictionary<byte, Type>>();
            foreach (var type in ReflectMessageTypes())
                GetNamespace(type.Value.Namespace).Add(type);
        }
        
        /// <summary>
        /// Parses a single line from an ANT device log and represents as a Message object.
        /// </summary>
        public static Message MessageFromLine(string line)
        {
            return MessageFromLine(DeviceType.Unassigned, line);
        }

        public static Message MessageFromLine(DeviceType deviceType, string line)
        {
            Message message = new Message(line);
            Type t = GetMessageType(deviceType, message.MessageId);
            if (t != null)
                return Activator.CreateInstance(t, message) as Message;
            else
                return message;
        }

        private static Type GetMessageType(DeviceType deviceType, byte messageId)
        {
            Type t = null;
            if (deviceType == DeviceType.Unassigned)
            {
                foreach (var ns in namespaces)
                {
                    t = GetMessageTypeFromNamespace(ns.Value, messageId);
                    if (t != null)
                        break;
                }
            }
            else
            {
                var deviceTypeMessages = GetNamespace(deviceType);
                if (deviceTypeMessages == null)
                    deviceTypeMessages = namespaces["Common"];
                t = GetMessageTypeFromNamespace(deviceTypeMessages, messageId);
            }
            return t;
        }

        private static Type GetMessageTypeFromNamespace(IDictionary<byte, Type> ns, byte messageId)
        {
            if (ns.ContainsKey(messageId))
            {
                Type t = ns[messageId];
                return t;
            }
            else
                return null;
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

        private static IDictionary<byte, Type> GetNamespace(DeviceType deviceType)
        {
            string ns;
            switch (deviceType)
            {
                case DeviceType.FeC:
                    ns = "Fec";
                    break;
                case DeviceType.PowerMeter:
                    ns = "BikePower";
                    break;
                default:
                    ns = "Common";
                    break;
            }
            return GetNamespace(ns);
        }

        private static IDictionary<byte, Type> GetNamespace(string ns) 
        {
            string name = ns.Split('.').Last();
            if (!namespaces.ContainsKey(name))
                namespaces.Add(name, new Dictionary<byte, Type>());
            return namespaces[name];
        }
    }
}
