using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using RemoteCamera.HubClient;
using RemoteCameraControl.Android;
using RemoteCameraControl.AsyncCommands;
using RemoteCameraControl.Interaction;
using RemoteCameraControl.Logger;
using RemoteCameraControl.RemoteCameraControl.Interaction;
using XLabs.Ioc;

namespace RemoteCameraControl
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public ILogger Logger { get; }
        public IDialogs DialogService { get; }
        protected INavigationService NavigationService { get; }
        public IAppContext AppContext { get; }

        public ViewModelBase()
        {
            Logger = Resolver.Resolve<ILogger>();
            DialogService = Resolver.Resolve<IDialogs>();
            NavigationService = Resolver.Resolve<INavigationService>();
            AppContext = Resolver.Resolve<IAppContext>();
        }

        private bool _offlineNotificationCanBeShown;
        private bool _isLoading;
        private AsyncRelayCommand<object> _closeCommand;
        private bool _canExecuteGoBack = true;
        private Task _initializeTask;

        private bool _isGoBackAlreadyPerformed;
        private string _title;
        private string _subTitle;
        private bool _disposed;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public string SubTitle
        {
            get => _subTitle;
            set => Set(ref _subTitle, value);
        }

        public object NavigationParameter { get; private set; }

        public virtual bool IsModified => false;

        public bool IsLoading
        {
            get => _isLoading;
            private set => Set(ref _isLoading, value);
        }

        /// <summary>
        /// Indicates if navigation parameter should be restored
        /// If this property is false - navigation parameter will not be restored after page recreation
        /// </summary>
        public virtual bool RestoreNavigationParameter => true;

        public async Task Init(object parameter = null)
        {
            if (_initializeTask == null)
            {
                ProcessNavigationParameter(parameter);
                _initializeTask = Initialize(parameter);
            }

            await InitializeTask();

            OnInit();
        }

        public virtual object GetStateForReuse() => null;

        public virtual void RestoreState(object state) { }

        public virtual void OnNavigationResult(object result) { }

        public async void Start()
        {
            await InitializeTask();

        }

        public async void Resume()
        {
            await InitializeTask();


            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(SubTitle));

            OnResume();
        }

        public async void Resumed()
        {
            await InitializeTask();

            OnResumed();
        }

        public void Pause()
        {
            OnPause();
        }

        public void Stop()
        {
            OnStop();

        }

        public void Activating()
        {
            OnActivating();
        }

        public void Deactivated()
        {
            OnDeactivated();
        }

        private Task InitializeTask() => _initializeTask ?? Task.CompletedTask;

        protected virtual Task Initialize(object parameter)
        {
            return Task.CompletedTask;
        }

        protected virtual void OnInit() { }

        protected virtual void OnResume() { }

        protected virtual void OnResumed() { }

        protected virtual void OnPause() { }

        protected virtual void OnStop() { }

        /// <summary>
        /// Called on view entering foreground state
        /// </summary>
        protected virtual void OnActivating() { }

        /// <summary>
        /// Called on view entered background state
        /// </summary>
        protected virtual void OnDeactivated() { }

        /// <summary>
        /// Called in Init Method should be as lightweight as it can be. 
        /// </summary>
        /// <param name="navigationParameter"></param>
        protected virtual void ProcessNavigationParameter(object navigationParameter)
        {
            if (navigationParameter == null)
            {
                return;
            }

            if (navigationParameter is ICloneable cloneable)
            {
                NavigationParameter = cloneable.Clone();
            }
            else
            {
                NavigationParameter = navigationParameter;
            }
        }

        #region Waiting for executing

        /// <summary>
        /// Executes task AND empty task with duration of minWaitPeriod. Waits for completing all tasks
        /// </summary>
        /// <returns>The with loading.</returns>
        /// <param name="task">Task.</param>
        /// <param name="minWaitPeriod">Minimum wait period.</param>
        protected async Task ExecuteWithLoading(Task task, int minWaitPeriod = 400)
        {
            ShowLoading();
            try
            {
                await Task.WhenAll(task, Task.Delay(minWaitPeriod));
            }
            finally
            {
                HideLoading();
            }
        }

        /// <summary>
        /// Executes the task with loading.
        /// </summary>
        /// <returns>The task with loading.</returns>
        /// <param name="task">Task.</param>
        protected async Task ExecuteTaskWithLoading(Task task, bool showLoader = true, string loadingMessage = null)
        {
            if (showLoader)
            {
                ShowLoading(loadingMessage);
            }

            try
            {
                await task;
            }
            finally
            {
                if (showLoader)
                {
                    HideLoading();
                }
            }
        }

        /// <summary>
        /// Executes the task with loading.
        /// </summary>
        /// <returns>Result of task execution</returns>
        /// <param name="task">Task.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected async Task<T> ExecuteTaskWithLoading<T>(Task<T> task, bool showLoader = true, string loadingMessage = null)
        {
            if (showLoader)
            {
                ShowLoading(loadingMessage);
            }

            T maybeResult;
            try
            {
                maybeResult = await task;
            }
            finally
            {
                if (showLoader)
                {
                    HideLoading();
                }
            }

            return maybeResult;
        }

        #endregion


        #region Go Back

        /// <summary>
        /// Be careful when override this method with async operations,
        /// it may cause unexpected results in GoBack method invocation chain.
        /// Please use GoBackAsync version instead.
        /// </summary>
        public virtual void GoBack(object parameter = null) => NavigateBack(parameter);

        public virtual async Task<NavigationResult> GoBackAsync(object parameter = null)
        {
            if (await ConfirmUnsavedDataAsync())
            {
                return NavigationResult.Canceled;
            }

            GoBack(parameter);
            return NavigationResult.Completed;
        }

        private async Task GoBackWrapperAsync(object parameter)
        {
            _canExecuteGoBack = false;
            await GoBackAsync(parameter);
            _canExecuteGoBack = true;
        }

        protected virtual bool CanExecuteGoBack(object parameter) =>
            _canExecuteGoBack &&
            !_isGoBackAlreadyPerformed &&
            !IsModified;

        public AsyncRelayCommand<object> CloseCommand => _closeCommand
            ?? (_closeCommand = new AsyncRelayCommand<object>(GoBackWrapperAsync, CanExecuteGoBack));

        protected void NavigateBack(object result = null)
        {
            // in addition to CanExecuteGoBack, check here as well to handle case
            // when navigation has not performed yet but second tap alredy made
            if (!_isGoBackAlreadyPerformed)
            {
                _isGoBackAlreadyPerformed = true;
                AppContext.NavigationResult = result;
            }
        }

        #endregion

        #region Loading

        public void ShowLoading(string message = null)
        {
            IsLoading = true;
            DialogService.ShowLoading(message);
        }

        public void HideLoading(bool forceInstantly = false)
        {
            IsLoading = false;
            DialogService.HideLoading(forceInstantly);
        }

        protected virtual void OnConnectivityChanged(object sender, bool e)
        {
        }

        public void SetLoading(bool loading)
        {
            if (loading)
            {
                ShowLoading();
            }
            else
            {
                HideLoading();
            }
        }

        #endregion

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _closeCommand = null;
            _disposed = true;
        }

        protected async Task<bool> ConfirmUnsavedDataAsync()
        {
            return IsModified && await DialogService.ConfirmOnUnsavedDataAsync();
        }


        public virtual void OnRcConnected()
        {
        }

        public virtual void OnSessionFinished()
        {
        }

        public virtual void OnControlMessageReceived(ControlMessage controlMessage)
        {
        }

        public virtual void OnDataMessageReceived(DataMessage dataMessage)
        {
        }

        public virtual void OnTextReceived(string text)
        {
        }

        public virtual void SetInner(IConnectionSignalsHandler inner)
        {
        }
    }
    public enum NavigationResult
    {
        Completed,
        Canceled,
    }
}