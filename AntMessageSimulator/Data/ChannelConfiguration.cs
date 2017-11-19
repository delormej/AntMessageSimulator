
namespace AntMessageSimulator
{
    public class ChannelConfiguration
    {
        public byte ChannelId { get; private set; }
        public byte DeviceTypeId { get; private set; }

        public ushort DeviceId { get; private set; }

        public byte DeviceIdLsb
        {
            get { return GetLsb(DeviceId); }
        }

        public byte DeviceIdMsb
        {
            get { return GetMsb(DeviceId); }
        }

        public ushort ChannelPeriod { get; private set; }

        public byte ChannelPeriodLsb
        {
            get { return GetLsb(ChannelPeriod); }
        }

        public byte ChannelPeriodMsb
        {
            get { return GetMsb(ChannelPeriod); }
        }

        public byte ChannelType { get; internal set; }

        public string ChannelDescription { get; internal set; }

        public ChannelConfiguration(DeviceType deviceType, DeviceSession session)
        {
            if (deviceType == DeviceType.PowerMeter)
            {
                ChannelId = session.PowerMeterChannelId;
                DeviceId = session.PowerMeterId;

                // ANT+ Bike Power Device Type spec
                DeviceTypeId = 0x0B;
                ChannelPeriod = 8182;
                ChannelType = 0x10; // bidirectional master
                ChannelDescription = "bi-directional master"; 
            }
            else if (deviceType == DeviceType.FeC)
            {
                ChannelId = session.FecChannelId;
                DeviceId = session.FecId;

                // ANT+ FE-C Device Type spec
                DeviceTypeId = 0x11;
                ChannelPeriod = 8192;
                ChannelType = 0x00; // slave
                ChannelDescription = "slave";
            }
        }

        private static byte GetLsb(ushort value)
        {
            return (byte)(value & 0xFF);
        }

        private static byte GetMsb(ushort value)
        {
            return (byte)((value & 0xFF00) >> 8);
        }
    }
}
