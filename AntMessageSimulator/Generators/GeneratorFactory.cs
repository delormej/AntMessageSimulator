namespace AntMessageSimulator
{
    public class GeneratorFactory
    {
        public static Generator Create(DeviceSession session, ExecutionOptions options)
        {
            Generator generator = null;
            if (options.Operation == OperationType.Ants)
                generator = new AutoAntsScriptGenerator(session, options.Device);
            else if (options.Operation == OperationType.Json)
                generator = new JsonGenerator(session, options.Device);
            else if (options.Operation == OperationType.Hz)
                generator = new WaveGenerator(session);
            else if (options.Operation == OperationType.CArray)
                generator = new CArrayGenerator(session, options.Device);
            else if (options.Operation == OperationType.HumanReadable)
                generator = new HumanReadableGenerator(session, options);
            return generator;
        }
    }
}
