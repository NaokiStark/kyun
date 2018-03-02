using DiscordRpcNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDemo
{
    class Program
    {
        private static bool Continue = true;

        static void Main(string[] args)
        {
            // Register callbacks
            var Callbacks = new DiscordRpc.EventHandlers();
            Callbacks.readyCallback = ReadyCallback;
            Callbacks.disconnectedCallback += DisconnectedCallback;
            Callbacks.errorCallback += ErrorCallback;

            Task.Run((Action)delegate
            {
                // Connect (Using the 'send-presence.c' id for demo MAKE_YOUR_OWN)
                DiscordRpc.Initialize("345229890980937739", ref Callbacks, true, null);

                // Register status
                var Status = new DiscordRpc.RichPresence();
                Status.details = "Doing a thing!";

                // Update status
                DiscordRpc.UpdatePresence(ref Status);

                while (Continue)
                {
                    // Callbacks
                    DiscordRpc.RunCallbacks();
                    System.Threading.Thread.Sleep(100);
                }
            });

            while (Continue)
            {
                System.Threading.Thread.Sleep(100);
            }

            // Clean up
            DiscordRpc.Shutdown();
        }

        private static void ReadyCallback()
        {
            Console.WriteLine("Discord::Ready()");
        }

        private static void DisconnectedCallback(int errorCode, string message)
        {
            Console.WriteLine("Discord::Disconnect({0}, {1})", errorCode, message);
            Continue = false;
        }

        private static void ErrorCallback(int errorCode, string message)
        {
            Console.WriteLine("Discord::Error({0}, {1})", errorCode, message);
        }
    }
}
