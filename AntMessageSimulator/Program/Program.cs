namespace AntMessageSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Printer.Welcome();
            try
            {
                ExecutionOptions options = new ExecutionOptions(args);
                Simulator simulator = new Simulator(options);
                simulator.Execute();
            }
            catch (ApplicationException e)
            {
                Printer.Error(e.Message);
                Printer.Usage();
            }
        }
    }
}
