using UnityEngine;
using UnityEngine.UIElements;

namespace Utils.Gui
{
    public class GuiPainter
    {
        public Painter2D Painter { get; private set; }
        public Rect ViewRect { get; private set; }
        public Rect RenderRect { get; private set; }
        public Vector2 ImageSize { get; private set; }
        public float RenderScale { get; private set; }
        public bool AutoTransform { get; set; }

        public GuiPainter(Rect viewRect, Vector2 imageSize, bool autoTransform = true)
        {
            Painter = new Painter2D();
            ViewRect = viewRect;
            var renderAspect = imageSize.x / imageSize.y;
            float renderWidth;
            float renderHeight;
            var viewAspect = viewRect.width / viewRect.height;
            if (viewAspect > renderAspect)
            {
                renderWidth = viewRect.width;
                renderHeight = viewRect.width / renderAspect;
            }
            else
            {
                renderWidth = viewRect.height * renderAspect;
                renderHeight = viewRect.height;
            }
            RenderRect = new Rect(viewRect.x - (renderWidth - viewRect.width) * .5f,
                viewRect.y - (renderHeight - viewRect.height) * .5f, renderWidth, renderHeight);
            ImageSize = imageSize;
            RenderScale = imageSize.x / RenderRect.width;
            AutoTransform = autoTransform;
        }

        // Main
        public VectorImage Print()
        {
            var result = ScriptableObject.CreateInstance<VectorImage>();
            Painter.SaveToVectorImage(result);
            Painter.Dispose();
            return result;
        }

        // Drawing
        public void DrawPolygon(Vector2[] points)
        {
            Start(points[0]);
            for (var i = 1; i < points.Length; i++)
            {
                Line(points[i]);
            }
            Close();
        }

        public void DrawRect(float x, float y, float w, float h)
        {
            Start(x, y);
            Line(x + w, y);
            Line(x + w, y + h);
            Line(x, y + h);
            Close();
        }

        public void DrawRoundedRect(float x, float y, float w, float h, float r)
        {
            Start(x, y + r);
            Arc(x, y, x + r, y, r);
            Line(x + w - r, y);
            Arc(x + w, y, x + w, y + r, r);
            Line(x + w, y + h - r);
            Arc(x + w, y + h, x + w - r, y + h, r);
            Line(x + r, y + h);
            Arc(x, y + h, x, y + h - r, r);
            Close();
        }

        // Pen
        public void Start()
        {
            Painter.BeginPath();
        }

        public void Start(float x, float y)
        {
            Start(new Vector2(x, y));
        }

        public void Start(Vector2 point)
        {
            Start();
            Move(point);
        }

        public void Move(float x, float y)
        {
            Move(new Vector2(x, y));
        }

        public void Move(Vector2 point)
        {
            Painter.MoveTo(MakePoint(point));
        }

        public void Line(float x, float y)
        {
            Line(new Vector2(x, y));
        }

        public void Line(Vector2 point)
        {
            Painter.LineTo(MakePoint(point));
        }

        public void Arc(float a, float b, float x, float y, float r)
        {
            Arc(new Vector2(a, b), new Vector2(x, y), r);
        }

        public void Arc(Vector2 control, Vector2 point, float radius)
        {
            Painter.ArcTo(MakePoint(control), MakePoint(point), MakeValue(radius));
        }

        public void Quadratic(float a, float b, float x, float y)
        {
            Quadratic(new Vector2(a, b), new Vector2(x, y));
        }

        public void Quadratic(Vector2 control, Vector2 point)
        {
            Painter.QuadraticCurveTo(MakePoint(control), MakePoint(point));
        }

        public void Bezier(float a, float b, float c, float d, float x, float y)
        {
            Bezier(new Vector2(a, b), new Vector2(c, d), new Vector2(x, y));
        }

        public void Bezier(Vector2 control1, Vector2 control2, Vector2 point)
        {
            Painter.BezierCurveTo(MakePoint(control1), MakePoint(control2), MakePoint(point));
        }

        public void Close()
        {
            Painter.ClosePath();
        }

        public void Fill()
        {
            Painter.Fill();
        }

        public void Stroke()
        {
            Painter.Stroke();
        }

        // Config
        public void SetFill(Color color)
        {
            Painter.fillColor = color;
        }

        public void SetStroke(Color color, float width)
        {
            Painter.strokeColor = color;
            Painter.lineWidth = width;
        }

        public void SetLine(LineCap cap, LineJoin join)
        {
            Painter.lineCap = cap;
            Painter.lineJoin = join;
        }

        // Math
        public Vector2 MakePoint(float x, float y)
        {
            return MakePoint(new Vector2(x, y));
        }

        public Vector2 MakePoint(Vector2 point)
        {
            return AutoTransform ? TransformPoint(point) : point;
        }

        public float MakeValue(float value)
        {
            return AutoTransform ? TransformValue(value) : value;
        }

        public Vector2 TransformPoint(float x, float y)
        {
            return TransformPoint(new Vector2(x, y));
        }

        public Vector2 TransformPoint(Vector2 point)
        {
            // Clamp to viewable area
            var result = new Vector2(Mathf.Clamp(point.x, ViewRect.xMin, ViewRect.xMax),
                Mathf.Clamp(point.y, ViewRect.yMin, ViewRect.yMax));

            // Convert from object coordinates to pixel coordinates
            return new Vector2(Mathf.InverseLerp(RenderRect.xMin, RenderRect.xMax, result.x) * ImageSize.x,
                Mathf.InverseLerp(RenderRect.yMin, RenderRect.yMax, -result.y) * ImageSize.y);
        }

        public float TransformValue(float value)
        {
            return value * RenderScale;
        }
    }
}
