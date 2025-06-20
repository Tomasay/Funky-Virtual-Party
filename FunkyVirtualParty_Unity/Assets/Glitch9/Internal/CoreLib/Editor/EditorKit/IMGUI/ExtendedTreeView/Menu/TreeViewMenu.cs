using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public class TreeViewMenu
    {
        private const float kSearchFieldY = 2f;
        private const float kDropdownMenuWidth = 120f;
        private const float kDropdownMenuHeight = 20f;

        public bool HasSearchField { get; private set; }
        public bool IsSearchFieldAtStart { get; private set; }

        private SearchField _searchField;
        private string _searchText;
        private readonly Action<string> _onSearchTextChanged;

        private readonly Dictionary<string, TreeViewMenuItem> _menuItems;
        private readonly List<float> _dropdownXPositions = new();
        private readonly float _totalMenuItemsWidth;
        private readonly bool _noMenuItems;


        public TreeViewMenu(IEnumerable<ITreeViewMenuEntry> entries, Action<string> onSearchTextChanged)
        {
            _menuItems = new Dictionary<string, TreeViewMenuItem>();
            _onSearchTextChanged = onSearchTextChanged;
            _dropdownXPositions.Add(0f);
            float currentX = 0f;

            foreach (ITreeViewMenuEntry entry in entries)
            {
                if (entry is TreeViewMenuItem menuItem)
                {
                    _menuItems.Add(menuItem.Name, menuItem);
                    currentX += menuItem.MenuWidth;
                    _dropdownXPositions.Add(currentX);
                    _totalMenuItemsWidth += menuItem.MenuWidth;
                }
                else if (entry is TreeViewMenuSearchField searchField)
                {
                    IsSearchFieldAtStart = searchField.IsAtStart;
                    HasSearchField = true;
                }
            }

            _noMenuItems = _menuItems.Count == 0;
        }

        private Rect GetDropdownRect(int buttonIndex)
        {
            float xPos = _dropdownXPositions[buttonIndex];
            return new Rect(xPos, 0, kDropdownMenuWidth, kDropdownMenuHeight);
        }

        private Rect GetSearchFieldRect(Rect position)
        {
            const float kPadding = 4f;

            if (_noMenuItems)
            {
                return new Rect(kPadding, kSearchFieldY, position.width - (kPadding * 2), 24);
            }

            float leftOverViewWidth = position.width - _totalMenuItemsWidth;
            float maxWidth = position.width * 0.5f;
            float searchFieldWidth = Mathf.Min(leftOverViewWidth, maxWidth);
            return new Rect(position.width - searchFieldWidth - kPadding, kSearchFieldY, searchFieldWidth, 24);
        }

        internal void Draw(Rect position)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (HasSearchField && IsSearchFieldAtStart)
                {
                    DrawSearchField(GetSearchFieldRect(position));
                }

                for (int i = 0; i < _menuItems.Count; i++)
                {
                    KeyValuePair<string, TreeViewMenuItem> kvp = _menuItems.ElementAt(i);

                    if (kvp.Value.Equals(null)) continue;

                    if (kvp.Value is TreeViewMenuDropdown menuItem)
                    {
                        string menuName = kvp.Key;
                        int menuWidth = menuItem.MenuWidth;

                        if (GUILayout.Button(menuName, ExStyles.menuBarButton, GUILayout.Width(menuWidth)))
                        {
                            menuItem.Action(GetDropdownRect(i));
                        }
                    }
                    else if (kvp.Value is TreeViewMenuToggle menuToggle)
                    {
                        string menuName = kvp.Key;
                        int menuWidth = menuToggle.MenuWidth;

                        bool isChecked = GUILayout.Toggle(menuToggle.IsChecked, menuName, EditorStyles.toolbarButton, GUILayout.Width(menuWidth));
                        if (isChecked != menuToggle.IsChecked)
                        {
                            menuToggle.IsChecked = isChecked;
                            menuToggle.Action(isChecked);
                        }
                    }
                }

                GUILayout.FlexibleSpace();

                if (HasSearchField && !IsSearchFieldAtStart)
                {
                    DrawSearchField(GetSearchFieldRect(position));
                }
            }
            GUILayout.EndHorizontal();
        }



        private void DrawSearchField(Rect rect)
        {
            _searchField ??= new();
            string searchString = _searchField.OnToolbarGUI(rect, _searchText);

            if (searchString != _searchText)
            {
                _searchText = searchString;
                _onSearchTextChanged?.Invoke(_searchText);
            }
        }
    }
}