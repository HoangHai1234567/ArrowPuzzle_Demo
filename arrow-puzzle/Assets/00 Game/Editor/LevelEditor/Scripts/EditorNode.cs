using System.Collections;
using System.Collections.Generic;
using DVLib.Graphics;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class EditorNode
    {
        public Vector2Int Index { get; private set; }

        private Rect _rect;
        public EditorNode(Vector2Int index)
        {
            Index = index;
            Vector2 size = new Vector2();
            var pos = EditorConfig.ConvertIndexToPosition(index.x, index.y);
            _rect = new Rect(pos, size);
        }
        public void Draw(MeshGenerationContext mc)
        {
            GraphicX.Rectangle.Draw(mc,_rect,1,Color.black);
        }
    }
}
