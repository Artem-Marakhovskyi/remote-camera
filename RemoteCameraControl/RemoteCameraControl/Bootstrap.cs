using Acr.UserDialogs;
using Autofac;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using RemoteCameraControl.Android.SelectMode;
using RemoteCameraControl.IO;
using RemoteCameraControl.Ioc;
using RemoteCameraControl.Logger;
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
            RegisterType<IDialogs, Dialogs>();
            RegisterType<IPermissionService, PermissionService>();
            RegisterType<ILoadingIndicator, LoadingIndicator>();

           
            RegisterViewModel<ModeSelectViewModel>();
            RegisterViewModel<PhotoViewModel>();
            RegisterViewModel<TakePhotoViewModel>();
            RegisterViewModel<SplashViewModel>();
            RegisterViewModel<PhotoMirrorViewModel>();

            ContainerBuilder.RegisterType<AppContext>().As<IAppContext>().SingleInstance();

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
