using System;
using System.IO;
using TrollRAT.Server;
using TrollRAT.Utils;

namespace TrollRAT.Actions
{
    public interface GlobalAction
    {
        string getHTML();
    }

    public abstract class GlobalActionServer : IDBase<GlobalActionServer>, GlobalAction
    {
        public virtual string Color => "default";
        public abstract string Title { get; }

        // Returns JavaScript to be executed by the client
        public abstract string execute();

        protected WebServer server;
        public WebServer Server => server;

        public GlobalActionServer(WebServer server)
        {
            this.server = server;
        }

        public virtual string getHTML()
        {
            return string.Format("<button class=\"btn btn-{2} navbar-btn\" onclick=\"globalAction({0});\">{1}</button>",
                id, Title, Color);
        }

        public virtual void writeToStream(BinaryWriter writer) { }
        public virtual void readFromStream(BinaryReader reader) { }
    }

    public abstract class GlobalActionClient : GlobalAction
    {
        public virtual string Color => "default";
        public abstract string Title { get; }
        public abstract string OnClick { get; }

        protected WebServer server;
        public WebServer Server => server;

        public GlobalActionClient(WebServer server)
        {
            this.server = server;
        }

        public virtual string getHTML()
        {
            return string.Format("<button class=\"btn btn-{2} navbar-btn\" onclick=\"{0}\">{1}</button>",
                OnClick, Title, Color);
        }
    }
}
