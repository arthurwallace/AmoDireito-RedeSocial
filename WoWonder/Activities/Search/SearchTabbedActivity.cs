﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using WoWonder.Activities.NativePost.Pages;
using WoWonder.Activities.Search.Fragment;
using WoWonder.Activities.Tabbes;
using WoWonder.Activities.Tabbes.Adapters;
using WoWonder.Adapters;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.User;
using WoWonderClient.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.Search
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SearchTabbedActivity : AppCompatActivity, TextView.IOnEditorActionListener
    {
        #region Variables Basic

        private AppBarLayout AppBarLayout;
        private TabLayout TabLayout;
        public ViewPager ViewPager;
        private AutoCompleteTextView SearchView;
        private RecyclerView HashRecyclerView;
        public string DataKey, SearchText = "";
        public string OffsetUser = "", OffsetPage = "", OffsetGroup = "";
        public SearchUserFragment UserTab;
        public SearchPagesFragment PagesTab;
        public SearchGroupsFragment GroupsTab;
        private FloatingActionButton FloatingActionButtonView;
        private Toolbar Toolbar;
        
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                Window.SetSoftInputMode(SoftInput.AdjustNothing);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.Search_Tabbed_Layout);

                DataKey = Intent.GetStringExtra("Key") ?? "Data not available";
                if (DataKey != "Data not available" && !string.IsNullOrEmpty(DataKey))
                {
                    SearchText = DataKey; 
                }

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
               

                if (SearchText == "Random" || SearchText == "Random_Groups" || SearchText == "Random_Pages")
                {
                    SearchText = "a";
                }
                else if (!string.IsNullOrEmpty(SearchText))
                {
                    Search(SearchText);
                }
                else
                {
                    if (SearchView == null) 
                        return;
                    //SearchView.SetQuery(SearchText, false);
                    SearchView.ClearFocus();
                    //SearchView.OnActionViewCollapsed();
                } 
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
                if (SearchView != null)
                {
                    //SearchView.SetQuery("", false);
                    SearchView.ClearFocus();
                    //SearchView.OnActionViewCollapsed();
                }

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
                TabLayout = FindViewById<TabLayout>(Resource.Id.Searchtabs);
                ViewPager = FindViewById<ViewPager>(Resource.Id.Searchviewpager);
                 
                AppBarLayout = FindViewById<AppBarLayout>(Resource.Id.mainAppBarLayout);
                AppBarLayout.SetExpanded(true);
                 
                HashRecyclerView = FindViewById<RecyclerView>(Resource.Id.HashRecyler);

                if (AppSettings.ShowTrendingHashTags)
                {
                    if (TabbedMainActivity.GetInstance()?.HashTagUserAdapter?.MHashtagList?.Count > 0)
                    {
                        HashRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal,false));
                        HashRecyclerView.SetAdapter(TabbedMainActivity.GetInstance().HashTagUserAdapter);
                        TabbedMainActivity.GetInstance().HashTagUserAdapter.ItemClick += HashTagUserAdapterOnItemClick;
                        HashRecyclerView.SetAdapter(TabbedMainActivity.GetInstance().HashTagUserAdapter);
                        TabbedMainActivity.GetInstance()?.HashTagUserAdapter.NotifyDataSetChanged();

                        HashRecyclerView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        HashRecyclerView.Visibility = ViewStates.Invisible;
                    }
                }
                else
                {
                    HashRecyclerView.Visibility = ViewStates.Invisible;
                }

                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);
                   
                ViewPager.OffscreenPageLimit = 3;
                SetUpViewPager(ViewPager);
                TabLayout.SetupWithViewPager(ViewPager); 
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
                Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (Toolbar != null)
                {
                    Toolbar.Title = " ";
                    Toolbar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(Toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true); 
                }

                SearchView = FindViewById<AutoCompleteTextView>(Resource.Id.searchBox);
                SearchView.SetOnEditorActionListener(this);
                //SearchView.ClearFocus();

                //Change text colors
                SearchView.SetHintTextColor(Color.ParseColor("#efefef"));
                SearchView.SetTextColor(Color.White);
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
                    FloatingActionButtonView.Click += FloatingActionButtonViewOnClick;
                }
                else
                {
                    FloatingActionButtonView.Click -= FloatingActionButtonViewOnClick;
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
                TabLayout = null;
                ViewPager = null;
                AppBarLayout = null;
                HashRecyclerView = null;
                Toolbar = null;
                SearchText = null;
                OffsetUser = "";
                OffsetPage = "";
                OffsetGroup = "";
                DataKey = "";
                SearchText = ""; 
                UserTab = null;
                PagesTab = null;
                GroupsTab = null;
                FloatingActionButtonView = null; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        #endregion

        #region Set Tab 

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                UserTab = new SearchUserFragment();
                PagesTab = new SearchPagesFragment();
                GroupsTab = new SearchGroupsFragment();

                var adapter = new MainTabAdapter(SupportFragmentManager);
                adapter.AddFragment(UserTab, GetText(Resource.String.Lbl_Users));
                adapter.AddFragment(PagesTab, GetText(Resource.String.Lbl_Pages));
                adapter.AddFragment(GroupsTab, GetText(Resource.String.Lbl_Groups));

                viewPager.OffscreenPageLimit = 3;
                viewPager.Adapter = adapter;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Events

        private void HashTagUserAdapterOnItemClick(object sender, HashtagUserAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = TabbedMainActivity.GetInstance()?.HashTagUserAdapter.GetItem(position);
                    if (item != null)
                    {
                        string id = item.Hash.Replace("#", "").Replace("_", " ");
                        string tag = item.Tag.Replace("#", "");
                        var intent = new Intent(this, typeof(HashTagPostsActivity));
                        intent.PutExtra("Id", id);
                        intent.PutExtra("Tag", tag);
                        StartActivity(intent);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Filter
        private void FloatingActionButtonViewOnClick(object sender, EventArgs e)
        {
            try
            {
                FilterSearchDialogFragment mFragment = new FilterSearchDialogFragment();
                mFragment.Show(SupportFragmentManager, mFragment.Tag);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SearchViewOnQueryTextSubmit(string newText)
        {
            try
            {
                SearchText = newText;

                SearchView.ClearFocus();

                UserTab.MAdapter.UserList.Clear();
                UserTab.MAdapter.NotifyDataSetChanged();

                PagesTab.MAdapter.PageList.Clear();
                PagesTab.MAdapter.NotifyDataSetChanged();

                GroupsTab.MAdapter.GroupList.Clear();
                GroupsTab.MAdapter.NotifyDataSetChanged();

                OffsetUser = "0";
                OffsetPage = "0";
                OffsetGroup = "0";

                if (Methods.CheckConnectivity())
                {  
                    if (UserTab.ProgressBarLoader != null)
                        UserTab.ProgressBarLoader.Visibility = ViewStates.Visible;

                    if (PagesTab.ProgressBarLoader != null)
                        PagesTab.ProgressBarLoader.Visibility = ViewStates.Visible;

                    if (GroupsTab.ProgressBarLoader != null)
                        GroupsTab.ProgressBarLoader.Visibility = ViewStates.Visible;

                    UserTab.EmptyStateLayout.Visibility = ViewStates.Gone;
                    PagesTab.EmptyStateLayout.Visibility = ViewStates.Gone;
                    GroupsTab.EmptyStateLayout.Visibility = ViewStates.Gone;


                    StartApiService();
                }
                else
                {
                    if (UserTab.Inflated == null)
                        UserTab.Inflated = UserTab.EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(UserTab.Inflated, EmptyStateInflater.Type.NoConnection);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click -= EmptyStateButtonOnClick;
                        x.EmptyStateButton.Click -= TryAgainButton_Click;
                        x.EmptyStateButton.Click += null;
                    }

                    x.EmptyStateButton.Click += TryAgainButton_Click;
                    UserTab.ProgressBarLoader.Visibility = ViewStates.Gone;
                    UserTab.EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion
         
        #region Load Data Search 

        public void Search(string text)
        {
            try
            {
                SearchText = text;

                if (!string.IsNullOrEmpty(SearchText))
                {  
                    if (Methods.CheckConnectivity())
                    {
                        UserTab.MAdapter?.UserList?.Clear();
                        UserTab.MAdapter?.NotifyDataSetChanged();

                        PagesTab.MAdapter?.PageList?.Clear();
                        PagesTab.MAdapter?.NotifyDataSetChanged();

                        GroupsTab.MAdapter?.GroupList?.Clear();
                        GroupsTab.MAdapter?.NotifyDataSetChanged();

                        if (UserTab.ProgressBarLoader != null)
                            UserTab.ProgressBarLoader.Visibility = ViewStates.Visible;

                        if (PagesTab.ProgressBarLoader != null)
                            PagesTab.ProgressBarLoader.Visibility = ViewStates.Visible;
                         
                        if (GroupsTab.ProgressBarLoader != null)
                            GroupsTab.ProgressBarLoader.Visibility = ViewStates.Visible;
                         
                        UserTab.EmptyStateLayout.Visibility = ViewStates.Gone;
                        PagesTab.EmptyStateLayout.Visibility = ViewStates.Gone;
                        GroupsTab.EmptyStateLayout.Visibility = ViewStates.Gone;

                        StartApiService(); 
                    }
                }
                else
                {
                    if (UserTab.Inflated == null)
                        UserTab.Inflated = UserTab.EmptyStateLayout?.Inflate();

                    EmptyStateInflater x1 = new EmptyStateInflater();
                    x1.InflateLayout(UserTab.Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x1.EmptyStateButton.HasOnClickListeners)
                    {
                        x1.EmptyStateButton.Click -= EmptyStateButtonOnClick;
                        x1.EmptyStateButton.Click -= TryAgainButton_Click;
                        x1.EmptyStateButton.Click += null;
                    }

                    x1.EmptyStateButton.Click += TryAgainButton_Click;
                    if (UserTab.EmptyStateLayout != null)
                    {
                        UserTab.EmptyStateLayout.Visibility = ViewStates.Visible;
                    } 

                    UserTab.ProgressBarLoader.Visibility = ViewStates.Gone;

                    //============================================== 
                    if (PagesTab.Inflated == null)
                        PagesTab.Inflated = PagesTab.EmptyStateLayout?.Inflate();

                    EmptyStateInflater x2 = new EmptyStateInflater();
                    x2.InflateLayout(PagesTab.Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x2.EmptyStateButton.HasOnClickListeners)
                    {
                        x2.EmptyStateButton.Click -= EmptyStateButtonOnClick;
                        x2.EmptyStateButton.Click -= TryAgainButton_Click;
                        x2.EmptyStateButton.Click += null;
                    }

                    x2.EmptyStateButton.Click += TryAgainButton_Click;
                    if (PagesTab.EmptyStateLayout != null)
                    {
                        PagesTab.EmptyStateLayout.Visibility = ViewStates.Visible;
                    }
                     
                    PagesTab.ProgressBarLoader.Visibility = ViewStates.Gone;

                    //============================================== 
                    if (GroupsTab.Inflated == null)
                        GroupsTab.Inflated = GroupsTab.EmptyStateLayout?.Inflate();

                    EmptyStateInflater x3 = new EmptyStateInflater();
                    x3.InflateLayout(GroupsTab.Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x3.EmptyStateButton.HasOnClickListeners)
                    {
                        x3.EmptyStateButton.Click -= EmptyStateButtonOnClick;
                        x3.EmptyStateButton.Click -= TryAgainButton_Click;
                        x3.EmptyStateButton.Click += null;
                    }

                    x3.EmptyStateButton.Click += TryAgainButton_Click;
                    if (GroupsTab.EmptyStateLayout != null)
                    {
                        GroupsTab.EmptyStateLayout.Visibility = ViewStates.Visible;
                    }
                     
                    GroupsTab.ProgressBarLoader.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { StartSearchRequest  });
        }

        private async Task StartSearchRequest()
        {
            if (UserTab.MainScrollEvent.IsLoading)
                return;

            UserTab.MainScrollEvent.IsLoading = true;
            PagesTab.MainScrollEvent.IsLoading = true;
            GroupsTab.MainScrollEvent.IsLoading = true;

            int countUserList = UserTab.MAdapter.UserList.Count;
            int countPageList = PagesTab.MAdapter.PageList.Count;
            int countGroupList = GroupsTab.MAdapter.GroupList.Count;
             
            var dictionary = new Dictionary<string, string>
            {
                {"user_id", UserDetails.UserId},
                {"limit", "30"},
                {"user_offset", OffsetUser},
                {"group_offset", OffsetGroup},
                {"page_offset", OffsetPage},
                {"gender", UserDetails.SearchGender},
                {"search_key", SearchText},
                {"country", UserDetails.SearchCountry},
                {"status", UserDetails.SearchStatus},
                {"verified", UserDetails.SearchVerified},
                {"filterbyage", UserDetails.SearchFilterByAge},
                {"age_from", UserDetails.SearchAgeFrom},
                {"age_to", UserDetails.SearchAgeTo},
                {"image", UserDetails.SearchProfilePicture}, 
            };

            (int apiStatus, var respond) = await RequestsAsync.Global.Get_Search(dictionary);
            if (apiStatus == 200)
            {
                if (respond is GetSearchObject result)
                {
                    var respondUserList = result.Users?.Count;
                    if (respondUserList > 0)
                    {
                        if (countUserList > 0)
                        {
                            foreach (var item in from item in result.Users let check = UserTab.MAdapter.UserList.FirstOrDefault(a => a.UserId == item.UserId) where check == null select item)
                            {
                                UserTab.MAdapter.UserList.Add(item);
                            }

                            RunOnUiThread(() => { UserTab.MAdapter.NotifyItemRangeInserted(countUserList - 1, UserTab.MAdapter.UserList.Count - countUserList); });
                        }
                        else
                        {
                            UserTab.MAdapter.UserList = new ObservableCollection<UserDataObject>(result.Users);
                            RunOnUiThread(() => { UserTab.MAdapter.NotifyDataSetChanged(); }); 
                        }
                    }
                    else
                    {
                        if (UserTab.MAdapter.UserList.Count > 10 && !UserTab.MRecycler.CanScrollVertically(1))
                            Toast.MakeText(this, GetText(Resource.String.Lbl_No_more_users), ToastLength.Short).Show();
                    }

                    var respondPageList = result.Pages?.Count;
                    if (respondPageList > 0)
                    {
                        if (countPageList > 0)
                        {
                            foreach (var item in from item in result.Pages let check = PagesTab.MAdapter.PageList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                            {
                                PagesTab.MAdapter.PageList.Add(item);
                            }

                            RunOnUiThread(() => { PagesTab.MAdapter.NotifyItemRangeInserted(countPageList - 1, PagesTab.MAdapter.PageList.Count - countPageList); });
                        }
                        else
                        {
                            PagesTab.MAdapter.PageList = new ObservableCollection<PageClass>(result.Pages);
                            RunOnUiThread(() => { PagesTab.MAdapter.NotifyDataSetChanged(); }); 
                        }
                    }
                    else
                    {
                        if (PagesTab.MAdapter.PageList.Count > 10 && !PagesTab.MRecycler.CanScrollVertically(1))
                            Toast.MakeText(this, GetText(Resource.String.Lbl_NoMorePages), ToastLength.Short).Show();
                    }

                    var respondGroupList = result.Groups?.Count;
                    if (respondGroupList > 0)
                    {
                        if (countGroupList > 0)
                        {
                            foreach (var item in from item in result.Groups let check = GroupsTab.MAdapter.GroupList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                            {
                                GroupsTab.MAdapter.GroupList.Add(item);
                            }

                            RunOnUiThread(() => { GroupsTab.MAdapter.NotifyItemRangeInserted(countGroupList - 1, GroupsTab.MAdapter.GroupList.Count - countGroupList); });
                        }
                        else
                        {
                            GroupsTab.MAdapter.GroupList = new ObservableCollection<GroupClass>(result.Groups);
                            RunOnUiThread(() => { GroupsTab.MAdapter.NotifyDataSetChanged(); });
                        }
                    }
                    else
                    {
                        if (GroupsTab.MAdapter.GroupList.Count > 10 && !GroupsTab.MRecycler.CanScrollVertically(1))
                            Toast.MakeText(this, GetText(Resource.String.Lbl_NoMoreGroup), ToastLength.Short).Show();
                    }
                }
            }
            else Methods.DisplayReportResult(this, respond);
             
            RunOnUiThread(ShowEmptyPage);
            UserTab.MainScrollEvent.IsLoading = false;
            PagesTab.MainScrollEvent.IsLoading = false;
            GroupsTab.MainScrollEvent.IsLoading = false;
        }

        private void ShowEmptyPage()
        {
            try
            {
                UserTab.ProgressBarLoader.Visibility = ViewStates.Gone;
                PagesTab.ProgressBarLoader.Visibility = ViewStates.Gone;
                GroupsTab.ProgressBarLoader.Visibility = ViewStates.Gone;

                if (UserTab.MAdapter.UserList.Count > 0)
                {
                    UserTab.EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (UserTab.Inflated == null)
                        UserTab.Inflated = UserTab.EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(UserTab.Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click -= EmptyStateButtonOnClick;
                        x.EmptyStateButton.Click -= TryAgainButton_Click;
                    }

                    x.EmptyStateButton.Click += TryAgainButton_Click;
                    UserTab.EmptyStateLayout.Visibility = ViewStates.Visible;
                }


                if (PagesTab.MAdapter.PageList.Count > 0)
                {
                    PagesTab.EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (PagesTab.Inflated == null)
                        PagesTab.Inflated = PagesTab.EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(PagesTab.Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click -= EmptyStateButtonOnClick;
                        x.EmptyStateButton.Click -= TryAgainButton_Click;
                    }

                    x.EmptyStateButton.Click += TryAgainButton_Click;
                    PagesTab.EmptyStateLayout.Visibility = ViewStates.Visible;
                }

                if (GroupsTab.MAdapter.GroupList.Count > 0)
                {
                    GroupsTab.EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (GroupsTab.Inflated == null)
                        GroupsTab.Inflated = GroupsTab.EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(GroupsTab.Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click -= EmptyStateButtonOnClick;
                        x.EmptyStateButton.Click -= TryAgainButton_Click;
                        x.EmptyStateButton.Click += null;
                    }

                    x.EmptyStateButton.Click += TryAgainButton_Click;
                    GroupsTab.EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                //SwipeRefreshLayout.Refreshing = false;
                Console.WriteLine(e);
            }
        }

        //No Internet Connection 
        private void TryAgainButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (UserTab.EmptyStateLayout != null) UserTab.EmptyStateLayout.Visibility = ViewStates.Gone;

                ViewPager.SetCurrentItem(0, true);

                Search("a");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                SearchView.ClearFocus();
                UserTab.MAdapter.UserList.Clear();
                UserTab.MAdapter.NotifyDataSetChanged();

                PagesTab.MAdapter.PageList.Clear();
                PagesTab.MAdapter.NotifyDataSetChanged();

                GroupsTab.MAdapter.GroupList.Clear();
                GroupsTab.MAdapter.NotifyDataSetChanged();
                 
                OffsetUser = "0";
                OffsetPage = "0";
                OffsetGroup = "0"; 

                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchText = "a";
                }

                ViewPager.SetCurrentItem(0, true);

                if (Methods.CheckConnectivity())
                {
                    UserTab.EmptyStateLayout.Visibility = ViewStates.Gone;
                    UserTab.ProgressBarLoader.Visibility = ViewStates.Visible;
                    StartApiService();
                }
                else
                {
                    if (UserTab.Inflated == null)
                        UserTab.Inflated = UserTab.EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(UserTab.Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click -= EmptyStateButtonOnClick;
                        x.EmptyStateButton.Click -= TryAgainButton_Click;
                        x.EmptyStateButton.Click += null;
                    }

                    x.EmptyStateButton.Click += TryAgainButton_Click;
                    UserTab.EmptyStateLayout.Visibility = ViewStates.Visible;
                    UserTab.ProgressBarLoader.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Search)
            {
                SearchText = v.Text;

                SearchView.ClearFocus();
                v.ClearFocus();

                SearchViewOnQueryTextSubmit(SearchText);

                SearchView.ClearFocus();
                v.ClearFocus();

                //var input = (InputMethodManager)Activity.GetSystemService(Android.Content.Context.InputMethodService);


                return true;
            }

            return false;
        }
         
    }
}