using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace Glitch9.IO.RSSReader
{
    public class RSSReader
    {
        private readonly string _feedUrl;
        private readonly List<FeedDocument> _feedDocuments = new();

        public RSSReader(string feedUrl)
        {
            _feedUrl = feedUrl;
        }

        public async UniTask Load(Action<List<FeedDocument>> onResult)
        {
            await GetRSSFeed(_feedUrl, onResult);
        }

        private async UniTask GetRSSFeed(string url, Action<List<FeedDocument>> onResult)
        {
            using UnityWebRequest www = UnityWebRequest.Get(url);
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string rssText = www.downloadHandler.text;
                await ParseRSS(rssText, onResult);
            }
            else
            {
                Debug.Log("RSS Download Error: " + www.error);
                onResult?.Invoke(null);
            }
        }

        private async UniTask ParseRSS(string rssText, Action<List<FeedDocument>> onResult)
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(rssText);

            XmlNamespaceManager manager = new(xmlDoc.NameTable);
            manager.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//item");

            if (itemNodes != null && itemNodes.Count > 0)
            {
                foreach (XmlNode itemNode in itemNodes)
                {
                    try
                    {
                        FeedDocument feedDoc = new()
                        {
                            Title = WebUtility.HtmlDecode(itemNode.SelectSingleNode("title").InnerText),
                            Url = itemNode.SelectSingleNode("link").InnerText,
                            PublishedDate = DateTime.Parse(itemNode.SelectSingleNode("pubDate").InnerText, CultureInfo.InvariantCulture),
                            Content = WebUtility.HtmlDecode(itemNode.SelectSingleNode("description").InnerText)
                        };

                        feedDoc.SetCategory(itemNode.SelectSingleNode("category")?.InnerText);

                        _feedDocuments.Add(feedDoc);

                        // Find the first image URL in the content.
                        string content = itemNode.SelectSingleNode("content:encoded", manager).InnerText;

                        int imgStart = content.IndexOf("<img ");
                        if (imgStart >= 0)
                        {
                            int srcStart = content.IndexOf("src=\"", imgStart) + 5;
                            int srcEnd = content.IndexOf("\"", srcStart);
                            if (srcStart >= 5 && srcEnd > srcStart)
                            {
                                string imgUrl = content.Substring(srcStart, srcEnd - srcStart);
                                await DownloadImage(feedDoc, imgUrl);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error parsing RSS item: " + e.Message);
                    }
                }
            }

            onResult?.Invoke(_feedDocuments);
        }


        private async UniTask DownloadImage(FeedDocument feedDoc, string url)
        {
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                feedDoc.Image = texture;
            }
            else
            {
                Debug.Log("Image Download Error: " + www.error);
            }
        }
    }
}