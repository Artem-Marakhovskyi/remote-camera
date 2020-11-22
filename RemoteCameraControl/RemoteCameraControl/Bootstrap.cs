using Autofac;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using RemoteCameraControl.Android.RemoteCameraControl.Permissions;
using RemoteCameraControl.Blue;
using RemoteCameraControl.Home;
using RemoteCameraControl.IO;
using RemoteCameraControl.Logger;
using RemoteCameraControl.RemoteCameraControl.Interaction;
using XLabs.Ioc;

namespace RemoteCameraControl.Android.RemoteCameraControl
{
    public abstract class Bootstrap
    {
        protected ContainerBuilder ContainerBuilder { get; private set; }
        protected ILogger Logger { get; private set; }
        
        public void Execute()
        {
            ContainerBuilder = new ContainerBuilder();

            RegisterType<IBluetooth, Bluetooth>();
            RegisterInstance<IBluetoothLE>(CrossBluetoothLE.Current);
    
            RegisterType<ILogger, Logger.Logger>();
            RegisterType<IPermissionsRequestor, PermissionsRequestor>();
            RegisterType<IFileService, FileService>();
            RegisterType<IDialogs, Dialogs>();
            RegisterType<ILoadingIndicator, LoadingIndicator>();
            
            RegisterViewModel<HomeViewModel>();
    
            
            RegisterPlatformSpecifics();
            Resolver.SetResolver(new Ioc.Resolver(ContainerBuilder.Build(), Logger));
        }

        private void Init()
        {
            InitLogger();
        }
        
        protected abstract void RegisterPlatformSpecifics();

        protected void RegisterViewModel<T>() where T : ViewModelBase
            => ContainerBuilder.RegisterType<T>().As<T>();

        protected void RegisterInstance<T>(T instance) where T : class
            => ContainerBuilder.RegisterInstance<T>(instance);

        protected void RegisterType<T, TImpl>() where TImpl : T 
            => ContainerBuilder.RegisterType<TImpl>().As<T>();
        
        private void InitLogger()
        {
            var logger = new Logger.Logger();
#if DEBUG
            logger.CurrentLevel = LogLevel.Debug;
            logger.AddSource(new ConsoleLogSource());
#else
            logger.CurrentLevel = LogLevel.Info;
            logger.AddSource(new ServerLogSource());
            logger.AddSource(new AppCenterLogSource(LogLevel.Warning));
#endif
            logger.AddSource(new FileLogSource("Logs"));
            Logger = logger;
        }
    }
}