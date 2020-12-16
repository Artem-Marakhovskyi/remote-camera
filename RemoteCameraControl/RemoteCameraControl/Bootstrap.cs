using Acr.UserDialogs;
using Autofac;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using RemoteCameraControl.Android.SelectMode;
using RemoteCameraControl.Hub;
using RemoteCameraControl.IO;
using RemoteCameraControl.Ioc;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Network;
using RemoteCameraControl.Permissions;
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
            
            RegisterInstance<ILogger>(Logger);

            RegisterInstance<IPermissions>(Plugin.Permissions.CrossPermissions.Current);
            RegisterType<IFileService, FileService>();
            RegisterType<ContractInitializer, ContractInitializer>();
            RegisterType<IDialogs, Dialogs>();
            RegisterType<IPermissionService, PermissionService>();
            RegisterType<ILoadingIndicator, LoadingIndicator>();

           
            RegisterViewModel<ModeSelectViewModel>();
            RegisterViewModel<PhotoViewModel>();
            RegisterViewModel<TakePhotoViewModel>();
            RegisterViewModel<SplashViewModel>();
            RegisterViewModel<PhotoMirrorViewModel>();

            var dataSignalHub = new DataSignalHub();
            RegisterInstance<IDataSignalPublisher>(dataSignalHub);
            RegisterInstance<IDataSignalHubManager>(dataSignalHub);

            var controlSignalHub = new ControlSignalHub();
            RegisterInstance<IControlSignalPublisher>(controlSignalHub);
            RegisterInstance<IControlSignalHubManager>(controlSignalHub);

            ContainerBuilder.RegisterType<AppContext>().As<IAppContext>().SingleInstance();
            ContainerBuilder.Register(x => x.Resolve<IAppContext>().ControlStreamManager).As<ControlStreamManager>();
            ContainerBuilder.Register(x => x.Resolve<IAppContext>().DataStreamManager).As<DataStreamManager>();

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