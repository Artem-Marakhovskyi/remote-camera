using System.Threading.Tasks;

namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
    public class LoadingIndicator : ILoadingIndicator
    {
        private readonly IPlatformLoadingIndicator _innerIndicator;

        private object _synchro = new object();

        private Task _currentIndicationTask;

        private bool? _isShowingPending;

        private bool _isCurrentTaskForShowing;

        public LoadingIndicator(IPlatformLoadingIndicator innerIndicator)
        {
            _innerIndicator = innerIndicator;
        }

        private bool AlreadyHidden
        {
            get => _currentIndicationTask.IsCompleted && !_isCurrentTaskForShowing;
        }

        private bool AlreadyShown
        {
            get => _currentIndicationTask.IsCompleted && _isCurrentTaskForShowing;
        }

        private bool IsHidingInProcess
        {
            get => !_currentIndicationTask.IsCompleted && !_isCurrentTaskForShowing;
        }

        private bool IsShowingInProcess
        {
            get => !_currentIndicationTask.IsCompleted && _isCurrentTaskForShowing;
        }

        public async Task<bool> IsShownAsync()
        {
            if (_currentIndicationTask == null)
            {
                return false;
            }

            if (!_currentIndicationTask.IsCompleted)
            {
                await _currentIndicationTask;
            }

            return _isCurrentTaskForShowing;
        }

        public Task HideAsync(bool forceInstantly)
        {
            lock (_synchro)
            {
                if (_currentIndicationTask == null || AlreadyShown)
                {
                    _currentIndicationTask = _innerIndicator.HideAsync();
                    _isCurrentTaskForShowing = false;
                }
                else if (IsShowingInProcess)
                {
                    if (forceInstantly)
                    {
                        _isShowingPending = null;
                        _currentIndicationTask = _innerIndicator.HideAsync();
                        _isCurrentTaskForShowing = false;
                    }
                    else if (!_isShowingPending.HasValue)
                    {
                        _isShowingPending = false;
                        AttachPendingTask();
                    }
                }
                else if (IsHidingInProcess)
                {
                    _isShowingPending = null;
                }
                else
                {
                    _currentIndicationTask = Task.CompletedTask;
                }

                return _currentIndicationTask;
            }
        }

        public Task ShowAsync(string status)
        {
            lock (_synchro)
            {
                if (_currentIndicationTask == null || AlreadyHidden)
                {
                    _currentIndicationTask = _innerIndicator.ShowAsync(status);
                    _isCurrentTaskForShowing = true;

                    return _currentIndicationTask;
                }
                else if (IsHidingInProcess)
                {
                    if (!_isShowingPending.HasValue)
                    {
                        _isShowingPending = true;
                        AttachPendingTask(status);
                    }
                }
                else if (IsShowingInProcess)
                {
                    _isShowingPending = null;
                }
                else
                {
                    _currentIndicationTask = _innerIndicator.ShowAsync(status);
                }

                return _currentIndicationTask;
            }
        }

        private void AttachPendingTask(string status = "")
        {
            if (_isShowingPending.HasValue)
            {
                _currentIndicationTask
                    = _currentIndicationTask.ContinueWith(
                        t =>
                        {
                            if (_isShowingPending.Value)
                            {
                                _currentIndicationTask = _innerIndicator.ShowAsync(status);
                                _isCurrentTaskForShowing = true;
                            }
                            else
                            {
                                _currentIndicationTask = _innerIndicator.HideAsync();
                                _isCurrentTaskForShowing = false;
                            }

                            _isShowingPending = null;
                        },
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}