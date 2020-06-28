using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using AppIntro;
using WoWonder.Activities.General;
using WoWonder.Activities.Suggested.User;
using WoWonder.Activities.Tabbes;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Utils;

namespace WoWonder.Activities.WalkTroutPage
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AppIntroWalkTroutPage : AppIntro2
    {
        private int Count =1;
        private string Caller = "";
         
        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                AddSlide(new AnimFragment1());
                AddSlide(new AnimFragment2());
                AddSlide(new AnimFragment4());
                AddSlide(new AnimFragment3());

                if (AppSettings.WalkThroughSetFlowAnimation)
                    SetFlowAnimation();
                else if (AppSettings.WalkThroughSetZoomAnimation)
                    SetZoomAnimation();
                else if (AppSettings.WalkThroughSetSlideOverAnimation)
                    SetSlideOverAnimation();
                else if (AppSettings.WalkThroughSetDepthAnimation)
                    SetDepthAnimation();
                else if (AppSettings.WalkThroughSetFadeAnimation) SetFadeAnimation();

                ShowStatusBar(false);

                //SetNavBarColor(Color.ParseColor(AppSettings.MainColor));
                SetIndicatorColor(Color.ParseColor(AppSettings.MainColor), Color.ParseColor("#888888"));
                //SetBarColor(Color.ParseColor("#3F51B5"));
                // SetSeparatorColor(Color.ParseColor("#2196f3"));

                Caller = Intent.GetStringExtra("class") ?? "";

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                }
                else
                {
                    RequestPermissions(new[]
                    {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation,
                        Manifest.Permission.Camera
                    }, 208);
                }

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { ApiRequest.GetGifts });
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

        #endregion

        #region Permissions && Result
         
        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

        #region Functions

        public override void OnSlideChanged()
        {
            try
            {
                base.OnSlideChanged();
                Pressed();
                Count++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void Pressed()
        {
            try
            {
                if (Count == 1)
                {
                    if ((int)Build.VERSION.SdkInt >= 23)
                    {
                        if (AppSettings.InvitationSystem)
                        {
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.ReadContacts,
                                Manifest.Permission.ReadPhoneNumbers,
                                Manifest.Permission.GetAccounts,
                            }, 2);
                        }
                        else
                        {
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.Camera
                            }, 2);
                        }
                    }
                }
                else if (Count == 2)
                {
                    if ((int)Build.VERSION.SdkInt >= 23)
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.RecordAudio,
                            Manifest.Permission.ModifyAudioSettings
                        }, 4);
                    }
                }
                else if (Count == 3)
                {

                }
                else if (Count == 4)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Do something when users tap on Done button.
        public override void OnDonePressed()
        {
            try
            {
                if (Caller.Contains("register"))
                {
                    if (AppSettings.ShowGoPro)
                    {
                        var intent = new Intent(this, typeof(GoProActivity));
                        intent.PutExtra("class", "register");
                        StartActivity(intent);
                    }
                    else if (AppSettings.ShowSuggestedUsersOnRegister)
                    {
                        Intent newIntent = new Intent(this, typeof(SuggestionsUsersActivity));
                        newIntent.PutExtra("class", "register");
                        StartActivity(newIntent);
                    }
                    else
                    {
                        StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                    }
                }
                else
                {
                    StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                }

                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Do something when users tap on Skip button.
        public override void OnSkipPressed()
        {
            try
            {
                if (Caller.Contains("register"))
                {
                    if (AppSettings.ShowGoPro)
                    {
                        var intent = new Intent(this, typeof(GoProActivity));
                        intent.PutExtra("class", "register");
                        StartActivity(intent);
                    }
                    else if (AppSettings.ShowSuggestedUsersOnRegister)
                    {
                        Intent newIntent = new Intent(this, typeof(SuggestionsUsersActivity));
                        newIntent.PutExtra("class", "register");
                        StartActivity(newIntent);
                    }
                    else
                    {
                        StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                    }
                }
                else
                {
                    StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                }

                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

    }
}