using Acr.UserDialogs;
using Moq;
using NUnit.Framework;
using RemoteCameraControl.RemoteCameraControl.Interaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteCamera.Tests.Interaction
{
    public class DialogsTests
    {
        private Mock<IUserDialogs> _userDialogs;
        private Mock<ILoadingIndicator> _loadingIndicatorMock;
        private Mock<IMultipleChoiceDialog> _multipleChoiceDialogMock;
        private Mock<ICancellableActionSheetDialogFactory> _cancellableActionSheetDialogMock;

        [SetUp]
        public void SetUp()
        {
            _userDialogs = new Mock<IUserDialogs>();
            _loadingIndicatorMock = new Mock<ILoadingIndicator>();
            _multipleChoiceDialogMock = new Mock<IMultipleChoiceDialog>();
            _cancellableActionSheetDialogMock = new Mock<ICancellableActionSheetDialogFactory>();
        }

        [Test]
        public async Task Should_DelegateToPluginWithProperParameters_When_ConfirmAsync()
        {
            var dialogs = new Dialogs(_userDialogs.Object, _loadingIndicatorMock.Object,
                _multipleChoiceDialogMock.Object, _cancellableActionSheetDialogMock.Object);

            var message = "Message";
            var title = "Title";
            var okText = "Ok";
            var cancelText = "Cancel";

            await dialogs.ConfirmAsync(message, title, okText, cancelText);

            _userDialogs.Verify(ud => ud.ConfirmAsync(message, title, okText, cancelText, CancellationToken.None));
        }

        [Test]
        public void Should_DelegateToLoadingIndicator_When_HideAsync()
        {
            var dialogs = new Dialogs(_userDialogs.Object, _loadingIndicatorMock.Object,
                _multipleChoiceDialogMock.Object, _cancellableActionSheetDialogMock.Object);

            dialogs.HideLoading();

            _loadingIndicatorMock.Verify((li) => li.HideAsync(It.IsAny<bool>()));
        }

        [Test]
        public async Task Should_UseCancellationToken_When_AlertAsync()
        {
            var dialogs = GetSut();

            var cts = new CancellationTokenSource();

            _loadingIndicatorMock.Setup(e => e.IsShownAsync()).ReturnsAsync(false);

            await dialogs.AlertAsync(
                string.Empty, string.Empty, string.Empty, cts.Token);

            _userDialogs.Verify(
                e => e.AlertAsync(
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    cts.Token),
                Times.Once);
        }

        [Test]
        public async Task Should_UseCancellationToken_When_ConfirmAsync()
        {
            var dialogs = GetSut();

            var cts = new CancellationTokenSource();

            _loadingIndicatorMock.Setup(e => e.IsShownAsync()).ReturnsAsync(false);

            await dialogs.ConfirmAsync(
                string.Empty, string.Empty, string.Empty, string.Empty, cts.Token);

            _userDialogs.Verify(
                e => e.ConfirmAsync(
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    cts.Token),
                Times.Once);
        }

        [Test]
        public void ShouldDelegateToLoadingIndicateor_When_ShowAsync()
        {
            var dialogs = new Dialogs(_userDialogs.Object, _loadingIndicatorMock.Object,
                _multipleChoiceDialogMock.Object, _cancellableActionSheetDialogMock.Object);

            dialogs.ShowLoading();

            _loadingIndicatorMock.Verify(li => li.ShowAsync(It.IsAny<string>()));
        }

        [Test]
        public async Task AlertAsync_CalledAfterLoading_AlertHidesAndRestoresLoadingWithSavedMessage()
        {
            // Arrange
            var dialogs = GetSut();
            var message = "loading";

            _loadingIndicatorMock.Setup(x => x.IsShownAsync()).ReturnsAsync(true);

            // Act
            // Assert
            dialogs.ShowLoading(message);

            await dialogs.AlertAsync(string.Empty, string.Empty, string.Empty);

            var mockSequence = new MockSequence();
            _loadingIndicatorMock.Verify(li => li.HideAsync(true));
            _loadingIndicatorMock.Verify(li => li.ShowAsync(message), Times.Exactly(2));
        }

        [Test]
        public async Task ConfirmAsync_CalledAfterLoading_ConfirmHidesAndRestoresLoadingWithSavedMessage()
        {
            // Arrange
            var dialogs = GetSut();
            var message = "loading";

            _loadingIndicatorMock.Setup(x => x.IsShownAsync()).ReturnsAsync(true);

            // Act
            // Assert
            dialogs.ShowLoading(message);

            await dialogs.ConfirmAsync(string.Empty, string.Empty, string.Empty, string.Empty);

            var mockSequence = new MockSequence();
            _loadingIndicatorMock.Verify(li => li.HideAsync(true));
            _loadingIndicatorMock.Verify(li => li.ShowAsync(message), Times.Exactly(2));
        }

        private Dialogs GetSut()
        {
            return new Dialogs(
                _userDialogs.Object,
                _loadingIndicatorMock.Object,
                _multipleChoiceDialogMock.Object,
                _cancellableActionSheetDialogMock.Object);
        }
    }
}
