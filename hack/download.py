#!/usr/bin/env python3
import serial

port = serial.Serial(
    port = "/dev/ttyUSB0",
    baudrate = 115200,
    bytesize = serial.EIGHTBITS,
    parity = serial.PARITY_NONE,
    stopbits = serial.STOPBITS_ONE,
    timeout = 1,
    xonxoff = 1)

port.write(b'\x7d\x81\xa6\x80\x80\x80\x80\x80\x80')
port.flush()

def parse(buffer):
    spo2 = buffer[0] & 0b0111_1111
    bpm = buffer[1] & 0b0111_1111

    return {'bpm': bpm, 'spo2': spo2}

data = port.readall()
def partition(seq, size):
    return (seq[pos:pos+size] for pos in range(0, len(seq), size))

for outerPacket in partition(data, 8):
    if outerPacket is None:
        exit(0)
    else:
        for innerPacket in partition(outerPacket[2:], 2):
            print(parse(innerPacket))
