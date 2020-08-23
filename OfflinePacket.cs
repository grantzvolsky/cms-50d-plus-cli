using System;
using System.Text;
using System.Text.Json;

namespace CMS50DPlusCli
{
  class OfflinePacket
  {
    byte[] _data;

    public OfflinePacket(byte[] data)
    {
      if (data.Length != 8) throw new ArgumentException($"Expected 8 bytes, got {data.Length}");
      _data = data;
    }

    public byte[] Value { get => _data; }

    public ValueTuple<OfflineRecord, OfflineRecord> Records()
    {
      return (
        new OfflineRecord
        {
          Bpm = _data[3] & 0b0111_1111,
          SpO2 = _data[4] & 0b0111_1111
        },
        new OfflineRecord
        {
          Bpm = _data[5] & 0b0111_1111,
          SpO2 = _data[6] & 0b0111_1111
        }
      );
    }
  }

  class OfflineRecord
  {
    public int Bpm { get; set; }
    public int SpO2 { get; set; }
  }

  class OfflinePacketFormatters
  {
    public static Func<OfflinePacket, byte[]> Raw = (p) => p.Value;

    public static Func<OfflinePacket, byte[]> Hex = (p) =>
    {
      return UTF8Encoding.UTF8.GetBytes(BitConverter.ToString(p.Value) + "\n");
    };

    public static Func<OfflinePacket, byte[]> NdJson = (packet) =>
    {
      var records = packet.Records();
      var r1 = JsonSerializer.Serialize<OfflineRecord>(records.Item1);
      var r2 = JsonSerializer.Serialize<OfflineRecord>(records.Item2);
      return UTF8Encoding.UTF8.GetBytes($"{r1}\n{r2}\n");
    };
  }
}
