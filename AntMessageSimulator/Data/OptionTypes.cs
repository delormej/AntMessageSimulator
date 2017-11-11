using System;

namespace AntMessageSimulator
{
    public enum DeviceType
    {
        Unassigned = 0,
        PowerMeter,
        FeC
    }

    public enum OutputType
    {
        File,
        Console,
        Simulator
    }

    public enum OperationType
    {
        Ants,
        Json,
        Hz,
        CArray
    }
}