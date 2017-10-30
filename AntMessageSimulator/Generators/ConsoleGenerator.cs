using System;
using System.Collections;
using System.Text;

namespace AntMessageSimulator
{
    public class ConsoleGenerator : Generator
    {
        DeviceSession session;
        DeviceType device;

        public ConsoleGenerator(DeviceSession session, DeviceType device)
        {
            this.session = session;
            this.device = device;
        }

        public string Generate()
        {
            MessageQuery query = new MessageQuery(session);
            IEnumerable events = null;

            if (device == DeviceType.FeC)
                events = query.FindAllFecEvents();
            else if (device == DeviceType.PowerMeter)
                events = query.FindAllPowerMeterBroadcastEvents();

            return GetEventPerLine(events);
        }

        private string GetEventPerLine(IEnumerable messages)
        {
            StringBuilder content = new StringBuilder();
            foreach (var message in messages)
                content.AppendLine(message.ToString());

            return content.ToString();
        }
    }
}
