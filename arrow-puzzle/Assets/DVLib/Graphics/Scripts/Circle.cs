using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DVLib.Graphics
{
    public partial class GraphicX
    {
        public class Circle
        {
            private static void Draw(MeshGenerationContext ctx, 
                Vector2 center, float radius)
            {
                var p = ctx.painter2D;
                p.BeginPath();
                p.Arc(center, radius, 0, 360);
                p.ClosePath();
            }

            public static void Draw(MeshGenerationContext ctx, 
                Vector2 center, float radius, float strokeWidth,
                Color strokeColor)
            {
                var p = ctx.painter2D;
                p.lineWidth = strokeWidth;
                p.strokeColor = strokeColor;
                Draw(ctx,center,radius);
                p.Stroke();
            }

            public static void DrawFill(MeshGenerationContext ctx,
                Vector2 center, float radius,Color fillColor,bool haveStroke, float strokeWidth,
                Color strokeColor, bool strokeOnTop)
            {
                var p = ctx.painter2D;
                p.fillColor = fillColor;
                if (haveStroke)
                {
                    p.lineWidth = strokeWidth;
                    p.strokeColor = strokeColor;
                }

                Draw(ctx, center, radius);

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

            public static void DrawFillGradientStroke(MeshGenerationContext ctx,
                Vector2 center, float radius, Color fillColor,float strokeWidth,
                Gradient strokeGradient, bool strokeOnTop = true)
            {
                var p = ctx.painter2D;
                p.fillColor = fillColor;
                p.lineWidth = strokeWidth;
                p.strokeGradient = strokeGradient;

                Draw(ctx, center, radius);

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

