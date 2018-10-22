namespace Unosquare.WiringPI
{
    using RaspberryIO.Abstractions;
    using Swan.Components;
    using Native;

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
                DependencyContainer.Current.Register<ITiming>(new Timing());

                Resources.EmbeddedResources.ExtractAll();
            }
        }
    }
}
