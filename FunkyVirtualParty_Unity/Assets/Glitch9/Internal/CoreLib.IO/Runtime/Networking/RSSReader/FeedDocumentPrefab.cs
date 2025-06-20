using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.IO.RSSReader
{
    public class FeedDocumentPrefab : MonoBehaviour
    {
        [SerializeField] private Image imgContent;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtContent;
        [SerializeField] private TextMeshProUGUI txtDate;

        public FeedDocument Document { get; set; }

        public void SetData(FeedDocument doc)
        {
            Document = doc;

            if (doc.Image != null)
            {
                Texture2D texture = doc.Image;  // Your Texture2D instance.
                Rect rect = new(0, 0, texture.width, texture.height);
                Vector2 pivot = new(0.5f, 0.5f);  // Pivot at center.
                Sprite sprite = Sprite.Create(texture, rect, pivot);
                imgContent.sprite = sprite;
            }
            else imgContent.gameObject.SetActive(false);

            if (doc.Title != null) txtTitle.text = doc.Title;
            else txtTitle.gameObject.SetActive(false);

            if (doc.Content != null) txtContent.text = doc.Content;
            else txtContent.gameObject.SetActive(false);

            string dayOfWeekName = doc.PublishedDate.ToString("dddd");
            txtDate.text = doc.PublishedDate.ToString("yyyy-MM-dd") + " " + dayOfWeekName;
        }
    }
}