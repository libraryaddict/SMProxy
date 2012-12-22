# SMProxy Plugins

When SMProxy starts up, it will find all *.dll files in the working directory and attempt
to load them into the AppDomain. Then, all classes that inherit from IPlugin will be loaded.
IPlugin is very simple, it just provides you means to hook up event handlers.

## How to use SMProxy's API

There are a few things you can do here:

* **Override a packet**: You can add new packets or override the default implementation of any
  packet by using `PacketReader.OverridePacket(Type)` and providing a type that inherits from
  `IPacket`.
* **Intercept a packet**: You can inspect packets as they come in, or stop them from being
  proxied by registering for the `Proxy.IncomingPacket` event. Set
  `IncomingPacketEventArgs.Handled` to `true` to prevent the packet from being proxied.
* **Handling command-line arguments**: If you wish to recieve input from the command-line, use
  the `Program.UnrecognizedArgument` event. Set `UnrecognizedArgumentEventArgs.Handled` to
  `true` if you handled this argument. `UnrecognizedArgumentEventArgs.Index` will be persisted
  if you eat up more arguments than the unrecognized one.
* **Inserting text into the log**: You can simply use `Proxy.Log` to log packets or text.
* **Writing directly to client/server**: `Proxy.ClientStream` and `Proxy.ServerStream`.

## Examples

Stop chat messages from being sent:

    public class StopChat : IPlugin
    {
        public void GlobalInitialize()
        {
        }
        
        public void SessionInitialize(Proxy proxy)
        {
            proxy.IncomingPacket += (s, e) =>
            {
                if (e.Packet is ChatMessagePacket)
                {
                    e.Handled = true;
                    proxy.Log.Write ("Stopped chat packet: " + ((ChatMessagePacket)e.Packet).Message);
                }
            };
        }
    }

Override HeldItemChangePacket:

    public class OverridePacket : IPlugin
    {
        public void GlobalInitialize()
        {
            PacketReader.OverridePacket(typeof(CustomHeldItemChangePacket));
        }
        
        public void SessionInitialize(Proxy proxy)
        {
        }
    }
    
    public struct CustomHeldItemChangePacket : IPacket
    {
        public short SlotIndex;
        public byte SomethingElse;

        public byte Id { get { return 0x10; } }

        public void ReadPacket(MinecraftStream stream)
        {
            SlotIndex = stream.ReadInt16();
            SomethingElse = stream.ReadUInt8();
        }

        public void WritePacket(MinecraftStream stream)
        {
            stream.WriteUInt8(Id);
            stream.WriteInt16(SlotIndex);
            stream.WriteUInt8(SomethingElse);
        }
    }

Handle a new command line argument:

    public class HelloWorld : IPlugin
    {
        public void GlobalInitialize()
        {
            Program.UnrecognizedArgument += (s, e) =>
            {
                if (e.Argument == "--hello-world")
                {
                    Console.WriteLine("Hello, world!");
                    e.Handled = true;
                }
            };
        }
        
        public void SessionInitialize(Proxy proxy)
        {
        }
    }

## What else?

I think this covers pretty much everything you'd want to make a plugin for, but if there's something
else you'd like, either make an issue, email me, or submit a pull request, and I'll see if I can
accomodate it. If there's enough interest, I might put together some sort of centralized place to find
plugins.