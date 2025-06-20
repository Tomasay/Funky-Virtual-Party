using System;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public class TreeViewPageBar
    {
        public enum PageSizeOption
        {
            [InspectorName("15 items per page")] _15 = 15,
            [InspectorName("30 items per page")] _30 = 30,
            [InspectorName("50 items per page")] _50 = 50,
            [InspectorName("100 items per page")] _100 = 100,
        }

        public int Page { get; private set; } = 0;
        public int PageSize { get; private set; } = 50;
        public int TotalItems { get; private set; } = 0;

        private bool HasNextPage(int totalPages)
        {
            if (hasUnknownPageSize) return _hasNextPage;
            return Page < totalPages - 1;
        }

        private string GetTotalPagesString(int totalPages)
        {
            if (hasUnknownPageSize) return "*";
            return totalPages.ToString();
        }

        private readonly Action<int> onPageChanged;
        private readonly bool hasUnknownPageSize;
        private bool _hasNextPage = false;

        public TreeViewPageBar(Action<int> onPageChanged, int initialPage = 0, bool hasUnknownPageSize = false)
        {
            Page = initialPage;
            this.onPageChanged = onPageChanged;
            this.hasUnknownPageSize = hasUnknownPageSize;
        }

        public void OnGUI()
        {
            int totalPages = 0;
            if (!hasUnknownPageSize) totalPages = Mathf.Max(1, Mathf.CeilToInt((float)TotalItems / PageSize));
            const float kPopupWidth = 160f;

            EditorGUILayout.BeginHorizontal(TreeViewStyles.PageBarStyle);
            {
                DrawLeftContent(kPopupWidth);
                GUILayout.FlexibleSpace();

                GUI.enabled = Page > 0;
                if (GUILayout.Button("<", GUILayout.Width(30)))
                {
                    Page--;
                    onPageChanged?.Invoke(Page);
                }
                GUI.enabled = true;

                GUILayout.Label($"Page {Page + 1} / {GetTotalPagesString(totalPages)}", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100), GUILayout.Height(18f));

                GUI.enabled = HasNextPage(totalPages);
                if (GUILayout.Button(">", GUILayout.Width(30)))
                {
                    Page++;
                    onPageChanged?.Invoke(Page);
                }
                GUI.enabled = true;

                GUILayout.FlexibleSpace();
                DrawRightContent(kPopupWidth);
            }
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawLeftContent(float popupWidth) { }
        protected virtual void DrawRightContent(float popupWidth) { }

        public void DrawPageSizePopup(params GUILayoutOption[] options)
        {
            PageSizeOption newPageSize = (PageSizeOption)EditorGUILayout.EnumPopup((PageSizeOption)PageSize, options);
            if (newPageSize != (PageSizeOption)PageSize)
            {
                PageSize = (int)newPageSize;
                Page = 0; // reset to first page
                onPageChanged?.Invoke(Page);
            }
        }

        public void SetTotalItems(int totalItems, bool resetPage = false)
        {
            TotalItems = totalItems;
            if (resetPage) Page = 0;
            else Page = Mathf.Clamp(Page, 0, Mathf.Max(0, Mathf.CeilToInt((float)TotalItems / PageSize) - 1));
        }

        public void SetHasNextPage(bool hasNextPage)
        {
            _hasNextPage = hasNextPage;
        }

        public void Reset()
        {
            Page = 0;
            PageSize = 50;
            TotalItems = 0;
        }

        public int GetStartIndex() => Page * PageSize;
        public int GetEndIndex() => Mathf.Min((Page + 1) * PageSize, TotalItems);
    }
}
