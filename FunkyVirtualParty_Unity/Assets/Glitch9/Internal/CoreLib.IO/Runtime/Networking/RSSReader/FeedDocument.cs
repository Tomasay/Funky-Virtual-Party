using System;
using UnityEngine;

namespace Glitch9.IO.RSSReader
{
    public class FeedDocument
    {
        public Texture2D Image;
        public string Title;
        public string Content;
        public string Url;
        public FeedCategory Category;
        public DateTime PublishedDate;

        public void SetCategory(string text)
        {
            Category = (FeedCategory)Enum.Parse(typeof(FeedCategory), text);
        }
    }
}