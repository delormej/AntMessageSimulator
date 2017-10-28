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
            SpeedEvent lastEvent = null;
            bool hasWrittenHeader = false;
            
            foreach (var speedEvent in GetSpeedEvents())
            {
                if (lastEvent != null)
                {
                    int seconds = (int)(speedEvent.Timestamp - lastEvent.Timestamp);
                    if (speedEvent.Speed != lastEvent.Speed && seconds > 0)
                    {
                        if (!hasWrittenHeader)
                        {
                            WriteHeader(seconds, lastEvent.Speed);
                            hasWrittenHeader = true;
                        }
                        else
                            WriteLine(seconds, lastEvent.Speed);
                    }
                }
                
                lastEvent = speedEvent;
            }
            WriteFinalLine(lastEvent);

            return content.ToString();
        }

        private IEnumerable<SpeedEvent> GetSpeedEvents()
        {
            MessageQuery query = new MessageQuery(session);
            IEnumerable<SpeedEvent> events =
                (IEnumerable<SpeedEvent>)query.FindAllGeneralFeMessages();

            return events;
        }

        private void WriteHeader(int durationSeconds, float speedMps)
        {
            content.AppendFormat(WAVE_STRING_FORMAT, SQUARE_FUNCTION, 
                CalculateHz(speedMps), durationSeconds);
        }

        private void WriteLine(int durationSeconds, float speedMps)
        {
            content.AppendFormat(WAVE_STRING_FORMAT, NO_CHANGE_FUNCTION, 
                CalculateHz(speedMps), durationSeconds);
        }

        private void WriteFinalLine(SpeedEvent lastEvent)
        {
            int finalDuration = (int)(session.GetSessionDuration().TotalSeconds - lastEvent.Timestamp);
            WriteLine(finalDuration, lastEvent.Speed);
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
