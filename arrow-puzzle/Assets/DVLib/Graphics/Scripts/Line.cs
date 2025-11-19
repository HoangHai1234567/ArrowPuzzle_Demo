using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DVLib.Graphics
{
    public partial class  GraphicX
    {
        public class Line
        {
            public static void Draw(MeshGenerationContext ctx,
                Vector2 startPoint, Vector2 endPoint,
                float lineWidth)
            {
                var p = ctx.painter2D;
                p.lineWidth = lineWidth;
                p.BeginPath();
                p.MoveTo(startPoint);
                p.LineTo(endPoint);
                p.Stroke();
            }

            public static void Draw(MeshGenerationContext ctx,
                Vector2 startPoint, Vector2 endPoint,
                float lineWidth, Color lineColor)
            {
                ctx.painter2D.strokeColor = lineColor;
                Draw(ctx, startPoint, endPoint, lineWidth);
            }


            public static void Draw(MeshGenerationContext ctx,
                Vector2 startPoint, Vector2 endPoint,
                float lineWidth, Color lineColor, LineCap lineCap)
            {
                ctx.painter2D.lineCap = lineCap;
                Draw(ctx, startPoint, endPoint, lineWidth, lineColor);
            }

            public static void DrawGradient(MeshGenerationContext ctx,
                Vector2 startPoint, Vector2 endPoint,
                float lineWidth, Gradient lineGradient)
            {
                ctx.painter2D.strokeGradient = lineGradient;
                Draw(ctx, startPoint, endPoint, lineWidth);
            }

            public static void DrawGradient(MeshGenerationContext ctx,
                Vector2 startPoint, Vector2 endPoint,
                float lineWidth, Gradient lineGradient, LineCap lineCap)
            {
                ctx.painter2D.lineCap = lineCap;
                DrawGradient(ctx, startPoint, endPoint, lineWidth, lineGradient);
            }
        }
    }
}

