using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public static class RectExtensions
    {
        public static Rect GetSingleLightHeightRow(this Rect rect, int row)
        {
            if (row == 0)
            {
                Debug.LogError("Row index must be greater than 0");
                return rect;
            }
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.singleLineHeight * row;
            if (row > 1) rect.height += EditorGUIUtility.standardVerticalSpacing * (row - 1);
            return rect;
        }

        public static Rect[] SplitVertically(this Rect rect, int count, float space = 0)
        {
            Rect[] rects = new Rect[count];
            float height = rect.height / count;
            for (int i = 0; i < count; i++)
            {
                rects[i] = new Rect(rect.x, rect.y + height * i + space * i, rect.width, height - space);
            }
            return rects;
        }

        public static Rect[] SplitHorizontally(this Rect rect, int count, float space = 0)
        {
            Rect[] rects = new Rect[count];
            float width = rect.width / count;
            for (int i = 0; i < count; i++)
            {
                rects[i] = new Rect(rect.x + width * i + space * i, rect.y, width - space, rect.height);
            }
            return rects;
        }

        public static Rect[] SplitHorizontallyFixed(this Rect rect, params float[] fixedWidths)
        {
            int count = fixedWidths.Length + 1; // +1 for the flexible last rect
            Rect[] rects = new Rect[count];

            float totalFixedWidth = fixedWidths.Sum();
            float flexibleWidth = Mathf.Max(0, rect.width - totalFixedWidth); // prevent negative width

            float cursorX = rect.x;

            // Create rects for fixed widths
            for (int i = 0; i < fixedWidths.Length; i++)
            {
                rects[i] = new Rect(cursorX, rect.y, fixedWidths[i], rect.height);
                cursorX += fixedWidths[i];
            }

            // Last rect takes the remaining width
            rects[count - 1] = new Rect(cursorX, rect.y, flexibleWidth, rect.height);

            return rects;
        }

        public static Rect[] SplitHorizontallyFixedReversed(this Rect rect, params float[] fixedWidths)
        {
            int count = fixedWidths.Length + 1; // +1 for the flexible area
            Rect[] rects = new Rect[count];

            float totalFixedWidth = fixedWidths.Sum();
            float flexibleWidth = Mathf.Max(0, rect.width - totalFixedWidth); // prevent negative width
            float cursorX = rect.x + rect.width; // Start from the right edge

            // Create fixed rects from right to left
            for (int i = fixedWidths.Length - 1; i >= 0; i--)
            {
                float width = fixedWidths[i];
                cursorX -= width;
                rects[i + 1] = new Rect(cursorX, rect.y, width, rect.height);
            }

            // Flexible rect fills the remaining space on the left
            rects[0] = new Rect(rect.x, rect.y, flexibleWidth, rect.height);

            return rects;
        }

        public static Rect[] SplitHorizontally(this Rect rect, float space, params float[] weights)
        {
            Rect[] rects = new Rect[weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                float width = rect.width * weights[i];
                float x = i == 0 ? rect.x : rects[i - 1].x + rects[i - 1].width + space;
                rects[i] = new Rect(x, rect.y, width, rect.height);
            }

            return rects;
        }

        public static Rect GetLabelRect(this Rect rowRect, float labelWidth = -1f)
        {
            if (Math.Abs(labelWidth - (-1f)) < ExGUI.kTolerance)
                labelWidth = EditorGUIUtility.labelWidth;


            return new Rect(
                rowRect.x,
                rowRect.y,
                labelWidth,
                rowRect.height
            );
        }

        public static Rect GetValueRect(this Rect rowRect, float labelWidth = -1)
        {
            if (Math.Abs(labelWidth - (-1)) < ExGUI.kTolerance)
            {
                labelWidth = EditorGUIUtility.labelWidth;
            }

            rowRect.x += labelWidth;
            rowRect.width -= labelWidth;
            return rowRect;
        }

        public static Rect Scale(this Rect rect, float scale)
        {
            Vector2 pivot = rect.center;
            float width = rect.width * scale;
            float height = rect.height * scale;

            return new Rect(
                pivot.x - width / 2f,
                pivot.y - height / 2f,
                width,
                height
            );
        }


        public static Rect SetSize(this Rect rect, float width, float height)
        {
            Vector2 pivot = rect.center;
            return new Rect(
                pivot.x - width / 2f,
                pivot.y - height / 2f,
                width,
                height
            );
        }

        public static Rect MoveY(this Rect rect, float y)
        {
            rect.y += y;
            return rect;
        }

        public static Rect MoveX(this Rect rect, float x)
        {
            rect.x += x;
            return rect;
        }
    }
}