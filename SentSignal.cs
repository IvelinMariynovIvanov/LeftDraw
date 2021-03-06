﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Telephony;
using Android.Runtime;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Android.Net;
using Android.Graphics.Drawables;
using System.Threading;

namespace DesignLibrary_Tutorial
{
    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;

        public static Bitmap bitmap;
    }

    [Activity(Label = "Сигнал за авария", Theme = "@style/Theme.DesignDemo")]
    public class SentSignal : AppCompatActivity
    {
        private string apiCommand = "api/postimage";

        public static string urlAPI = "http://1192.168.2.222/VIKWebApi/";

        private Button addPicFromGalary;
        private Button addPicFromCamera;
        private Button sent;
        private ImageView pic;
        private Bitmap picImage;
        private Android.Telephony.TelephonyManager mTelephonyMgr;
        private EditText mCity;
        private EditText mAddress;
        private EditText mDescription;
        private EditText mFullName;
        private EditText mPhoneNumber;

        private bool mIsFromGalleryPressed;
        private Android.Support.V7.Widget.Toolbar mToolBar;
        private TextView mError;

        private Uri mSaveImageUri;
        Android.App.ProgressDialog progress;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AccidentView1);


            mToolBar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.basicToolbar);
            SetSupportActionBar(mToolBar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            sent = FindViewById<Button>(Resource.Id.Sent);
            addPicFromCamera = FindViewById<Button>(Resource.Id.Camera);
            addPicFromGalary = FindViewById<Button>(Resource.Id.Gallery);
            mCity = FindViewById<EditText>(Resource.Id.City);
            mAddress = FindViewById<EditText>(Resource.Id.Address);
            mDescription = FindViewById<EditText>(Resource.Id.Description);
            mFullName = FindViewById<EditText>(Resource.Id.FullName);
            mPhoneNumber = FindViewById<EditText>(Resource.Id.phoneNumber);
            mError = FindViewById<TextView>(Resource.Id.error);

            pic = FindViewById<ImageView>(Resource.Id.imageView1);

            mIsFromGalleryPressed = false;
            mError.Visibility = ViewStates.Invisible;


            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                addPicFromCamera = FindViewById<Button>(Resource.Id.Camera);
                pic = FindViewById<ImageView>(Resource.Id.imageView1);

                addPicFromCamera.Click += TakeAPicture;
            }

            addPicFromGalary.Click += (object sender, EventArgs e) =>
            {
                mIsFromGalleryPressed = true;

                var intent = new Intent();
                intent.SetType("image/*");
                intent.SetAction(Intent.ActionGetContent);
                this.StartActivityForResult(Intent.CreateChooser(intent, "Select a photo"), 0);

            };

            sent.Visibility = ViewStates.Visible;
            sent.Click += Sent_Click;

            #region Make sent button invisible
            // MakeVisibleSentButton();

            //  sent.Visibility = ViewStates.Invisible;

            //mAddress.TextChanged += MAddress_TextChanged;
            //mCity.TextChanged += MCity_TextChanged;
            //mDescription.TextChanged += MDescription_TextChanged;
            //mPhoneNumber.TextChanged += MPhoneNumber_TextChanged;
            #endregion

        }

        private void MPhoneNumber_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            UnlockSentButton();
        }

        private void MDescription_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            UnlockSentButton();
        }

        private void MCity_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            UnlockSentButton();
        }

        private void MAddress_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            UnlockSentButton();
        }

        private void MakeVisibleSentButton()
        {
            if (mCity.Text.Trim().Length > 0
                                         && mAddress.Text.Trim().Length > 0
                                         && mDescription.Text.Trim().Length > 0
                                         && mPhoneNumber.Text.Trim().Length > 0)
            {
                sent.Visibility = ViewStates.Visible;
            }
            else
            {
                sent.Visibility = ViewStates.Invisible;
            }
        }

        private void UnlockSentButton()
        {

            if (mCity.Text.Trim().Length > 0
                                         && mAddress.Text.Trim().Length > 0
                                         && mDescription.Text.Trim().Length > 0
                                         && mPhoneNumber.Text.Trim().Length > 0)
            {
                sent.Enabled = true;
            }
            else
            {
                sent.Enabled = false;
            }
        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //from gallery
            if (mIsFromGalleryPressed)
            {
                if (resultCode == Result.Ok)
                {

                    //Stream stream = ContentResolver.OpenInputStream(data.Data);  // data parameter from method
                    //Bitmap bitmap = BitmapFactory.DecodeStream(stream);
                    // pic = FindViewById<ImageView>(Resource.Id.imageView2);
                    //pic.SetImageBitmap(bitmap);

                    //  pic = FindViewById<ImageView>(Resource.Id.imageView2);
                    pic = FindViewById<ImageView>(Resource.Id.imageView1);
                    pic.SetImageURI(data.Data);

                    mSaveImageUri = data.Data;
                    //  pic.Visibility = ViewStates.Visible;
                    mIsFromGalleryPressed = false;

                }

            }
            //from camera
            else
            {
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Uri contentUri = Uri.FromFile(App._file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);

                // Display in ImageView. We will resize the bitmap to fit the display.
                // Loading the full sized image will consume to much memory
                // and cause the application to crash.

                int height = Resources.DisplayMetrics.HeightPixels;
                int width = pic.Height;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                if (App.bitmap != null)
                {
                    pic.SetImageBitmap(App.bitmap);
                    mSaveImageUri = Uri.FromFile(App._file);
                    //   pic.Visibility = ViewStates.Visible;
                    App.bitmap = null;
                }
                // Dispose of the Java side bitmap.
                GC.Collect();
            }

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.basic_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }

        private void TakeAPicture(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            App._file = new Java.IO.File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));

            StartActivityForResult(intent, 0);
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new Java.IO.File(
        Environment.GetExternalStoragePublicDirectory(
            Environment.DirectoryPictures), "CameraAppDemo");

            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);

            return availableActivities != null && availableActivities.Count > 0;
        }

        private void Sent_Click(object sender, EventArgs e)
        {
            mError.Visibility = ViewStates.Invisible;

            RunOnUiThread(() => { ShowProgressDialog(); });

            Thread thread = new Thread(SentAccindentSignelToApi);
            thread.Start();

        }

        private void SentAccindentSignelToApi()
        {
            ConnectToApi connectToApi = new ConnectToApi();

            bool connection = connectToApi.CheckConnectionOfVikSite();

            if (mCity.Text.Trim().Length > 3
                         && mAddress.Text.Trim().Length > 3
                         && mDescription.Text.Trim().Length > 3
                         && mPhoneNumber.Text.Trim().Length > 3)
            {
                #region old stuff
                //// casting imageview to bitmap
                //Android.Graphics.Drawables.BitmapDrawable bd =
                //    (Android.Graphics.Drawables.BitmapDrawable)pic.Drawable;

                //Android.Graphics.Bitmap bitmap = bd.Bitmap;

                //using (var stream = new MemoryStream())
                //{
                //    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
                //    PostItem(stream);
                //}
                #endregion
                if (connection == true)
                {
                    if (mSaveImageUri != null)
                    {
                        Stream stream = ContentResolver.OpenInputStream(mSaveImageUri);

                        PostAccidentToDBwithImage(stream);

                    }
                    else
                    {
                        PostAccidentToDBwithoutImage();
                    }
                }
                else
                {
                    RunOnUiThread(() => RefreshProgressDialogAndToastWhenThereIsNoInternet());
                }
            }
            else
            {
                progress.Dismiss();

                Looper.Prepare();
                //Toast.MakeText(this, "Попълнете полетата", ToastLength.Long);

                RunOnUiThread(() => { UpdateError(); });
            }
        }

        private void UpdateError()
        {
            mError.Text = "Попълнете полетата";
            mError.Visibility = ViewStates.Visible;
        }

        private void ShowProgressDialog()
        {
            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Зареждане... Моля изчакайте...");
            progress.SetCancelable(false);
            progress.Show();
        }

        private void PostAccidentToDBwithoutImage()
        {
            Looper.Prepare();

            HttpClientHandler handler = new HttpClientHandler();

            CookieContainer cookies = new CookieContainer();

            handler.CookieContainer = cookies;

            var client = new System.Net.Http.HttpClient(handler, false);
            {
                try
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();

                    StringContent number = new StringContent(mPhoneNumber.Text.ToString());
                    StringContent description = new StringContent(mDescription.Text.ToString());
                    StringContent city = new StringContent(mCity.Text.ToString());
                    StringContent address = new StringContent(mAddress.Text.ToString());
                    StringContent name = new StringContent(mFullName.Text.ToString());


                    form.Add(number, "number");
                    form.Add(description, "description");
                    form.Add(city, "city");
                    form.Add(address, "address");
                    form.Add(name, "name");

                    HttpResponseMessage response = client.PostAsync(urlAPI + apiCommand, form).Result;

                    if (response.StatusCode == HttpStatusCode.OK)  // response.IsSuccessStatusCode
                    {
                        RunOnUiThread(() => { Toast.MakeText(this, "Успешно изпратихте сигнал", ToastLength.Long).Show(); });

                        var intent = new Intent(this, typeof(MainActivity));

                        StartActivity(intent);

                    }
                    //HttpResponseMessage response = 
                    //   client.PostAsync("http://192.168.2.222/VIKWebApi/api/postimage", form).Result;
                    else
                    {
                        Toast.MakeText(this, "Възника грешка при изпращане", ToastLength.Long).Show();
                    }


                }
                catch (Exception ex)
                {

                    RunOnUiThread(() => { RefreshProgressDialogAndToastWhenCanNotSentSignalWithOutImage(); });
                }
            }
        }

        private void RefreshProgressDialogAndToastWhenCanNotSentSignalWithOutImage()
        {
            //Looper.Prepare();

            Toast.MakeText(this, "Възникна грешка при изпращане", ToastLength.Long).Show();
            progress.Dismiss();
            mError.Text = "Възникна грешка при изпращане";
            mError.Visibility = ViewStates.Visible;
        }

        public void CheckOverdue(List<Customer> abonatiList)
        {
            HttpClientHandler handler = new HttpClientHandler();

            var client = new System.Net.Http.HttpClient(handler, false);

            string json = JsonConvert.SerializeObject(abonatiList);

            StringContent data = new StringContent("=" + json, Encoding.UTF8, "application/x-www-form-urlencoded");

            //data.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            HttpResponseMessage response =
                client.PostAsync("http://192.168.2.222/VIKWebApi/api/CheckOverdue/", data).Result;

            string result = response.Content.ReadAsStringAsync().Result;

            List<long> abonatiListReturn = new List<long>();

            JsonConvert.PopulateObject(result, abonatiListReturn);

            string res = response.ToString();
        }

        private void PostAccidentToDBwithImage(Stream stream)
        {
            HttpClientHandler handler = new HttpClientHandler();

            CookieContainer cookies = new CookieContainer();

            handler.CookieContainer = cookies;

            var client = new System.Net.Http.HttpClient(handler, false);
            {
                try
                {

                    MultipartFormDataContent form = new MultipartFormDataContent();

                    StreamContent imageContent = new StreamContent(stream);

                    StringContent number = new StringContent(mPhoneNumber.Text.ToString());
                    StringContent description = new StringContent(mDescription.Text.ToString());
                    StringContent city = new StringContent(mCity.Text.ToString());
                    StringContent address = new StringContent(mAddress.Text.ToString());
                    StringContent name = new StringContent(mFullName.Text.ToString());

                    form.Add(imageContent, "image", "image.jpg");
                    form.Add(number, "number");
                    form.Add(description, "description");
                    form.Add(city, "city");
                    form.Add(address, "address");
                    form.Add(name, "name");

                    HttpResponseMessage response = client.PostAsync(urlAPI + apiCommand, form).Result;

                    if (response.StatusCode == HttpStatusCode.OK)  // response.IsSuccessStatusCode
                    {
                        RunOnUiThread(() => { Toast.MakeText(this, "Успешно изпратихте сигнал", ToastLength.Long).Show(); });

                        var intent = new Intent(this, typeof(MainActivity));

                        StartActivity(intent);

                    }
                    //HttpResponseMessage response = 
                    //   client.PostAsync("http://192.168.2.222/VIKWebApi/api/postimage", form).Result;
                    else
                    {
                        Toast.MakeText(this, "Възникна грешка при изпращане", ToastLength.Long).Show();
                    }


                }
                catch (Exception ex)
                {
                    //  Looper.Prepare();

                    RunOnUiThread(()
                        =>
                    {
                        Toast.MakeText(this, "Възникна грешка при изпращане", ToastLength.Long).Show();
                        progress.Dismiss();
                        mError.Text = "Възникна грешка при изпращане";
                        mError.Visibility = ViewStates.Visible;
                    }
                    );
                }
            }


        }

        private void RefreshProgressDialogAndToastWhenThereIsNoInternet()
        {
            Toast.MakeText(this, "Проверете интернет връзката", ToastLength.Long);
            mError.Text = "Проверете интернет връзката";
            mError.Visibility = ViewStates.Visible;

            progress.Dismiss();
        }
    }
}