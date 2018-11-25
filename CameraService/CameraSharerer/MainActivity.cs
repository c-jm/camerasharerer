using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;

using CameraSharerer.Managers;

using AndroidFile = Java.IO.File;

namespace CameraSharerer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private string _providerName = $"com.camerasharer.fileprovider";
        // private const string IP = "192.168.0.111";

        private static string[] PERMISSIONS_NEEDED =
        {
            Manifest.Permission.Camera,
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };

        private ImageView _imgView;
        private AndroidFile _file;
        private AndroidFile _directory;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var btnTakeAPicture = FindViewById<Button>(Resource.Id.btnTakeAPicture);
            btnTakeAPicture.Click += BtnTakeAPicture_Click;

            var btnSendPicture = FindViewById<Button>(Resource.Id.btnSendPicture);
            btnSendPicture.Click += BtnSendPicture_Click;

            _imgView = FindViewById<ImageView>(Resource.Id.imgView);

            //@TODO(cjm): This should probably be done at app Startup, not in the main activity.
            var permissionManager = new PermissionsManager(this, PERMISSIONS_NEEDED);
            permissionManager.EnablePermissions();
        }

        private void BtnSendPicture_Click(object sender, System.EventArgs e)
        {
        }

        private void BtnTakeAPicture_Click(object sender, System.EventArgs e)
        {
            Intent imageIntent = new Intent(MediaStore.ActionImageCapture);

            //@TODO: Make this directory code better. Move it through etc.
            _directory = new AndroidFile(Environment.ExternalStorageDirectory, "cam-share-pics");
            if (!_directory.Exists())
            {
                _directory.Mkdirs();
            }

            _file = new AndroidFile(_directory, UniqueImgFileName());

            // This file provider was a bit of a PITA. 
            // Spent a couple days just trying to get it working.
            imageIntent.PutExtra(MediaStore.ExtraOutput, FileProvider.GetUriForFile(this, _providerName, _file));
            imageIntent.SetFlags(ActivityFlags.GrantWriteUriPermission);
            StartActivityForResult(imageIntent, 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imgView.Width;

            var bitmap = LoadAndResizeBitmap(_file.Path, width, height);

            _imgView.SetImageBitmap(bitmap);
        }

        private string UniqueImgFileName() => $"img_{System.DateTime.Now.Ticks.ToString()}.jpg";

        public static Bitmap LoadAndResizeBitmap(string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }
}

