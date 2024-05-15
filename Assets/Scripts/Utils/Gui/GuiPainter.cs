using UnityEngine;
using UnityEngine.UIElements;
using Utils.Ext;

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
            RenderRect = ViewRect.ForceAspect(imageSize.Aspect());
            ImageSize = imageSize;
            RenderScale = imageSize.x / RenderRect.width;
            AutoTransform = autoTransform;

            _painter = new Painter2D();
        }

        public VectorImage Print()
        {
            var result = ScriptableObject.CreateInstance<VectorImage>();
            _painter.SaveToVectorImage(result);
            _painter.Dispose();
            return result;
        }

        // Drawing functions
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

        public void DrawRect(Rect rect, float radius)
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

        // Pen functions
        public void Start()
        {
            _painter.BeginPath();
        }

        public void Start(float x, float y)
        {
            Start(MakePoint(x, y));
        }

        public void Start(Vector2 point)
        {
            Start();
            Move(point);
        }

        public void Move(float x, float y)
        {
            Move(MakePoint(x, y));
        }

        public void Move(Vector2 point)
        {
            _painter.MoveTo(MakePoint(point));
        }

        public void Line(float x, float y)
        {
            Line(MakePoint(x, y));
        }

        public void Line(Vector2 point)
        {
            _painter.LineTo(MakePoint(point));
        }

        public void Arc(float a, float b, float x, float y, float r)
        {
            Arc(MakePoint(a, b), MakePoint(x, y), r);
        }

        public void Arc(Vector2 control, Vector2 point, float radius)
        {
            _painter.ArcTo(MakePoint(control), MakePoint(point), MakeValue(radius));
        }

        public void Quadratic(float a, float b, float x, float y)
        {
            Quadratic(MakePoint(a, b), MakePoint(x, y));
        }

        public void Quadratic(Vector2 control, Vector2 point)
        {
            _painter.QuadraticCurveTo(MakePoint(control), MakePoint(point));
        }

        public void Bezier(float a, float b, float c, float d, float x, float y)
        {
            Bezier(MakePoint(a, b), MakePoint(c, d), MakePoint(x, y));
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

        public void Fill(Color color)
        {
            SetFill(color);
            Fill();
        }

        public void Stroke()
        {
            _painter.Stroke();
        }

        public void Stroke(Color color, float width)
        {
            SetStroke(color, width);
            Stroke();
        }

        public void Stroke(Color color, float width, LineCap cap, LineJoin join)
        {
            SetStroke(color, width, cap, join);
            Stroke();
        }

        // Config functions
        public void SetFill(Color color)
        {
            _painter.fillColor = color;
        }

        public void SetStroke(Color color, float width)
        {
            _painter.strokeColor = color;
            _painter.lineWidth = width;
        }

        public void SetStroke(Color color, float width, LineCap cap, LineJoin join)
        {
            _painter.strokeColor = color;
            _painter.lineWidth = width;
            _painter.lineCap = cap;
            _painter.lineJoin = join;
        }

        // Math functions
        private Vector2 MakePoint(float x, float y)
        {
            return MakePoint(new Vector2(x, y));
        }

        private Vector2 MakePoint(Vector2 point)
        {
            return AutoTransform ? TransformPoint(point) : point;
        }

        private float MakeValue(float value)
        {
            return AutoTransform ? TransformValue(value) : value;
        }

        private Vector2 TransformPoint(float x, float y)
        {
            return TransformPoint(new Vector2(x, y));
        }

        private Vector2 TransformPoint(Vector2 point)
        {
            var result = MathfExt.Clamp(point, ViewRect);
            return new Vector2((result.x - RenderRect.xMin) / (RenderRect.xMax - RenderRect.xMin) * ImageSize.x,
                (result.y - RenderRect.yMax) / (RenderRect.yMin - RenderRect.yMax) * ImageSize.y);
        }

        private float TransformValue(float value)
        {
            return value * RenderScale;
        }
    }
}
