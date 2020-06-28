//##############################################
//Cᴏᴘʏʀɪɢʜᴛ 2020 DᴏᴜɢʜᴏᴜᴢLɪɢʜᴛ Codecanyon Item 19703216
//Elin Doughouz >> https://www.facebook.com/Elindoughouz
//====================================================

//For the accuracy of the icon and logo, please use this website " http://nsimage.brosteins.com " and add images according to size in folders " mipmap " 

using WoWonder.Helpers.Model;

namespace WoWonder
{
    public static class AppSettings
    {
        public static string TripleDesAppServiceProvider = "iwjLdtDnEKUcMV+dROrgjL/0BVfrtCEC7ffusa/ATTjNyO63DmM67kNXi3guJhqzyjBoUT+g99FrPrJlqxsUSomYauoTe7QthJBc2szEHQbF7NzaqaLPS53+PyIf3Vb40Y7cbiZNZ5qIioqzrZdwsv89F5ssQKiMKn/SvDfIIpFTZnB+PxZSzyktB9g5Q6lKFgnZsnqRUPpi6kdwYSYl3U/mtRQztdqGpK8uiB7QGDErilzYoSaghdF1a1d19pw9q6QTyBaIgAlkBCppXeCb/Q==";

        //Main Settings >>>>>
        //*********************************************************
        public static string Version = "3.1";
        public static string ApplicationName = "amo Direito";
        public static string YoutubeKey = "AIzaSyA-JSf9CU1cdMpgzROCCUpl4wOve9S94ZU";

        // Friend system = 0 , follow system = 1
        public static int ConnectivitySystem = 0;

        public static PostButtonSystem PostButton = PostButtonSystem.Reaction;
        public static bool ShowTextShareButton = false;
        public static bool ShowShareButton = true; //#New

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#174374";
        public static string StoryReadColor = "#808080";

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = "pt"; //Default language ar_AE

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "64974c58-9993-40ed-b782-0814edc401ea";

        // WalkThrough Settings >>
        //*********************************************************
        public static bool ShowWalkTroutPage = false;
        public static bool WalkThroughSetFlowAnimation = false;
        public static bool WalkThroughSetZoomAnimation = false;
        public static bool WalkThroughSetSlideOverAnimation = false;
        public static bool WalkThroughSetDepthAnimation = false;
        public static bool WalkThroughSetFadeAnimation = false;

        //Main Messenger settings
        //*********************************************************
        public static bool MessengerIntegration = false;
        public static string MessengerPackageName = "com.wowondermessenger.app"; //APK name on Google Play

        //AdMob >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdMobBanner = false;
        public static bool ShowAdMobInterstitial = false;
        public static bool ShowAdMobRewardVideo = false;
        public static bool ShowAdMobNative = false;
        public static bool ShowAdMobNativePost = false; //#New

        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/3584502890";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/2518408206";
        public static string AdAdMobNativeKey = "ca-app-pub-5135691635931982/2280543246";

        //Three times after entering the ad is displayed
        public static int ShowAdMobInterstitialCount = 3;
        public static int ShowAdMobRewardedVideoCount = 3;
        public static int ShowAdMobNativeCount = 40;

        //FaceBook Ads >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowFbBannerAds = false; //#New
        public static bool ShowFbInterstitialAds = false; //#New
        public static bool ShowFbRewardVideoAds = false; //#New
        public static bool ShowFbNativeAds = false; //#New
         
        //YOUR_PLACEMENT_ID
        public static string AdsFbBannerKey = "250485588986218_554026418632132"; //#New
        public static string AdsFbInterstitialKey = "250485588986218_554026125298828"; //#New
        public static string AdsFbRewardVideoKey = "250485588986218_554072818627492"; //#New
        public static string AdsFbNativeKey = "250485588986218_554706301897477"; //#New

        //Three times after entering the ad is displayed
        public static int ShowFbNativeAdsCount = 40; //#New
         
        //********************************************************* 
        public static bool EnableRegisterSystem = true; //#New
        public static bool ShowGenderOnRegister = true; 

        //Set Theme Welcome Pages 
        //*********************************************************
        //Types >> Gradient or Video or Image
        public static string BackgroundScreenWelcomeType = "Image";

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;

        //Code Time Zone (true => Get from Internet , false => Get From #CodeTimeZone )
        //*********************************************************
        public static bool AutoCodeTimeZone = true;
        public static string CodeTimeZone = "UTC";

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = false;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file 
        //Facebook >> ../values/analytic.xml .. line 10-11 
        //Google >> ../values/analytic.xml .. line 15 
        //*********************************************************
        public static bool ShowFacebookLogin = false;
        public static bool ShowGoogleLogin = false;

        public static readonly string ClientId = "430795656343-679a7fus3pfr1ani0nr0gosotgcvq2s8.apps.googleusercontent.com";
        
        //########################### 

        //Main Slider settings
        //*********************************************************
        public static bool ShowAlbum = true;
        public static bool ShowArticles = true;
        public static bool ShowPokes = true;
        public static bool ShowCommunitiesGroups = true;
        public static bool ShowCommunitiesPages = true;
        public static bool ShowMarket = true;
        public static bool ShowPopularPosts = true;
        public static bool ShowMovies = false;
        public static bool ShowNearBy = false;
        public static bool ShowStory = true;
        public static bool ShowSavedPost = true;
        public static bool ShowUserContacts = true; 
        public static bool ShowJobs = true; 
        public static bool ShowCommonThings = true; 
        public static bool ShowFundings = false;
        public static bool ShowMyPhoto = true; 
        public static bool ShowMyVideo = false; 
        public static bool ShowGames = false;
        public static bool ShowMemories = false; //#New 
        public static bool ShowOffers = true; //#New 
        public static bool ShowNearbyShops = false;  //#New 

        public static bool ShowSuggestedGroup = true;
        public static bool ShowSuggestedUser = true; //#New 


        //count times after entering the Suggestion is displayed
        public static int ShowSuggestedGroupCount = 70; //#New 
        public static int ShowSuggestedUserCount = 50; //#New 

        //Events settings
        //*********************************************************
        public static bool ShowEvents = true; 
        public static bool ShowEventGoing = true; //#New 
        public static bool ShowEventInvited = true; //#New 
        public static bool ShowEventInterested = true; //#New 
        public static bool ShowEventPast = true; //#New 

        //Set a story duration >> 10 Sec
        public static long StoryDuration = 10000L; //#New 
        //*********************************************************
        /// <summary>
        ///  Currency
        /// CurrencyStatic = true : get currency from app not api 
        /// CurrencyStatic = false : get currency from api (default)
        /// </summary>
        public static readonly bool CurrencyStatic = false; //#New
        public static readonly string CurrencyIconStatic = "R$"; //#New
        public static readonly string CurrencyCodeStatic = "BRL"; //#New
        public static readonly string CurrencyFundingPriceStatic = "R$"; //#New

        //Profile settings
        //*********************************************************
        public static bool ShowGift = true;
        public static bool ShowWallet = false; //#New
        public static bool ShowGoPro = true; //#New
        public static bool ShowWithdrawals = true; //#New

        //Native Post settings
        //*********************************************************
        public static int AvatarPostSize = 60;
        public static int ImagePostSize = 200;
        public static string PostApiLimitOnScroll = "15";

        public static bool AutoPlayVideo = false;
         
        public static bool EmbedPlayTubePostType = true;
        public static bool EmbedDeepSoundPostType = true;
        public static bool EmbedFacebookVideoPostType = false;
        public static bool EmbedVimeoVideoPostType = false;//#New
        public static bool ShowSearchForPosts = true;//#New
        public static bool EmbedLivePostType = false;//#New

        public static bool ShowAddPostOnNewsFeed = false; //#New 
        public static bool ShowCountSharePost = true; //#New 

        /// <summary>
        /// Post Privacy
        /// ShowPostPrivacyForAllUser = true : all posts user have icon Privacy 
        /// ShowPostPrivacyForAllUser = false : just my posts have icon Privacy (default)
        /// </summary>
        public static bool ShowPostPrivacyForAllUser = false; //#New 

        public static bool ShowFullScreenVideoPost = true;
         
        //UsersPages
        public static bool ShowProUsersMembers = true;
        public static bool ShowPromotedPages = true;
        public static bool ShowTrendingHashTags = true;
        public static bool ShowLastActivities = true;

        public static bool ShowUserPoint = true;

        //Add Post
        public static bool ShowGalleryImage = true;
        public static bool ShowGalleryVideo = false;
        public static bool ShowMention = true;
        public static bool ShowLocation = true;
        public static bool ShowFeelingActivity = true;
        public static bool ShowFeeling = true;
        public static bool ShowListening = true;
        public static bool ShowPlaying = true;
        public static bool ShowWatching = true;
        public static bool ShowTraveling = true;
        public static bool ShowCamera = true;
        public static bool ShowGif = true;
        public static bool ShowFile = true;
        public static bool ShowMusic = true;
        public static bool ShowPolls = true;
        public static bool ShowColor = true;

        //Settings Page >> General Account
        public static bool ShowSettingsGeneralAccount = true;
        public static bool ShowSettingsAccount = true;
        public static bool ShowSettingsSocialLinks = true;
        public static bool ShowSettingsPassword = true;
        public static bool ShowSettingsBlockedUsers = true;
        public static bool ShowSettingsDeleteAccount = true;
        public static bool ShowSettingsTwoFactor = true; 
        public static bool ShowSettingsManageSessions = true;  
        public static bool ShowSettingsVerification = true; //#New 

        //Settings Page >> Privacy
        public static bool ShowSettingsPrivacy = true;
        public static bool ShowSettingsNotification = true;

        //Settings Page >> Tell a Friends (Earnings)
        public static bool ShowSettingsInviteFriends = false;

        public static bool ShowSettingsShare = false;
        public static bool ShowSettingsMyAffiliates = false;
         
        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// Just replace it with this 5 lines of code
        /// <uses-permission android:name="android.permission.READ_CONTACTS" />
        /// <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" />
        /// <uses-permission android:name="android.permission.SEND_SMS" />
        /// </summary>
        public static bool InvitationSystem = false;

        public static int LimitGoProPlansCountsTo = 4;

        //Settings Page >> Help && Support
        public static bool ShowSettingsHelpSupport = true;

        public static bool ShowSettingsHelp = true;
        public static bool ShowSettingsReportProblem = true;
        public static bool ShowSettingsAbout = true;
        public static bool ShowSettingsPrivacyPolicy = true;
        public static bool ShowSettingsTermsOfUse = true;

        public static bool ShowSettingsInvitationLinks = false; //#New 
        public static bool ShowSettingsMyInformation = false; //#New 

        public static bool ShowSuggestedUsersOnRegister = true;

        public static bool ImageCropping = true;

        //Set Theme Tab
        //*********************************************************
        public static bool SetTabColoredTheme = false;
        public static bool SetTabDarkTheme = false;
        public static bool SetBeautyTheme = true;
        public static readonly string TabColoredColor = MainColor;
        public static bool SetTabOnButton = true;
        public static bool SetTabIsTitledWithText = false;

        //Bypass Web Errors  
        //*********************************************************
        public static bool TurnTrustFailureOnWebException = false;
        public static bool TurnSecurityProtocolType3072On = false;

        //*********************************************************
        public static bool RenderPriorityFastPostLoad = false;

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="com.android.vending.BILLING" />
        /// </summary>
        public static bool ShowInAppBilling = false; //#New 

        public static bool ShowPaypal = true; //#New 
        public static bool ShowBankTransfer = true; //#New 
        public static bool ShowCreditCard = true; //#New 
        //*********************************************************

    }
}