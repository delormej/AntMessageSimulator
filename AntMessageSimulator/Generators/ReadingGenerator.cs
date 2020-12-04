using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AntMessageSimulator
{    
    /// <summary>
    /// Creates a collection of objects representing each second.
    /// </summary>
    public class ReadingGenerator : Generator
    {
        DeviceSession session;

        public IEnumerable<Reading> Readings { get; private set; }

        public ReadingGenerator(DeviceSession session)
        {
            this.session = session;
            Readings = new List<Reading>();
        }

        public string Generate()
        {
            var readings = (List<Reading>)Readings;
            CreateReadingVisitor visitor = new CreateReadingVisitor();

            foreach (var e in session.Messages)
            {
                var reading = e.Accept(visitor);
                if (reading != null)
                    readings.Add(reading);
            }

            string output = JsonConvert.SerializeObject(readings);
            return output;
        }
    }
}
