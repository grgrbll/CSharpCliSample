using SampleConsoleApp;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Tests;

public class UnitTest1
{
    [Fact]
    public async void AsciiPlugin()
    {
        TextWriter writer = new StringWriter();
        string input = "abcdefg";

        var console = ConsoleRunner.Run($"to ascii {input}");

        var asciiCodes = console.GetOutput().Split(' ').Select(s => Byte.Parse(s));
        Assert.Equal(asciiCodes.Count(), input.Length);

        var expectedBytes = Encoding.ASCII.GetBytes(input);

        Assert.Empty(expectedBytes.Except(asciiCodes));
        Assert.Empty(asciiCodes.Except(expectedBytes));
    }

    [Theory]
    [InlineData("abcdefg")]
    [InlineData("a")]
    [InlineData("123456789")]
    public async void Base64Plugin(string input)
    {
        TextWriter writer = new StringWriter();

        var console = ConsoleRunner.Run($"to base64 {input}");

        var asciiCodes = console.GetOutput().Split(' ').Select(s => Byte.Parse(s));
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
        var bytes = System.Convert.FromBase64String(console.GetOutput());
        var output = System.Text.Encoding.UTF8.GetString(bytes);
        Assert.Equal(input, output);
    }
}