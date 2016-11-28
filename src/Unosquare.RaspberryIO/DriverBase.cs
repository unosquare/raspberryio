using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.RaspberryIO
{
    public abstract class DriverBase
    {

        private void Setup()
        {

        }

        private void ValidatePinLocks()
        {

        }

        public ReadOnlyCollection<GpioPin> Pins
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
