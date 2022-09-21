
namespace SampleConsoleApp
{
    [PluginDefinition("cmd", "group")]
    class ExamplePlugin : Plugin
    {
        [PositionalArgument(0)]
        public string? PositionalArg1 { get; set; }

        [PositionalArgument(1)]
        public string? PositionalArg2 { get; set; }

        [TaggedArgument("tagged", "t")]
        public string? Tagged1 { get; set; }

        public ExamplePlugin()
        {
        }

        public override async Task<int> Execute()
        {
            return await Task.FromResult<int>(1);
        }
    }
}