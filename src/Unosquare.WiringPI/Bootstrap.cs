namespace Unosquare.WiringPI
{
    using RaspberryIO.Abstractions;
    using Swan.Components;

    internal static class Bootstrap
    {
        private static readonly object SyncLock = new object();

        static Bootstrap()
        {
            lock (SyncLock)
            {
                DependencyContainer.Current.Register<IGpioController>(new GpioController());
                DependencyContainer.Current.Register<ISpiBus>(new SpiBus());
                DependencyContainer.Current.Register<II2CBus>(new I2CBus());
                DependencyContainer.Current.Register<ISystemInfo>(new SystemInfo());

                Resources.EmbeddedResources.ExtractAll();
            }
        }
    }
}
