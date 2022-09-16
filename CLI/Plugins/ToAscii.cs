using System.Text;

namespace SampleConsoleApp
{
    [PluginDefinition("ascii", "to")]
    public class ToAscii : Plugin
    {
        [PositionalArgument(0)]
        public string? input { get; set; }

        [TaggedArgument("hex", "h")]
        public bool hex { get; set; }

        public ToAscii()
        {
        }

        public override async Task<int> Execute()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            if (hex)
            {
                string output = Convert.ToHexString(bytes);
                for (int i = 2; i <= output.Length; i += 2)
                {
                    output = output.Insert(i, " ");
                    i++;
                }
                OutputWriter.WriteLine(output);
            }
            else
            {
                OutputWriter.WriteLine(String.Join(" ", Encoding.ASCII.GetBytes(input).Select(b => b.ToString())));
            }

            return 0;
        }
    }
}