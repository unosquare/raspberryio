# Raspberry IO [![NuGet version](https://badge.fury.io/nu/Unosquare.Raspberry.IO.svg)](https://badge.fury.io/nu/Unosquare.Raspberry.IO)

The Raspberry Pi's IO Functionality in an easy-to use API for Mono/.NET/C#

This library enables developers to use the various Raspberry Pi's hardware modules
* ```Pi.Camera``` Provides access to the offical Raspberry Pi Camera module.
* ```Pi.Gpio``` Provides access to the Raspberry Pi's GPIO as a collection of GPIO Pins.
* ```Pi.Info``` Provides information on this Raspberry Pi's CPU and form factor.
* ```Pi.Timing``` Provides access to The PI's Timing and threading API
* ```Pi.Spi``` Provides access to the 2-channel SPI bus
* ```Pi.I2c``` Provides access to the functionality of the i2c bus.

Please note you program needs to be run with ```sudo```. Example ```sudo mono myprogram.exe```

The ```Pi.Camera``` module uses ```raspivid``` and ```raspistill``` to access to camera so they must be installed in order for your program to work propely.

The rest of the modules mostly use the wonderful ```wiring Pi``` library. You do not need to install this library yourself. The Raspberry IO assembly will automatically extract the compiled library in the same path as the entry assembly.
