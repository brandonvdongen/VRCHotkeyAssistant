using Rug.Osc;
using System;
using System.Net;
using System.Threading;

namespace VRCHotkeyAssistant.Data
{
    public static class OSCThreadHandler {

        private static OscReceiver receiver = new OscReceiver(9001);
        private static OscSender sender = new OscSender(IPAddress.Parse("127.0.0.1"), 9000);

        private static Thread thread = new Thread(new ThreadStart(Update));

        public static bool IsConnected { get { return receiver.State == OscSocketState.Connected; } }
        public static bool IsAlive { get { return thread.IsAlive; } }
        public static OscPacket Receive() {
            return receiver.Receive();
        }

        public static void start() {
            thread = new Thread(new ThreadStart(Update));
            receiver.Connect();
            thread.Start();
            OutputLogger.Log("Connected");
        }

        public static void stop() {
            if (IsConnected) {
                receiver.Close();
                OutputLogger.Log("Disconnected");
            }
        }

        private static void Update()
        {
            try
            {
                while (IsConnected)
                {
                    OscPacket packet = receiver.Receive();
                    if (packet == null) continue;
                    OutputLogger.Log($"Received: {packet}");
                    string[] data = packet.ToString().Split(',');
                    string address = data[0];
                    string value = data[1];

                    foreach (var binding in Bindings.All)
                    {
                        if (address == $"/avatar/parameters/{binding.Address}")
                        {
                            binding.TryExecute(value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (IsConnected)
                {
                    OutputLogger.Log("Exception in listen loop");
                    OutputLogger.Log(ex.Message);
                }
            }
        }
    }

}
