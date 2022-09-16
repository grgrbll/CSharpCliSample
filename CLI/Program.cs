// See https://aka.ms/new-console-template for more information
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SampleConsoleApp
{
    class SampleConsoleApp
    {
        static IEnumerable<Type> Plugins = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(PluginDefinitionAttribute)));

        static void PrintHelp()
        {

        }

        static void PrintGroupHelp(string groupName)
        {

        }

        static async Task<int> ExecutePlugin(Type pluginType, IEnumerable<string> args)
        {
            Plugin? plugin = Activator.CreateInstance(pluginType) as Plugin;
            if (plugin == null)
                throw new ApplicationException("Bad plugin type. Must inherit from Plugin.");
            return await plugin.ExecuteWithArgs(args);
        }

        static async Task<int> Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintHelp();
                return 1;
            }

            string group = args[0];

            if (args.Length < 2)
            {
                PrintGroupHelp(args[1]);
                return 1;
            }

            string cmd = args[1];

            foreach (Type pluginType in Plugins)
            {
                var pluginDefinition = pluginType.GetCustomAttribute<PluginDefinitionAttribute>();

                if (pluginDefinition == null)
                    throw new ApplicationException("Bad plugin type. Must inherit from Plugin.");

                if (pluginDefinition.Group == group && pluginDefinition.Name == cmd)
                    return await ExecutePlugin(pluginType, args.Skip(2));
            }

            return 0;
        }
    }
}