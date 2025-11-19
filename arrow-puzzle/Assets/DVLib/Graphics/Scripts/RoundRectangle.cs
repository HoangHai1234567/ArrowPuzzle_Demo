using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DVLib.Graphics
{
    public partial class GraphicX
    {
        public class RoundRectangle
        {
            private static void Draw(MeshGenerationContext ctx, Rect rect, float radius)
            {
                var p = ctx.painter2D;
                //p.fillColor = Color.magenta;
                float x = rect.xMin;
                float y = rect.yMin;
                float width = rect.width;
                float height = rect.height;
                // Các điểm góc
                Vector2 topLeft = new Vector2(x, y);
                Vector2 topRight = new Vector2(x + width, y);
                Vector2 bottomRight = new Vector2(x + width, y + height);
                Vector2 bottomLeft = new Vector2(x, y + height);

                p.BeginPath();

                // Bắt đầu từ góc trên trái
                p.MoveTo(topLeft + new Vector2(radius, 0));

                // Top edge
                p.LineTo(topRight - new Vector2(radius, 0));
                p.ArcTo(topRight, topRight + new Vector2(0, radius), radius);

                // Right edge
                p.LineTo(bottomRight - new Vector2(0, radius));
                p.ArcTo(bottomRight, bottomRight - new Vector2(radius, 0), radius);

                // Bottom edge
                p.LineTo(bottomLeft + new Vector2(radius, 0));
                p.ArcTo(bottomLeft, bottomLeft - new Vector2(0, radius), radius);

                // Left edge
                p.LineTo(topLeft + new Vector2(0, radius));
                p.ArcTo(topLeft, topLeft + new Vector2(radius, 0), radius);

                p.ClosePath();

            }

            public static void Draw(MeshGenerationContext ctx, Rect rect,float radius, float strokeWith, Color strokeColor)
            {
                var p = ctx.painter2D;
                p.strokeColor = strokeColor;
                p.lineWidth = strokeWith;
                Draw(ctx, rect,radius);
                p.Stroke();
            }

            public static void DrawFill(MeshGenerationContext ctx, Rect rect, float radius, Color fillColor,
                bool haveStroke,float strokeWidth, Color strokeColor, bool strokeOnTop = true)
            {
                var p = ctx.painter2D;
                p.fillColor = fillColor;

                Draw(ctx,rect,radius);
                if (haveStroke)
                {
                    p.lineWidth = strokeWidth;
                    p.strokeColor = strokeColor;
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

            public static void DrawFillGradientStroke(MeshGenerationContext ctx, Rect rect, float radius, Color fillColor,
                float strokeWidth, Gradient strokeGradient, bool strokeOnTop = true)
            {
                var p = ctx.painter2D;
                p.fillColor = fillColor;
                p.strokeGradient = strokeGradient;
                p.lineWidth = strokeWidth;
                Draw(ctx, rect, radius);

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

