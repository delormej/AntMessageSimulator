using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AntMessageSimulator
{    
    /// <summary>
    /// Creates a representatin of Readings in Json format.
    /// </summary>
    public class JsonReadingGenerator : ReadingGenerator
    {
        public JsonReadingGenerator(DeviceSession session) : base(session) {}

        protected override string Serialize(IEnumerable<Reading> readings)
        {
            string output = JsonConvert.SerializeObject(readings);
            return output;
        }
    }       
}