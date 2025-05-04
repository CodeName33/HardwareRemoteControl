# IMPORTANT PROJECT UPDATE!

Introducing HadwareRemoteControlCP - crossplatform web-server/GUI. Support Windows/Linux-x64/Linux-arm64 platforms.

Usage:
    
    HadwareRemoteControlCP [parameters]

    Can run without parameters (will ask in console)
    Parameters:
        listcams - list all capture devices in system
        listports - list all serial ports in system
        help - show little help

        -c=NAME - Serial port name (like COM3 for windows or /dev/ttyXXX for linux, can be enumerated with 'listports' parameter)
        -p=INDEX - Capture device index (can be enumerated with 'listcams' parameter)
        -w=PORT_NUMBER - port number for web-server
        -gui - GUI mode
        -q=NUMBER - 0-100 JPEG quality for images (default 60)
        -v:w=VALUE - Preferred image width for capture device
        -v:h=VALUE - Preferred image height for capture device
        -v:f=VALUE - Preferred fps for capture device
        -v:c=VALUE - Preferred 4 letter format for capture device (like MJPG, NV12 etc.)
        -v:b=VALUE - Capture buffer size (frames count, default 4)
        -ffmpeg - ffmpeg mode (only for web-server, linux only). If your capture device suport MJPG stream, this flag will use ffmpeg to copy strem to clients direct. Good for slow devices. But it will not analyse changes and always send full frames (traffic warning). ffmpeg must be installed
Just start it with your settings? for example:

    HadwareRemoteControlCP -c=COM13 -p=2 -w=1800

As web-server port is set to 1800, enter in browser 

    http://[localhost or machine address]:1800/
and have fun.

I've tested it on Windows and arm64 linux (old device Orange Pi Win Plus). 

## Orange Pi Notes

~~As we need to decode capture stream, find changes and reencode to JPEG, Old Orange Pi Can't do it as fast as we want and we have only 5-8 fps + input delay, so I set -v:b=1 to make input delay lower (about 0.5~0.8 second). Also I set -v:c to MJPG and -v:f 30, because by default it choose YUY2 with maximum 5 fps input (my capture device limitation in YUY2)~~
**UPDATE**
For slow devices (linux only) as web-server you can use ffmpeg mode (parameter -ffmpeg). If your capture device supports MJPG stream in this mode stream will be copied directly to clients, without frame decoding. It's very fast but in this mode there are no changes analyzed and always full frames send (traffic warning). You can set lower fps falue (parameter -v:f) for traffic economy. ffmpeg must be installed

# Warning
Never open web-server port to internet directly. It's not safe because there is no authorization and encryption. Use NGINX with HTTPS and Auth for proxy to this service.


# About

This is software part for project HardwareRemoteControl. It physically connects two computers and allow one to control another. Any OS on controlled PC (you can even enter BIOS and reinstall OS), only Windows on controlling PC.

To use it you need:
 - USB-HDMI Video Capture Device to get video from controlled pc (May be just web-camera co capture monitor, depends on how much of a freak you are:)
 - ESP32 S3 Board to emulate mouse and keyboard
![Alt text](./Images/hardware.jpg?raw=true "Image")
 - Optional 2 Relays, if you want to control Reset/Power SW on controlled PC.
![Alt text](./Images/relay.jpg?raw=true "Image")

# How To

1. You need to prepare your ESP32 S3 Device:
 - Install Arduino IDE
 - Setup Arduino IDE to work with ESP32 boards
 - Open **Sources/ESP32S3/USB-Mouse-Keyboard.ino**
 - If you use relays to control Reset/Power SW on controlled PC, solder relays to pins (I've used 17,18 you can change it in code)
 - Compile code and upload to ESP32 S3 Board
2. If you use relays to control Reset/Power SW, solder cables with 2-pin connector on the end to relays
![Alt text](./Images/2pin.png?raw=true "Image")
3. Connect devices to PC using scheme:
![Alt text](./Images/scheme.png?raw=true "Image")
4. If you use relays to control Reset/Power SW, put 2 pin connectors to FPanel on motherboard on controlled PC
![Alt text](./Images/pins.png?raw=true "Image")
5. Download, extract and run on controlling pc **Releases** or compile from source code (.Net, WinForms)
6. Select your video capture device (or webcam) and COM port with ESP32 S3, press connect. You will see picture from controlled PC's monitor and can use mouse and keyboard. It will work without any OS on controlled PC and you can even enter BIOS and install it.

# If you use camera

If you can't use USB-HDMI Video Capture Device and use camera, in image transformation section you can stretch your video to make remote screen to fit window without any borders and this will make your mouse cursor more precise.
![Alt text](./Images/transform.png?raw=true "Image")

# Tests

With USB-HDMI Video Capture Device:

![Alt text](./Images/test.gif?raw=true "Image")

With Web-Camera:

![Alt text](./Images/test2.gif?raw=true "Image")

# 3D Printed Box for ESP32 S3 With/Without Relays

You can print box for your device. Look in **Sources/3D Printed Box** folder. I've made box to use with specific relays, but it's OpenSCAD and you can change any sizes in this model. Look at variables: RelayWidth, RelayLength, RelayHeight and RelaysCount


# License

My project parts is MIT, but it uses modified AForge lib version and it's GPL/LGPL
