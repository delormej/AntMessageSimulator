using System;

namespace AntMessageSimulator
{
    public class MessageFactory
    {
        /// <summary>
        /// Parses a single line from an ANT device log and represents as a Message object.
        /// </summary>
        public static Message MessageFromLine(string line)
        {
            Message message = new Message(line);
            if (message.MessageId == 0xF1)
                return new IrtExtraInfoMessage(message);

            return message;
        }

    }
}
