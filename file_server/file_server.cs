using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;


namespace Application
{
    class file_server
    {
        /// <summary>
        /// The BUFSIZE
        /// </summary>
        private const int BUFSIZE = 1000;
        private const string APP = "FILE_SERVER";

        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// </summary>
        private file_server()
        {
            // TO DO Your own code
            Console.WriteLine(" >> Server Started");
            Transport transport = new Transport(BUFSIZE, APP);
            byte[] buffer = new byte[BUFSIZE];

            while (true)
            {
                try
                {
                    int size = transport.receive(ref buffer);
                    Console.WriteLine(" >> Filename requested from client!");
                    String fileName = ((new UTF8Encoding()).GetString(buffer)).Substring(0, size);
                    Console.WriteLine($" >> Filename from client: {fileName}");
                    Console.WriteLine(" >> Filesize requested from client!");
                    long fileSize = LIB.check_File_Exists(fileName);
                    Console.WriteLine($" >> Filesize from server: {fileSize}");
                    Array.Clear(buffer, 0, buffer.Length);
                    byte[] tempBuffer = BitConverter.GetBytes(fileSize);
                    Array.Copy(tempBuffer, 0, buffer, 0, tempBuffer.Length);
                    transport.send(buffer, tempBuffer.Length);
                    if (fileSize != 0)
                    {
                        Console.WriteLine(" >> Sending file...");
                        sendFile(fileName, fileSize, transport);
                        Console.WriteLine(" >> File sent!");
                    }
                    else
                    {
                        Console.WriteLine(" >> Oops, file not found");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='fileSize'>
        /// File size.
        /// </param>
        /// <param name='tl'>
        /// Tl.
        /// </param>
        private void sendFile(String fileName, long fileSize, Transport transport) //ændre link til transport
        {
            // TO DO Your own code
            FileStream fileStream = File.OpenRead(@fileName);
            int bytesRead;
            byte[] buffer = new byte[BUFSIZE];
            long bytesLeft = fileSize;

            while (bytesLeft > 0)
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                transport.send(buffer, bytesRead);
                bytesLeft -= bytesRead;
            }

            fileStream.Close();
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Server starts...");
            new file_server();
        }
    }
}