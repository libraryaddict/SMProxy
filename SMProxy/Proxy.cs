using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SMProxy
{
    public class Proxy
    {
        public const int ProtocolVersion = 49;
        public const int MillisecondsBetweenUpdates = 10;

        public Timer Timer { get; set; }
        public NetworkStream Client { get; set; }
        public NetworkStream Server { get; set; }
        private MinecraftStream ClientStream { get; set; }
        private MinecraftStream ServerStream { get; set; }
        private byte[] ServerSharedKey { get; set; }
        private byte[] ClientSharedKey { get; set; }
        private RSAParameters ServerKey { get; set; }
        private RSACryptoServiceProvider CryptoServiceProvider { get; set; }
        // Temporary variables that are used while initializing encryption
        private EncryptionKeyRequestPacket ClientEncryptionRequest { get; set; }
        private EncryptionKeyResponsePacket ServerEncryptionResponse { get; set; }

        public Proxy(NetworkStream client, NetworkStream server)
        {
            Client = client;
            Server = server;
            ClientStream = new MinecraftStream(new BufferedStream(Client));
            ServerStream = new MinecraftStream(new BufferedStream(Server));
            CryptoServiceProvider = new RSACryptoServiceProvider(1024);
            ServerKey = CryptoServiceProvider.ExportParameters(true);
        }

        public void Start()
        {
            Timer = new Timer(Tick, null, MillisecondsBetweenUpdates, Timeout.Infinite);
        }

        public void Stop()
        {
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Tick(object discarded)
        {
            // TODO: Fallback to raw proxy
            UpdateServer();
            UpdateClient();
            // We do the timer this way in case there's some enormous packet or something that takes more than
            // MillisecondsBetweenUpdates to deal with. This way, we don't have a race condition where Tick is
            // running several times simultaneously.
            Timer.Change(MillisecondsBetweenUpdates, Timeout.Infinite);
        }

        private void UpdateClient()
        {
            if (!Client.DataAvailable)
                return;
            var packet = PacketReader.ReadPacket(ClientStream);
            Console.WriteLine("[CLIENT->SERVER]: " + packet.GetType().Name);

            if (packet is EncryptionKeyResponsePacket)
                FinializeClientEncryption((EncryptionKeyResponsePacket)packet);
            else
            {
                Console.WriteLine("Packet proxied.");
                packet.WritePacket(ServerStream);
                // We use a BufferedStream to make sure packets get sent in one piece, rather than
                // a field at a time. Flushing it here sends the assembled packet.
                ServerStream.Flush();
            }
        }

        private void UpdateServer()
        {
            if (!Server.DataAvailable)
                return;
            var packet = PacketReader.ReadPacket(ServerStream);
            Console.WriteLine("[SERVER->CLIENT]: " + packet.GetType().Name);

            if (packet is EncryptionKeyRequestPacket)
                InitializeEncryption((EncryptionKeyRequestPacket)packet);
            else if (packet is EncryptionKeyResponsePacket)
                FinializeServerEncryption((EncryptionKeyResponsePacket)packet);
            else
            {
                Console.WriteLine("Packet proxied.");
                packet.WritePacket(ClientStream);
                ClientStream.Flush();
            }
        }

        private void InitializeEncryption(EncryptionKeyRequestPacket packet)
        {
            // We have to hijack the encryption here to be able to sniff the
            // connection. What we do is set up two unrelated crypto streams,
            // one for the server, one for the client. We actually act a bit
            // more like a real client or a real server in this particular
            // stage of the connection, because we generate a shared secret
            // as a client and a public key as a server, and liase with each
            // end of the connection without tipping them off to this. After
            // this is done, we wrap the connection in an AesStream and
            // everything works fine.

            // Interact with the server (acting as a client)

            // Generate our shared secret
            var secureRandom = RandomNumberGenerator.Create();
            ServerSharedKey = new byte[16];
            secureRandom.GetBytes(ServerSharedKey);

            // Parse the server public key
            var parser = new AsnKeyParser(packet.PublicKey);
            var key = parser.ParseRSAPublicKey();

            // Encrypt shared secret and verification token
            var crypto = new RSACryptoServiceProvider();
            crypto.ImportParameters(key);
            byte[] encryptedSharedSecret = crypto.Encrypt(ServerSharedKey, false);
            byte[] encryptedVerification = crypto.Encrypt(packet.VerificationToken, false);

            // Create an 0xFC response to give the server
            ServerEncryptionResponse = new EncryptionKeyResponsePacket
            {
                SharedSecret = encryptedSharedSecret,
                VerificationToken = encryptedVerification
            };

            // Authenticate with minecraft.net if need be
            if (packet.ServerId != "-")
            {
                // TODO
            }

            // Interact with the client (acting as a server)

            // Generate verification token
            var verificationToken = new byte[4];
            secureRandom.GetBytes(verificationToken);
            // Encode public key as an ASN X509 certificate
            var encodedKey = AsnKeyBuilder.PublicKeyToX509(ServerKey);

            ClientEncryptionRequest = new EncryptionKeyRequestPacket
            {
                VerificationToken = verificationToken,
                ServerId = "-",
                PublicKey = encodedKey.GetBytes()
            };
            // Send the client our encryption details and await its response
            Console.WriteLine("Sending client encryption request...");
            ClientEncryptionRequest.WritePacket(ClientStream);
            ClientStream.Flush();
        }

        private void FinializeClientEncryption(EncryptionKeyResponsePacket encryptionKeyResponsePacket)
        {
            // Here, we need to prepare everything to enable client<->proxy
            // encryption, but we can't turn it on quite yet.

            // Decrypt shared secret
            ClientSharedKey = CryptoServiceProvider.Decrypt(encryptionKeyResponsePacket.SharedSecret, false);
            Console.WriteLine("Client shared key decrypted.");
            // TODO: The verification token is discarded, we should raise a warning if it isn't correct

            // Send unencrypted response
            Console.WriteLine("Sending server encryption response...");
            ServerEncryptionResponse.WritePacket(ServerStream);
            ServerStream.Flush();

            // We wait for the server to respond, then set up encryption
            // for both sides of the connection.
        }

        private void FinializeServerEncryption(EncryptionKeyResponsePacket encryptionKeyResponsePacket)
        {
            // Here, we have all the details we need to initialize our
            // proxy<->server crypto stream. This happens *after* we have
            // already completed the crypto handshake with the client.

            // Wrap the server stream in a crypto stream
            ServerStream = new MinecraftStream(new AesStream(Server, ServerSharedKey));
            Console.WriteLine("Encrypted server connection established.");

            // Wrap the client stream in a crypto stream
            ClientStream = new MinecraftStream(new AesStream(Client, ClientSharedKey));
            Console.WriteLine("Encrypted client connection established.");

            // Write the response. This is the first encrypted packet
            // sent to the client. The correct response is to send
            // an 0xFC EncryptionKeyResponse with both fields as empty
            // arrays.
            var response = new EncryptionKeyResponsePacket
            {
                SharedSecret = new byte[0],
                VerificationToken = new byte[0]
            };
            response.WritePacket(ClientStream);
            ClientStream.Flush();

            // And now we're done with encryption and everything can
            // continue normally.
        }
    }
}
