﻿using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Lang;
using Newtonsoft.Json;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Posts;
using WoWonderClient.Requests;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.EditPost
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditPostActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private ImageView PostSectionImage;
        private TextView TxtAddPost, TxtUserName;
        private EditText TxtContentPost;
        private Button  PostPrivacyButton;
        private string IdPost = "", PostPrivacy = "", TypeDialog = "";
        private PostDataObject PostData;
       
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
                SetContentView(Resource.Layout.EditPost_Layout);

                PostData = JsonConvert.DeserializeObject<PostDataObject>(Intent.GetStringExtra("PostItem"));
                 
                var id = Intent.GetStringExtra("PostId") ?? "Data not available";
                if (id != "Data not available" && !string.IsNullOrEmpty(id))
                    IdPost = id;

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
                TxtAddPost = FindViewById<TextView>(Resource.Id.toolbar_title);
                TxtContentPost = FindViewById<EditText>(Resource.Id.editTxtEmail);
                TxtUserName = FindViewById<TextView>(Resource.Id.card_name);
                PostSectionImage = FindViewById<ImageView>(Resource.Id.postsectionimage);
                PostPrivacyButton = FindViewById<Button>(Resource.Id.cont);

                TxtContentPost.Text = Methods.FunString.DecodeString(PostData.Orginaltext);

                Methods.SetColorEditText(TxtContentPost, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);

                GetPrivacyPost();
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
                    toolbar.Title = GetText(Resource.String.Lbl_EditPost);
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
                    PostPrivacyButton.Click += PostPrivacyButton_Click;
                    TxtAddPost.Click += TxtAddPostOnClick;
                }
                else
                {
                    PostPrivacyButton.Click -= PostPrivacyButton_Click;
                    TxtAddPost.Click -= TxtAddPostOnClick;
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
                PostSectionImage = null;
                TxtUserName = null;
                TxtAddPost  = null;
                TxtContentPost = null;
                PostPrivacyButton = null;
                IdPost   = null; PostPrivacy = null; TypeDialog = null;
                PostData = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        #endregion

        #region Events

        private async void TxtAddPostOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading) + "...");

                    var (apiStatus, respond) = await RequestsAsync.Global.Post_Actions(IdPost, "edit","", TxtContentPost.Text, PostPrivacy);
                    if (apiStatus == 200)
                    {
                        if (respond is PostActionsObject result)
                        {
                            if (result.Action.Contains("edited"))
                            {
                                Toast.MakeText(this, result.Action, ToastLength.Short).Show();
                                AndHUD.Shared.Dismiss(this);

                                // put the String to pass back into an Intent and close this activity
                                var resultIntent = new Intent();
                                resultIntent.PutExtra("PostId", IdPost);
                                resultIntent.PutExtra("PostText", TxtContentPost.Text);
                                SetResult(Result.Ok, resultIntent);
                                Finish();
                            }
                            else
                            {
                                //Show a Error image with a message
                                AndHUD.Shared.ShowError(this, result.Action, MaskType.Clear, TimeSpan.FromSeconds(2));
                            }
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss(this);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
   
        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "PostPrivacy")
                {
                    PostPrivacyButton.Text = itemString.ToString();

                    if (itemString.ToString() == GetString(Resource.String.Lbl_Everyone))
                        PostPrivacy = "0";
                    else if (itemString.ToString() == GetString(Resource.String.Lbl_People_i_Follow))
                        PostPrivacy = "2";
                    else if (itemString.ToString() == GetString(Resource.String.Lbl_People_Follow_Me))
                        PostPrivacy = "1";
                    else if (itemString.ToString() == GetString(Resource.String.Lbl_No_body))
                        PostPrivacy = "3";
                    else
                        PostPrivacy = "0";
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (TypeDialog == "PostPrivacy")
                {
                    if (p1 == DialogAction.Positive) p0.Dismiss();
                }
                else if (TypeDialog == "PostBack")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        p0.Dismiss();
                        Finish();
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else
                {
                    if (p1 == DialogAction.Positive)
                    {
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Privacy

        private void LoadDataUser()
        {
            try
            {
                var dataUser = ListUtils.MyProfileList?.FirstOrDefault();
                if (dataUser != null)
                {
                    GlideImageLoader.LoadImage(this, dataUser.Avatar, PostSectionImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                    TxtUserName.Text = WoWonderTools.GetNameFinal(dataUser);
                      
                    if (PostData.PostPrivacy.Contains("0"))
                        PostPrivacyButton.Text = GetString(Resource.String.Lbl_Everyone);
                    else if (PostData.PostPrivacy.Contains("ifollow") || PostData.PostPrivacy == "2")
                        PostPrivacyButton.Text = GetString(Resource.String.Lbl_People_i_Follow);
                    else if (PostData.PostPrivacy.Contains("me") || PostData.PostPrivacy == "1")
                        PostPrivacyButton.Text = GetString(Resource.String.Lbl_People_Follow_Me);
                    else
                        PostPrivacyButton.Text = GetString(Resource.String.Lbl_No_body);
                     
                    PostPrivacy = PostData.PostPrivacy;
                }
                else
                {
                    TxtUserName.Text = UserDetails.Username;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void GetPrivacyPost()
        {
            try
            {
                if (!string.IsNullOrEmpty(PostData.GroupId) && PostData.GroupId != "0")
                {
                    var dataGroup = PostData.GroupRecipient;
                    if (dataGroup != null)
                    {
                        PostPrivacyButton.Visibility = ViewStates.Gone;
                        GlideImageLoader.LoadImage(this, dataGroup.Avatar, PostSectionImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                        TxtUserName.Text = dataGroup.GroupName;
                    }
                    else
                    {
                        LoadDataUser();
                    }
                }
                else if (!string.IsNullOrEmpty(PostData.PageId) && PostData.PageId != "0")
                {
                    PostPrivacyButton.Visibility = ViewStates.Gone;
                    GlideImageLoader.LoadImage(this, PostData.Publisher.Avatar, PostSectionImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                    TxtUserName.Text = PostData.Publisher.Name;
                }
                else if (!string.IsNullOrEmpty(PostData.EventId) && PostData.EventId != "0")  
                {
                    if (PostData.Event != null)
                    {
                        var dataEvent = PostData.Event.Value.EventClass;
                        if (dataEvent != null)
                        {
                            PostPrivacyButton.Visibility = ViewStates.Gone;
                            GlideImageLoader.LoadImage(this, dataEvent.Cover, PostSectionImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                            TxtUserName.Text = dataEvent.Name;
                        }
                        else
                        {
                            LoadDataUser();
                        }
                    }
                    else
                    {
                        LoadDataUser();
                    }
                }
                else
                {
                    LoadDataUser();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void PostPrivacyButton_Click(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "PostPrivacy";

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                arrayAdapter.Add(GetString(Resource.String.Lbl_Everyone));
                arrayAdapter.Add(GetString(Resource.String.Lbl_People_i_Follow));
                arrayAdapter.Add(GetText(Resource.String.Lbl_People_Follow_Me));
                arrayAdapter.Add(GetString(Resource.String.Lbl_No_body));

                dialogList.Title(GetText(Resource.String.Lbl_PostPrivacy));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.ItemsCallback(this).Build().Show();
                dialogList.AlwaysCallSingleChoiceCallback();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

    }
}