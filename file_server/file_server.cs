using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

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
		private file_server ()
		{
		    // TO DO Your own code
            Transport transport = new Transport(BUFSIZE, APP);
            byte[] buffer = new byte[BUFSIZE];
		    transport.receive(ref buffer);

            Console.WriteLine(" >> Server Started");
            while (true)
            {
                try
                {
                    transport.receive(ref buffer);
                    Console.WriteLine(" >> Filename requested from client!");
                    string fileName = Encoding.ASCII.GetString(buffer);
                    Console.WriteLine($" >> Filename from client: {fileName}");
                    Console.WriteLine(" >> Filesize requested from client!");
                    long fileSize = LIB.check_File_Exists(fileName);
                    Console.WriteLine($" >> Filesize from server: {fileSize}");
                    Array.Clear(buffer, 0, buffer.Length);
                    buffer = BitConverter.GetBytes(fileSize);
                    transport.send(buffer, buffer.Length);
                    if (fileSize != 0)
                    {
                        Console.WriteLine(" >> Sending file...");
                        sendFile(fileName, fileSize, transport);
                        Console.WriteLine(" >> File sent!");
                    }
                    else
                    {
                        buffer = Encoding.ASCII.GetBytes("File not found");
                        transport.send(buffer, buffer.Length);
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
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
            // TO DO Your own code
		    FileStream fileStream = File.OpenRead(@fileName);
		    byte[] fileBytes = new byte[fileSize];
            byte[] buffer = new byte[BUFSIZE];
		    fileStream.Read(fileBytes, 0, fileBytes.Length);
		    long bytesLeft = fileSize;
		    long bytesSent = 0;
		    for (int i = 0; i < fileSize; i += 1000)
		    {
		        if (bytesLeft > 1000)
		        {
                    Array.Copy(fileBytes, bytesSent, buffer, 0, 1000 );
		            transport.send(buffer, buffer.Length);
		            bytesLeft -= 1000;
		            bytesSent += 1000;
		        }
		        else if (bytesLeft < 1000 && bytesLeft > 0)
		        {
		            Array.Copy(fileBytes, bytesSent, buffer, 0, bytesLeft);
                    transport.send(buffer, buffer.Length);
		            bytesSent += bytesLeft;
		            bytesLeft = 0;
		        }
		        Console.WriteLine(" >> Sent bytes: " + bytesSent.ToString());
		        Console.WriteLine(" >> Bytes left: " + bytesLeft.ToString());
		    }
		    fileStream.Close();
        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
		    Console.WriteLine("Server starts...");
            new file_server();
		}
	}
}