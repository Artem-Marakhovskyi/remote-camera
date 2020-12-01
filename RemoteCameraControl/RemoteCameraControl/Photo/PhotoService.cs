using System;
namespace RemoteCameraControl.Photo
{
    public class PhotoService : IPhotoService
    {
        private readonly IPermissionService _permissionService;
        private readonly IDialogs _dialogs;
        private readonly INavigationService _navigationService;
        private readonly ILogger _logger;

        public PhotoService(
            IPermissionService permissionService,
            IDialogs dialogs,
            INavigationService navigationService,
            ILogger logger)
        {
            this._permissionService = permissionService;
            this._dialogs = dialogs;
            this._navigationService = navigationService;
            this._logger = logger;
        }

        public async Task<IRelatedFile> TakeOrSelectPhoto(FileRelationshipType fileRelationshipType)
        {
            var sourceType = await SelectPhotoSourceType();
            if (sourceType == PhotoSourceType.None) { return null; }
            return await TakeOrSelectPhotoImpl(sourceType, fileRelationshipType, null, false);
        }

        public Task<IRelatedFile> TakePhoto(FileRelationshipType fileRelationshipType, TakePhotoViewModelNavigationParameter parameter = null)
        {
            if (parameter == null)
            {
                parameter = new TakePhotoViewModelNavigationParameter { CanSkip = false, SourceType = PhotoSourceType.Gallery, ReturnBackType = null };
            }

            parameter.SourceType = PhotoSourceType.Camera;
            return TakeOrSelectPhotoImpl(fileRelationshipType, parameter);
        }

        public async Task TakePhotoAndNavigateTo(Type targetViewModelType, bool canSkip = true, CompoundNavigationParameter navigationParameter = null)
        {
            navigationParameter = navigationParameter ?? new CompoundNavigationParameter();
            var takePhotoParameter = new TakePhotoViewModelNavigationParameter { SourceType = PhotoSourceType.Camera, ReturnBackType = targetViewModelType, CanSkip = canSkip };
            navigationParameter.Push(takePhotoParameter);
            await _navigationService.NavigateTo<TakePhotoViewModel, IEnumerable<PhotoResult>>(navigationParameter);
        }

        public Task<IRelatedFile> SelectPhoto(FileRelationshipType fileRelationshipType)
        {
            return TakeOrSelectPhotoImpl(PhotoSourceType.Gallery, fileRelationshipType, null, false);
        }

        private Task<IRelatedFile> TakeOrSelectPhotoImpl(PhotoSourceType photoSourceType, FileRelationshipType fileRelationshipType, Type targetVMType, bool canSkip = true)
        {
            var navigationParameter = new TakePhotoViewModelNavigationParameter { CanSkip = canSkip, SourceType = photoSourceType, ReturnBackType = targetVMType };
            return TakeOrSelectPhotoImpl(fileRelationshipType, navigationParameter);
        }

        private Task<IRelatedFile> TakeOrSelectPhotoImpl(FileRelationshipType fileRelationshipType, TakePhotoViewModelNavigationParameter navigationParameter)
        {
            var compoundNavigationParameter = new CompoundNavigationParameter();
            compoundNavigationParameter.Push(navigationParameter);
            return TakeOrSelectPhotoImpl(fileRelationshipType, compoundNavigationParameter);
        }

        private async Task<IRelatedFile> TakeOrSelectPhotoImpl(FileRelationshipType fileRelationshipType, CompoundNavigationParameter compoundNavigationParameter)
        {
            var takePhotoResults = await _navigationService.NavigateTo<TakePhotoViewModel, IEnumerable<PhotoResult>>(compoundNavigationParameter);

            return await GetRelatedFileFromResult(fileRelationshipType, takePhotoResults);
        }

        public Task<IRelatedFile> GetRelatedFileFromResult(FileRelationshipType fileRelationshipType,
            IEnumerable<PhotoResult> takePhotoResults)
        {
            // only one photo is supported for now
            var photo = takePhotoResults.FirstOrDefault();
            // itemId will be linked later during file save to local storage
            string emptyItemId = null;
            if (photo?.PhotoStream != null && photo.State == TakePhotoResultType.Success)
            {
                return CreateRelatedFile(photo, emptyItemId, fileRelationshipType);
            }
            return Task.FromResult<IRelatedFile>(null);
        }

        public void ViewPhoto(IRelatedFile photo)
        {
            // Do not open View mode if file is not image since View mode for other types is not implemented yet.
            if (photo.Type != FileType.Image)
            {
                return;
            }

            var compoundNavigationParameter = new CompoundNavigationParameter();
            compoundNavigationParameter.Push(photo);

            _navigationService.ShowModal<ViewPhotoViewModel>(compoundNavigationParameter);
        }

        private async Task<IRelatedFile> CreateRelatedFile(PhotoResult photoResult, string itemId, FileRelationshipType fileRelationshipType)
        {
            if (photoResult?.PhotoStream == null)
            {
                _logger.LogError("Photo Stream can not be null");
                return null;
            }

            using (var stream = photoResult.PhotoStream)
            {
                var now = DateTime.Now;
                // TODO: this will be used for camera photo and selected images,
                // in the future the original filename can be pass on when selecting an
                // image from the galley so it be more relatable to the user 
                var filename = $"Photo_{now:yyyyddmmHHmmss}.jpg";
                return await RelatedFileBuilder.FromStream(stream, filename)
                    .WithRelationship(fileRelationshipType, itemId)
                    .CreateAsync();
            }
        }

        private async Task<PhotoSourceType> SelectPhotoSourceType()
        {
            string[] buttons = { Resources.Resources.Camera, Resources.Resources.Library };
            string x = await _dialogs.ShowCancellableActionSheetAsync(Resources.Resources.AddPhoto, Resources.Resources.Cancel, buttons);

            return PhotoSourceTypeSelected(x);
        }

        private PhotoSourceType PhotoSourceTypeSelected(string selectedType)
        {
            if (selectedType == Resources.Resources.Library) { return PhotoSourceType.Gallery; }
            if (selectedType == Resources.Resources.Camera) { return PhotoSourceType.Camera; }
            if (selectedType == Resources.Resources.Cancel) { return PhotoSourceType.None; }
            else
            {
                throw new NotSupportedException($"SourceType {selectedType} is not supported");
            }
        }
    }
}
