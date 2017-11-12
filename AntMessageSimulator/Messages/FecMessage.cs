using System;

namespace AntMessageSimulator
{
    public class FecMessage
    { 
        public static object GetFecData(Message message)
        {
            object data = null;

            // Determine the message type
            switch (message.MessageId)
            {
                case 0xF1: // IrtExtraInfoPage
                    //data = GetIrtInfoFromMessage(message);
                    throw new NotImplementedException();
                    break;
                case 0x19: // SpecificTrainerDataPage
                    throw new NotImplementedException();
                    break;
                case 0x10:
                    throw new NotImplementedException();
                    break;
                case 0x33:
                    throw new NotImplementedException();
                    break;
                case 0x47:
                    throw new NotImplementedException();
                    break;
                default:
                    // just default to a normal message if we can't identify it.
                    data = message.ToString();
                    break;
            }

            return data;
        }
    }
}
