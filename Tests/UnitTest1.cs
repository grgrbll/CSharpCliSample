using SampleConsoleApp;
using System.Text;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public async void Test1()
    {
        TextWriter writer = new StringWriter();

        var plugin = new ToAscii();
        plugin.OutputWriter = writer;

        string input = "abcdefg";
        await plugin.ExecuteWithArgs(new List<string>() { input });

        var asciiCodes = writer.ToString().Split(' ').Select(s => Byte.Parse(s));
        Assert.Equal(asciiCodes.Count(), input.Length);

        var expectedBytes = Encoding.ASCII.GetBytes(input);

        Assert.Empty(expectedBytes.Except(asciiCodes));
        Assert.Empty(asciiCodes.Except(expectedBytes));
        Console.WriteLine(expectedBytes.Count());
    }
}