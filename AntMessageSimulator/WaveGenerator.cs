using System;
using System.Collections.Generic;
using System.Text;

namespace AntMessageSimulator
{
    public class WaveGenerator
    {
        #region Constants
        /* Example lines:
            2   55.1   0   3.3   60  
            0   70.0   0   3.3   15  
 
            Line 1: Generator outputs square wave at 55.1Hz, 0V offset and 3.3Vpp output voltage for 60 seconds.  
            Line 2: Generator outputs square wave at 70.0Hz, 0V offset and 3.3Vpp output voltage for 15 seconds.  
        */
        // 
        const string WAVE_STRING_FORMAT = "{0}\t{1:f}\t3.3\t{2}\r\n";
        const int SQUARE_FUNCTION = 2;
        const int NO_CHANGE_FUNCTION = 0;
        #endregion

        DeviceSession session;
        StringBuilder content;

        public WaveGenerator(DeviceSession session)
        {
            this.session = session;
            content = new StringBuilder();
        }

        public string CreateScript()
        {
            float lastTimestamp = 0F;

            foreach (var speedEvent in GetSpeedEvents())
            {
                if (lastTimestamp == 0)
                {
                    lastTimestamp = speedEvent.Timestamp;
                    WriteHeader(speedEvent.Speed);
                }
                else
                {
                    int seconds = (int)(lastTimestamp - speedEvent.Timestamp);
                    WriteLine(seconds, speedEvent.Speed);
                }
            }
            
            return content.ToString();
        }

        private IEnumerable<SpeedEvent> GetSpeedEvents()
        {
            MessageQuery query = new MessageQuery(session);
            foreach (var message in query.FindAllGeneralFeMessages())
                yield return message as SpeedEvent;
        }

        private void WriteHeader(float speedMps)
        {
            content.AppendFormat(WAVE_STRING_FORMAT, SQUARE_FUNCTION, CalculateHz(speedMps));
        }

        private void WriteLine(int durationSeconds, float speedMps)
        {
            content.AppendFormat(WAVE_STRING_FORMAT, NO_CHANGE_FUNCTION, CalculateHz(speedMps));
        }

        private Single CalculateHz(float speedMph)
        {
            const float METERS_PER_FLYWHEEL_REV = 0.11176f;
            const float MPH_TO_KMH = 0.44704f;
            float hz = 0.0f;

            // TODO: Explain why this math works... /2 * 4, /2 ???
            hz = ((((speedMph * MPH_TO_KMH) / METERS_PER_FLYWHEEL_REV) / 2.0f) * 4.0f) / 2.0f;

            return hz;
        }
    }
}
