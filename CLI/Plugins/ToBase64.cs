using System.Text;

namespace SampleConsoleApp
{
    [PluginDefinition("base64", "to")]
    class ToBase64 : Plugin
    {
        [PositionalArgument(0)]
        public string? input { get; set; }

        public ToBase64()
        {
        }

        public override async Task<int> Execute()
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
            Console.WriteLine(System.Convert.ToBase64String(plainTextBytes));
            return 0;
        }
    }
}