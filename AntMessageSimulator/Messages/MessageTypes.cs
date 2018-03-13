using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AntMessageSimulator.Messages
{
    class MessageTypes
    { 
        public static IEnumerable<KeyValuePair<byte, Type>> GetTypes()
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
    }
}
