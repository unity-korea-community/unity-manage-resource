using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UNKO.ManageUGUI
{
    // 출처: https://www.youtube.com/watch?v=CGsEJToeXmA
    /// <summary>
    /// 해상도에 맞게 대응하는 grid
    /// </summary>
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns
        }

        public enum FitTypeXY
        {
            No,
            Yes,
            Yes_WhenLesserMin,
            Yes_WhenGreaterMax,
        }

        public enum FitTypeAdvense
        {
            None,
            X_IsY,
            Y_IsX,
        }

        public FitType fitType;
        public FitTypeAdvense fitTypeAdvense;

        public int rows;
        public int columns;
        public Vector2 cellSize;
        public Vector2 spacing;

        public FitTypeXY fitX = FitTypeXY.Yes;
        public FitTypeXY fitY = FitTypeXY.Yes;

        bool _fitX;
        bool _fitY;

        public override void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputHorizontal();

            if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
            {
                _fitX = true;
                _fitY = true;
                float sqrRt = Mathf.Sqrt(transform.childCount);
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(sqrRt);
            }

            if (fitType == FitType.Width || fitType == FitType.FixedColumns || fitType == FitType.Uniform)
            {
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
            }
            if (fitType == FitType.Height || fitType == FitType.FixedRows || fitType == FitType.Uniform)
            {
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
            }

            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
            float cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * (rows - 1)) - (padding.top / (float)rows) - (padding.bottom / (float)rows);

            _fitX = fitX == FitTypeXY.Yes;
            if (fitX == FitTypeXY.Yes_WhenLesserMin)
                _fitX = cellSize.x < cellWidth;
            if (fitX == FitTypeXY.Yes_WhenGreaterMax)
                _fitX = cellSize.x > cellWidth;

            _fitY = fitY == FitTypeXY.Yes;
            if (fitY == FitTypeXY.Yes_WhenLesserMin)
                _fitY = cellSize.y < cellHeight;
            if (fitY == FitTypeXY.Yes_WhenGreaterMax)
                _fitY = cellSize.y > cellHeight;

            cellSize.x = _fitX ? cellWidth : cellSize.x;
            cellSize.y = _fitY ? cellHeight : cellSize.y;
            if (fitTypeAdvense == FitTypeAdvense.X_IsY)
                cellSize.x = cellSize.y;
            else if (fitTypeAdvense == FitTypeAdvense.Y_IsX)
                cellSize.y = cellSize.x;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                int rowCount = i / columns;
                int columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        public override void SetLayoutHorizontal()
        {

        }

        public override void SetLayoutVertical()
        {

        }
    }
}