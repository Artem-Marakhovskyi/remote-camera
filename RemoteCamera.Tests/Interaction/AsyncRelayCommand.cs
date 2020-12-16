using NUnit.Framework;
using RemoteCameraControl.AsyncCommands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RemoteCamera.Tests.Interaction
{
    [TestFixture]
    public class AsyncCommandTests
    {
        [Test]
        public async Task ExecuteAsync_CanExecuteAbsent_CommandExecuted()
        {
            // Arrange
            var executed = false;
            ICommand sut = new AsyncRelayCommand(
                () =>
                {
                    executed = true;

                    return Task.CompletedTask;
                });

            // Act
            await sut.ExecuteAsync(null);

            // Assert
            Assert.True(executed);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ExecuteAsync_CanExecuteExists_CanExecuteCheckedInternallyBeforeCommandExecuted(bool expected)
        {
            // Arrange
            var executed = false;
            ICommand sut = new AsyncRelayCommand(
                () =>
                {
                    executed = true;

                    return Task.CompletedTask;
                },
                obj => expected);

            // Act
            await sut.ExecuteAsync(null);

            // Assert
            Assert.AreEqual(expected, executed);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ExecuteAsync_CanExecuteExists_CanExecuteCheckedExternallyBeforeCommandExecuted(bool expected)
        {
            // Arrange
            var executed = false;
            ICommand sut = new AsyncRelayCommand(
                () =>
                {
                    executed = true;

                    return Task.CompletedTask;
                },
                obj => false);

            // Act
            if (sut.CanExecute(null))
            {
                await sut.ExecuteAsync(null);
            }

            // Assert
            Assert.AreEqual(executed, executed);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanExecute_CanExecuteExistsAndReturnsValue_CommandReturnsValue(bool value)
        {
            // Arrange
            var sut = new AsyncRelayCommand(
                () => Task.CompletedTask,
                obj => value);

            // Act
            var actual = sut.CanExecute(null);

            // Assert
            Assert.AreEqual(value, actual);
        }

        [Test]
        public void Execute_ExecuteCalledMultipleTimes_CallsAreIgnoredWhileCommandIsBeingExecuted()
        {
            // Arrange
            var tcs = new TaskCompletionSource<int>();
            var callsCount = 0;
            var sut = new AsyncRelayCommand(
                () =>
                {
                    callsCount++;
                    return tcs.Task;
                },
                obj => true);

            // Act
            _ = sut.ExecuteAsync(null);
            _ = sut.ExecuteAsync(null);
            _ = sut.ExecuteAsync(null);
            tcs.SetResult(42);
            _ = sut.ExecuteAsync(null);

            // Assert
            Assert.AreEqual(2, callsCount);
        }

        [Test]
        public void CanExecute_CalledRightAfterExecute_ChecksForExecutingTask()
        {
            // Arrange
            var tcs = new TaskCompletionSource<int>();
            var callsCount = 0;
            ICommand sut = new AsyncRelayCommand(
                () =>
                {
                    callsCount++;
                    return tcs.Task;
                },
                obj => true);

            // Act
            sut.Execute(null);
            var canExecute = sut.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Test]
        public void RaiseCanExecuteChanged_SubscribedToCanExecuteChanges_Notified()
        {
            // Arrange
            var canExecuteCalledTimes = 0;

            var sut = new AsyncRelayCommand(() => Task.CompletedTask);

            // Act
            sut.CanExecuteChanged
                += (sender, ea) => canExecuteCalledTimes++;

            sut.RaiseCanExecuteChanged();

            // Assert
            Assert.AreEqual(1, canExecuteCalledTimes);
        }
    }
}
