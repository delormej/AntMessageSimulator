using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json;
using CsvHelper;

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

            string text = null;
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteHeader<Reading>();
                csv.NextRecord();
                csv.WriteRecords(readings);

                stream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream);
                text = reader.ReadToEnd();
            }
            
            // string output = JsonConvert.SerializeObject(readings);
            return text; // output;
        }
    }
}
