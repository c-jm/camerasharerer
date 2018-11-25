using System.Linq;

using Android.App;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;


namespace CameraSharerer.Managers
{
    public class PermissionsManager
    {
        private const int PERMISSIONS_REQUEST_CODE = 1001;
        private string[] _permissions;
        private Activity _activity;


        public PermissionsManager(Activity currentActivity, string[] permissions)
        {
            _activity = currentActivity;
            _permissions = permissions;
        }

        private bool IsPermissionEnabled(string name) => ContextCompat.CheckSelfPermission(_activity.ApplicationContext, name) != Permission.Granted;

        public void EnablePermissions()
        {
            var neededPermissions = _permissions.Where(p => !IsPermissionEnabled(p)).ToArray();
            ActivityCompat.RequestPermissions(_activity, neededPermissions, PERMISSIONS_REQUEST_CODE);
        }
    }
}