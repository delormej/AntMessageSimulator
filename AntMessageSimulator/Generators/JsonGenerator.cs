using System;
using Newtonsoft.Json;

namespace AntMessageSimulator
{
    class JsonGenerator : Generator
    {
        DeviceSession session;

        public JsonGenerator(DeviceSession session) : this(session, DeviceType.FeC)
        {
        }

        public JsonGenerator(DeviceSession session, DeviceType deviceType)
        {
            if (deviceType != DeviceType.FeC)
                throw new NotImplementedException("Only FE-C supported for JSON output.");

            this.session = session;
        }

        public string Generate()
        {
            MessageQuery query = new MessageQuery(session);
            var events = query.FindAllFecEvents();
            return JsonConvert.SerializeObject(events, Formatting.Indented);
        }
    }
}
