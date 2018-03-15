namespace Unosquare.RaspberryIO.Playground
{
    using Camera;
    using Computer;
    using Gpio;
    using Peripherals;
    using Swan;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Main entry point class
    /// </summary>
    public partial class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The CLI arguments.</param>
        public static void Main(string[] args)
        {
            $"Starting program at {DateTime.Now}".Info();

            Terminal.Settings.DisplayLoggingMessageType = LogMessageType.Info | LogMessageType.Warning | LogMessageType.Error;

            try
            {
                // A set of very simple tests:
                // TestCaptureImage();
                // TestCaptureVideo();
                // TestLedStripGraphics();
                // TestLedStrip();
                // TestTag();
                TestSystemInfo();
                TestInfraredSensor();
            }
            catch (Exception ex)
            {
                "Error".Error(nameof(Main), ex);
            }
            finally
            {
                "Program finished.".Info();
                Terminal.Flush(TimeSpan.FromSeconds(2));
                if (System.Diagnostics.Debugger.IsAttached)
                    "Press any key to exit . . .".ReadKey();
            }
        }

        /// <summary>
        /// Tests the infrared sensor HX1838.
        /// </summary>
        public static void TestInfraredSensor()
        {
            var inputPin = Pi.Gpio.Pin04; // BCM Pin 23 or Physical pin 16 on the right side of the header.
            var sensor = new InfraRedSensorHX1838(inputPin);

            sensor.DataAvailable += (s, e) =>
            {
                if (e.TrainDurationUsecs < 50000)
                {
                    $"Spurious Signal of {e.TrainDurationUsecs} micro seconds.".Error("IR");
                    return;
                }

                var necData = InfraRedSensorHX1838.NecDecoder.DecodePulses(e.Pulses);
                var debugData = InfraRedSensorHX1838.DebugPulses(e.Pulses);

                $"Pulses Length: {e.Pulses.Length}; Duration: {e.TrainDurationUsecs}; Reason: {e.State}".Warn("IR");
                if (necData != null)
                    ("NEC Raw Data: " + BitConverter.ToString(necData).Replace("-", " ")).Warn("IR");

                debugData.Info("IR");
            };

            Console.ReadLine();
        }

        /// <summary>
        /// Tests the SPI bus functionality.
        /// </summary>
        public static void TestSpi()
        {
            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;

            var request = Encoding.UTF8.GetBytes("Hello over SPI");
            $"SPI Request: {BitConverter.ToString(request)}".Info();
            var response = Pi.Spi.Channel0.SendReceive(request);
            $"SPI Response: {BitConverter.ToString(response)}".Info();

            $"SPI Base Stream Request: {BitConverter.ToString(request)}".Info();
            Pi.Spi.Channel0.Write(request);
            response = Pi.Spi.Channel0.SendReceive(new byte[request.Length]);
            $"SPI Base Stream Response: {BitConverter.ToString(response)}".Info();
        }

        /// <summary>
        /// Tests the display.
        /// </summary>
        public static void TestDisplay()
        {
            var input = string.Empty;

            while (input.Equals("x") == false)
            {
                "Enter brightness value (0 to 255). Enter b to toggle Backlight, Enter x to Exit".Info();
                input = Console.ReadLine();

                if (input?.Equals("b") == true)
                {
                    Pi.PiDisplay.IsBacklightOn = !Pi.PiDisplay.IsBacklightOn;
                }
                else
                {
                    if (byte.TryParse(input, out var value))
                    {
                        if (value != Pi.PiDisplay.Brightness)
                        {
                            $"Current Value: {Pi.PiDisplay.Brightness}, New Value: {value}".Info();
                            Pi.PiDisplay.Brightness = value;
                        }
                    }
                }

                $"Display Status - Backlight: {Pi.PiDisplay.IsBacklightOn}, Brightness: {Pi.PiDisplay.Brightness}"
                    .Info();
            }

            Pi.PiDisplay.IsBacklightOn = true;
            Pi.PiDisplay.Brightness = 96;
            $"Display Status - Backlight: {Pi.PiDisplay.IsBacklightOn}, Brightness: {Pi.PiDisplay.Brightness}".Info();
        }

        /// <summary>
        /// Tests the LED blinking logic.
        /// </summary>
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
                Thread.Sleep(500);
            }
        }

        private static void TestSystemInfo()
        {
            $"GPIO Controller initialized successfully with {Pi.Gpio.Count} pins".Info();
            $"{Pi.Info}".Info();
            $"Microseconds Since GPIO Setup: {Pi.Timing.MicrosecondsSinceSetup}".Info();
            $"Uname {Pi.Info.OperatingSystem}".Info();
            $"HostName {NetworkSettings.Instance.HostName}".Info();
            $"Uptime (seconds) {Pi.Info.Uptime}".Info();
            var timeSpan = Pi.Info.UptimeTimeSpan;
            $"Uptime (timespan) {timeSpan.Days} days {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}"
                .Info();

            foreach (var adapter in NetworkSettings.Instance.RetrieveAdapters())
            {
                $"Adapter: {adapter.Name,6} | IPv4: {adapter.IPv4,16} | IPv6: {adapter.IPv6,28} | AP: {adapter.AccessPointName,16} | MAC: {adapter.MacAddress,18}"
                    .Info();
            }
        }

        private static void TestCaptureImage()
        {
            var pictureBytes = Pi.Camera.CaptureImageJpeg(640, 480);
            var targetPath = "/home/pi/picture.jpg";
            if (File.Exists(targetPath))
                File.Delete(targetPath);

            File.WriteAllBytes(targetPath, pictureBytes);
            $"Took picture -- Byte count: {pictureBytes.Length}".Info();
        }

        private static void TestCaptureVideo()
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
                    onDataCallback: (data) =>
                    {
                        videoByteCount += data.Length;
                        videoEventCount++;
                    },
                    onExitCallback: null);

                // Wait for user interaction
                startTime = DateTime.UtcNow;
                "Press any key to stop reading the video stream . . .".ReadKey();
            }
            catch (Exception ex)
            {
                $"{ex.GetType()}: {ex.Message}".Error();
            }
            finally
            {
                // Always close the video stream to ensure raspivid quits
                Pi.Camera.CloseVideoStream();

                // Output the stats
                var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000");
                var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000");
                $"Capture Stopped. Received {megaBytesReceived} Mbytes in {videoEventCount} callbacks in {recordedSeconds} seconds"
                    .Info();
            }
        }

        private static void TestColors()
        {
            var colors = new CameraColor[]
            {
                CameraColor.Black,
                CameraColor.White,
                CameraColor.Red,
                CameraColor.Green,
                CameraColor.Blue
            };

            foreach (var color in colors)
            {
                $"{color.Name,-15}: RGB Hex: {color.ToRgbHex(false)}    YUV Hex: {color.ToYuvHex(true)}".Info();
            }
        }

        private static void Tag()
        {
            var mfrc522 = new Mfrc522Controller();

            while (true)
            {
                // Scan for cards
                var result = mfrc522.Request(Mfrc522Controller.PICC_REQIDL);
                var status = result.Item1;

                // If a card is found
                if (status == Mfrc522Controller.MI_OK)
                {
                    "Card detected".Info();
                }

                // Get the UID of the card
                var resultAnticoll = mfrc522.Anticoll();
                var status2 = resultAnticoll.status;
                var uid = resultAnticoll.data;

                // If we have the UID, continue
                if (status2 == Mfrc522Controller.MI_OK)
                {
                    // Print UID
                    $"Card read UID: {uid[0]},{uid[1]},{uid[2]},{uid[3]}".Info();

                    // This is the default key for authentication
                    var key = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

                    // Select the scanned tag
                    mfrc522.SelectTag(uid);

                    // Authenticate
                    var status3 = mfrc522.Auth(Mfrc522Controller.PICC_AUTHENT1A, 8, key, uid);

                    // Check if authenticated
                    if (status3 == Mfrc522Controller.MI_OK)
                    {
                        mfrc522.ReadSpi(8);
                        mfrc522.StopCrypto1();
                    }
                    else
                    {
                        "Authentication error".Error();
                    }
                }
            }
        }
    }
}