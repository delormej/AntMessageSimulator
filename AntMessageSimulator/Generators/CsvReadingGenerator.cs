using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration;

namespace AntMessageSimulator
{    
    /// <summary>
    /// Creates a representatin of Readings in CSV format.
    /// </summary>
    public class CsvReadingGenerator : ReadingGenerator
    {
        public CsvReadingGenerator(DeviceSession session) : base(session) {}

        protected override string Serialize(IEnumerable<Reading> readings)
        {
            string text = null;
            
            using (MemoryStream stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.Configuration.RegisterClassMap<ReadingMap>();
                
                csv.WriteHeader<Reading>();
                csv.NextRecord();
                csv.WriteRecords(readings);
                
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                
                StreamReader reader = new StreamReader(stream);
                text = reader.ReadToEnd();
            }

            return text;
        }

        /// <summary>
        /// Creates a config object to ignore certain fields when serializing csv.
        /// </summary>
        private sealed class ReadingMap : ClassMap<Reading>
        {
            public ReadingMap()
            {
                AutoMap(CultureInfo.InvariantCulture);
                Map(m => m.Timestamp).Ignore();
                Map(m => m.Acceleration).Ignore();
                Map(m => m.TargetPower).Ignore();
            }
        }    
    }
}