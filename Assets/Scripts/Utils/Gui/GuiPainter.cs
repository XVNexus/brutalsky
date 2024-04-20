using UnityEngine;
using UnityEngine.UIElements;

namespace Utils.Gui
{
    public class GuiPainter
    {
        public Rect ViewRect { get; private set; }
        public Rect RenderRect { get; private set; }
        public Vector2 ImageSize { get; private set; }
        public float RenderScale { get; private set; }
        public bool AutoTransform { get; set; }

        private readonly Painter2D _painter;

        public GuiPainter(Rect viewRect, Vector2 imageSize, bool autoTransform = true)
        {
            ViewRect = viewRect;
            var renderAspect = imageSize.x / imageSize.y;
            Vector2 renderSize = viewRect.width / viewRect.height > renderAspect
                ? new Vector2(viewRect.width, viewRect.width / renderAspect)
                : new Vector2(viewRect.height * renderAspect, viewRect.height);
            RenderRect = new Rect(viewRect.x - (renderSize.x - viewRect.width) * .5f,
                viewRect.y - (renderSize.y - viewRect.height) * .5f, renderSize.x, renderSize.y);
            ImageSize = imageSize;
            RenderScale = imageSize.x / RenderRect.width;
            AutoTransform = autoTransform;

            _painter = new Painter2D();
        }

        // Main
        public VectorImage Print()
        {
            var result = ScriptableObject.CreateInstance<VectorImage>();
            _painter.SaveToVectorImage(result);
            _painter.Dispose();
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

        public void DrawRect(Rect rect)
        {
            Start(rect.xMin, rect.yMin);
            Line(rect.xMax, rect.yMin);
            Line(rect.xMax, rect.yMax);
            Line(rect.xMin, rect.yMax);
            Close();
        }

        public void DrawRoundedRect(Rect rect, float radius)
        {
            Start(rect.xMin, rect.yMin + radius);
            Arc(rect.xMin, rect.yMin, rect.xMin + radius, rect.yMin, radius);
            Line(rect.xMax - radius, rect.yMin);
            Arc(rect.xMax, rect.yMin, rect.xMax, rect.yMin + radius, radius);
            Line(rect.xMax, rect.yMax - radius);
            Arc(rect.xMax, rect.yMax, rect.xMax - radius, rect.yMax, radius);
            Line(rect.xMin + radius, rect.yMax);
            Arc(rect.xMin, rect.yMax, rect.xMin, rect.yMax - radius, radius);
            Close();
        }

        public void DrawCircle(Vector2 position, float radius)
        {
            Start(position.x, position.y - radius);
            Arc(position.x + radius, position.y - radius, position.x + radius, position.y, radius);
            Arc(position.x + radius, position.y + radius, position.x, position.y + radius, radius);
            Arc(position.x - radius, position.y + radius, position.x - radius, position.y, radius);
            Arc(position.x - radius, position.y - radius, position.x, position.y - radius, radius);
            Close();
        }

        // Pen
        public void Start()
        {
            _painter.BeginPath();
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
            _painter.MoveTo(MakePoint(point));
        }

        public void Line(float x, float y)
        {
            Line(new Vector2(x, y));
        }

        public void Line(Vector2 point)
        {
            _painter.LineTo(MakePoint(point));
        }

        public void Arc(float a, float b, float x, float y, float r)
        {
            Arc(new Vector2(a, b), new Vector2(x, y), r);
        }

        public void Arc(Vector2 control, Vector2 point, float radius)
        {
            _painter.ArcTo(MakePoint(control), MakePoint(point), MakeValue(radius));
        }

        public void Quadratic(float a, float b, float x, float y)
        {
            Quadratic(new Vector2(a, b), new Vector2(x, y));
        }

        public void Quadratic(Vector2 control, Vector2 point)
        {
            _painter.QuadraticCurveTo(MakePoint(control), MakePoint(point));
        }

        public void Bezier(float a, float b, float c, float d, float x, float y)
        {
            Bezier(new Vector2(a, b), new Vector2(c, d), new Vector2(x, y));
        }

        public void Bezier(Vector2 control1, Vector2 control2, Vector2 point)
        {
            _painter.BezierCurveTo(MakePoint(control1), MakePoint(control2), MakePoint(point));
        }

        public void Close()
        {
            _painter.ClosePath();
        }

        public void Fill()
        {
            _painter.Fill();
        }

        public void Stroke()
        {
            _painter.Stroke();
        }

        // Config
        public void SetFill(Color color)
        {
            _painter.fillColor = color;
        }

        public void SetStroke(Color color, float width)
        {
            _painter.strokeColor = color;
            _painter.lineWidth = width;
        }

        public void SetLine(LineCap cap, LineJoin join)
        {
            _painter.lineCap = cap;
            _painter.lineJoin = join;
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
