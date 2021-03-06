﻿using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using Exception = System.Exception;

namespace WoWonder.Activities.Jobs
{
    public class FilterJobDialogFragment : BottomSheetDialogFragment, SeekBar.IOnSeekBarChangeListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private JobsActivity ContextJobs;
        private Button BtnApply;
        private TextView IconBack, IconDistance, IconJobType, TxtJobType, JobTypeMoreIcon, IconCategories, TxtCategories, CategoriesMoreIcon, TxtDistanceCount;
        private SeekBar DistanceBar;                                                     
        private RelativeLayout LayoutJobType, LayoutCategories;                          
        private int DistanceCount;
        private string JobType, CategoryId, TypeDialog = "";

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            ContextJobs = (JobsActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                //    View view = inflater.Inflate(Resource.Layout.ButtomSheetJobhFilter, container, false);
                Context contextThemeWrapper = AppSettings.SetTabDarkTheme ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark_Base) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Base);
                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = localInflater.Inflate(Resource.Layout.ButtomSheetJobhFilter, container, false);

                InitComponent(view);

                LayoutJobType.Click += LayoutJobTypeOnClick;
                LayoutCategories.Click += LayoutCategoriesOnClick;
                IconBack.Click += IconBackOnClick; 
                BtnApply.Click += BtnApplyOnClick;
                 
                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
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

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                IconBack = view.FindViewById<TextView>(Resource.Id.IconBack);

                IconDistance = view.FindViewById<TextView>(Resource.Id.IconDistance);
                TxtDistanceCount = view.FindViewById<TextView>(Resource.Id.Distancenumber);

                LayoutJobType = view.FindViewById<RelativeLayout>(Resource.Id.LayoutJobType);
                LayoutCategories = view.FindViewById<RelativeLayout>(Resource.Id.LayoutCategories);

                DistanceBar = view.FindViewById<SeekBar>(Resource.Id.distanceSeeker);
                DistanceBar.Max = 300;
                DistanceBar.SetOnSeekBarChangeListener(this);
                 
                IconJobType = view.FindViewById<TextView>(Resource.Id.IconJobType);
                TxtJobType = view.FindViewById<TextView>(Resource.Id.textJobType);
                JobTypeMoreIcon = view.FindViewById<TextView>(Resource.Id.JobTypeMoreIcon);
               
                IconCategories = view.FindViewById<TextView>(Resource.Id.IconCategories);
                TxtCategories = view.FindViewById<TextView>(Resource.Id.textCategories);
                CategoriesMoreIcon = view.FindViewById<TextView>(Resource.Id.CategoriesMoreIcon);
                 
                BtnApply = view.FindViewById<Button>(Resource.Id.ApplyButton);
                 
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconDistance, FontAwesomeIcon.StreetView);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconBack, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, IconCategories, FontAwesomeIcon.Buromobelexperte);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconJobType, FontAwesomeIcon.Briefcase);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, JobTypeMoreIcon, IonIconsFonts.ChevronRight);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, CategoriesMoreIcon, IonIconsFonts.ChevronRight);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    DistanceBar.SetProgress(string.IsNullOrEmpty(UserDetails.FilterJobLocation) ? 300 : int.Parse(UserDetails.FilterJobLocation), true);
                else  // For API < 24 
                    DistanceBar.Progress = string.IsNullOrEmpty(UserDetails.FilterJobLocation) ? 300 : int.Parse(UserDetails.FilterJobLocation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Event

        //Back
        private void IconBackOnClick(object sender, EventArgs e)
        {
            try
            {
                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Save data 
        private void BtnApplyOnClick(object sender, EventArgs e)
        {
            try
            {
                UserDetails.FilterJobType = JobType; 
                UserDetails.FilterJobLocation = DistanceCount.ToString();
                UserDetails.FilterJobCategories = CategoryId;

                ContextJobs.MAdapter.JobList.Clear();
                ContextJobs.MAdapter.NotifyDataSetChanged();
                ContextJobs.SwipeRefreshLayout.Refreshing = true;

                ContextJobs.StartApiService();

                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
           
        //Categories
        private void LayoutCategoriesOnClick(object sender, EventArgs e)
        {
            try
            { 
                if (CategoriesController.ListCategoriesJob.Count > 0)
                {
                    TypeDialog = "Categories";

                    var dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                    var arrayAdapter = CategoriesController.ListCategoriesJob.Select(item => item.CategoriesName).ToList();

                    dialogList.Title(GetText(Resource.String.Lbl_SelectCategories));
                    dialogList.Items(arrayAdapter);
                    dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Methods.DisplayReportResult(Activity, "Not have List Categories Job");
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //JobType
        private void LayoutJobTypeOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "JobType";

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                arrayAdapter.Add(GetText(Resource.String.Lbl_full_time));
                arrayAdapter.Add(GetText(Resource.String.Lbl_part_time));
                arrayAdapter.Add(GetText(Resource.String.Lbl_internship));
                arrayAdapter.Add(GetText(Resource.String.Lbl_volunteer));
                arrayAdapter.Add(GetText(Resource.String.Lbl_contract)); 

                dialogList.Title(GetText(Resource.String.Lbl_JobType));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
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

                switch (TypeDialog)
                {
                    case "Categories":
                        CategoryId = CategoriesController.ListCategoriesJob.FirstOrDefault(categories => categories.CategoriesName == itemString.ToString())?.CategoriesId;
                        TxtCategories.Text = itemString.ToString();
                        break;
                    case "JobType":
                    {
                        TxtJobType.Text = text;

                        if (text == GetText(Resource.String.Lbl_full_time))
                            JobType = "full_time";
                        else if (text == GetText(Resource.String.Lbl_part_time))
                            JobType = "part_time";
                        else if (text == GetText(Resource.String.Lbl_internship))
                            JobType = "internship";
                        else if (text == GetText(Resource.String.Lbl_volunteer))
                            JobType = "volunteer";
                        else if (text == GetText(Resource.String.Lbl_contract))
                            JobType = "contract";
                        break;
                    }
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
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

        #region SeekBar

        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            try
            {
                TxtDistanceCount.Text = progress + " " + GetText(Resource.String.Lbl_km);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {

        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            try
            {
                DistanceCount = seekBar.Progress;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

    }
}