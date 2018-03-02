using DiscordRpcNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyun.game.Utils
{
    public class Discord_Handler
    {
        public static bool Continue = true;
        DiscordRpc.RichPresence Status = new DiscordRpc.RichPresence();

        public Discord_Handler()
        {
            var Callbacks = new DiscordRpc.EventHandlers();
            Callbacks.readyCallback = ReadyCallback;
            Callbacks.disconnectedCallback += DisconnectedCallback;
            Callbacks.errorCallback += ErrorCallback;

            Task.Run((Action)delegate
            {
                // Connect (Using the 'send-presence.c' id for demo MAKE_YOUR_OWN)
                DiscordRpc.Initialize("411953384032174082", ref Callbacks, true, null);
                

                // Register status
               
                Status.details = "Loading";
                Status.smallImageKey = "idle";

                // Update status
                DiscordRpc.UpdatePresence(ref Status);

                while (Continue)
                {
                    // Callbacks
                    DiscordRpc.RunCallbacks();
                    System.Threading.Thread.Sleep(100);
                }
            });

            /*
            while (Continue)
            {
                System.Threading.Thread.Sleep(100);
            }*/

            // Clean up
           
        }

        private static void ReadyCallback()
        {
            Logger.Instance.Info("Discord::Ready()");
        }

        private static void DisconnectedCallback(int errorCode, string message)
        {
            Logger.Instance.Info("Discord::Disconnect({0}, {1})", errorCode, message);
            
        }

        private static void ErrorCallback(int errorCode, string message)
        {
            Logger.Instance.Info("Discord::Error({0}, {1})", errorCode, message);
        }

        public void SetState(string state, string details = "", string asset = "idle_large", string small_asset = "")
        {
            // Status = new DiscordRpc.RichPresence();
            Status.startTimestamp = 0;
            Status.details = details;
            Status.state = state;

            Status.smallImageKey = small_asset;
            Status.largeImageKey = asset;

            DiscordRpc.UpdatePresence(ref Status);
        }

        public void Shutdown()
        {
            Continue = false;
            DiscordRpc.Shutdown();
        }
    }
}
