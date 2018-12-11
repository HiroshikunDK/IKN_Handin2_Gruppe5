using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

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
	        Transport transport = new Transport(BUFSIZE, APP);
	        byte[] buffer = new byte[BUFSIZE];
            Console.WriteLine(" >> Client connecting");
            buffer = Encoding.ASCII.GetBytes(args[0]);
            transport.send(buffer, buffer.Length);

	        long size = transport.receive(ref buffer);
            if (size != 0)
            {
                string fileName = LIB.extractFileName(args[0]);
                receiveFile(fileName, transport, size);
            }
            else
            {
                Console.WriteLine(transport.receive(ref buffer));
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
        private void receiveFile (String fileName, Transport transport, long fileSize)
		{
		    // TO DO Your own code
		    Console.WriteLine(" >> Recieving file");
		    FileStream fileStream = File.Create("/root/Desktop/IKN/Exercise11" + fileName);
		    byte[] recieveBytes = new byte[BUFSIZE];
		    int readBytes = 0;
		    while (readBytes < fileSize)
		    {
		        readBytes += transport.receive(ref recieveBytes);

		        Console.WriteLine(" >> Read bytes: " + readBytes.ToString());
		    }

		    fileStream.Write(recieveBytes, 0, (int)fileSize);

		    fileStream.Close();
        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
		    Console.WriteLine("Client starts...");
            new file_client(args);
		}
	}
}