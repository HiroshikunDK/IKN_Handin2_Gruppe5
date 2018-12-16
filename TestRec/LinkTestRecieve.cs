using System;
using Linklaget;
using System.Text;

/// <summary>
/// Transport.
/// </summary>
namespace TestRecieve
{
    /// <summary>
    /// Transport.
    /// </summary>
    public class LinkTestRecieve
    {
        int size = 0;
        private LinkTestRecieve()
        {
            Console.WriteLine("Hej");
            Link link = new Link(1004, "TEST");
            byte[] buffer = new byte[1000];
            size = link.receive(ref buffer);
            Console.WriteLine($"Size: {size} Buffer: {Encoding.ASCII.GetString(buffer)}");
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("LinkTestRecieve starts...");
            new LinkTestRecieve();
        }
    }
}