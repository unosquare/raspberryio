[![NuGet version](https://badge.fury.io/nu/Unosquare.Raspberry.IO.svg)](https://badge.fury.io/nu/Unosquare.Raspberry.IO)
[![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/raspberryio/)](https://github.com/igrigorik/ga-beacon)

# <img src="https://github.com/unosquare/raspberryio/raw/master/logos/raspberryio-logo-32.png"></img> RaspberryIO - *Pi's hardware access from Mono*
The Raspberry Pi's IO Functionality in an easy-to-use API for Mono/.NET/C#. Our mission is to make Mono a first-class citizen in the Python-centric community of Raspberry Pi developers.

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

This library depends on the wonderful ```WiringPi``` library avaialble [here](http://wiringpi.com/). You do not need to install this library yourself. The ```RaspberryIO``` assembly will automatically extract the compiled binary of the library in the same path as the entry assembly.

## NuGet Installation:
```
PM> Install-Package Unosquare.Raspberry.IO
```

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

## Running .NET Core 2

This project can also run in .NET Core 2.

Run the fallowing commands to install .NET Core 2.

```
# Update Ubuntu 16.04
sudo apt-get -y update

# Install the packages necessary for .NET Core
sudo apt-get -y install libunwind8 libunwind8-dev gettext libicu-dev liblttng-ust-dev libcurl4-openssl-dev libssl-dev uuid-dev

# Download the latest binaries for .NET Core 2 
wget https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz

# Make a directory for .NET Core to live in
mkdir /home/ubuntu/dotnet

# Unzip the binaries into the directory you just created
tar -xvf dotnet-runtime-latest-linux-arm.tar.gz -C /home/ubuntu/dotnet

# Now add the path to the dotnet executable to the environment path
# This ensures the next time you log in, the dotnet exe is on your path
echo "PATH=\$PATH:/home/ubuntu/dotnet" >> dotnetcore.sh
sudo mv dotnetcore.sh /etc/profile.d

# Then run the command below to add the path to the dotnet executable to the current session
PATH=$PATH:/home/ubuntu/dotnet
```

After that you can reboot the raspberry. To check if dotnet is installed just run "dotnet" and a message should show.

```
ubuntu@ubuntu:~$ dotnet

Usage: dotnet [options]
Usage: dotnet [path-to-application]

Options:
  -h|--help            Display help.
  --version         Display version.

path-to-application:
  The path to an application .dll file to execute.
```
### Run the app in the raspberry

- You need to publish the project and copy the result folder to the raspberry pi

```
PM> dotnet publish -r ubuntu.16.04-arm .\src\Unosquare.RaspberryIO.Playground -f netcoreapp2.0
```

- Give permissions to run the project

```
ubuntu@ubuntu:~/publish$ sudo chmod u+x *
```

- Run the project

```
ubuntu@ubuntu:~/publish$ ./Unosquare.RaspberryIO.Playground
```

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
Capturing video streams is somewhat different but it is still very easy to do. The concept behind it is to _Open_ a video stream providing your own callback. When opening the stream ```Raspberry IO``` will spawn a separte thread and will not block the execution of your code, but it will continually call your callback method containing the bytes that are being read from the camera until the _Close_ method is called or until the timeout is reached.

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

But wait a second, Where are Wiring Pi (wPi) pins 17 through 20? The above diagram shows the pins of GPIO Header P1. There is an additional GPIO header on the Pi called P5. [More info available here](http://www.raspberrypi-spy.co.uk/2012/09/raspberry-pi-p5-header/)

In order to access the pins, use ```Pi.Gpio```. The pins can have multiple behaviors and fortunately ```Pi.Gpio``` can be iterated, addressed by index, addessed by Wiring Pi pin number and provides the pins as publicly accessible properties.

Here is an example of addressing the pins in all the various ways:
```csharp
public static void TestLedBlinking()
{
    // Get a reference to the pin you need to use.
    // All 3 methods below are exactly equivalente
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
All pins have handy properties and methods that you can use to drive them. For example you can examine the ```Capabilities``` property to find out which features are avilable on the pin. You can also use the ```PinMode``` property to get or set the operating mode of the pin. Please note that the value of the ```PinMode``` property is by default set to _Input_ and it will return the last mode you set the property to.

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
TODO

### Software PWM
TODO

### Tone Generation
Create a software controlled tone pin example:

```csharp
// Get a reference to the pin
var passiveBuzzer = Pi.Gpio[WiringPiPin.Pin01];
// Make it a software controlled tone pin
Unosquare.RaspberryIO.Native.WiringPi.softToneCreate(passiveBuzzer.PinNumber);
// Set the frequency to Alto Do (523Hz)
passiveBuzzer.SoftToneFrequency = 523
```


### Interrupts and Callbacks
TODO

## Using the SPI Bus
I really liked the following description from [Neil's Log Book](http://nrqm.ca/nrf24l01/serial-peripheral-interface/): _The SPI (Serial Peripheral Interface) protocol behaves like a ring buffer, so that whenever the master sends a byte to the slave, the slave sends a byte back to the master. The slave can use this behaviour to return a status byte, a response to a previous byte, or null data (the master may choose to read the returned byte, or ignore it). The bus operates on a 4-wire interface._

```RaspberryIO``` provides easy access to the 2 SPI channels available on the Raspberry. The functionality depends on ```Wiring Pi```'s SPI library. Please note that you may need to issue the command ```gpio load spi``` before starting your application (or as a ```System.Diagnostics.Process``` when your application starts) if the SPI kernel drivers have not been loaded.

In order to use an SPI channel you **MUST** always set the ```Channel0Frequency``` or ```Channel1Frequency``` (depending on the channel you want to use) before calling the ```SendReceive``` method. If the property is not set beforehand the SPI channel will fail initialization. See an example below: 

Example of using the SPI Bus
```csharp
Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;
var request = System.Text.Encoding.ASCII.GetBytes("HELLO!");
var response = Pi.Spi.Channel0.SendReceive(request);
```

## I2C to connect ICs
The Inter IC Bus (I2C) is a cousin of the SPI bus but it is somewhat more complex and it does not work as a ring buffer like the SPI bus. It also connects all of its slave devices in series and depends on 2 lines only. There is a nice tutorial on setting up and using the I2C bus at [Robot Electronics](http://www.robot-electronics.co.uk/i2c-tutorial). From their site: _The physical bus is just two wires, called SCL and SDA. SCL is the clock line. It is used to synchronize all data transfers over the I2C bus. SDA is the data line. The SCL & SDA lines are connected to all devices on the I2C bus. There needs to be a third wire which is just the ground or 0 volts. There may also be a 5volt wire is power is being distributed to the devices. Both SCL and SDA lines are "open drain" drivers. What this means is that the chip can drive its output low, but it cannot drive it high. For the line to be able to go high you must provide pull-up resistors to the 5v supply. There should be a resistor from the SCL line to the 5v line and another from the SDA line to the 5v line. You only need one set of pull-up resistors for the whole I2C bus, not for each device._

```RaspberryIO``` provides easy access to the I2C bus available on the Raspberry. The functionality depends on ```Wiring Pi```'s I2C library. Please note that you may need to issue the command ```gpio load i2c``` before starting your application (or as a ```System.Diagnostics.Process``` when your application starts) if the I2C kernel drivers have not been loaded. The default baud rate is 100Kbps. If you wish to initialize the bus at a different baud rate you may issue for example, ```gpio load i2c 200```. This will load the bus at 200kbps.

In order to detect I2C devices you could use the ```i2cdetect``` system command. Just remember that on a Rev 1 Raspberry Pi it's device 0, and on a Rev. 2 it's device 1. e.g.
```
i2cdetect -y 0 # Rev 1
i2cdetect -y 1 # Rev 2
```

Example of using the I2C Bus
```csharp
// Register a device on the bus
var myDevice = Pi.I2c.AddDevice(0x20);

// Simple Write and Read (there are algo register read and write methods)
myDevice.Write(0x44);
var response = myDevice.Read();

// List registered devices on the I2C Bus
foreach (var device in Pi.I2c.Devices)
{
    Console.WriteLine($"Registered I2C Device: {device.DeviceId}");
}
```

## Timing and Threading
TODO

## Serial Ports (UART)
Where is the serial port API? Well, it is something we will most likely add in the future. For now, you can simply use the built-in ```SerialPort``` class the .NET framework provides.

## Similar Projects
- <a href="https://github.com/raspberry-sharp/raspberry-sharp-io">Raspberry# IO</a>
- <a href="https://github.com/danriches/WiringPi.Net">WiringPi.Net</a>
- <a href="https://github.com/andycb/PiSharp">PiSharp</a>
