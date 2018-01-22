using System;
using FTD2XX_NET;

namespace speed
{
    class Program
    {
        const byte FQ_UD = 0x01;  /* TX  (orange) */
        const byte DATA  = 0x02;  /* RX  (yellow) */
        const byte RESET = 0x04;  /* RTS (green)  */
        const byte W_CLK = 0x08;  /* CTS (brown) */
        static FTDI _device;

        static void Main(string[] args)
        {
            OpenDevice();
            SetBitBangMode();
            for (int i = 0; i < 4; i++)
            {
                Prompt();
                byte pin = (byte)(1 << i);
                Write(pin, true);
                Console.WriteLine("Wrote Pin:" + pin);
            }
            Prompt();
            Clear();
            CloseDevice();
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

        static void Write(byte pin, bool high)
        {
            FTDI.FT_STATUS status = FTDI.FT_STATUS.FT_OK;
            byte[] buffer = { high ? pin : (byte)0 };
            uint written = 0;
            status = _device.Write(buffer, 1, ref written);
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Couldn't write to device.");
            }
        }

        static void Clear()
        {
            Write(0, false);
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

        static void Prompt()
        {
            Console.WriteLine("Waiting to write...");
            Console.ReadKey();
        }
    }
}
