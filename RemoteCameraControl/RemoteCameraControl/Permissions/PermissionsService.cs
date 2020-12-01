using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;

namespace RemoteCameraControl.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly TimeSpan RequestTimeout = TimeSpan.FromHours(1d);

        private readonly IPermissions _permissions;

        private IDictionary<FeaturePermission, DateTime> _previousRequests = new Dictionary<FeaturePermission, DateTime>();

        public PermissionService(
           IPermissions permissions)
        {
            _permissions = permissions;
        }

        public async Task<PermissionStatus> CheckPermissionStatusAsync(FeaturePermission featurePermission)
        {
            return (PermissionStatus)await _permissions.CheckPermissionStatusAsync(ToPermission(featurePermission));
        }

        public async Task<PermissionStatus> RequestPermissionAsync(FeaturePermission featurePermission)
        {
            var permission = ToPermission(featurePermission);
            var status = await _permissions.CheckPermissionStatusAsync(permission);

            var now = DateTime.Now;

            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted
                && (!_previousRequests.ContainsKey(featurePermission) || _previousRequests[featurePermission].Add(RequestTimeout) < now))
            {
                _previousRequests[featurePermission] = now;
                var results = await _permissions.RequestPermissionsAsync(permission);

                if (results.ContainsKey(permission))
                {
                    status = results[permission];
                }
            }

            return (PermissionStatus)status;
        }

        private Permission ToPermission(FeaturePermission featurePermission)
        {
            var name = Enum.GetName(typeof(FeaturePermission), featurePermission);

            return (Permission)Enum.Parse(typeof(Permission), name);
        }
    }
}
