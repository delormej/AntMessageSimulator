using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AntMessageSimulator
{
    public class AutoAntsScriptGenerator : IDisposable
    {
        private PowerMeterSession session;
        private StreamWriter writer;

        #region String Constants
        const string SCRIPT_HEADER =
    @"###ANT_SCRIPT_VERSION: 0.01

#System Reset (and pauses for 2 seconds)
w[4A]
p2000

";
        const string CHANNEL_CONFIGURATION_BLOCK =
    @"#Channel Configuration Block Begin
w  [42][{0:X2}][10][00]     # Assign channel {0}, bidirectional master, network 0
r! [40][{0:X2}][42][00]     # Wait for RESPONSE_NO_ERROR
w  [46][00][B9][A5][21][FB][BD][72][C3][45]  # Set ANT+ key to network 0
r! [40][00][46][00]
w  [51][{0:X2}][{1:X2}][{2:X2}][0B][05]   #Set Channel ID. In this solution we've used Dev# xxxx, DevType 11, TxType 5
r! [40][{0:X2}][51][00]
w  [43][{0:X2}][F6][1F]     #Set Channel Period: (8182/32768) = 4.005Hz
r! [40][{0:X2}][43][00]
w  [45][{0:X2}][39]         #Set Channel RF Frequency
r! [40][{0:X2}][45][00]
w  [4B][{0:X2}]             #Open Channel {0}
r! [40][{0:X2}][4B][00]
#Channel Configuration Block End

";
        const string CHANNEL_CLOSE_BLOCK =
    @"
#Channel Close Block Begin
w  [4C][{0:X2}]          # Close channel {0}
r! [40][{0:X2}][01][07]  # Response: channel closed
w  [41][{0:X2}]          # Un-assign channel {0}
r! [40][{0:X2}][41][00]
#Channel Close Block End

";
        #endregion

        public AutoAntsScriptGenerator(PowerMeterSession session)
        {
            this.session = session;
        }

        public Stream CreateScriptStream()
        {
            Stream stream = new MemoryStream();
            writer = new StreamWriter(stream);

            WriteHeader();
            WriteChannelConfiguration();
            WriteBroadcastMessages();
            WriteChannelClose();

            writer.Flush();

            // IMPORTANT: do not close the writer as that will dispose of the stream.

            stream.Position = 0;
            return stream;
        }

        private static string CreateResponseScriptLine(Message message)
        {
            // Look for successful response.
            return string.Format("r [40][{0:X2}][{1:X2}][03]",
                message.ChannelId,
                message.MessageId);
        }

        /// <summary>
        /// Returns a line of executable Auto ANT script.
        /// </summary>
        public static string ToAutoAntScriptLine(Message message)
        {
            // Write a message, then wait for a response.
            //w [4E][01][10][10][FF][FF][00][10][00][01]
            //r [40][01][01][03]
            //  |-- Reponse Event
            //      |-- ChannelID
            //          |-- Message that was sent
            //              |-- EVENT_TX

            StringBuilder script = new StringBuilder();

            script.Append("w ");

            for (int i = Message.EVENT_ID_POSITION; i < message.PayloadLength; i++)
                script.AppendFormat("[{0:X2}]", message.Bytes[i]);

            script.AppendLine();
            script.AppendLine(CreateResponseScriptLine(message));

            return script.ToString();
        }

        private void WriteHeader()
        {
            writer.Write(SCRIPT_HEADER);
        }

        private void WriteChannelConfiguration()
        {
            byte deviceLsb = (byte)(session.DeviceId & 0xFF);
            byte deviceMsb = (byte)((session.DeviceId & 0xFF00) >> 8);
            writer.Write(CHANNEL_CONFIGURATION_BLOCK, 
                session.ChannelId,
                deviceLsb,
                deviceMsb);
        }

        private void WriteChannelClose()
        {
            writer.Write(CHANNEL_CLOSE_BLOCK, session.ChannelId);
        }

        private void WriteBroadcastMessages()
        {
            IEnumerable<Message> messages = GetPowerMeterBroadcastMessages();
            foreach (Message message in messages)
                writer.Write(ToAutoAntScriptLine(message));
        }

        /// <summary>
        /// // Get Broadcast messages for the power meter channel.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Message> GetPowerMeterBroadcastMessages()
        {
            IEnumerable<Message> messages =
                from message in session.Messages
                where message.IsBroadcastEvent() && 
                    message.ChannelId == session.ChannelId && 
                    message.MessageId < 0xF0    // Ignore manufacturer specific pages.
                select message;

            return messages;
        }

        public void Dispose()
        {
            // Disposing of the writer will close the underlying stream as well.
            if (writer != null)
                writer.Dispose();
        }
    }
}
