using System;
using System.Threading;
using FTD2XX_NET;

namespace speed
{
    class Program
    {
        const byte FQ_UD = 0x01;  /* TX  (orange: FTDI, green: other) */
        const byte DATA  = 0x02;  /* RX  (yellow: FTDI, white: other) */
        const byte RESET = 0x04;  /* RTS (green: FTDI, yellow: other)  */
        const byte W_CLK = 0x08;  /* CTS (brown: FTDI, blue: other) */
        const UInt32 AD9850_CLOCK = 125000000;   // 125mhz clock

        static FTDI _device;
        static Timer _timer;

        static void Main(string[] args)
        {
            OpenDevice();
            SetBitBangMode();
            SetSpeedMph(15f);
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
            CloseDevice();
        }

        static void SetSpeedMph(float mph)
        {
            float mps = mph * 0.44704f;
            int hz = (int)GetHzFromMps(mps);
            SetTimer(hz);
        }

        static void SetTimer(int hz)
        {
            int timeoutMs = 1000 / hz;
            _timer = new Timer(HandleTimer, null, 0, timeoutMs);
        }

        private static void HandleTimer(object state)
        {
            try
            {
                Pulse(DATA);
            }
            catch
            {
                Console.Write(System.DateTime.Now.ToLongTimeString() + " Had an error pulsing...");
            }
        }

        static void Prompt()
        {
            Console.WriteLine("Waiting to write...");
            Console.ReadKey();
        }

        static UInt32 GetFrequencyTuningWord(float speedMps)
        {
            UInt32 frequency = (UInt32)GetHzFromMps(speedMps);
            UInt32 tuningWord = frequency * 4294967295 / AD9850_CLOCK;  // 2^32 == 4294967296 
            return tuningWord;
        }

        static float GetHzFromMps(float speedMps)
        {
            const float ticksPerMeter = 1.0f / 0.11176f;
            float hz = speedMps * ticksPerMeter;
            return hz;
        }

        static Int32 CalculatePhase()
        {
            return 0;
        }

        static void OpenDevice()
        {
            FTDI.FT_STATUS status = FTDI.FT_STATUS.FT_OK;
            _device = new FTDI();
            status = _device.OpenByIndex(0);
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Couldn't open device.");
            }
        }

        static void SetBitBangMode()
        {
            FTDI.FT_STATUS status = FTDI.FT_STATUS.FT_OK;
            status = _device.SetBitMode(W_CLK | FQ_UD | DATA | RESET, 1);
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Couldn't set bit mode.");
            }
            Clear();
        }

        static void WriteEachPin()
        {
            for (int i = 0; i < 4; i++)
            {
                Prompt();
                byte pin = (byte)(1 << i);
                Write(pin);
                Console.WriteLine("Wrote Pin:" + pin);
            }
        }

        static void WriteRegister(long register)
        {
            // Write 40 bits to DATA.
            for (int i = 0; i < 40; i++)
            {
                if ((register & ((long)1 << i)) > 0)
                    WriteDataBit();
                else
                    Pulse(W_CLK);
            }
            Pulse(FQ_UD); 
        }

        static void WriteDataBit()
        {
            Write(DATA);         // Set data pin
            Write(W_CLK | DATA); // Clock it in, but don't overwrite DATA.
        }

        static void Pulse(byte pin)
        {
            Write(pin);
            Clear();
        }

        static void Clear()
        {
            Write(0);
        }

        static void Write(byte pin)
        {
            FTDI.FT_STATUS status = FTDI.FT_STATUS.FT_OK;
            byte[] buffer = { pin };
            uint written = 0;
            status = _device.Write(buffer, 1, ref written);
            if (status != FTDI.FT_STATUS.FT_OK)
            { 
                throw new Exception("Couldn't write to device.");
            }
        }

       static void CloseDevice()
        {
            FTDI.FT_STATUS status = FTDI.FT_STATUS.FT_OK;
            status = _device.Close();
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Couldn't close device.");
            }
        }
    }
}
