using System;
using Newtonsoft.Json;

namespace AntMessageSimulator
{
    class JsonGenerator : Generator
    {
        DeviceSession session;

        public JsonGenerator(DeviceSession session)
        {
            this.session = session;
        }

        public string Generate()
        {
            MessageQuery query = new MessageQuery(session);
            var events = query.FindAllPowerMeterAndFecMessages();
            string output = JsonConvert.SerializeObject(events, Formatting.Indented);

            if (output.Length > 10)
                return output;
            else
                throw new ApplicationException("No events were found to generate JSON with.");
        }
    }
}
