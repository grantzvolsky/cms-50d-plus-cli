using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CMS50DPlusCli
{
  class LivePacket
  {
    byte[] _data;

    public LivePacket(byte[] data)
    {
      if (data.Length != 9) throw new ArgumentException($"Expected 9 bytes, got {data.Length}");
      var syncBitsValid = data.Skip(1).All(b => (b & 0b1000_0000) > 0);
      if (!syncBitsValid) throw new ArgumentException("Invalid synchronization bits");

      _data = data;
    }

    public byte[] Value { get => _data; }

    public LiveRecord Record(long timestamp)
    {
      return new LiveRecord
      {
        Timestamp = timestamp,
        SignalStrength = (_data[2] & 0b0000_1111),
        Unknown1 = (_data[2] & 0b0001_0000) > 0,
        Unknown2 = (_data[2] & 0b0010_0000) > 0,
        PulseBeep = (_data[2] & 0b0100_0000) > 0,

        RelativePressure = (_data[3] & 0b0111_1111),

        Unknown3 = (_data[4] & 0b0000_1111),
        Unknown4 = (_data[4] & 0b0001_0000) > 0,
        Unknown5 = (_data[4] & 0b0010_0000) > 0,

        Bpm = ((_data[4] << 1) & 0b0100_0000) | (_data[5] & 0b0111_1111),
        SpO2 = _data[6] & 0b0111_1111
      };
    }
  }

  class LiveRecord
  {
    public long Timestamp { get; set; }
    public int SignalStrength { get; set; }
    public bool Unknown1 { get; set; }
    public bool Unknown2 { get; set; }
    public bool PulseBeep { get; set; }
    public int RelativePressure { get; set; }
    public int Unknown3 { get; set; } // Is this 'RelativeSpO2'?
    public bool Unknown4 { get; set; }
    public bool Unknown5 { get; set; }
    public int Bpm { get; set; }
    public int SpO2 { get; set; }
    private bool SyncBitsValid { get; set; }
  }

  static class LivePacketFormatter
  {
    public static Func<LivePacket, byte[]> Raw = (p) => p.Value;

    public static Func<LivePacket, byte[]> Hex = (p) =>
    {
      return UTF8Encoding.UTF8.GetBytes(BitConverter.ToString(p.Value) + "\n");
    };

    public static Func<LivePacket, byte[]> NdJson = (p) =>
    {
      var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      return JsonSerializer
          .SerializeToUtf8Bytes<LiveRecord>(p.Record(ts))
          .Concat(UTF8Encoding.UTF8.GetBytes("\n")).ToArray();
    };
  }
}
