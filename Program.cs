using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO.Ports;

namespace CMS50DPlusCli
{
  class Program
  {
    static private Command LsDevicesCmd()
    {
      return new Command("ls-devices")
      {
        Description = "List serial devices",
        Handler = CommandHandler.Create(() =>
        {
          Console.Write(String.Join('\n', SerialPort.GetPortNames()));
        })
      };
    }

    static private Command StreamCmd()
    {
      var cmd = new Command("stream")
      {
        Description = "Stream live data to stdout",
        Handler = CommandHandler.Create<string, OutputFormat>(CMS50DPlus.Stream),
      };
      cmd.AddOption(new Option<OutputFormat>("--format", () => OutputFormat.NdJson, "Output format"));
      cmd.AddArgument(new Argument<string>("devicePath"));
      return cmd;
    }

    static private Command DownloadCmd()
    {
      var cmd = new Command("download")
      {
        Description = "Download recorded data. This command may require prior device restart.",
        Handler = CommandHandler.Create<string, OutputFormat>(CMS50DPlus.Download),
      };
      cmd.AddOption(new Option<OutputFormat>("--format", () => OutputFormat.NdJson, "Output format"));
      cmd.AddArgument(new Argument<string>("devicePath"));
      return cmd;
    }

    static int Main(string[] args)
    {
      var cmd = new RootCommand {
                LsDevicesCmd(),
                StreamCmd(),
                DownloadCmd(),
            };

      return cmd.Invoke(args);
    }
  }
}
