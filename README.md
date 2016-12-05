# Raspberry IO - *Pi's hardware access from Mono*
The Raspberry Pi's IO Functionality in an easy-to-use API for Mono/.NET/C#
[![NuGet version](https://badge.fury.io/nu/Unosquare.Raspberry.IO.svg)](https://badge.fury.io/nu/Unosquare.Raspberry.IO)
[![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/raspberryio/)](https://github.com/igrigorik/ga-beacon)
*:star:Please star this project if you find it useful!*

## Features

This library enables developers to use the various Raspberry Pi's hardware modules
* ```Pi.Camera``` Provides access to the offical Raspberry Pi Camera module.
* ```Pi.Info``` Provides information on this Raspberry Pi's CPU and form factor.
* ```Pi.Gpio``` Provides access to the Raspberry Pi's GPIO as a collection of GPIO Pins.
* ```Pi.Spi``` Provides access to the 2-channel SPI bus.
* ```Pi.I2c``` Provides access to the functionality of the i2c bus.
* ```Pi.Timing``` Provides access to The PI's Timing and threading API.

_Please note you program needs to be run with ```sudo```. Example ```sudo mono myprogram.exe``` in order to work correctly._

This library depends on the wonderful ```WiringPi``` library avaialble [here](http://wiringpi.com/). You do not need to install this library yourself. The ```Raspberry IO``` assembly will automatically extract the compiled binary of the library in the same path as the entry assembly.

## Installing
TODO

## Running the latest version of Mono
It is recommended that you install the latest available release of Mono because what is available in the Raspbian repo is quite old (3.X). These commands were tested using Raspbian Jessie. The version of Mono that is installed at the time of this writing is:
``` 
Mono JIT compiler version 4.6.2 (Stable 4.6.2.7/08fd525 Mon Nov 14 12:43:54 UTC 2016) 
```

The commands to get Mono installed are the following:
```
sudo apt-get update
sudo apt-get upgrade
sudo apt-get install mono-complete
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update
sudo apt-get dist-upgrade
```

Now, verify your version of Mono by running ```mono --version```. Version 4.6 and above should be good enough.

## The Camera Module
The ```Pi.Camera``` module uses ```raspivid``` and ```raspistill``` to access to camera so they must be installed in order for your program to work propely. ```raspistill``` arguments are specified in an instance of the ```CameraStillSettings``` class, while the ```raspivid``` arguments are specified in an instance of the ```CameraVideoSettings``` class. 

### Capturing Images
The ```Pi.Camera.CaptureImage*``` methods simply return an array of bytes containing the capture image. There are synchronous and asynchronous falvors of these methods so you can use the familiar ```async``` and ```await``` pattern to capture your images. All ```raspistill``` arguments (except for those that control user interaction such as ```-k```) are available via the ```CameraStillSettings```. To start, create a new instance of the ```CameraStillSettings``` class and pass it on to your choice of the ```Pi.Camera.CaptureImage*``` methods. There are shortcut methods available that simply take a JPEG image at the given Width and Height. By default, the shortcut methods set the JPEG quality at 90%.

Example using a shortcut method:
```csharp
static void TestCaptureImage()
{
    var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
    var targetPath = "/home/pi/picture.jpg";
    if (File.Exists(targetPath))
        File.Delete(targetPath);

    File.WriteAllBytes(targetPath, pictureBytes);
    Console.WriteLine($"Took picture -- Byte count: {pictureBytes.Length}");
}
```

Example using a CaptureImage method:
```csharp
// TODO: example code here
```

### Capturing Video
Capturing video streams is somewhat different but it is still very easy to do. The concept behind it is to _Open_ a video stream providing your own callback. When opening the stream ```Raspberry IO``` will spawn a separte thread and will not block the execution of your code, but it will continually call your callback method containing the bytes that are being read from the camera until the _Close_ method is called.

Example of capturing a stream of H.264 video
```csharp
// TODO: Add example here
```

## Obtaining Board and System Information
TODO

## Using the GPIO Pins
TODO

### Pin Information
TODO

### Digital Read and Write
TODO

### Hardware PWM
TODO

### Software PWM
TODO

### Tone Generation
TODO

### Interrupts and Callbacks
TODO

## Using the SPI Bus
TODO

## The I2C Bus
TODO

## Timing and Threading
TODO

## Serial Ports (UART)
Where is the serial port API? Well, it is something we will most likely add in the future. For now, you can simply use the built-in ```SerialPort``` class the .NET framework provides.

## Similar Projects
- <a href="https://github.com/raspberry-sharp/raspberry-sharp-io">Raspberry# IO</a>
- <a href="https://github.com/danriches/WiringPi.Net">WiringPi.Net</a>
- <a href="https://github.com/andycb/PiSharp">PiSharp</a>
