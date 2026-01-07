using Server;
using Server.Mobiles;

namespace Server.Engines.ConPVP
{
    public static class DuelPitSystem
    {
        public static void Configure()
        {
            // Initialize event handlers
            DuelPitEventHandler.Initialize();

            // Register the addon type for the [add command
            // This is handled automatically by the [Constructible] attribute

            LogHelper.Log("Duel Pit System initialized successfully!");
        }
    }

    public static class LogHelper
    {
        public static void Log(string message)
        {
            // Simple logging - in a real implementation, you might want to use a proper logging system
            System.Console.WriteLine($"[DuelPitSystem] {message}");
        }
    }
}
