﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AntMessageSimulator.Messages
{
    public class MessageType
    {
        private static IDictionary<string, IDictionary<byte, Type>> namespaces;

        static MessageType()
        {
            namespaces = new Dictionary<string, IDictionary<byte, Type>>();
            foreach (var type in MessageTypes.GetTypes())
                GetNamespace(type.Value.Namespace).Add(type);
        }

        public static Type GetType(DeviceType deviceType, byte messageId)
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
