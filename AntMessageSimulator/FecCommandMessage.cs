using System;

namespace AntMessageSimulator
{
    public class FecCommandMessage
    {
        Message message;
        byte command;

        public FecCommandMessage(Message message)
        {
            this.message = message;
            command = message.GetMessageId();

            if (command == null)
                throw new Exception("Message is not an FE-C command: " + 
                    message.ToString());
        }
        
    }
}
