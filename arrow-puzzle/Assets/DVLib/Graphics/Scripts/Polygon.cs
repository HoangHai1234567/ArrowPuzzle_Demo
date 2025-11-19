using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DVLib.Graphics
{
    public partial class GraphicX
    {
        public class Polygon
        {
            public static void Draw(MeshGenerationContext ctx,
                Vector2[] polygon, float lineWidth,bool closed = false)
            {
                if (polygon.Length > 1)
                {
                    var p = ctx.painter2D;
                    p.lineWidth = lineWidth;
                    p.BeginPath();
                    p.MoveTo(polygon[0]);

                    for (int i = 1; i < polygon.Length; i++)
                    {
                        p.LineTo(polygon[i]);
                    }

                    if (closed)
                    {
                        p.ClosePath();
                    }
                    p.Stroke();
                }

            }

            public static void Draw(MeshGenerationContext ctx,
                Vector2[] polygon, float lineWidth, Color lineColor, bool closed = false)
            {
                ctx.painter2D.strokeColor = lineColor;
                Draw(ctx, polygon, lineWidth,closed);
            }

            public static void Draw(MeshGenerationContext ctx,
                Vector2[] polygon, float lineWidth, Color lineColor, LineJoin lineJoin, bool closed = false)
            {
                ctx.painter2D.lineJoin = lineJoin;
                Draw(ctx, polygon, lineWidth, lineColor,closed);
            }

            public static void Draw(MeshGenerationContext ctx,
                Vector2[] polygon, float lineWidth, Color lineColor,
                LineJoin lineJoin, LineCap lineCap,bool closed = false)
            {
                ctx.painter2D.lineCap = lineCap;
                Draw(ctx, polygon, lineWidth, lineColor, lineJoin,closed);
            }

            public static void DrawGradient(MeshGenerationContext ctx,
                Vector2[] polygon, float lineWidth, Gradient lineGradient, bool closed = false)
            {
                ctx.painter2D.strokeGradient = lineGradient;
                Draw(ctx, polygon, lineWidth,closed);
            }

            public static void DrawGradient(MeshGenerationContext ctx,
                Vector2[] polygon, float lineWidth, Gradient lineGradient, LineJoin lineJoin, bool closed = false)
            {
                ctx.painter2D.lineJoin = lineJoin;
                DrawGradient(ctx, polygon, lineWidth, lineGradient,closed);
            }

            public static void DrawGradient(MeshGenerationContext ctx,
                Vector2[] polygon, float lineWidth, Gradient lineGradient, LineJoin lineJoin, LineCap lineCap, bool closed = false)
            {
                ctx.painter2D.lineCap = lineCap;
                DrawGradient(ctx, polygon, lineWidth, lineGradient, lineJoin,closed);
            }

            public static void DrawFill(MeshGenerationContext ctx,
                Vector2[] polygon, Color fillColor)
            {
                DrawFill(ctx,polygon,fillColor,false,1,Color.white);
            }

            public static void DrawFill(MeshGenerationContext ctx,
                Vector2[] polygon, Color fillColor, bool haveStroke,
                float strokeWidth, Color strokeColor, bool strokeOnTop = true,
                LineCap strokeCap = LineCap.Butt, LineJoin strokeJoin = LineJoin.Miter)
            {
                if (polygon.Length > 1)
                {
                    var p = ctx.painter2D;
                    p.fillColor = fillColor;
                    if (haveStroke)
                    {
                        p.strokeColor = strokeColor;
                        p.lineWidth = strokeWidth;
                        p.lineJoin = strokeJoin;
                        p.lineCap = strokeCap;
                    }
                    p.BeginPath();
                    p.MoveTo(polygon[0]);

                    for (int i = 1; i < polygon.Length; i++)
                    {
                        p.LineTo(polygon[i]);
                    }
                    p.ClosePath();

                    if (haveStroke)
                    {
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
            }

            public static void DrawFillGradient(MeshGenerationContext ctx,
                Vector2[] polygon, Color fillColor, bool haveStroke,
                float strokeWidth, Gradient strokeGradient, bool strokeOnTop = true,
                LineCap strokeCap = LineCap.Butt, LineJoin strokeJoin = LineJoin.Miter)
            {
                if (polygon.Length > 1)
                {
                    var p = ctx.painter2D;
                    
                    if (haveStroke)
                    {
                        p.strokeGradient = strokeGradient;
                        p.lineWidth = strokeWidth;
                        p.lineJoin = strokeJoin;
                        p.lineCap = strokeCap;
                    }

                    p.fillColor = fillColor;
                    p.BeginPath();
                    p.MoveTo(polygon[0]);

                    for (int i = 1; i < polygon.Length; i++)
                    {
                        p.LineTo(polygon[i]);
                    }
                    p.ClosePath();

                    if (haveStroke)
                    {
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
            }
        }
    }

}
