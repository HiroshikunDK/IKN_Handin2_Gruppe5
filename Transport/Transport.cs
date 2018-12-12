using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
    /// <summary>
    /// Transport.
    /// </summary>
    public class Transport
    {
        /// <summary>
        /// The link.
        /// </summary>
        private Link link;
        /// <summary>
        /// The 1' complements checksum.
        /// </summary>
        private Checksum checksum;
        /// <summary>
        /// The buffer.
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// The seq no.
        /// </summary>
        private byte seqNo;
        /// <summary>
        /// The old_seq no.
        /// </summary>
        private byte old_seqNo;
        /// The old_ack seq no.
        /// </summary>
        private byte old_ackSeqNo;
        /// <summary>
        /// The error count.
        /// </summary>
        private int errorCount;
        /// <summary>
        /// The DEFAULT_SEQNO.
        /// </summary>
        private const int DEFAULT_SEQNO = 2;
        /// <summary>
        /// The data received. True = received data in receiveAck, False = not received data in receiveAck
        /// </summary>
        private bool dataReceived;
        /// <summary>
        /// The number of data the recveived.
        /// </summary>
        private int recvSize = 0;
        /// <summary>
        /// The size of the header.
        /// </summary>
        private const int HEADER_SIZE = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transport"/> class.
        /// </summary>
        public Transport(int BUFSIZE, string APP)
        {
            link = new Link(BUFSIZE + (int)TransSize.ACKSIZE, APP);
            checksum = new Checksum();
            buffer = new byte[BUFSIZE + (int)TransSize.ACKSIZE];
            seqNo = 0;
            old_seqNo = DEFAULT_SEQNO;
            errorCount = 0;
            dataReceived = false;
            old_ackSeqNo = DEFAULT_SEQNO;
        }

        /// <summary>
        /// Receives the ack.
        /// </summary>
        /// <returns>
        /// The ack.
        /// </returns>
        private bool receiveAck()
        {
            recvSize = link.receive(ref buffer);
            dataReceived = true;

            if (recvSize == (int)TransSize.ACKSIZE)
            {
                dataReceived = false;
                if (!checksum.checkChecksum(buffer, (int)TransSize.ACKSIZE) ||
                  buffer[(int)TransCHKSUM.SEQNO] != seqNo ||
                  buffer[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
                {
                    return false;
                }
                seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            }

            return true;
        }

        /// <summary>
        /// Sends the ack.
        /// </summary>
        /// <param name='ackType'>
        /// Ack type.
        /// </param>
        private void sendAck(bool ackType)
        {
            byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
            ackBuf[(int)TransCHKSUM.SEQNO] = (byte)
                (ackType ? (byte)buffer[(int)TransCHKSUM.SEQNO] : (byte)(buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            ackBuf[(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
            checksum.calcChecksum(ref ackBuf, (int)TransSize.ACKSIZE);
			if (++errorCount == 10) // Simulate noise
            {
                ackBuf[1]++; // Important: Only spoil a checksum-field (ackBuf[0] or ackBuf[1])
                Console.WriteLine("Noise!byte #1 is spoiled in transmitted ACK-package");
				errorCount = 0;
            }
            link.send(ackBuf, (int)TransSize.ACKSIZE);
        }

        /// <summary>
        /// Send the specified buffer and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public void send(byte[] buf, int size)
        {

            int numberOfTries = 0;
            do
            {
                buffer[(int)TransCHKSUM.SEQNO] = (byte)seqNo;
                buffer[(int)TransCHKSUM.TYPE] = (byte)TransType.DATA;

                Array.Copy(buf, 0, buffer, HEADER_SIZE, size);

                checksum.calcChecksum(ref buffer, size + HEADER_SIZE);
                //buffer now contains header + payload

                if (++errorCount == 10) // Simulate noise
                {
                    buffer[1]++; // Important: Only spoil a checksum-field (buffer[0] or buffer[1])
                    Console.WriteLine("Noise!-byte #1 is spoiled in transmission");
					errorCount = 0;
                }
                link.send(buffer, size + HEADER_SIZE);
                numberOfTries++;
            } while (!receiveAck() && numberOfTries <= 5);

            old_seqNo = DEFAULT_SEQNO;
        }

        /// <summary>
        /// Receive the specified buffer.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        public int receive(ref byte[] buf)
        {
            bool checksumValidation;
            bool statusCheck;
            int size = 0;
            do
            {
                size = link.receive(ref buffer);

                checksumValidation = checksum.checkChecksum(buffer, size);
                statusCheck = checksumValidation && buffer[(int)TransCHKSUM.SEQNO] != old_seqNo;
                sendAck(checksumValidation);
            } while (!statusCheck);

            old_seqNo = buffer[(int)TransCHKSUM.SEQNO];

            Array.Copy(buffer, (int)HEADER_SIZE, buf, 0, size - HEADER_SIZE);
            return size - HEADER_SIZE;
        }
    }
}