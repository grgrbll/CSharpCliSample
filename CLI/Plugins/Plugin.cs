using System.Reflection;
public abstract class Plugin
{
    public TextWriter OutputWriter { get; set; }

    public Plugin()
    {
        OutputWriter = Console.Out;
    }

    public abstract Task<int> Execute();

    public async Task<int> ExecuteWithArgs(IEnumerable<string> args)
    {
        var pluginType = this.GetType();
        Console.WriteLine(pluginType);

        var argsQueue = new Queue<string>(args);

        // Handle positional arguments.
        // Currently, all positional args are required.
        var positionalProperties = new Queue<PropertyInfo>(
                                                pluginType.GetProperties()
                                                .Where(p => p.GetCustomAttribute<PositionalArgumentAttribute>() != null)
                                                .OrderBy(p => p.GetCustomAttribute<PositionalArgumentAttribute>().Index)
                                                );

        if (argsQueue.Count() < positionalProperties.Count())
            throw new ApplicationException("Too few arguments");

        while (positionalProperties.Count() > 0)
        {
            var prop = positionalProperties.Dequeue();
            var value = argsQueue.Dequeue();
            if (value.StartsWith("-"))
                throw new ApplicationException("Unexpected");
            prop.SetValue(this, value);
        }

        // Handle Tagged properties
        // Currently, all tagged properties are optional
        Dictionary<string, PropertyInfo> longNamedProperties = new Dictionary<string, PropertyInfo>();
        Dictionary<string, PropertyInfo> shortNamedProperties = new Dictionary<string, PropertyInfo>();
        {
            var tmp_namedProperties = pluginType.GetProperties().Where(p => p.GetCustomAttribute<TaggedArgumentAttribute>() != null);
            foreach (var prop in tmp_namedProperties)
            {
                var attr = prop.GetCustomAttribute<TaggedArgumentAttribute>();

                if (attr == null)
                    throw new ApplicationException("Unexpected");

                Console.WriteLine("Writing parameter: " + attr.LongName);
                longNamedProperties[attr.LongName] = prop;
                shortNamedProperties[attr.ShortName] = prop;
            }
        }

        while (argsQueue.Count() > 0)
        {
            var arg = argsQueue.Dequeue();

            PropertyInfo prop;
            if (arg.StartsWith("--"))
            {
                var key = arg.Substring(2);

                Console.WriteLine(key);
                if (!longNamedProperties.ContainsKey(key))
                    throw new ApplicationException("Unknown tagged parameter.");
                prop = longNamedProperties[key];
            }
            else if (arg.StartsWith('-'))
            {
                var key = arg.Substring(1);
                if (!shortNamedProperties.ContainsKey(key))
                    throw new ApplicationException("Unknown tagged parameter.");
                prop = shortNamedProperties[key];
            }
            else
            {
                throw new ApplicationException("Expected parameter tag.");
            }

            if (prop.PropertyType == typeof(bool))
            {
                prop.SetValue(this, true);
                Console.WriteLine($"Set Property {arg} = True");
            }
            else
            {
                if (argsQueue.Count() == 0)
                    throw new ApplicationException("Tagged parameter has no value");
                var value = argsQueue.Dequeue();
                prop.SetValue(this, value);
                Console.WriteLine($"Set Property {arg} = {value}");
            }
        }

        return await Execute();
    }
}

class PluginDefinitionAttribute : System.Attribute
{
    public string Name { get; private set; }
    public string Group { get; private set; }
    public PluginDefinitionAttribute(string name, string group)
    {
        Name = name;
        Group = group;
    }
}

class PositionalArgumentAttribute : System.Attribute
{
    public uint Index { get; private set; }
    public PositionalArgumentAttribute(uint Index)
    {
        this.Index = Index;
    }

}

class TaggedArgumentAttribute : System.Attribute
{
    public string LongName { get; private set; }
    public string ShortName { get; private set; }
    public string HelpText { get; private set; }
    public TaggedArgumentAttribute(string LongName, string ShortName, string HelpText = "")
    {
        this.LongName = LongName;
        this.ShortName = ShortName;
        this.HelpText = HelpText;
    }

}