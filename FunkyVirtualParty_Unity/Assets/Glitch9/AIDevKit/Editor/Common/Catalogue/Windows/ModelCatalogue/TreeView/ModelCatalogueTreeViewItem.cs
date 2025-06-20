using System;
using System.Collections.Generic;
using Glitch9.Editor.IMGUI;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    public class ModelCatalogueTreeViewItem : ExtendedTreeViewItem
        <
            ModelCatalogueTreeViewItem,
            ModelCatalogueEntry,
            ModelCatalogueFilter
        >
    {
        private const string kUnknownModel = "Unknown Model";

        public string Id
        {
            get
            {
                if (Data == null) return null;
                return Data.Id;
            }
        }

        public string Name
        {
            get
            {
                if (Data == null) return kUnknownModel;
                return Data.Name;
            }
        }

        public string Description
        {
            get
            {
                if (Data == null) return "-";
                return Data.Description;
            }
        }


        public int? InputTokenLimit
        {
            get
            {
                if (Data == null || Data.InputTokenLimit == null) return null;
                return Data.InputTokenLimit;
            }
        }

        public int? OutputTokenLimit
        {
            get
            {
                if (Data == null || Data.OutputTokenLimit == null) return null;
                return Data.OutputTokenLimit;
            }
        }

        public UnixTime CreatedAt
        {
            get
            {
                if (Data == null || Data.CreatedAt == null) return UnixTime.MinValue;
                return Data.CreatedAt.Value;
            }
        }

        public Api Api
        {
            get
            {
                if (Data == null) return Api.None;
                return Data.Api;
            }
        }

        public string ModelProvider
        {
            get
            {
                if (Data == null) return "-";
                return Data.Provider;
            }
        }

        public ModelFeature Capability
        {
            get
            {
                if (Data == null) return ModelFeature.TextGeneration;
                return Data.Feature;
            }
        }

        public int Performance
        {
            get
            {
                if (Data == null) return 0;
                return Data.Performance;
            }
        }

        public int Speed
        {
            get
            {
                if (Data == null) return 0;
                return Data.Speed;
            }
        }

        public Modality InputModality
        {
            get
            {
                if (Data == null) return 0;
                return Data.InputModality;
            }
        }

        public Modality OutputModality
        {
            get
            {
                if (Data == null) return 0;
                return Data.OutputModality;
            }
        }

        public Dictionary<UsageType, double> Pricing
        {
            get
            {
                if (Data == null) return new Dictionary<UsageType, double>();
                return Data.Pricing;
            }
        }

        public bool IsCustom
        {
            get
            {
                if (Data == null) return false;
                return Data.IsFineTuned && Api != Api.Ollama;
            }
        }

        public bool IsLocal
        {
            get
            {
                if (Data == null) return false;
                return Api == Api.Ollama;
            }
        }

        public bool IsLegacy
        {
            get
            {
                if (Data == null) return false;
                return Data.IsLegacy;
            }
        }

        public bool IsObsolete
        {
            get
            {
                if (Data == null) return false;
                return Data.IsDeprecated;
            }
        }

        public string FamilyDisplayName => GetFamilyDisplayName();
        public bool IsNew => GetIsNew();
        public bool IsDefault => _isDefault ??= ModelUtil.IsDefaultModel(Id, Api, Capability);
        public double Per1MInputToken => GetPer1MInputToken();
        public double Per1MOutputToken => GetPer1MOutputToken();
        public bool CanDelete => !IsDefault && !IsLocked;
        public bool InMyLibrary { get; internal set; }
        public readonly bool IsLocked;

        public ModelCatalogueTreeViewItem(int id, int depth, string displayName, ModelCatalogueEntry data) : base(id, depth, displayName, data)
        {
            try
            {
                InMyLibrary = !string.IsNullOrEmpty(data.Id) && ModelLibrary.Contains(data.Id);
                if (InMyLibrary)
                {
                    var modelData = ModelLibrary.Get(data.Id);
                    IsLocked = modelData.IsLocked;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize ModelCatalogueTreeViewItem.HasThisVoice: {e}");
                InMyLibrary = false;
            }
        }

        public override int CompareTo(ModelCatalogueTreeViewItem anotherItem, int columnIndex, bool ascending)
        {
            try
            {
                return columnIndex switch
                {
                    ModelCatalogueWindow.ColumnIndex.IN_LIB => CompareByBool(ascending, anotherItem, data => data.InMyLibrary),
                    ModelCatalogueWindow.ColumnIndex.API => CompareByString(ascending, anotherItem, data => data.Api.ToString()),
                    //ModelCatalogueWindow.ColumnIndex.PROVIDER => CompareByString(ascending, anotherItem, data => data.ModelProvider),
                    ModelCatalogueWindow.ColumnIndex.NAME => CompareByString(ascending, anotherItem, data => data.Name),
                    ModelCatalogueWindow.ColumnIndex.FAMILY => CompareByString(ascending, anotherItem, data => data.Data?.Family),
                    ModelCatalogueWindow.ColumnIndex.FEATURES => CompareByFlags(ascending, anotherItem, data => data.Capability),
                    ModelCatalogueWindow.ColumnIndex.CREATED => CompareByUnixTime(ascending, anotherItem, data => data.CreatedAt),
                    ModelCatalogueWindow.ColumnIndex.PER_INPUT_TOKEN => CompareByDouble(ascending, anotherItem, data => data.Per1MInputToken),
                    ModelCatalogueWindow.ColumnIndex.PER_OUTPUT_TOKEN => CompareByDouble(ascending, anotherItem, data => data.Per1MOutputToken),
                    ModelCatalogueWindow.ColumnIndex.PERFORMANCE => CompareByInt(ascending, anotherItem, data => data.Performance),
                    ModelCatalogueWindow.ColumnIndex.SPEED => CompareByInt(ascending, anotherItem, data => data.Speed),
                    _ => 0
                };
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return 0;
            }
        }

        public override bool Search(string searchString)
        {
            if (Data == null) return false;
            if (string.IsNullOrEmpty(searchString)) return true;
            if (!string.IsNullOrEmpty(Id) && Id.Contains(searchString)) return true;
            if (!string.IsNullOrEmpty(Name) && Name.Contains(searchString)) return true;
            return false;
        }



        #region Utility Methods 

        private string _familyDisplayName;
        private string GetFamilyDisplayName()
        {
            if (_familyDisplayName == null)
            {
                if (Data == null)
                {
                    _familyDisplayName = "-";
                }
                else
                {
                    _familyDisplayName = Data.Family;

                    if (!_familyDisplayName.Contains('(')
                        && !string.IsNullOrEmpty(Data.Provider))
                    {
                        _familyDisplayName = $"{Data.Family} ({Data.Provider})";
                    }
                }
            }
            return _familyDisplayName;
        }

        private bool? _isDefault;

        private bool? _isNew;
        private bool GetIsNew()
        {
            if (_isNew == null)
            {
                // check if this model is released within the last 30 days
                if (CreatedAt == null)
                {
                    _isNew = false;
                }
                else
                {
                    var timeDiff = DateTime.UtcNow - CreatedAt.ToDateTime();
                    _isNew = timeDiff.TotalDays < 30;
                }
            }
            return _isNew.Value;
        }

        private double? _per1MInputToken;
        private double GetPer1MInputToken()
        {
            if (_per1MInputToken == null)
            {
                if (Pricing.ContainsKey(UsageType.Free))
                {
                    _per1MInputToken = AIDevKitConfig.FreePriceMagicNumber;
                }
                else if (Pricing.TryGetValue(UsageType.InputToken, out double value))
                {
                    _per1MInputToken = value * 1_000_000;
                }
                else
                {
                    _per1MInputToken = -2;
                }
            }
            return _per1MInputToken.Value;
        }

        private double? _per1MOutputToken;
        private double GetPer1MOutputToken()
        {
            if (_per1MOutputToken == null)
            {
                if (Pricing.ContainsKey(UsageType.Free))
                {
                    _per1MOutputToken = AIDevKitConfig.FreePriceMagicNumber;
                }
                else if (Pricing.TryGetValue(UsageType.OutputToken, out double value))
                {
                    _per1MOutputToken = value * 1_000_000;
                }
                else
                {
                    _per1MOutputToken = -2;
                }
            }
            return _per1MOutputToken.Value;
        }

        #endregion Utility Methods
    }
}
