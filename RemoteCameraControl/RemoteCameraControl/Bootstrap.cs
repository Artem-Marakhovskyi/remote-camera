using Acr.UserDialogs;
using Autofac;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using RemoteCameraControl.Android.RemoteCameraControl.Permissions;
using RemoteCameraControl.Android.SelectMode;
using RemoteCameraControl.Blue;
using RemoteCameraControl.Home;
using RemoteCameraControl.Hub;
using RemoteCameraControl.IO;
using RemoteCameraControl.Ioc;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Network;
using RemoteCameraControl.Photo;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Android.RemoteCameraControl
{
    public abstract class Bootstrap
    {
        protected ContainerBuilder ContainerBuilder { get; private set; }
        protected ILogger Logger { get; private set; }
        
        public void Execute()
        {
            Init();
            
            ContainerBuilder = new ContainerBuilder();

            RegisterType<IBluetooth, Bluetooth>();
    
            RegisterInstance<ILogger>(Logger);
            RegisterType<IPermissionsRequestor, PermissionsRequestor>();
            RegisterInstance<IPermissions>(PermissionsImplementation.Current);
            RegisterType<IFileService, FileService>();
            RegisterType<ContractInitializer, ContractInitializer>();
            RegisterType<IDialogs, Dialogs>();
            RegisterType<DataStreamManager, DataStreamManager>();
            RegisterType<ControlStreamManager, ControlStreamManager>();
            RegisterType<ILoadingIndicator, LoadingIndicator>();

            RegisterInstance<IAppContext>(new AppContext());
            RegisterViewModel<HomeViewModel>();
            RegisterViewModel<ModeSelectViewModel>();
            RegisterViewModel<PhotoViewModel>();
            RegisterViewModel<PhotoMirrorViewModel>();

            var controlSignalHub = new ControlSignalHub();
            RegisterInstance<IControlSignalPublisher>(controlSignalHub);
            RegisterInstance<IControlSignalHubManager>(controlSignalHub);


            RegisterPlatformSpecifics();
            
            RegisterInstance<IUserDialogs>(UserDialogs.Instance);
            
            XLabs.Ioc.Resolver.SetResolver(new Resolver(ContainerBuilder.Build(), Logger));
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

            logger.CurrentLevel = LogLevel.Debug;
            logger.AddSource(new ConsoleLogSource());

            Logger = logger;
        }
    }
}