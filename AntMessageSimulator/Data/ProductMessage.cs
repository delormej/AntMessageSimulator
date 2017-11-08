
namespace AntMessageSimulator
{
    public class ProductMessage
    {
        public static string ParseProductVersion(Message message)
        {
            const string format = "{0}.{1}.{2}";
            byte build = message.Bytes[2 + Message.MESSAGE_HEADER_LENGTH];
            byte major = (byte)(message.Bytes[3 + Message.MESSAGE_HEADER_LENGTH] >> 4);
            byte minor = (byte)(message.Bytes[3 + Message.MESSAGE_HEADER_LENGTH] & 0x0F);

            return string.Format(format, major, minor, build);
        }

        private string ParseSerialNumber()
        {
            //memcpy((uint8_t*)&(ant_product_page[4]), (uint32_t*)&(SERIAL_NUMBER), sizeof(uint32_t));
            return "";
        }
    }
}
