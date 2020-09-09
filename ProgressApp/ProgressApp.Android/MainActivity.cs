using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ZXing;
using System.IO;
using Android.Net;
using Uri = Android.Net.Uri;
using Environment = Android.OS.Environment;
using Plugin.Permissions;
using Android.Util;
using Android.Graphics;

namespace ProgressApp.Droid
{
    [Activity(Label = "ProgressApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
          
            //var status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
            //if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            //{
            //    //var a = Environment.ExternalStorageDirectory;
            //    //var png = SearchFileAsync(a, "erweima.png");
            //    Bitmap bitmap = BitmapFactory.DecodeFile("/storage/emulated/0/erweima.png");
            //    FileStream sf = new FileStream("/storage/emulated/0/erweima.png", FileMode.Open);//"/storage/emulated/0/erweima.png"
            //    var fbs = new byte[sf.Length];
            //    sf.Read(fbs, 0, (int)sf.Length);
            //    ZXing.
            //    var source = new ZXing.RGBLuminanceSource(fbs, bitmap.Width, bitmap.Height);
            //    var bitmap1 = new ZXing.BinaryBitmap(new ZXing.Common.HybridBinarizer(source));
            //    var reader = new ZXing.QrCode.QRCodeReader();
            //    var result = reader.decode(bitmap1).Text;
            //}


        }


        string SearchFileAsync(Java.IO.File file, string searchKey)
        {

            var files = file.ListFiles();

            for (int i = 0; i < files.Length; i++)
            {

                var f = files[i];
                if (f.IsFile)
                {
                    var path = f.AbsolutePath;
                    var ext = System.IO.Path.GetExtension(path);
                    Log.Debug("55", ext);
                    if (f.Name == searchKey)
                    {
                        return path;
                    }
                }
                else if (!f.IsHidden && f.IsDirectory)
                {
                    SearchFileAsync(f, searchKey);
                }
            }
            return null;
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}