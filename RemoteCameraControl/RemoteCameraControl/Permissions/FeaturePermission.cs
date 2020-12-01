namespace RemoteCameraControl.Permissions
{
    public enum FeaturePermission
    {
        Location,
        Camera,

        /// <summary>
        /// Android: External Storage
        /// iOS: Nothing
        /// </summary>
        Storage,

        /// <summary>
        /// Android: Nothing
        /// iOS: Photos
        /// </summary>
        Photos,
    }
}
