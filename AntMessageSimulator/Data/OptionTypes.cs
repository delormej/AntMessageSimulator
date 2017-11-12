
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
        PowerMeter,
        FeC
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
        CArray
    }
}
