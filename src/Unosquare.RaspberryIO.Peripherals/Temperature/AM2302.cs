using System;

namespace Unosquare.RaspberryIO.Peripherals
{
    using System.Threading;    
    using Unosquare.RaspberryIO;
    using Unosquare.RaspberryIO.Gpio;
    using Unosquare.RaspberryIO.Native;    

    /// <summary>
    /// Manager to a DHT22 sensor
    /// </summary>
    public class AM2302 : IDisposable
    {   
        public delegate void SensorReadEventHandler(THData measure);
        public event SensorReadEventHandler OnSensorRead = null;
        private readonly TimeSpan _measureTime = TimeSpan.FromSeconds(2);        
        private readonly TimeSpan _bitDataTime = new TimeSpan(10 * 50); // 26µs -> 50 for "0", 50 -> 70µs for "1"                           
        private readonly Timing _timing;
        private bool _started = false;        
        private GpioPin _pin;
        private Thread _th;        

        public bool IsStarted { get { return _started; } }

        public AM2302(int pinNumber)
        {
            _timing = Timing.Instance;
            _pin = CreatePin(pinNumber);             
            _th = new Thread(GetMeasure);            
        }

        private void GetMeasure()
        {
            var hrt = new HighResolutionTimer();
            var lastElapsedTime = TimeSpan.FromSeconds(0);
            while (_started)
            {                               
                try
                {                    
                    THData measure = null;
                    //Start to comunicate with sensor
                    //Inform sensor that must finish last execution and put it's state in idle                
                    _pin.PinMode = GpioPinDriveMode.Output;

                    //Waiting for sensor init
                    _pin.Write(GpioPinValue.High);
                    if (lastElapsedTime < _measureTime)
                        Thread.Sleep(_measureTime - lastElapsedTime);

                    //Start to counter measure time
                    hrt.Start();
                    //Send request to trasmission from board to sensor
                    _pin.Write(GpioPinValue.Low);
                    _timing.SleepMicroseconds(1000);                                        
                    _pin.Write(GpioPinValue.High);                    
                    _timing.SleepMicroseconds(20);
                    _pin.Write(GpioPinValue.Low);                    

                    //Acquire measure
                    measure = TryGetMeasure();                    
                    OnSensorRead(measure);                    
                }
                catch
                {
                    //ignored                    
                }
                lastElapsedTime = hrt.Elapsed;
                if(hrt.IsRunning)
                    hrt.Stop();
                hrt.Reset();
            }            
        }
        
        private THData TryGetMeasure()
        {
            //Prepare buffer to store measure and checksum
            var data = new byte[5];
            for (var i = 0; i < 5; i++)
                data[i] = 0;

            //Wait for sensor response                
            _pin.PinMode = GpioPinDriveMode.Input;
            //Read acknowledgement from sensor            
            _pin.WaitForValue(GpioPinValue.High, 100);
            _pin.WaitForValue(GpioPinValue.Low, 100);

            //Read 40 bits to acquire:
            //16 bit -> Humidity
            //16 bit -> Temperature
            //8 bit -> Checksum
            var cnt = 7; //bit counter
            var idx = 0; //bit index            
            var hrt = new HighResolutionTimer();
            for (var i = 0; i < 40; i++)
            {
                hrt.Reset();
                _pin.WaitForValue(GpioPinValue.High, 100);
                hrt.Start();
                _pin.WaitForValue(GpioPinValue.Low, 100);
                hrt.Stop();
                //Check if signal is 1 or 0               
                if (hrt.Elapsed > _bitDataTime)
                    data[idx] |= (byte)(1 << cnt);
                if (cnt == 0)
                {
                    idx++; //next bit
                    cnt = 7; //restart with next byte
                }
                else
                    cnt--;                
            }

            var checkSum = data[0] + data[1] + data[2] + data[3];
            if ((checkSum & 0xff) != data[4])
                return null;                            

            var sign = 1;
            //Check negative temperature 
            if ((data[2] & 0x80) != 0)
            {
                data[2] = (byte)(data[2] & 0x7F);
                sign = -1;
            }
                       
            return new THData
            {
                HumidityPercentage = ((data[0] << 8) + data[1]) / 10,
                TemperatureCelsius = (sign * ((data[2] << 8) + data[3])) / 10
            };
        }

        public void StartListener()
        {            
            _started = true;
            _th.Start();                      
        }        

        public void StopListener()
        {            
            _started = false;
            _th.Abort();
        }

        public void Dispose()
        {
            if (_started)
                StopListener();            
        }
        
        private GpioPin CreatePin(int pin)
        {
            switch (pin)
            {
                case 7:
                    return Pi.Gpio.Pin07;
                case 11:
                    return Pi.Gpio.Pin00;
                case 12:
                    return Pi.Gpio.Pin01;
                case 13:
                    return Pi.Gpio.Pin02;
                case 15:
                    return Pi.Gpio.Pin03;
                case 16:
                    return Pi.Gpio.Pin04;
                case 18:
                    return Pi.Gpio.Pin05;
                case 22:
                    return Pi.Gpio.Pin06;
                case 29:
                    return Pi.Gpio.Pin21;
                case 31:
                    return Pi.Gpio.Pin22;
                case 32:
                    return Pi.Gpio.Pin26;
                case 33:
                    return Pi.Gpio.Pin23;
                case 35:
                    return Pi.Gpio.Pin24;
                case 36:
                    return Pi.Gpio.Pin27;
                case 37:
                    return Pi.Gpio.Pin25;
                case 38:
                    return Pi.Gpio.Pin28;
                case 40:
                    return Pi.Gpio.Pin29;
                default:
                    throw new Exception("Pin " + pin + " is not green GPIO.");
            }
        }
    }
}
