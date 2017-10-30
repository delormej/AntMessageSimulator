namespace AntMessageSimulator
{
    public class GeneratorFactory
    {
        public static Generator Create(DeviceSession session, ExecutionOptions options)
        {
            Generator generator = null;
            if (options.OutputAnts)
                generator = new AutoAntsScriptGenerator(session, options.Device);
            else if (options.OutputJson)
                generator = new JsonGenerator(session, options.Device);
            else if (options.OutputSpeed)
                generator = new WaveGenerator(session);
            else if (options.OutputConsole)
                generator = new ConsoleGenerator(session, options.Device);
            return generator;
        }
    }
}
