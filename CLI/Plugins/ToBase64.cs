using System.Text;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using System;

namespace SampleConsoleApp
{
    [PluginDefinition("base64", "to")]
    class ToBase64 : Plugin
    {
        [RequestService]
        public TextWriter? Output { get; set; }

        [RequestService]
        public Microsoft.Extensions.Logging.ILoggerFactory? LoggerFactory { get; set; }

        [PositionalArgument(0)]
        public string? input { get; set; }

        public ToBase64()
        {   
        }

        public override async Task Execute()
        {
	        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var output = System.Convert.ToBase64String(plainTextBytes);
	        Output?.WriteLine(output);
	        Log.Information("Converted {input} to {output}", input, output);
	        return;
        }
    }
}