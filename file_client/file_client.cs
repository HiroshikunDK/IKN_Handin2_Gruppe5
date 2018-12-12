using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;

namespace Application
{
    class file_client
    {
        /// <summary>
        /// The BUFSIZE.
        /// </summary>
        private const int BUFSIZE = 1000;
        private const string APP = "FILE_CLIENT";

        /// <summary>
        /// Initializes a new instance of the <see cref="file_client"/> class.
        /// 
        /// file_client metoden opretter en peer-to-peer forbindelse
        /// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
        /// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
        /// Lukker alle streams og den modtagede fil
        /// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
        /// </summary>
        /// <param name='args'>
        /// Filnavn med evtuelle sti.
        /// </param>
        private file_client(String[] args)
        {

            // TO DO Your own code
            if (args.Length == 1)
            {
                Transport transport = new Transport(BUFSIZE, APP);
                byte[] buffer = new byte[BUFSIZE];
                Console.WriteLine(" >> Client connecting");
                buffer = Encoding.UTF8.GetBytes(args[0]);
                transport.send(buffer, buffer.Length);
                transport.receive(ref buffer);
                long size = (long)BitConverter.ToInt64(buffer, 0);
                Console.WriteLine($"filesize: {size}");

                if (size != 0)
                {
                    string fileName = LIB.extractFileName(args[0]);
                    receiveFile(fileName, transport, size);
                }
                else
                {
                    Console.WriteLine("File not found");
                }
            }
            else
            {
                Console.WriteLine("Use one argument");
            }
        }

        /// <summary>
        /// Receives the file.
        /// </summary>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='transport'>
        /// Transportlaget
        /// </param>
        private void receiveFile(String fileName, Transport transport, long fileSize) // ændre link til transport
        {
            // TO DO Your own code
            Console.WriteLine(" >> Recieving file");
            FileStream fileStream = File.Create("/root/Desktop/IKN/Exercise11/" + fileName);
            byte[] recieveBytes = new byte[BUFSIZE];
            int writtenBytes = 0;

            while (writtenBytes < fileSize)
            {
                int amountRecieved = transport.receive(ref recieveBytes);

                fileStream.Write(recieveBytes, 0, amountRecieved);
                writtenBytes += amountRecieved;

                //Console.WriteLine(" >> Written bytes: " + writtenBytes.ToString()); //debug
            }
            Console.WriteLine(" >> File recieved");

            fileStream.Close();
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// First argument: Filname
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Client starts...");
            new file_client(args);
        }
    }
}