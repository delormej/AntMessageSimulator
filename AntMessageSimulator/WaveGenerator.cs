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
            this.content = new StringBuilder();
        }

        public string CreateScript()
        {
            StringBuilder wave = new StringBuilder();
            // Dummy first line @1hz, 0 power

            WriteHeader();

            foreach (float speed in GetSpeeds())
                wave.AppendFormat(WAVE_STRING_FORMAT, NO_CHANGE_FUNCTION, CalculateHz(speed));

            return wave.ToString();
        }

        private void WriteHeader()
        {
            // 1st line sets initial function, contains a dummy speed.
            content.AppendFormat(WAVE_STRING_FORMAT, SQUARE_FUNCTION, 1.0F /*DUMMY_HZ*/);
        }

        private IEnumerable<float> GetSpeeds()
        {
            MessageQuery query = new MessageQuery(session);
            foreach (SpeedEvent message in query.FindAllGeneralFeMessages())
                yield return message.Speed;
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
