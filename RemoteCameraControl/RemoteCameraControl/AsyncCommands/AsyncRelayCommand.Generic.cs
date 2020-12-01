﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Helpers;

namespace RemoteCameraControl.AsyncCommands
{
  public class AsyncRelayCommand<T> : ICommand
    {
        private readonly WeakFunc<T, Task> _execute;

        private readonly WeakFunc<T, bool> _canExecute;

        private bool _canExecuteInternal;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class that can always execute.
        /// </summary>
        /// <param name="asyncExecute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public AsyncRelayCommand(
            Func<T, Task> asyncExecute,
            bool keepTargetAlive = true)
            : this(null, asyncExecute, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class that can always execute.
        /// </summary>
        /// <param name="asyncExecute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public AsyncRelayCommand(
            Func<Task> asyncExecute,
            bool keepTargetAlive = true)
            : this(null, arg => asyncExecute(), keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class that checks before execution.
        /// </summary>
        /// <param name="asyncExecute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="canExecute">Check that happens before execution.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public AsyncRelayCommand(
            Func<T, Task> asyncExecute,
            Func<T, bool> canExecute,
            bool keepTargetAlive = true)
            : this(canExecute, asyncExecute, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class that checks before execution.
        /// </summary>
        /// <param name="asyncExecute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="canExecute">Check that happens before execution.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public AsyncRelayCommand(
            Func<Task> asyncExecute,
            Func<T, bool> canExecute,
            bool keepTargetAlive = true)
            : this(canExecute, arg => asyncExecute(), keepTargetAlive)
        {
        }

        private AsyncRelayCommand(
            Func<T, bool> canExecute,
            Func<T, Task> execute,
            bool keepTargetAlive = true)
        {
            _execute = new WeakFunc<T, Task>(execute, keepTargetAlive);

            _canExecute = new WeakFunc<T, bool>(
                canExecute == null
                    ? obj => true
                    : new Func<T, bool>(obj => canExecute(obj)),
                keepTargetAlive);

            _canExecuteInternal = true;
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
            => CanExecute((T)parameter);

        public bool CanExecute(T parameter)
        {
            if (!_canExecuteInternal)
            {
                return false;
            }

            return (_canExecute.IsStatic || _canExecute.IsAlive)
                && _canExecute.Execute(parameter);
        }

        public async void Execute(object parameter)
            => await ExecuteAsync((T)parameter);

        public async void Execute(T parameter)
            => await ExecuteAsync(parameter);

        public async Task ExecuteAsync(T parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                _canExecuteInternal = false;
                if (_execute.IsStatic || _execute.IsAlive)
                {
                    await _execute.Execute((T)parameter);
                }
            }
            finally
            {
                _canExecuteInternal = true;
            }
        }

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
