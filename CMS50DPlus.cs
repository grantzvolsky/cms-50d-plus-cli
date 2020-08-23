using System;
using System.IO.Ports;

namespace CMS50DPlusCli
{
  static class DeviceProtocol
  {
    public static byte[] StartStream = new byte[] { 0x7d, 0x81, 0xa1, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 };
    public static byte[] StartDownload = new byte[] { 0x7d, 0x81, 0xa6, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 };
  }

  enum OutputFormat
  {
    Hex,
    NdJson,
    Raw,
  }

  class CMS50DPlus
  {
    string _devicePath;

    public CMS50DPlus(string devicePath)
    {
      _devicePath = devicePath;
    }

    public static void Stream(string devicePath, OutputFormat format)
    {
      var formatter = format switch
      {
        OutputFormat.Hex => LivePacketFormatter.Hex,
        OutputFormat.NdJson => LivePacketFormatter.NdJson,
        OutputFormat.Raw => LivePacketFormatter.Raw,
        _ => throw new ArgumentException($"OutputFormat {format} not supported")
      };

      LiveStream(devicePath, 10_000, formatter);
    }

    public static void Download(string devicePath, OutputFormat format)
    {
      var formatter = format switch
      {
        OutputFormat.Hex => OfflinePacketFormatters.Hex,
        OutputFormat.NdJson => OfflinePacketFormatters.NdJson,
        OutputFormat.Raw => OfflinePacketFormatters.Raw,
        _ => throw new ArgumentException($"OutputFormat {format} not supported")
      };

      Download(devicePath, 1_000, formatter);
    }

    private static void Download(string devicePath, int timeout, Func<OfflinePacket, byte[]> format)
    {
      using var port = new SerialPort(devicePath, 115200, Parity.None, 8, StopBits.One)
      {
        ReadTimeout = timeout,
        WriteTimeout = 1_000,
        Handshake = Handshake.XOnXOff
      };
      port.Open();
      port.Write(DeviceProtocol.StartDownload, 0, 8);
      var packetBuffer = new byte[8];
      while (true)
      {
        try
        {
          port.Read(packetBuffer, 0, 8);
          Console.OpenStandardOutput().Write(format(new OfflinePacket(packetBuffer)));
        }
        catch (ObjectDisposedException)
        {
          port.Close();
          Environment.Exit(1);
        }
        catch (TimeoutException)
        {
          port.Close();
          Environment.Exit(2);
        }
      }
    }

    private static void LiveStream(string devicePath, int timeout, Func<LivePacket, byte[]> format)
    {
      using var port = new SerialPort(devicePath, 115200, Parity.None, 8, StopBits.One)
      {
        ReadTimeout = timeout,
        WriteTimeout = 1_000,
        Handshake = Handshake.XOnXOff
      };
      port.Open();
      port.Write(DeviceProtocol.StartStream, 0, 9);
      var packetBuffer = new byte[9];
      while (true)
      {
        try
        {
          port.Read(packetBuffer, 0, 9);
          Console.OpenStandardOutput().Write(format(new LivePacket(packetBuffer)));
        }
        catch (ObjectDisposedException)
        {
          port.Close();
          Environment.Exit(1);
        }
        catch (TimeoutException)
        {
          port.Close();
          Environment.Exit(2);
        }
      }
    }
  }
}
