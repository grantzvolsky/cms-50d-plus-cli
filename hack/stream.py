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

port.write(b'\x7d\x81\xa1\x80\x80\x80\x80\x80\x80')
port.flush()

def parse(buffer):
    #syncBitsValid = all([(byte & 0b1000_0000) > 0 for byte in data[1:]])
    #if not syncBitsValid return None

    signalStrength = buffer[2] & 0b00001111
    #unknown1            = (buffer[2] & 0b0001_0000) > 0
    #unknown2            = (buffer[2] & 0b0010_0000) > 0
    #pulseBeep           = (buffer[2] & 0b0100_0000) > 0

    #relativePressure    = (buffer[3] & 0b0111_1111)

    #unknown3            = (buffer[4] & 0b0000_1111)
    #unknown4            = (buffer[4] & 0b0001_0000) > 0
    #unknown5            = (buffer[4] & 0b0010_0000) > 0

    bpm = ((buffer[4] << 1) & 0b0100_0000) | (buffer[5] & 0b0111_1111)
    spo2 = buffer[6] & 0b0111_1111

    return {'signalStrength': signalStrength, 'bpm': bpm, 'spo2': spo2}

while True:
    data = port.read(size=9)
    if data is None:
        exit(0)
    else:
        print(data.hex())
        #print(parse(data))
