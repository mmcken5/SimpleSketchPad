using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleSketchPad
{
    class Line : GraphicObject
    {
        private Color colour;
        private int thickness;

        private Point startPoint;
        private Point endPoint;

        public Line()
        {

        }

        public Line(Point _startPoint, Color _colour, int _thickness)
        {
            colour = _colour;
            thickness = _thickness;

            startPoint = _startPoint;
        }

        // Update the end point of the line
        public override void Update(Point _currentPoint)
        {
            endPoint = _currentPoint;
        }

        // Draw the line
        public override void Draw(Graphics g)
        {
            // Make the line render smooth
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Create a pen object (safely)
            using (Pen pen = new Pen(colour, thickness))
            {
                Point p = new Point(0,0);

                // Draw the updated version of this line
                if (!endPoint.Equals(p))
                {
                    g.DrawLine(pen, startPoint, endPoint);
                }
            } 
        }
    }
}
