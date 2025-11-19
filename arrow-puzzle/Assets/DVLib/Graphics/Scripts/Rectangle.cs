using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DVLib.Graphics
{
    public partial class GraphicX
    {
        public class Rectangle
        {
            private static void Draw(MeshGenerationContext ctx, Rect rect)
            {
                var p = ctx.painter2D;
                p.BeginPath();
                p.MoveTo(rect.position);
                p.LineTo(new Vector2(rect.xMax, rect.y));
                p.LineTo(new Vector2(rect.xMax, rect.yMax));
                p.LineTo(new Vector2(rect.x, rect.yMax));
                p.ClosePath();
            }
            public static void Draw(MeshGenerationContext ctx, Rect rect, float strokeWidth,Color strokeColor)
            {
                var p = ctx.painter2D;
                p.strokeColor = strokeColor;
                p.lineWidth = strokeWidth;
                Draw(ctx,rect);
                p.Stroke();
            }

            public static void DrawGradient(MeshGenerationContext ctx, Rect rect,
                float strokeWith, Gradient strokeGradient)
            {
                var p = ctx.painter2D;
                p.strokeGradient = strokeGradient;
                p.lineWidth = strokeWith;
                Draw(ctx, rect);
                p.Stroke();
            }
            public static void DrawFill(MeshGenerationContext ctx, Rect rect,Color fillColor,
                bool haveStroke, float strokeWith, Color strokeColor, bool strokeOnTop = true)
            {
                var p = ctx.painter2D;

                Draw(ctx,rect);
                p.fillColor = fillColor;
                if (haveStroke)
                {
                    p.strokeColor = strokeColor;
                    p.lineWidth = strokeWith;
                    if (strokeOnTop)
                    {
                        p.Fill();
                        p.Stroke();
                    }
                    else
                    {
                        p.Stroke();
                        p.Fill();
                    }
                }
                else
                {
                    p.Fill();
                }
            }

            public static void DrawFillGradientStroke(MeshGenerationContext ctx, Rect rect, Color fillColor,
                float strokeWith, Gradient strokeGradient, bool strokeOnTop = true)
            {
                var p = ctx.painter2D;

                Draw(ctx, rect);
                p.fillColor = fillColor;
                p.strokeGradient = strokeGradient;
                p.lineWidth = strokeWith;
                if (strokeOnTop)
                {
                    p.Fill();
                    p.Stroke();
                }
                else
                {
                    p.Stroke();
                    p.Fill();
                }
            }
        }
    }

}

