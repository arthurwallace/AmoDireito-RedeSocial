﻿using Android.Support.V7.Widget;
using Android.Views;
using System;
using WoWonder.Activities.Comment.Adapters;
using WoWonderClient.Classes.Posts;

namespace WoWonder.Activities.NativePost.Post
{
 
    public class GlobalClickEventArgs : EventArgs
    {
        public int Position { get; set; }
        public AdapterHolders.SoundPostViewHolder HolderSound { get; set; }  
        public View View { get; set; }
        public PostDataObject NewsFeedClass { get; set; }
    }

    public class CommentReplyClickEventArgs : EventArgs
    {
        public int Position { get; set; }
        public View View { get; set; }
        public CommentObjectExtra CommentObject { get; set; }
    }

    public class ProfileClickEventArgs : EventArgs
    {
        public int Position { get; set; }
        public RecyclerView.ViewHolder Holder { get; set; }
        public View View { get; set; }
        public CommentObjectExtra CommentClass { get; set; }
        public PostDataObject NewsFeedClass { get; set; }
    }
}