using System;
using Linklaget;
using System.Text;

/// <summary>
/// Transport.
/// </summary>
namespace TestSender
{
    /// <summary>
    /// Transport.
    /// </summary>
    public class LinkTestSender
    {

		private LinkTestSender()
        {
            Link link = new Link(1004, "TEST");
            byte[] buffer = Encoding.ASCII.GetBytes("AXBY");
			link.send(buffer, buffer.Length);
        }

        public static void Main(string[] args)
        {
			Console.WriteLine("LinkTestSender starts...");
			Console.WriteLine("Sending AXBY");

			new LinkTestSender();
        }
    }
}