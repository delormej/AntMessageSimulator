
namespace AntMessageSimulator
{
    public enum TransmitType
    {
        Tx,
        Rx
    }

    public enum DeviceType
    {
        Unassigned = 0,
        PowerMeter = 0x0B,
        FeC = 0x11,
        Speed = 0x7B
    }

    public enum OutputType
    {
        Console,
        File,
        Simulator   /* Chart, SpeedHz, Ants */
    }

    public enum OperationType
    {
        Unassigned,
        SummaryOnly,
        HumanReadable,
        Ants,
        Json,
        Hz,
        CArray,
        Readings
    }
}
