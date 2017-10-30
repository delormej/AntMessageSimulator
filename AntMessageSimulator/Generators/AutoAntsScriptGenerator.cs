using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AntMessageSimulator
{
    public class AutoAntsScriptGenerator : Generator, IDisposable
    {
        private DeviceSession session;
        private StreamWriter writer;
        private DeviceType device;
        private ChannelConfiguration config;
        
        public AutoAntsScriptGenerator(DeviceSession session, DeviceType device)
        {
            this.session = session;
            this.device = device;
            config = new ChannelConfiguration(device, session);
        }

        public string Generate()
        {
            string script = string.Empty;
            Stream stream = CreateScriptStream();
            TextReader reader = new StreamReader(stream);
            script = reader.ReadToEnd();
            Dispose();
                
            return script;
        }

        /// <summary>
        /// Creates a readable stream containing the ANTS script.  
        /// </summary>
        /// <remarks>
        /// Be sure to call Dispose() on the object when done reading.
        /// </remarks>
        public Stream CreateScriptStream()
        {
            Stream stream = new MemoryStream();
            writer = new StreamWriter(stream);

            WriteHeader();
            WriteChannelConfiguration();
            WriteMessages();
            WriteChannelClose();

            writer.Flush();

            // IMPORTANT: do not close the writer as that will dispose of the stream.
            // This class implements IDisposable, so the caller should Dispose of the 
            // instance when done reading.

            stream.Position = 0;
            return stream;
        }

        private void WriteHeader()
        {
            const string SCRIPT_HEADER =
        @"###ANT_SCRIPT_VERSION: 0.01

# Auto Generated at: {0}
#System Reset (and pauses for 2 seconds)
w[4A]
p2000

";
            writer.Write(SCRIPT_HEADER, DateTime.Now);
        }

        private void WriteChannelConfiguration()
        {
            // {0} == ChannelId, {1} == Device Lsb, {2} == Device Msb, {3} DeviceId, {4} DeviceType
            // {5} == ChannelPeriod Lsb, {6} == ChannelPeriod Msb, {7} == ChannelPeriod
            const string CHANNEL_CONFIGURATION_BLOCK =
        @"#Channel Configuration Block Begin
w  [42][{0:X2}][10][00]     # Assign channel {0}, bidirectional master, network 0
r! [40][{0:X2}][42][00]     # Wait for RESPONSE_NO_ERROR
w  [46][00][B9][A5][21][FB][BD][72][C3][45]  # Set ANT+ key to network 0
r! [40][00][46][00]
w  [51][{0:X2}][{1:X2}][{2:X2}][{4:X2}][05]   #Set Channel ID. In this solution we've used Dev# {3}, DevType {4}, TxType 5
r! [40][{0:X2}][51][00]
w  [43][{0:X2}][{5:X2}][{6:X2}]     #Set Channel Period: ({7}/32768)
r! [40][{0:X2}][43][00]
w  [45][{0:X2}][39]         #Set Channel RF Frequency
r! [40][{0:X2}][45][00]
w  [4B][{0:X2}]             #Open Channel {0}
r! [40][{0:X2}][4B][00]
#Channel Configuration Block End

";
            writer.Write(CHANNEL_CONFIGURATION_BLOCK,
                config.ChannelId,
                config.DeviceIdLsb,
                config.DeviceIdMsb,
                config.DeviceId,
                config.DeviceTypeId,
                config.ChannelPeriodLsb,
                config.ChannelPeriodMsb,
                config.ChannelPeriod);
        }

        private void WriteMessages()
        {
            foreach (Message message in GetMessages())
                writer.Write(ToAutoAntScriptLine(message));
        }

        private IEnumerable<Message> GetMessages()
        {
            IEnumerable<Message> messages = null;
            if (device == DeviceType.PowerMeter)
                messages = GetPowerMeterMessages();
            else if (device == DeviceType.FeC)
                messages = GetFecMessages();
            return messages;
        }

        private IEnumerable<Message> GetPowerMeterMessages()
        {
            MessageQuery query = new MessageQuery(session);
            return query.FindAllPowerMeterBroadcastEvents();
        }

        private IEnumerable<Message> GetFecMessages()
        {
            MessageQuery query = new MessageQuery(session);
            return query.FindAllFecTransmitMessages();
        }

        /// <summary>
        /// Returns a line of executable Auto ANT script.
        /// </summary>
        public static string ToAutoAntScriptLine(Message message)
        {
            StringBuilder script = new StringBuilder();

            script.Append("w ");
            script.AppendLine(message.GetPayloadAsString());
            script.AppendLine(CreateResponseScriptLine(message));

            return script.ToString();
        }

        private static string CreateResponseScriptLine(Message message)
        {
            // # A write command looks like this:
            //w [4E][01][10][10][FF][FF][00][10][00][01]
            //  |-- Broadcast Event
            //      |-- ChannelID
            //          |-- Payload

            // # A response command looks like this:
            //r [40][01][01][03]
            //  |-- Reponse Event
            //      |-- ChannelID
            //          |-- Message that was sent
            //              |-- EVENT_TX
            // ? is a wildcard
            const string RESPONSE_COMMAND_FORMAT = "r [40][{0:X2}][?][03]";

            return string.Format(RESPONSE_COMMAND_FORMAT,
                message.ChannelId);
        }

        private void WriteChannelClose()
        {
            const string CHANNEL_CLOSE_BLOCK =
        @"
#Channel Close Block Begin
w  [4C][{0:X2}]          # Close channel {0}
r! [40][{0:X2}][01][07]  # Response: channel closed
w  [41][{0:X2}]          # Un-assign channel {0}
r! [40][{0:X2}][41][00]
#Channel Close Block End

";
            writer.Write(CHANNEL_CLOSE_BLOCK, config.ChannelId);
        }

        public void Dispose()
        {
            // Disposing of the writer will close the underlying stream as well.
            if (writer != null)
                writer.Dispose();
        }
    }
}
