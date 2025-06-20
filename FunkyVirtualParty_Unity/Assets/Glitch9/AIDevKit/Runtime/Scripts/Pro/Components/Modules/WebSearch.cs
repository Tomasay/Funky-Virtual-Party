using UnityEngine;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Web Search")]
    public class WebSearch : AIModuleComponent
    {
        [SerializeField] private WebSearchOptions webSearchOptions = new();
        public WebSearchOptions WebSearchOptions => webSearchOptions ??= new();

        public SearchContextSize? SearchContextSize
        {
            get => webSearchOptions.SearchContextSize;
            set
            {
                webSearchOptions ??= new WebSearchOptions();
                webSearchOptions.SearchContextSize = value;
            }
        }

        public Country? Country
        {
            get => webSearchOptions.UserLocation?.Approximate?.Country;
            set
            {
                webSearchOptions ??= new WebSearchOptions();
                webSearchOptions.UserLocation ??= new UserLocation();
                webSearchOptions.UserLocation.Approximate ??= new Location();
                webSearchOptions.UserLocation.Approximate.Country = value;
            }
        }

        public string City
        {
            get => webSearchOptions.UserLocation?.Approximate?.City;
            set
            {
                webSearchOptions ??= new WebSearchOptions();
                webSearchOptions.UserLocation ??= new UserLocation();
                webSearchOptions.UserLocation.Approximate ??= new Location();
                webSearchOptions.UserLocation.Approximate.City = value;
            }
        }

        public string Region
        {
            get => webSearchOptions.UserLocation?.Approximate?.Region;
            set
            {
                webSearchOptions ??= new WebSearchOptions();
                webSearchOptions.UserLocation ??= new UserLocation();
                webSearchOptions.UserLocation.Approximate ??= new Location();
                webSearchOptions.UserLocation.Approximate.Region = value;
            }
        }

        public TimeZone? TimeZone
        {
            get => webSearchOptions.UserLocation?.Approximate?.Timezone;
            set
            {
                webSearchOptions ??= new WebSearchOptions();
                webSearchOptions.UserLocation ??= new UserLocation();
                webSearchOptions.UserLocation.Approximate ??= new Location();
                webSearchOptions.UserLocation.Approximate.Timezone = value;
            }
        }
    }
}