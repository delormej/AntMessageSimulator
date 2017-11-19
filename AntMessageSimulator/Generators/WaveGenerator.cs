using System;
using System.Collections.Generic;
using System.Text;

namespace AntMessageSimulator
{
    public class WaveGenerator : Generator
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
        private SpeedEvent lastEvent;

        public WaveGenerator(DeviceSession session)
        {
            this.session = session;
        }

        public string Generate()
        {
            Reset();
            foreach (var speedEvent in GetSpeedEvents())
                WriteEvent(speedEvent);
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

        private void Reset()
        {
            lastEvent = null;
            content = new StringBuilder();
        }

        private void WriteEvent(SpeedEvent speedEvent)
        {
            int seconds = 0;
            if (lastEvent == null)
            {
                WriteLine((int)speedEvent.Timestamp, 0, SQUARE_FUNCTION);
                lastEvent = speedEvent;
            }
            else
            {
                seconds = (int)(speedEvent.Timestamp - lastEvent.Timestamp);
                if (seconds > 0 && speedEvent.Speed != lastEvent.Speed)
                {
                    WriteLine(seconds, lastEvent.Speed, NO_CHANGE_FUNCTION);
                    lastEvent = speedEvent;
                }
            }            
        }

        private void WriteLine(int durationSeconds, float speedMps, int function)
        {
            content.AppendFormat(WAVE_STRING_FORMAT, function, 
                CalculateHz(speedMps), durationSeconds);
        }

        private void WriteFinalLine(SpeedEvent lastEvent)
        {
            int finalDuration = (int)(session.GetSessionDuration().TotalSeconds - lastEvent.Timestamp);
            WriteLine(finalDuration, lastEvent.Speed, NO_CHANGE_FUNCTION);
        }

        private Single CalculateHz(float speedMps)
        {
            const float METERS_PER_FLYWHEEL_REV = 0.11176f;
            float hz = (speedMps / METERS_PER_FLYWHEEL_REV);
            
            // PCLab200 tool does not like 0, it has a special meaning.
            if (hz == 0.0f)
                hz = 0.01f;
            return hz;
        }
    }
}
