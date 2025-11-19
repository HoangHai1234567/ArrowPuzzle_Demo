using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DVLib.Graphics
{
    public partial class GraphicX
    {
        public class TextureElement
        {
            public static void Draw(MeshGenerationContext ctx,Texture texture, Rect rect)
            {
                Draw(ctx,texture,rect,Color.white);
            }

            public static void Draw(MeshGenerationContext ctx, Texture texture, Rect rect, Color color)
            {
                if (texture == null) return;

                var mesh = ctx.Allocate(4, 6, texture);
                mesh.SetAllVertices(new[]
                {
                    new Vertex() { position = new Vector3(rect.xMin, rect.yMin, 0), uv = new Vector2(0, 1), tint = color },
                    new Vertex() { position = new Vector3(rect.xMax, rect.yMin, 0), uv = new Vector2(1, 1), tint = color },
                    new Vertex() { position = new Vector3(rect.xMax, rect.yMax, 0), uv = new Vector2(1, 0), tint = color },
                    new Vertex() { position = new Vector3(rect.xMin, rect.yMax, 0), uv = new Vector2(0, 0), tint = color },
                });

                mesh.SetAllIndices(new ushort[] { 0, 1, 2, 2, 3, 0 });
            }
        }
    }

}

