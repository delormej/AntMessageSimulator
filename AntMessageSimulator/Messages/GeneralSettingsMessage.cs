using System;

namespace AntMessageSimulator
{
    public class GeneralSettingsMessage : Message
    {
        public GeneralSettingsMessage(Message message) : base(message)
        {
            if (message.MessageId != GENERAL_SETTINGS_PAGE)
                throw new ApplicationException("Not a valid General Settings page.");

            ResistanceLevel = ParseResistanceLevel();
        }

        public float ResistanceLevel { get; private set; }

        private float ParseResistanceLevel()
        {
            byte value = Bytes[6 + MESSAGE_HEADER_LENGTH];
            if (value <= 200)
                return (value / 200F) * 100F;
            else
                return 0;
        }

        public override string ToString()
        {
            const string format = "{{ Timestamp = {0:F3}, Resistance Level = {1:F1}% }}";
            return string.Format(format, Timestamp, ResistanceLevel);
        }
    }
}
