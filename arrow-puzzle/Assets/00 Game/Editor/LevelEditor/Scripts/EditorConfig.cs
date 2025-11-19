using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ArrowsPuzzle.Editor
{
    public enum EditorToolType
    {
        SelectArrow,
        AddArrow,
        RemoveArrow,
        ////AddHeadNode,
        ////RemoveHeadNode,
        ////AddTailNode,
        ////RemoveTailNode,
    }

    public class EditorConfig
    {
        public static int GridSize = 25;

        public static int OffsetX = 5;

        public static int OffsetY = 5;

        public static int CanvasWidth = 2000;

        public static int CanvasHeight = 2500;

        public static int LevelWidth = 10;

        public static int LevelHeight = 10;

        public static int Rows = LevelHeight + OffsetY*2;

        public static int Columns = LevelWidth + OffsetX*2;

        public static float ArrowLineWidth = 5;




        public static EditorToolType CurrentToolType { get; private set; }

        #region events

        public static event Action<EditorToolType, EditorToolType> OnEditorChangeTool; 
        #endregion

        public static void ChangeTool(EditorToolType toolType)
        {
            var lastTool = CurrentToolType;
            CurrentToolType = toolType;
            OnEditorChangeTool?.Invoke(CurrentToolType,lastTool);
        }
        public static Vector2 ConvertIndexToPosition(int col, int row)
        {
            return new Vector2((col + OffsetX) * GridSize,
                (Rows - row - OffsetY) * GridSize);
        }

        public static Vector2Int ConvertPositionToIndex(Vector2 position)
        {
            int col = Mathf.RoundToInt((position.x - GridSize / 2.0f) / GridSize) - OffsetX;
            int row = Rows - Mathf.RoundToInt((position.y - GridSize / 2.0f) / GridSize) - OffsetY - 1;

            return new Vector2Int(col, row);
        }

        public static Vector2 ConvertIndexToNodePosition(int col, int row)
        {
            return new Vector2((col + OffsetX) * GridSize + GridSize / 2.0f,
                (Rows - row - OffsetY) * GridSize - GridSize / 2.0f);
        }

        public static Rect GetBoundingRect()
        {
            Vector2 topUp = ConvertIndexToPosition(LevelWidth, LevelHeight);
            Vector2 bottomDown = ConvertIndexToPosition(0, 0);
            float width = topUp.x - bottomDown.x;
            float height = bottomDown.y - topUp.y;
            Rect rect = new Rect(bottomDown.x,bottomDown.y - height,width,height);
            
            return rect;
        }
    }
}
