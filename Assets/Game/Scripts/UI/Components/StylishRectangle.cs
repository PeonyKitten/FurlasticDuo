using UnityEngine;
using UnityEngine.UIElements;

namespace FD.UI.Components
{
    public class StylishRectangle : VisualElement
    {
        public StylishRectangle()
        {
            generateVisualContent += GenerateVisualContent;
        }

        private void GenerateVisualContent(MeshGenerationContext context)
        {
            var width  = contentRect.width;
            var height = contentRect.height;
            
            var painter = context.painter2D;
            painter.lineWidth = 10.0f;
            painter.lineCap = LineCap.Butt;
            painter.fillColor = Color.green;

            // Draw the track
            painter.strokeColor = Color.blue;
            painter.BeginPath();
            painter.LineTo(new Vector2(0.5f * width, 0));
            painter.LineTo(new Vector2(width, height));
            painter.LineTo(new Vector2(0, height));
            painter.ClosePath();
            painter.Fill();
            painter.Stroke();
        }
    }
}
