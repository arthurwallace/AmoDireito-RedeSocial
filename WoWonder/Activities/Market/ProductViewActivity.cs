﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Me.Relex;
using Newtonsoft.Json;
using WoWonder.Activities.Comment.Adapters;
using WoWonder.Activities.Market.Adapters;
using WoWonder.Activities.NativePost.Extra;
using WoWonder.Activities.NativePost.Post;
using WoWonder.Activities.Tabbes;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.Library.Anjo;
using WoWonder.Library.Anjo.SuperTextLibrary;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Classes.Posts;
using WoWonderClient.Classes.Product;
using WoWonderClient.Requests;
using ClipboardManager = Android.Content.ClipboardManager;
using Exception = System.Exception;
using String = Java.Lang.String;

namespace WoWonder.Activities.Market
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class ProductViewActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TextView TxtProductPrice, TxtProductNew, TxtProductInStock,  TxtProductLocation, TxtProductCardName, TxtProductTime;
        private SuperTextView TxtProductDescription;
        private ImageView ImageMore, UserImageAvatar, IconBack;
        private Button BtnContact;
        private ProductDataObject ProductData;
        private ViewPager ViewPagerView;
        private CircleIndicator CircleIndicatorView;
        private string TypeDialog, PostId;
        private RecyclerView CommentsRecyclerView;
        private CommentAdapter CommentsAdapter;

        private ReactButton LikeButton;
        private LinearLayout BtnLike, BtnComment, BtnWonder;
        private ImageView ImgWonder;
        private TextView TxtWonder;
        private LinearLayout MainSectionButton;
        private PostClickListener ClickListener;
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
                SetContentView(Resource.Layout.ProductView_Layout);

                PostId = Intent.GetStringExtra("Id") ?? string.Empty;

                ClickListener = new PostClickListener(this);

                //Get Value And Set Toolbar
                InitComponent();
                SetRecyclerViewAdapters();

                ProductData = JsonConvert.DeserializeObject<ProductDataObject>(Intent.GetStringExtra("ProductView"));
                if (ProductData == null) return;
                 
                Get_Data_Product(ProductData);
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
                ViewPagerView = FindViewById<ViewPager>(Resource.Id.pager);
                CircleIndicatorView = FindViewById<CircleIndicator>(Resource.Id.indicator);
                 
                TxtProductPrice = (TextView)FindViewById(Resource.Id.tv_price);
                TxtProductNew = (TextView)FindViewById(Resource.Id.BoleanNew);
                TxtProductInStock = (TextView)FindViewById(Resource.Id.BoleanInStock);
                TxtProductDescription = (SuperTextView)FindViewById(Resource.Id.tv_description);
                TxtProductLocation = (TextView)FindViewById(Resource.Id.tv_Location);
                TxtProductCardName = (TextView)FindViewById(Resource.Id.card_name);
                TxtProductTime = (TextView)FindViewById(Resource.Id.card_dist);

                BtnContact = (Button)FindViewById(Resource.Id.cont);

                UserImageAvatar = (ImageView)FindViewById(Resource.Id.card_pro_pic);
                ImageMore = (ImageView)FindViewById(Resource.Id.Image_more);
                IconBack = (ImageView)FindViewById(Resource.Id.iv_back);


                BtnLike = FindViewById<LinearLayout>(Resource.Id.linerlike);
                BtnComment = FindViewById<LinearLayout>(Resource.Id.linercomment);

                MainSectionButton = FindViewById<LinearLayout>(Resource.Id.mainsection);
                BtnWonder = FindViewById<LinearLayout>(Resource.Id.linerSecondReaction);
                ImgWonder = FindViewById<ImageView>(Resource.Id.image_SecondReaction);
                TxtWonder = FindViewById<TextView>(Resource.Id.SecondReactionText);

                LikeButton = FindViewById<ReactButton>(Resource.Id.beactButton);

                if (AppSettings.PostButton == PostButtonSystem.ReactionDefault || AppSettings.PostButton == PostButtonSystem.ReactionSubShine || AppSettings.PostButton == PostButtonSystem.Like)
                {
                    MainSectionButton.WeightSum = 2;
                    BtnWonder.Visibility = ViewStates.Gone;
                }
                else if (AppSettings.PostButton == PostButtonSystem.Wonder)
                {
                    MainSectionButton.WeightSum = 3;
                    BtnWonder.Visibility = ViewStates.Visible;

                    ImgWonder.SetImageResource(Resource.Drawable.ic_action_wowonder);
                    TxtWonder.Text = Application.Context.GetText(Resource.String.Btn_Wonder);
                }
                else if (AppSettings.PostButton == PostButtonSystem.DisLike)
                {
                    MainSectionButton.WeightSum = 3;
                    BtnWonder.Visibility = ViewStates.Visible;

                    ImgWonder.SetImageResource(Resource.Drawable.ic_action_dislike);
                    TxtWonder.Text = Application.Context.GetText(Resource.String.Btn_Dislike);
                }

                if (!AppSettings.SetTabDarkTheme)
                    ImageMore.SetColorFilter(Color.Black);

                if (AppSettings.FlowDirectionRightToLeft)
                    IconBack.SetImageResource(Resource.Drawable.ic_action_ic_back_rtl);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            { 
                CommentsRecyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
                CommentsRecyclerView.NestedScrollingEnabled = false;
                CommentsAdapter = new CommentAdapter(this, CommentsRecyclerView, "Light", PostId)
                {
                    CommentList = new ObservableCollection<CommentObjectExtra>()
                };

                StartApiService();
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
                    BtnContact.Click += BtnContactOnClick;
                    UserImageAvatar.Click += UserImageAvatarOnClick;
                    TxtProductCardName.Click += UserImageAvatarOnClick; 
                    ImageMore.Click += ImageMoreOnClick;
                    IconBack.Click += IconBackOnClick;
                    TxtProductDescription.LongClick += TxtProductDescriptionOnLongClick;
                    BtnComment.Click += BtnCommentOnClick;

                    if (AppSettings.PostButton == PostButtonSystem.Wonder || AppSettings.PostButton == PostButtonSystem.DisLike)
                        BtnWonder.Click += BtnWonderOnClick;

                    LikeButton.Click += (sender, args) => LikeButton.ClickLikeAndDisLike(new GlobalClickEventArgs()
                    {
                        NewsFeedClass = PostData,
                    } , null, "ProductViewActivity");

                    if (AppSettings.PostButton == PostButtonSystem.ReactionDefault || AppSettings.PostButton == PostButtonSystem.ReactionSubShine)
                        LikeButton.LongClick += (sender, args) => LikeButton.LongClickDialog(new GlobalClickEventArgs()
                        {
                            NewsFeedClass = PostData,
                        }, null, "ProductViewActivity");
                }
                else
                {
                    BtnContact.Click -= BtnContactOnClick;
                    UserImageAvatar.Click -= UserImageAvatarOnClick;
                    TxtProductCardName.Click -= UserImageAvatarOnClick; 
                    ImageMore.Click -= ImageMoreOnClick;
                    IconBack.Click -= IconBackOnClick;
                    TxtProductDescription.LongClick -= TxtProductDescriptionOnLongClick;
                    BtnComment.Click -= BtnCommentOnClick;

                    if (AppSettings.PostButton == PostButtonSystem.Wonder || AppSettings.PostButton == PostButtonSystem.DisLike)
                        BtnWonder.Click -= BtnWonderOnClick;

                    LikeButton.Click += null;
                    if (AppSettings.PostButton == PostButtonSystem.ReactionDefault || AppSettings.PostButton == PostButtonSystem.ReactionSubShine)
                        LikeButton.LongClick -= null; 
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
                ViewPagerView = null;
                CircleIndicatorView = null;
                TxtProductPrice = null;
                TxtProductNew = null;
                TxtProductInStock = null;
                TxtProductDescription = null;
                TxtProductLocation = null;
                TxtProductCardName = null;
                TxtProductTime = null;
                BtnContact = null;
                UserImageAvatar = null;
                ImageMore = null;
                IconBack  = null;
                BtnLike = null;
                BtnComment = null;
                MainSectionButton = null;
                BtnWonder = null;
                ImgWonder = null;
                TxtWonder = null;
                LikeButton = null;
                ClickListener = null;
                PostData = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void TxtProductDescriptionOnLongClick(object sender, View.LongClickEventArgs e)
        {
            try
            {
                if (Methods.FunString.StringNullRemover(ProductData.Description) != "Empty")
                {
                    var clipboardManager = (ClipboardManager)GetSystemService(ClipboardService);

                    var clipData = ClipData.NewPlainText("text", Methods.FunString.DecodeString(ProductData.Description));
                    clipboardManager.PrimaryClip = clipData;

                    Toast.MakeText(this, GetText(Resource.String.Lbl_Copied), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Contact seller 
        private void BtnContactOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (AppSettings.MessengerIntegration)
                {
                    var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                    dialog.Title(Resource.String.Lbl_Warning);
                    dialog.Content(GetText(Resource.String.Lbl_ContentAskOPenAppMessenger));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                    {
                        try
                        {
                            Methods.App.OpenAppByPackageName(this, AppSettings.MessengerPackageName, "SendMsgProduct", new ChatObject() { UserId = ProductData.Seller.UserId , Avatar = ProductData.Seller.Avatar, Name = ProductData.Seller.Name , LastMessage = new LastMessageUnion() {LastMessageClass = new MessageData() { ProductId = ProductData.Id , Product = new ProductUnion(){ProductClass = ProductData}} }} );
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    });
                    dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                } 
                else
                {
                    if (!Methods.CheckConnectivity())
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        return;
                    }

                    var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    var time = unixTimestamp.ToString();

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.Send_Message(ProductData.Seller.UserId, time, "", "", "", "", "", "", ProductData.Id) });
                    Toast.MakeText(this, GetString(Resource.String.Lbl_MessageSentSuccessfully), ToastLength.Short).Show(); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        // Event Open User Profile
        private void UserImageAvatarOnClick(object sender, EventArgs e)
        {
            try
            {
                WoWonderTools.OpenProfile(this, ProductData.Seller.UserId, ProductData.Seller);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Event Back
        private void IconBackOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event More >> Show Menu (CopeLink , Share)
        private void ImageMoreOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                if (ProductData.Seller.UserId == UserDetails.UserId)
                {
                    arrayAdapter.Add(GetString(Resource.String.Lbl_EditProduct));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_Delete));
                }
                
                arrayAdapter.Add(GetString(Resource.String.Lbl_CopeLink));
                arrayAdapter.Add(GetString(Resource.String.Lbl_Share));
                 
                dialogList.Title(GetString(Resource.String.Lbl_More));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Menu >> Copy Link
        private void OnCopyLink_Button_Click()
        {
            try
            {
                var clipboardManager = (ClipboardManager)GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", ProductData.Url);
                clipboardManager.PrimaryClip = clipData;

                Toast.MakeText(this, GetText(Resource.String.Lbl_Copied), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Menu >> Share
        private void OnShare_Button_Click()
        {
            try
            {
                ClickListener.SharePostClick(new GlobalClickEventArgs() { NewsFeedClass = PostData, }, PostModelType.ProductPost);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        //Event Menu >> Edit Info Product if user == is_owner  
        private void EditInfoProduct_OnClick()
        {
            try
            {
                Intent intent = new Intent(this, typeof(EditProductActivity));
                intent.PutExtra("ProductView", JsonConvert.SerializeObject(ProductData));
                StartActivityForResult(intent, 3500);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        //Event Menu >> Edit Info Product if user == is_owner  
        private void DeleteProduct_OnClick()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                   TypeDialog = "DeletePost";

                    var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);
                    dialog.Title(GetText(Resource.String.Lbl_DeletePost));
                    dialog.Content(GetText(Resource.String.Lbl_AreYouSureDeletePost));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive(this);
                    dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
          
        //Event Add Comment
        private void BtnCommentOnClick(object sender, EventArgs e)
        {
            try
            {
                ClickListener.CommentPostClick(new GlobalClickEventArgs()
                {
                    NewsFeedClass = PostData,
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Event Add Wonder / Disliked
        private void BtnWonderOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    return;
                }
                 

                if (PostData.IsWondered != null && (bool)PostData.IsWondered)
                {
                    var x = Convert.ToInt32(PostData.PostWonders);
                    if (x > 0)
                        x--;
                    else
                        x = 0;

                    ImgWonder.SetColorFilter(Color.White);

                    PostData.IsWondered = false;
                    PostData.PostWonders = Convert.ToString(x, CultureInfo.InvariantCulture);

                    if (AppSettings.PostButton == PostButtonSystem.Wonder)
                        TxtWonder.Text = GetText(Resource.String.Btn_Wonder);
                    else if (AppSettings.PostButton == PostButtonSystem.DisLike)
                        TxtWonder.Text = GetText(Resource.String.Btn_Dislike);

                    BtnWonder.Tag = "false";
                }
                else
                {
                    var x = Convert.ToInt32(PostData.PostWonders);
                    x++;

                    PostData.IsWondered = true;
                    PostData.PostWonders = Convert.ToString(x, CultureInfo.InvariantCulture);

                    ImgWonder.SetColorFilter(Color.ParseColor("#f89823"));

                    if (AppSettings.PostButton == PostButtonSystem.Wonder)
                        TxtWonder.Text = GetText(Resource.String.Lbl_wondered);
                    else if (AppSettings.PostButton == PostButtonSystem.DisLike)
                        TxtWonder.Text = GetText(Resource.String.Lbl_disliked);

                    BtnWonder.Tag = "true";
                }

                switch (AppSettings.PostButton)
                {
                    case PostButtonSystem.Wonder:
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Post_Actions(PostData.PostId, "wonder") });
                        break;
                    case PostButtonSystem.DisLike:
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Post_Actions(PostData.PostId, "dislike") });
                        break;
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
                string text = itemString.ToString();
                if (text == GetString(Resource.String.Lbl_CopeLink))
                {
                    OnCopyLink_Button_Click();
                }
                else if (text == GetString(Resource.String.Lbl_Share))
                {
                    OnShare_Button_Click();
                }
                else if (text == GetString(Resource.String.Lbl_EditProduct))
                {
                    EditInfoProduct_OnClick();
                }
                else if (text == GetString(Resource.String.Lbl_Delete))
                {
                     DeleteProduct_OnClick();
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
                if (TypeDialog == "DeletePost")
                {
                    try
                    {
                        if (!Methods.CheckConnectivity())
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                            return;
                        }

                        var adapterGlobal = WRecyclerView.GetInstance()?.NativeFeedAdapter;
                        var diff = adapterGlobal?.ListDiffer;
                        var dataGlobal = diff?.Where(a => a.PostData?.PostId == ProductData?.PostId);
                        if (dataGlobal != null)
                        {
                            foreach (var postData in dataGlobal)
                            {
                                WRecyclerView.GetInstance()?.RemoveByRowIndex(postData);
                            }
                        }

                        var recycler = TabbedMainActivity.GetInstance()?.NewsFeedTab?.MainRecyclerView;
                        var dataGlobal2 = recycler?.NativeFeedAdapter.ListDiffer?.Where(a => a.PostData?.PostId == ProductData?.PostId);
                        if (dataGlobal2 != null)
                        {
                            foreach (var postData in dataGlobal2)
                            {
                                recycler.RemoveByRowIndex(postData);
                            }
                        }

                        if (TabbedMarketActivity.GetInstance()?.MyProductsTab?.MAdapter?.MarketList != null)
                        {
                            TabbedMarketActivity.GetInstance().MyProductsTab.MAdapter.MarketList?.Remove(ProductData);
                            TabbedMarketActivity.GetInstance().MyProductsTab.MAdapter.NotifyDataSetChanged();
                        }

                        if (TabbedMarketActivity.GetInstance()?.MarketTab?.MAdapter?.MarketList != null)
                        {
                            TabbedMarketActivity.GetInstance().MarketTab.MAdapter.MarketList?.Remove(ProductData);
                            TabbedMarketActivity.GetInstance().MarketTab.MAdapter.NotifyDataSetChanged();
                        }
                         
                        Toast.MakeText(this, GetText(Resource.String.Lbl_postSuccessfullyDeleted), ToastLength.Short).Show();
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Post_Actions(ProductData.PostId, "delete") }); 

                        Finish();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
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

        #region Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                
                if (requestCode == 3500 && resultCode == Result.Ok)
                {
                    if (string.IsNullOrEmpty(data.GetStringExtra("itemData"))) return;
                    var item = JsonConvert.DeserializeObject<ProductDataObject>(data.GetStringExtra("itemData"));
                    if (item != null)
                    {
                        ProductData = item;
                        Get_Data_Product(item);
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private void Get_Data_Product(ProductDataObject productData)
        {
            try
            {
                ProductData = productData;

                PostData = new PostDataObject()
                {
                    PostId = productData.PostId,
                    Product = new ProductUnion()
                    {
                        ProductClass = productData,
                    },
                    ProductId = productData.Id,
                    UserId = productData.UserId,
                    UserData = productData.Seller,
                    Url = productData.Url,
                    PostUrl = productData.Url,
                };

                List<string> listImageUser = new List<string>();
                if (productData.Images?.Count > 0)
                    listImageUser.AddRange(productData.Images.Select(t => t.Image));
                else
                    listImageUser.Add(productData.Images?[0]?.Image);

                if (ViewPagerView.Adapter == null)
                {
                    ViewPagerView.Adapter = new MultiImagePagerAdapter(this, listImageUser);
                    ViewPagerView.CurrentItem = 0;
                    CircleIndicatorView.SetViewPager(ViewPagerView);
                }
                ViewPagerView.Adapter.NotifyDataSetChanged();

                GlideImageLoader.LoadImage(this, productData.Seller.Avatar, UserImageAvatar, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                     
                var (currency, currencyIcon) = WoWonderTools.GetCurrency(productData.Currency); 
                TxtProductPrice.Text = productData.Price + " " + currencyIcon;
              
                Console.WriteLine(currency);
                var readMoreOption = new StReadMoreOption.Builder()
                    .TextLength(200, StReadMoreOption.TypeCharacter)
                    .MoreLabel(GetText(Resource.String.Lbl_ReadMore))
                    .LessLabel(GetText(Resource.String.Lbl_ReadLess))
                    .MoreLabelColor(Color.ParseColor(AppSettings.MainColor))
                    .LessLabelColor(Color.ParseColor(AppSettings.MainColor))
                    .LabelUnderLine(true)
                    .Build();

                if (Methods.FunString.StringNullRemover(productData.Description) != "Empty")
                {
                    var description =Methods.FunString.DecodeString(productData.Description); 
                     readMoreOption.AddReadMoreTo(TxtProductDescription, new String(description));
                }
                else
                {
                    TxtProductDescription.Text = GetText(Resource.String.Lbl_Empty);
                }

                TxtProductLocation.Text = productData.Location;
                TxtProductCardName.Text = Methods.FunString.SubStringCutOf(WoWonderTools.GetNameFinal(productData.Seller), 14);
                TxtProductTime.Text = productData.TimeText;

                if (productData.Seller.UserId == UserDetails.UserId)
                    BtnContact.Visibility = ViewStates.Gone;
                 
                //Type == "0" >>  New // Type != "0"  Used
                TxtProductNew.Visibility = productData.Type == "0" ? ViewStates.Visible : ViewStates.Gone;

                // Status InStock
                TxtProductInStock.Visibility = productData.Status == "0" ? ViewStates.Visible : ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadPostDataAsync, () => CommentsAdapter.FetchPostApiComments("0", PostId) });
        }

        private async Task LoadPostDataAsync()
        {
            if (Methods.CheckConnectivity())
            {
                (int apiStatus, var respond) = await RequestsAsync.Global.Get_Post_Data(PostId, "post_data");
                if (apiStatus == 200)
                {
                    if (respond is GetPostDataObject result)
                    {
                        PostData = result.PostData;

                        if (PostData.IsLiked != null && (bool)PostData.IsLiked)
                        {
                            BtnLike.Tag = "true";
                        }
                        else
                        {
                            BtnLike.Tag = "false";
                        }

                        if (PostData.IsWondered != null && (bool)PostData.IsWondered)
                        {
                            BtnWonder.Tag = "true";
                            ImgWonder.SetColorFilter(Color.ParseColor("#f89823"));

                            TxtWonder.Text = GetText(Resource.String.Lbl_wondered);
                        }
                        else
                        {
                            BtnWonder.Tag = "false";
                            ImgWonder.SetColorFilter(Color.White);
                            TxtWonder.Text = GetText(Resource.String.Btn_Wonder);

                        }

                        if (AppSettings.PostButton == PostButtonSystem.ReactionDefault || AppSettings.PostButton == PostButtonSystem.ReactionSubShine)
                        {
                            if (PostData.Reaction == null)
                                PostData.Reaction = new WoWonderClient.Classes.Posts.Reaction();

                            if ((bool)(PostData.Reaction != null & PostData.Reaction?.IsReacted))
                            {
                                if (!string.IsNullOrEmpty(PostData.Reaction.Type))
                                {
                                    switch (PostData.Reaction.Type)
                                    {
                                        case "1":
                                        case "Like":
                                            LikeButton.SetReactionPack(ReactConstants.Like);
                                            break;
                                        case "2":
                                        case "Love":
                                            LikeButton.SetReactionPack(ReactConstants.Love);
                                            break;
                                        case "3":
                                        case "HaHa":
                                            LikeButton.SetReactionPack(ReactConstants.HaHa);
                                            break;
                                        case "4":
                                        case "Wow":
                                            LikeButton.SetReactionPack(ReactConstants.Wow);
                                            break;
                                        case "5":
                                        case "Sad":
                                            LikeButton.SetReactionPack(ReactConstants.Sad);
                                            break;
                                        case "6":
                                        case "Angry":
                                            LikeButton.SetReactionPack(ReactConstants.Angry);
                                            break;
                                        default:
                                            LikeButton.SetReactionPack(ReactConstants.Default);
                                            break; 
                                    } 
                                }
                            }
                        }
                        else
                        {
                            if (PostData.IsLiked != null && (bool)PostData.IsLiked)
                                LikeButton.SetReactionPack(ReactConstants.Like);

                            if (AppSettings.PostButton == PostButtonSystem.Wonder)
                            {
                                if (PostData.IsWondered != null && (bool)PostData.IsWondered)
                                {
                                    ImgWonder.SetImageResource(Resource.Drawable.ic_action_wowonder);
                                    ImgWonder.SetColorFilter(Color.ParseColor(AppSettings.MainColor));

                                    TxtWonder.Text = GetString(Resource.String.Lbl_wondered);
                                    TxtWonder.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                                }
                                else
                                {
                                    ImgWonder.SetImageResource(Resource.Drawable.ic_action_wowonder);
                                    ImgWonder.SetColorFilter(Color.ParseColor("#666666"));

                                    TxtWonder.Text = GetString(Resource.String.Btn_Wonder);
                                    TxtWonder.SetTextColor(Color.ParseColor("#444444"));
                                }
                            }
                            else if (AppSettings.PostButton == PostButtonSystem.DisLike)
                            {
                                if (PostData.IsWondered != null && (bool)PostData.IsWondered)
                                {
                                    ImgWonder.SetImageResource(Resource.Drawable.ic_action_dislike);
                                    ImgWonder.SetColorFilter(Color.ParseColor(AppSettings.MainColor));

                                    TxtWonder.Text = GetString(Resource.String.Lbl_disliked);
                                    TxtWonder.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                                }
                                else
                                {
                                    ImgWonder.SetImageResource(Resource.Drawable.ic_action_dislike);
                                    ImgWonder.SetColorFilter(Color.ParseColor("#666666"));

                                    TxtWonder.Text = GetString(Resource.String.Btn_Dislike);
                                    TxtWonder.SetTextColor(Color.ParseColor("#444444"));
                                }
                            }
                        }
                    }
                }
                else Methods.DisplayReportResult(this, respond);  
            }
            else
            { 
                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
        }
    }
}