
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Widget;

using AndroidFile = Java.IO.File;

namespace CameraSharerer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        private static string[] PERMISSIONS =
        {
            Manifest.Permission.Camera,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.ReadExternalStorage
        };

        private string _path;
        //private string _providerName = $"{BuildConfig.ApplicationId}.fileprovider";
        private string _providerName = $"test.fileprovider";

        private ImageView _imgView;
        private AndroidFile _file;
        private AndroidFile _directory;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var btnTakeAPicture = FindViewById<Button>(Resource.Id.btnTakeAPicture);
            btnTakeAPicture.Click += BtnTakeAPicture_Click;

            _imgView = FindViewById<ImageView>(Resource.Id.imgView);
            _path = $"{Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDcim)}";

            ActivityCompat.RequestPermissions(this, PERMISSIONS, 1);
        }

        private void BtnTakeAPicture_Click(object sender, System.EventArgs e)
        {
            Intent imageIntent = new Intent(MediaStore.ActionImageCapture);

            _directory = new AndroidFile(Environment.ExternalStorageDirectory, "cam-share-pics");
            if (!_directory.Exists())
            {
                _directory.Mkdirs();
            }

            _file = new AndroidFile(_directory, "test.png");

            var uri = FileProvider.GetUriForFile(this, _providerName, _file);

            imageIntent.PutExtra(MediaStore.ExtraOutput, uri);
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

        public static Bitmap LoadAndResizeBitmap(string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }

    }
}

