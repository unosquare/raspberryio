[![NuGet](https://img.shields.io/nuget/dt/Unosquare.Raspberry.IO.svg)](https://www.nuget.org/packages/Unosquare.Raspberry.IO)
[![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/raspberryio/)](https://github.com/igrigorik/ga-beacon)
[![Build status](https://ci.appveyor.com/api/projects/status/ufr4k5y96phtafqj/branch/master?svg=true)](https://ci.appveyor.com/project/geoperez/raspberryio/branch/master)
[![Build Status](https://travis-ci.org/unosquare/raspberryio.svg?branch=master)](https://travis-ci.org/unosquare/raspberryio)

# <img src="https://github.com/unosquare/raspberryio/raw/master/logos/raspberryio-logo-32.png"></img> RaspberryIO - *Pi's hardware access from .NET*

:star: *Please star this project if you find it useful!*

The Raspberry Pi's IO Functionality in an easy-to-use API for .NET (Mono/.NET Core). Our mission is to make .NET a first-class citizen in the Python-centric community of Raspberry Pi developers.

 Table of contents
=================
  * [Features](#features)
    * [Peripherals](#peripherals)
  * [Installation](#installation)
  * [Usage](#usage)
  * [Running the latest version of Mono](#running-the-latest-version-of-mono)
    * [For Debian Wheezy](#for-debian-wheezy)
    * [For Debian Stretch](#for-debian-stretch)
  * [Running .NET Core 2](#running-net-core-2)
    * [Run the app on the raspberry](#run-the-app-on-the-raspberry)
  * [The Camera Module](#the-camera-module)
    * [Capturing Images](#capturing-images)
    * [Capturing Video](#capturing-video)
  * [Obtaining Board and System Information](#obtaining-board-and-system-information)
  * [Using the GPIO Pins](#using-the-gpio-pins)
    * [Pin Information](#pin-information)
    * [Digital Read and Write](#digital-read-and-write)
    * [Analog (Level) Read and Write](#analog-level-read-and-write)
    * [Hardware PWM](#hardware-pwm)
    * [Software PWM](#software-pwm)
    * [Tone Generation](#tone-generation)
    * [Interrupts and Callbacks](#interrupts-and-callbacks)
  * [Using the SPI Bus](#using-the-spi-bus)
  * [I2C to connect ICs](#i2c-to-connect-ics)
  * [Timing and Threading](#timing-and-threading)
  * [Serial Ports (UART)](#serial-ports-uart)
  * [Abstraction Package](#abstraction-package)
  * [Handy Notes](#handy-notes)
  * [Similar Projects](#similar-projects)
  
## Features

This library enables developers to use the various Raspberry Pi's hardware modules:

* ```Pi.Camera``` Provides access to the official Raspberry Pi Camera module.
* ```Pi.Info``` Provides information on this Raspberry Pi's CPU and form factor.
* ```Pi.Gpio``` Provides access to the Raspberry Pi's GPIO as a collection of GPIO Pins.
* ```Pi.Spi``` Provides access to the 2-channel SPI bus.
* ```Pi.I2c``` Provides access to the functionality of the i2c bus.
* ```Pi.Timing``` Provides access to The PI's Timing and threading API.

The default low-level provider is the wonderful ```WiringPi``` library available [here](http://wiringpi.com/). You do not need to install this library yourself. The ```Unosquare.WiringPi``` assembly will automatically extract the compiled binary of the library in the same path as the entry assembly.

### Peripherals

We offer an additional package with helpful classes to use peripherals, many of them are from pull requests from our contributors. The current set of peripherals supported are:

* Infrared Sensor HX-1838
* Led Strip APA-102C
* NFC/RFID Controller MFRC-522
* Temperature Sensor AM-2302
* Generic Button

## Installation

Install basic Raspberry.IO package:
[![NuGet version](https://badge.fury.io/nu/Unosquare.Raspberry.IO.svg)](https://badge.fury.io/nu/Unosquare.Raspberry.IO)

```
PM> Install-Package Unosquare.Raspberry.IO
```

Install Raspberry.IO Peripherals package:
[![NuGet version](https://badge.fury.io/nu/Unosquare.RaspberryIO.Peripherals.svg)](https://badge.fury.io/nu/Unosquare.RaspberryIO.Peripherals)
```
PM> Install-Package Unosquare.RaspberryIO.Peripherals
```

## Usage

Before using **Pi** class it is necessary to initialize it with the specific bootstrapping class implementation (for WiringPi).

```csharp
Pi.Init<BootstrapWiringPi>();
```

## Running the latest version of Mono
It is recommended that you install the latest available release of Mono because what is available in the Raspbian repo is quite old (3.X). These commands were tested using Raspbian Jessie. The version of Mono that is installed at the time of this writing is:
``` 
Mono JIT compiler version 5.4.1.6 (tarball Wed Nov  8 21:42:16 UTC 2017)
```

The commands to get Mono installed are the following:

### For Debian Wheezy

```
sudo apt-get update
sudo apt-get upgrade
sudo apt-get install mono-complete
sudo apt-get install dirmngr
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update
sudo apt-get dist-upgrade
```

### For Debian Stretch

```
sudo apt-get update
sudo apt-get upgrade
sudo apt-get install mono-complete
sudo apt-get install dirmngr
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian stretch main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update
sudo apt-get dist-upgrade
```

Now, verify your version of Mono by running ```mono --version```. Version 4.6 and above should be good enough.

## Running .NET Core 2.2

This project can also run in .NET Core 2.2. To install please execute the following commands:

```
$ sudo apt-get -y update
$ sudo apt-get -y install libunwind8 gettext
$ wget https://download.visualstudio.microsoft.com/download/pr/e64d0771-52f3-444c-b174-8be5923ca6da/e0d7f36a0017162f5ff7a81b919ef434/dotnet-runtime-2.2.1-linux-arm.tar.gz
$ sudo mkdir /opt/dotnet
$ sudo tar -xvf dotnet-runtime-2.2.1-linux-arm.tar.gz -C /opt/dotnet/
$ sudo ln -s /opt/dotnet/dotnet /usr/local/bin
$ dotnet --info
```

### Run the app on the raspberry


- You need to publish the project and you can accomplish this by using [dotnet-sshdeploy](https://github.com/unosquare/sshdeploy) but first, you must edit these properties inside the Playground's csproj file in order to establish an ssh connection with your raspberry
``` xml
<SshDeployHost>172.16.17.54</SshDeployHost>
<SshDeployTargetPath>/home/pi/Playground</SshDeployTargetPath>
<SshDeployUsername>pi</SshDeployUsername>
<SshDeployPassword>raspberry</SshDeployPassword>
```

- Install dotnet-sshdeploy as a global tool by running 
```
$ dotnet tool install -g dotnet-sshdeploy
```

- Execute `dotnet-sshdeploy push` in the same folder where Unosquare.RaspberryIO.Playground.csproj resides and if everything executes correctly you should see an output like this:
```
SSH Deployment Tool [Version 0.3.1.0]
(c)2015 - 2017 Unosquare SA de CV. All Rights Reserved.
For additional help, please visit https://github.com/unosquare/sshdeploy
Deploying...
    Configuration   Debug
    Framework       net46
    Source Path     C:\raspberryio\src\Unosquare.RaspberryIO.Playground\bin\Debug\net46\publish
    Excluded Files  .ready|.vshost.exe|.vshost.exe.config
    Target Address  192.16.17.54:22
    Username        pi
    Target Path     /home/pi/Playground
    Clean Target    NO
    Pre Deployment  
    Post Deployment 
Connecting to host 192.16.17.54:22 via SSH.
Connecting to host 192.16.17.54:22 via SFTP.

    Target Path '/home/pi/Playground' does not exist. -- Will attempt to create.
    Target Path '/home/pi/Playground' created successfully.
    Cleaning Target Path '/home/pi/Playground'

    Deploying 8 files.
    Finished deployment in 1.25 seconds.
Completed.
```
* **The default TargetFramework is** `net46` **but you can change this by either modifying the RuntimeIdentifier property inside the csproj file or supplying it as a parameter like this**`dotnet-sshdeploy push -f netcoreapp2.0`. **More information about dotnet-sshdeploy see [this](https://github.com/unosquare/sshdeploy)**
- Give permissions to run the project

```
ubuntu@ubuntu:~/publish$ sudo chmod u+x *
```

- Run the project

```
ubuntu@ubuntu:~/publish$ ./Unosquare.RaspberryIO.Playground
```

## The Camera Module
The ```Pi.Camera``` module uses ```raspivid``` and ```raspistill``` to access the camera so they must be installed in order for your program to work properly. ```raspistill``` arguments are specified in an instance of the ```CameraStillSettings``` class, while the ```raspivid``` arguments are specified in an instance of the ```CameraVideoSettings``` class. 

### Capturing Images
The ```Pi.Camera.CaptureImage*``` methods simply return an array of bytes containing the captured image. There are synchronous and asynchronous falvors of these methods so you can use the familiar ```async``` and ```await``` pattern to capture your images. All ```raspistill``` arguments (except for those that control user interaction such as ```-k```) are available via the ```CameraStillSettings```. To start, create a new instance of the ```CameraStillSettings``` class and pass it on to your choice of the ```Pi.Camera.CaptureImage*``` methods. There are shortcut methods available that simply take a JPEG image at the given Width and Height. By default, the shortcut methods set the JPEG quality at 90%.

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
Capturing video streams is somewhat different but it is still very easy to do. The concept behind it is to _Open_ a video stream providing your own callback. When opening the stream ```Raspberry IO``` will spawn a separate thread and will not block the execution of your code, but it will continually call your callback method containing the bytes that are being read from the camera until the _Close_ method is called or until the timeout is reached.

Example of capturing a stream of H.264 video
```csharp
static void TestCaptureVideo()
{
    // Setup our working variables
    var videoByteCount = 0;
    var videoEventCount = 0;
    var startTime = DateTime.UtcNow;

    // Configure video settings
    var videoSettings = new CameraVideoSettings()
    {
        CaptureTimeoutMilliseconds = 0,
        CaptureDisplayPreview = false,
        ImageFlipVertically = true,
        CaptureExposure = CameraExposureMode.Night,
        CaptureWidth = 1920,
        CaptureHeight = 1080
    };

    try
    {
        // Start the video recording
        Pi.Camera.OpenVideoStream(videoSettings,
            onDataCallback: (data) => { videoByteCount += data.Length; videoEventCount++; },
            onExitCallback: null);

        // Wait for user interaction
        startTime = DateTime.UtcNow;
        Console.WriteLine("Press any key to stop reading the video stream . . .");
        Console.ReadKey(true);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ex.GetType()}: {ex.Message}");
    }
    finally
    {
        // Always close the video stream to ensure raspivid quits
        Pi.Camera.CloseVideoStream();

        // Output the stats
        var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000");
        var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000");
        Console.WriteLine($"Capture Stopped. Received {megaBytesReceived} Mbytes in {videoEventCount} callbacks in {recordedSeconds} seconds");
    }            
}
```

## Obtaining Board and System Information
```RaspberryIO``` contains useful utilities to obtain information about the board it is running on. You can simply call the ```Pi.Info.ToString()``` method to obtain a dump of all system properties as a single ```string```, or you can use the individual properties such as Installed RAM, Processor Count, Raspberry Pi Version, Serial Number, etc. There's not a lot more to this.
Please note ```Pi.Info``` depends on ```Wiring Pi```, and the ```/proc/cpuinfo``` and ```/proc/meminfo``` files.

## Using the GPIO Pins
Pin reference for the B plus (B+) - Header P1

| BCM | wPi | Name    | Mode | V   | L      | R      | V   | Mode | Name    | wPi | BCM |
| --- | --- | ------- | ---- | --- | ------ | ------ | --- | ---- | ------- | --- | --- |
|     |     |    3.3v |      |     | **01** | **02** |     |      | 5v      |     |     |
|   2 |   8 |   SDA.1 | ALT0 | 1   | **03** | **04** |     |      | 5V      |     |     |
|   3 |   9 |   SCL.1 | ALT0 | 1   | **05** | **06** |     |      | 0v      |     |     |
|   4 |   7 | GPIO. 7 |   IN | 1   | **07** | **08** | 1   | ALT0 | TxD     | 15  | 14  |
|     |     |      0v |      |     | **09** | **10** | 1   | ALT0 | RxD     | 16  | 15  |
|  17 |   0 | GPIO. 0 |   IN | 0   | **11** | **12** | 0   | IN   | GPIO. 1 | 1   | 18  |
|  27 |   2 | GPIO. 2 |   IN | 0   | **13** | **14** |     |      | 0v      |     |     |
|  22 |   3 | GPIO. 3 |   IN | 0   | **15** | **16** | 0   | IN   | GPIO. 4 | 4   | 23  |
|     |     |    3.3v |      |     | **17** | **18** | 0   | IN   | GPIO. 5 | 5   | 24  |
|  10 |  12 |    MOSI |   IN | 0   | **19** | **20** |     |      | 0v      |     |     |
|   9 |  13 |    MISO |   IN | 0   | **21** | **22** | 0   | IN   | GPIO. 6 | 6   | 25  |
|  11 |  14 |    SCLK |   IN | 0   | **23** | **24** | 1   | IN   | CE0     | 10  | 8   |
|     |     |      0v |      |     | **25** | **26** | 1   | IN   | CE1     | 11  | 7   |
|   0 |  30 |   SDA.0 |   IN | 1   | **27** | **28** | 1   | IN   | SCL.0   | 31  | 1   |
|   5 |  21 | GPIO.21 |   IN | 1   | **29** | **30** |     |      | 0v      |     |     |
|   6 |  22 | GPIO.22 |   IN | 1   | **31** | **32** | 0   | IN   | GPIO.26 | 26  | 12  |
|  13 |  23 | GPIO.23 |   IN | 0   | **33** | **34** |     |      | 0v      |     |     |
|  19 |  24 | GPIO.24 |   IN | 0   | **35** | **36** | 0   | IN   | GPIO.27 | 27  | 16  |
|  26 |  25 | GPIO.25 |   IN | 0   | **37** | **38** | 0   | IN   | GPIO.28 | 28  | 20  |
|     |     |      0v |      |     | **39** | **40** | 0   | IN   | GPIO.29 | 29  | 21  |

But wait for a second, Where are Wiring Pi (wPi) pins 17 through 20? The above diagram shows the pins of GPIO Header P1. There is an additional GPIO header on the Pi called P5. [More info available here](http://www.raspberrypi-spy.co.uk/2012/09/raspberry-pi-p5-header/)

In order to access the pins, use ```Pi.Gpio```. The pins can have multiple behaviors and fortunately ```Pi.Gpio``` can be iterated, addressed by index, addressed by Wiring Pi pin number and provides the pins as publicly accessible properties.

Here is an example of addressing the pins in all the various ways:
```csharp
public static void TestLedBlinking()
{
    // Get a reference to the pin you need to use.
    // All 3 methods below are exactly equivalent
    var blinkingPin = Pi.Gpio[0];
    blinkingPin = Pi.Gpio[WiringPiPin.Pin00];
    blinkingPin = Pi.Gpio.Pin00;

    // Configure the pin as an output
    blinkingPin.PinMode = GpioPinDriveMode.Output;

    // perform writes to the pin by toggling the isOn variable
    var isOn = false;
    for (var i = 0; i < 20; i++)
    {
        isOn = !isOn;
        blinkingPin.Write(isOn);
        System.Threading.Thread.Sleep(500);
    }
}
```

### Pin Information
All pins have handy properties and methods that you can use to drive them. For example, you can examine the ```Capabilities``` property to find out which features are available on the pin. You can also use the ```PinMode``` property to get or set the operating mode of the pin. Please note that the value of the ```PinMode``` property is by default set to _Input_ and it will return the last mode you set the property to.

### Digital Read and Write
It is very easy to read and write values to the pins. In general, it is a 2-step process.
- Set the pin mode
- Read or write the bit value

Reading the value of a pin example:
```csharp
Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Input;
// The below lines are reoughly equivalent
var isOn = Pi.Gpio.Pin02.Read(); // Reads as a boolean
var pinValue = Pi.Gpio.Pin02.ReadValue(); // Reads as a GpioPinValue
```

Writing to a pin example
```csharp
Pi.Gpio.Pin02.PinMode = GpioPinDriveMode.Output;
// The below lines are reoughly equivalent
Pi.Gpio.Pin02.Write(true); // Writes a boolean
Pi.Gpio.Pin02.Write(GpioPinValue.High); // Writes a pin value
```

### Analog (Level) Read and Write
TODO

### Hardware PWM
Simple code for led dimming:

```csharp
   var pin = Pi.Gpio[P1.Gpio18];
   pin.PinMode = GpioPinDriveMode.PwmOutput;
   pin.PwmMode = PwmMode.Balanced;
   pin.PwmClockDivisor = 2; 
   while (true)
   {
      for (var x = 0; x <= 100; x++)
      {
         pin.PwmRegister = (int)pin.PwmRange / 100 * x;
         Thread.Sleep(10);
      }

      for (var x = 0; x <= 100; x++)
      {
         pin.PwmRegister = (int)pin.PwmRange - ((int)pin.PwmRange / 100 * x);
         Thread.Sleep(10);
      }
   }
```

**PwmRange** is the maximun value of the pulse width, than means 100% of pulse width. Changing this value allows you to have a more fine or coarse control of the pulse width (default 1024).

**PwmRegister** is the current pulse width. Changing this value allows you to change the current pulse width and thus the duty cycle.

**Duty Cycle** is equals to **PwmRegister** divide by **PwmRange**. Assuming a **PwmRange** value of 1024 (default), we have:

| PwmRegister | Duty Cycle | 
| :---: | :---: |
| 0 | 0% |
| 256 | 25% |
| 512 | 50% |
| 768 | 75% |
| 1024 | 100% |

**_Note:_** Hardware PWM can be used only in GPIO13 and GPIO18.

### Software PWM
Simple code for led dimming:

```csharp
   var range = 100;
   var pin = Pi.Gpio[P1.Gpio18];
   pin.PinMode = GpioPinDriveMode.Output;
   pin.StartSoftPwm(0, range);
   
   while (true)
   {
      for (var x = 0; x <= 100; x++)
      {
         pin.SoftPwmValue = range / 100 * x;
         Thread.Sleep(10);
      }

      for (var x = 0; x <= 100; x++)
      {
         pin.SoftPwmValue = range - (range / 100 * x);
         Thread.Sleep(10);
      }
   }
```

**SoftPwmRange** is the range of the pulse width, than means 100% of pulse width (We notice better performance using a range value of 100).

**SoftPwmValue** is the current pulse width. Changing this value allows you to change the current pulse width and thus the duty cycle.

**_Note:_** Software PWM can be used in any GPIO.

### Tone Generation
You can emit tones by using SoftToneFrequency. Example:

```csharp
// Get a reference to the pin
var passiveBuzzer = Pi.Gpio[WiringPiPin.Pin01];
// Set the frequency to Alto Do (523Hz)
passiveBuzzer.SoftToneFrequency = 523
// Wait 1 second
System.Threading.Thread.Sleep(1000);
// And stop
passiveBuzzer.SoftToneFrequency = 0;
```

### Interrupts and Callbacks
Register an Interrupt Callback example:

```csharp
using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

class Program
{
  // Define the implementation of the delegate;
  static void ISRCallback()
  {
     Console.WriteLine("Pin Activated...");         
  }

  static void Main(string[] args)
  {
        Console.WriteLine("Gpio Interrupts");
        var pin = Pi.Gpio.Pin00;
        pin.PinMode = GpioPinDriveMode.Input;
        pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, ISRCallback);
        Console.ReadKey();
  }
}
```

## Using the SPI Bus
I really liked the following description from [Neil's Log Book](http://nrqm.ca/nrf24l01/serial-peripheral-interface/): _The SPI (Serial Peripheral Interface) protocol behaves like a ring buffer so that whenever the master sends a byte to the slave, the slave sends a byte back to the master. The slave can use this behavior to return a status byte, a response to a previous byte, or null data (the master may choose to read the returned byte or ignore it). The bus operates on a 4-wire interface._

```RaspberryIO``` provides easy access to the 2 SPI channels available on the Raspberry. The functionality depends on ```Wiring Pi```'s SPI library. Please note that you may need to issue the command ```gpio load spi``` before starting your application (or as a ```System.Diagnostics.Process``` when your application starts) if the SPI kernel drivers have not been loaded.

In order to use an SPI channel you **MUST** always set the ```Channel0Frequency``` or ```Channel1Frequency``` (depending on the channel you want to use) before calling the ```SendReceive``` method. If the property is not set beforehand the SPI channel will fail initialization. See an example below: 

Example of using the SPI Bus
```csharp
Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;
var request = System.Text.Encoding.ASCII.GetBytes("HELLO!");
var response = Pi.Spi.Channel0.SendReceive(request);
```

**_Note_**: In order to enable the second SPI channel (SPI1) you need to add `dtoverlay=spi1-1cs` to the config file.

## I2C to connect ICs
The Inter IC Bus (I2C) is a cousin of the SPI bus but it is somewhat more complex and it does not work as a ring buffer like the SPI bus. It also connects all of its slave devices in series and depends on 2 lines only. There is a nice tutorial on setting up and using the I2C bus at [Robot Electronics](http://www.robot-electronics.co.uk/i2c-tutorial). From their site: _The physical bus is just two wires, called SCL and SDA. SCL is the clock line. It is used to synchronize all data transfers over the I2C bus. SDA is the data line. The SCL & SDA lines are connected to all devices on the I2C bus. There needs to be a third wire which is just the ground or 0 volts. There may also be a 5volt wire is power is being distributed to the devices. Both SCL and SDA lines are "open drain" drivers. What this means is that the chip can drive its output low, but it cannot drive it high. For the line to be able to go high you must provide pull-up resistors to the 5v supply. There should be a resistor from the SCL line to the 5v line and another from the SDA line to the 5v line. You only need one set of pull-up resistors for the whole I2C bus, not for each device._

```RaspberryIO``` provides easy access to the I2C bus available on the Raspberry. The functionality depends on ```Wiring Pi```'s I2C library. Please note that you may need to issue the command ```gpio load i2c``` before starting your application (or as a ```System.Diagnostics.Process``` when your application starts) if the I2C kernel drivers have not been loaded. The default baud rate is 100Kbps. If you wish to initialize the bus at a different baud rate you may issue, for example, ```gpio load i2c 200```. This will load the bus at 200kbps.

In order to detect I2C devices, you could use the ```i2cdetect``` system command. Just remember that on a Rev 1 Raspberry Pi it's device 0, and on a Rev. 2 it's device 1. e.g.
```
i2cdetect -y 0 # Rev 1
i2cdetect -y 1 # Rev 2
```

Example of using the I2C Bus:

```csharp
// Register a device on the bus
var myDevice = Pi.I2C.AddDevice(0x20);

// Simple Write and Read (there are algo register read and write methods)
myDevice.Write(0x44);
var response = myDevice.Read();

// List registered devices on the I2C Bus
foreach (var device in Pi.I2C.Devices)
{
    Console.WriteLine($"Registered I2C Device: {device.DeviceId}");
}
```

## Timing and Threading
TODO

## Serial Ports (UART)
Where is the serial port API? Well, it is something we will most likely add in the future. For now, you can simply use the built-in ```SerialPort``` class the .NET framework provides.

## Abstraction Package

If you want to implement your own provider for RaspberryIO, you must use the following package to implement all the `Pi` providers.

Install Unosquare.Raspberry.Abstractions package:
[![NuGet version](https://badge.fury.io/nu/Unosquare.Raspberry.Abstractions.svg)](https://badge.fury.io/nu/Unosquare.Raspberry.Abstractions)

```
PM> Install-Package Unosquare.Raspberry.Abstractions
```

## Handy Notes

In order to setup Wi-Fi, run: `sudo nano /etc/wpa_supplicant/wpa_supplicant.conf`

A good file should look like this:
```
country=US
ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev
update_config=1

network={
    ssid="your_real_wifi_ssid"
    scan_ssid=1
    psk="your_real_password"
}
```

And then restart the services as follows:
```
sudo systemctl daemon-reload
sudo systemctl restart dhcpcd
```

You can also configure most boot options by running: `sudo raspi-config`

## Similar Projects
- <a href="https://github.com/raspberry-sharp/raspberry-sharp-io">Raspberry# IO</a>
- <a href="https://github.com/danriches/WiringPi.Net">WiringPi.Net</a>
- <a href="https://github.com/andycb/PiSharp">PiSharp</a>
