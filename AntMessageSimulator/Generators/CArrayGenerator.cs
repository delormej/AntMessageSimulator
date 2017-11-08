using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AntMessageSimulator
{
    class CArrayGenerator : Generator
    {
        DeviceSession session;
        DeviceType device;
        StringBuilder content;

        public CArrayGenerator(DeviceSession session, DeviceType device)
        {
            this.session = session;
            this.device = device;
            content = new StringBuilder();
        }

        public string Generate()
        {
            WriteHeader();
            WriteLines();
            WriteFooter();
            return content.ToString();
        }

        private IEnumerable<Message> GetMessages()
        {
            MessageQuery query = new MessageQuery(session);
            IEnumerable<Message> messages = null;

            if (device == DeviceType.FeC)
                throw new NotImplementedException("FEC not supported for C Array generation.");
            else if (device == DeviceType.PowerMeter)
                messages = query.FindAllPowerMeterBroadcastEvents()
                    .Where(m => m.GetMessageId() == 0x20);

            return messages;
        }

        private void WriteHeader()
        {
            const string header = "const uint8_t power[][8] = { ";
            content.AppendLine(header);
        }

        private void WriteLines()
        {
            IEnumerable<Message> messages = GetMessages();

            foreach (var message in messages)
                WriteLine(message);
        }

        private void WriteLine(Message message)
        {
            const string line = "\t{{ 0x{0:X2}, 0x{1:X2}, 0x{2:X2}, 0x{3:X2}, 0x{4:X2}, 0x{5:X2}, 0x{6:X2}, 0x{7:X2} }} ,\r\n";
            content.AppendFormat(line, message.Bytes[0 + 4], message.Bytes[1 + 4], message.Bytes[2 + 4], message.Bytes[3 + 4],
                message.Bytes[4 + 4], message.Bytes[5 + 4], message.Bytes[6 + 4], message.Bytes[7 + 4]);
        }

        private void WriteFooter()
        {
            const string line = "\t{ 0,0,0,0,0,0,0,0 }\r\n}";
            content.AppendLine(line);
        }
    }
}
