using System;
namespace RemoteCameraControl.Photo
{
    public interface IPhotoService
    {
        /// <summary>
        /// Shows a action sheet with options to take a photo with the camera or select one from the device gallery.
        /// </summary>
        /// <param name="fileRelationshipType">The file relationship of the created <c>IRelatedFile</c></param>
        /// <returns>A new <c>IRelatedFile</c> with the content of the new or selected photo, or <c>null</c> if canceled</returns>
        Task<IRelatedFile> TakeOrSelectPhoto(FileRelationshipType fileRelationshipType);

        /// <summary>
        /// Take a photo with the camera and create a new <c>IRelatedFile</c>.
        /// </summary>
        /// <param name="fileRelationshipType">The file relationship of the created <c>IRelatedFile</c></param>
        /// <returns>A new <c>IRelatedFile</c> with the content of the new photo, or <c>null</c> if canceled</returns>
        Task<IRelatedFile> TakePhoto(FileRelationshipType fileRelationshipType, TakePhotoViewModelNavigationParameter parameter = null);

        /// <summary>
        /// Take a photo with the camera followed by navigating to the <paramref name="targetViewModelType"/>
        /// view model with the create <c>IRelatedFile</c> that should be retrieved using <see cref="GetRelatedFileFromResult"/>
        /// </summary>
        /// <param name="targetViewModelType">Type of the next screen view model</param>
        /// <param name="canSkip">If take photo action can be skipped</param>
        /// <returns>A task to the navigation service</returns>
        Task TakePhotoAndNavigateTo(Type targetViewModelType, bool canSkip = true, CompoundNavigationParameter navigationParameter = null);

        /// <summary>
        /// Select a photo from the gallery a and create a new <c>IRelatedFile</c>.
        /// </summary>
        /// <param name="fileRelationshipType">The file relationship of the created <c>IRelatedFile</c></param>
        /// <returns>A new <c>IRelatedFile</c> with the content of the selected photo, or <c>null</c> if canceled</returns>
        Task<IRelatedFile> SelectPhoto(FileRelationshipType fileRelationshipType);

        /// <summary>
        /// Retrieve the <c>IRelatedFile</c> from the navigation parameters 
        /// </summary>
        /// <param name="fileRelationshipType">The file relationship of the created <c>IRelatedFile</c></param>
        /// <param name="takePhotoResults">The parameter pass on the CompoundNavigationParameter </param>
        /// <returns>A new <c>IRelatedFile</c> with the content of the selected or taken photo</returns>
        Task<IRelatedFile> GetRelatedFileFromResult(FileRelationshipType fileRelationshipType, IEnumerable<PhotoResult> takePhotoResults);

        /// <summary>
        /// Open a full screen view to display a <c>IRelatedFile</c>
        /// </summary>
        /// <param name="photo">The <c>IRelatedFile</c> whose content should be displayed</param>
        void ViewPhoto(IRelatedFile photo);
    }
}
