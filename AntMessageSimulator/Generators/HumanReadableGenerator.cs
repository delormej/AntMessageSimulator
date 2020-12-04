using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AntMessageSimulator
{
    public class HumanReadableGenerator : Generator
    {
        DeviceSession session;
        ExecutionOptions options;

        public HumanReadableGenerator(DeviceSession session, ExecutionOptions options)
        {
            this.session = session;
            this.options = options;
        }

        public string Generate()
        {
            MessageQuery query = new MessageQuery(session);
            IQueryable<Message> events = null;

            if (options.Device == DeviceType.FeC)
                events = query.FindAllFecMessages(); //.Where("MessageId != 79");
            else if (options.Device == DeviceType.PowerMeter)
                events = query.FindAllPowerMeterBroadcastEvents();

            return GetEventPerLine(events);
        }

        private string GetEventPerLine(IEnumerable messages)
        {
            StringBuilder content = new StringBuilder();
            foreach (Message message in messages)
                content.AppendLine(GetString(message));

            return content.ToString();
        }

        private string GetString(Message message)
        {
            return message.ToString();
        }
    }
}
