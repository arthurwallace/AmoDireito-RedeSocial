﻿using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Com.Theartofdev.Edmodo.Cropper;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Event;
using WoWonderClient.Classes.Global;
using WoWonderClient.Requests;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Events
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class CreateEventActivity : AppCompatActivity, View.IOnClickListener
    {
        #region Variables Basic

        private TextView IconStartDate, IconEndDate, IconLocation, TxtAdd;
        private EditText TxtEventName, TxtStartDate, TxtStartTime,TxtEndDate, TxtEndTime, TxtLocation, TxtDescription;
        private ImageView ImageEvent;
        private Button BtnImage; 
        private string EventPathImage = "";

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.CreateEvent_Layout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        protected override void OnDestroy()
        {
            try
            {
                DestroyBasic();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                TxtEventName = FindViewById<EditText>(Resource.Id.eventname);
                IconStartDate = FindViewById<TextView>(Resource.Id.StartIcondate);
                TxtStartDate = FindViewById<EditText>(Resource.Id.StartDateTextview);
                TxtStartTime = FindViewById<EditText>(Resource.Id.StartTimeTextview);
                IconEndDate = FindViewById<TextView>(Resource.Id.EndIcondate);
                TxtEndDate = FindViewById<EditText>(Resource.Id.EndDateTextview);
                TxtEndTime = FindViewById<EditText>(Resource.Id.EndTimeTextview);
                IconLocation = FindViewById<TextView>(Resource.Id.IconLocation);
                TxtLocation = FindViewById<EditText>(Resource.Id.LocationTextview);
                TxtDescription = FindViewById<EditText>(Resource.Id.description);

                ImageEvent = FindViewById<ImageView>(Resource.Id.EventCover);
                BtnImage = FindViewById<Button>(Resource.Id.btn_selectimage);

                TxtAdd = FindViewById<TextView>(Resource.Id.toolbar_title);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconStartDate, IonIconsFonts.AndroidTime);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconEndDate, IonIconsFonts.AndroidTime);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconLocation, IonIconsFonts.Location);

                Methods.SetColorEditText(TxtEventName, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtStartTime, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtStartDate, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtEndDate, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtEndTime, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtLocation, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtDescription, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                 
                TxtStartTime.SetOnClickListener(this);
                TxtEndTime.SetOnClickListener(this);
                TxtStartDate.SetOnClickListener(this);
                TxtEndDate.SetOnClickListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetText(Resource.String.Lbl_Create_Events);
                    toolbar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    TxtAdd.Click += TxtAddOnClick;
                    TxtLocation.FocusChange += TxtLocationOnFocusChange;
                    BtnImage.Click += BtnImageOnClick;
                }
                else
                {
                    TxtAdd.Click -= TxtAddOnClick;
                    TxtLocation.FocusChange -= TxtLocationOnFocusChange;
                    BtnImage.Click -= BtnImageOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DestroyBasic()
        {
            try
            {
                TxtEventName = null;
                IconStartDate= null;
                TxtStartDate = null;
                TxtStartTime = null;
                IconEndDate = null;
                TxtEndDate = null;
                TxtEndTime = null;
                IconLocation = null;
                TxtLocation = null;
                TxtDescription = null; 
                ImageEvent = null;
                BtnImage = null; 
                TxtAdd = null;
                EventPathImage = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        #endregion

        #region Events

        private void BtnImageOnClick(object sender, EventArgs e)
        {
            try
            {
                OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TxtLocationOnFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                if (e.HasFocus)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        //Open intent Location when the request code of result is 502
                        new IntentController(this).OpenIntentLocation();
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted && CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                        {
                            //Open intent Location when the request code of result is 502
                            new IntentController(this).OpenIntentLocation();
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(105);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void TxtAddOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {
                    if (string.IsNullOrEmpty(TxtEventName.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_name), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtStartDate.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_start_date), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtEndDate.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_end_date), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtLocation.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Location), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtStartTime.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_start_time), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtEndTime.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_end_time), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtDescription.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_Description), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(EventPathImage))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_select_Image), ToastLength.Short).Show();
                    }
                    else
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                        var (apiStatus, respond) = await RequestsAsync.Event.Create_Event(TxtEventName.Text, TxtLocation.Text,TxtDescription.Text, TxtStartDate.Text.Replace("/","-"), TxtEndDate.Text.Replace("/", ""), TxtStartTime.Text.Replace("AM","").Replace("PM", "").Replace(" ", ""), TxtEndTime.Text.Replace(" ", "-").Replace("AM", "").Replace("PM", ""), EventPathImage);
                        if (apiStatus == 200)
                        {
                            if (respond is CreateEvent result)
                            {
                                //Add new item to my Event list
                                var user = ListUtils.MyProfileList?.FirstOrDefault();
                                EventDataObject data = new EventDataObject
                                {
                                    Id = result.EventId.ToString(),
                                    Description = TxtDescription.Text,
                                    Cover = EventPathImage,
                                    EndDate = TxtEndDate.Text,
                                    EndTime = TxtEndTime.Text,
                                    IsOwner = true,
                                    Location = TxtLocation.Text,
                                    Name = TxtEventName.Text,
                                    StartDate = TxtStartDate.Text,
                                    StartTime = TxtStartTime.Text,
                                    UserData = user,
                                };

                                if (EventMainActivity.GetInstance()?.MyEventTab?.MAdapter?.EventList != null)
                                {
                                    EventMainActivity.GetInstance()?.MyEventTab.MAdapter?.EventList?.Insert(0, data);
                                    EventMainActivity.GetInstance()?.MyEventTab.MAdapter?.NotifyItemInserted(EventMainActivity.GetInstance().MyEventTab.MAdapter.EventList.IndexOf(data));
                                }

                                if (EventMainActivity.GetInstance()?.EventTab?.MAdapter?.EventList != null)
                                {
                                    EventMainActivity.GetInstance()?.EventTab.MAdapter?.EventList?.Insert(0, data);
                                    EventMainActivity.GetInstance()?.EventTab.MAdapter?.NotifyItemInserted(EventMainActivity.GetInstance().EventTab.MAdapter.EventList.IndexOf(data));
                                }
                                 
                                AndHUD.Shared.ShowSuccess(this);
                                Finish();
                            }
                        }
                        else  
                        {
                            if (respond is ErrorObject error)
                            {
                                var errorText = error.Error.ErrorText;
                                AndHUD.Shared.Dismiss(this);
                                Snackbar.Make(TxtDescription, errorText, Snackbar.LengthLong).Show();
                            }
                            Methods.DisplayReportResult(this, respond);
                        } 

                        AndHUD.Shared.Dismiss(this); 
                       
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                //If its from Camera or Gallery
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                EventPathImage = resultUri.Path;

                                File file2 = new File(resultUri.Path);
                                var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                                Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(ImageEvent); 
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong),ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long).Show();
                        }
                    } 
                }
                else if (requestCode == 502 && resultCode == Result.Ok) // Location
                {
                    var placeAddress = data.GetStringExtra("Address") ?? "";
                    //var placeLatLng = data.GetStringExtra("latLng") ?? "";
                    if (!string.IsNullOrEmpty(placeAddress))
                    {
                        TxtLocation.Text = placeAddress;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        OpenDialogGallery();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 105)
                {
                    //Open intent Location when the request code of result is 502
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        new IntentController(this).OpenIntentLocation(); 
                    else
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion
         
        public void OnClick(View v)
        {
            try
            {
                if (v.Id == TxtStartTime.Id)
                {
                    var frag = PopupDialogController.TimePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtStartTime.Text = time.ToShortTimeString();
                    });

                    frag.Show(SupportFragmentManager, PopupDialogController.TimePickerFragment.Tag);
                }
                else if (v.Id == TxtEndTime.Id)
                {
                    var frag = PopupDialogController.TimePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtEndTime.Text = time.ToShortTimeString();
                    });

                    frag.Show(SupportFragmentManager, PopupDialogController.TimePickerFragment.Tag);
                }
                else if (v.Id == TxtStartDate.Id)
                {
                    var frag = PopupDialogController.DatePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtStartDate.Text = time.Date.ToString("yy-MM-dd");
                    });

                    frag.Show(SupportFragmentManager, PopupDialogController.DatePickerFragment.Tag);
                }
                else if (v.Id == TxtEndDate.Id)
                {
                    var frag = PopupDialogController.DatePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtEndDate.Text = time.Date.ToString("yy-MM-dd");
                    });
                    frag.Show(SupportFragmentManager, PopupDialogController.DatePickerFragment.Tag);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OpenDialogGallery()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Methods.Path.Chack_MyFolder();

                    //Open Image 
var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder();

                        //Open Image 
    var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(108);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}