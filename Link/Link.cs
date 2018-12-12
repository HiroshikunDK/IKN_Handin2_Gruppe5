using System;
using System.IO.Ports;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
    /// <summary>
    /// Link.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// The DELIMITE for slip protocol.
        /// </summary>
        const byte DELIMITER = (byte)'A';
        const byte B_ESCAPE = (byte)'B';
        const byte C_CONV = (byte)'C';
        const byte D_CONV = (byte)'D';
        /// <summary>
        /// The buffer for link.
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// The serial port.
        /// </summary>
        SerialPort serialPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="link"/> class.
        /// </summary>
        public Link(int BUFSIZE, string APP)
        {
            // Create a new SerialPort object with default settings.
            /*#if DEBUG
                if(APP.Equals("FILE_SERVER"))
                {
                    serialPort = new SerialPort("/dev/ttySn0",115200,Parity.None,8,StopBits.One);
                }
                else
                {
                    serialPort = new SerialPort("/dev/ttySn1",115200,Parity.None,8,StopBits.One);
                }
            #else*/
            serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);
            //#endif
            if (!serialPort.IsOpen)
                serialPort.Open();

            buffer = new byte[(BUFSIZE * 2)];

            // Uncomment the next line to use timeout
            //serialPort.ReadTimeout = 500;

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        /// <summary>
        /// Send the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public void send(byte[] buf, int size)
        {
            // TO DO Your own code
            buffer[0] = (byte)'A';
            int location = 1;
            for (int i = 0; i < size; i++)
            {
                if (buf[i] == (byte)'A')
                {
                    buffer[location] = (byte)'B';
                    buffer[location + 1] = (byte)'C';
                    location += 2;
                }
                else if (buf[i] == (byte)'B')
                {
                    buffer[location] = (byte)'B';
                    buffer[location + 1] = (byte)'D';
                    location += 2;
                }
                else
                {
                    buffer[location] = buf[i];
                    location++;
                }
            }
            buffer[location] = (byte)'A';         
            serialPort.Write(buffer, 0, buffer.Length);
            Array.Clear(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Receive the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public int receive(ref byte[] buf)
        {
            byte b;
            do
            {
                b = (byte)serialPort.ReadByte();
            } while (b != (byte)'A');

            int i = 0;

            do
            {
                b = (byte)serialPort.ReadByte();
				if (b != (byte)'A')
				{
					buffer[i] = b;
					i++;
				}
			} while (b != (byte)'A');

            int n = 0;
            
            for (int j = 0; j < i; j++)
            {
                if (buffer[j] == (byte)'B' && buffer[j + 1] == (byte)'C')
                {
                    buf[n] = (byte)'A';
                    j++;
                }
                else if (buffer[j] == (byte)'B' && buffer[j + 1] == (byte)'D')
                {
                    buf[n] = (byte)'B';
                    j++;
                }
                else
                {
                    buf[n] = buffer[j];
                }
                n++;
            }
            return n;


        }
    }
}
