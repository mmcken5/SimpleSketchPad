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
    class Circle : Ellipse
    {
        private Color colour;
        private int thickness;

        private Point initialPoint;
        private Point startPoint;
        private Point endPoint;

        private int width;
        private int height;

        public Circle()
        {

        }

        public Circle(Point _startPoint, Color _colour, int _thickness)
        {
            colour = _colour;
            thickness = _thickness;

            initialPoint = _startPoint;
        }

        // Update the end point of the line
        public override void Update(Point _currentPoint)
        {
            // DEBUG ONLY
            //Console.WriteLine("Initial point x = " + initialPoint.X + ", initial point y = " + initialPoint.Y);
            //Console.WriteLine("Current point x = " + _currentPoint.X + ", current point y = " + _currentPoint.Y);

            // The start point will be the lesser value between the initial (mouse down) point and the current mouse point
            startPoint.X = Math.Min(initialPoint.X, _currentPoint.X);
            startPoint.Y = Math.Min(initialPoint.Y, _currentPoint.Y);

            // DEBUG ONLY
            //Console.WriteLine("Start point x = " + startPoint.X + ", start point y = " + startPoint.Y);

            // The end point will be the greater value between the initial (mouse down) point and the current mouse point
            endPoint.X = Math.Max(initialPoint.X, _currentPoint.X);
            endPoint.Y = Math.Max(initialPoint.Y, _currentPoint.Y);

            // DEBUG ONLY
            //Console.WriteLine("End point x = " + endPoint.X + ", start end y = " + endPoint.Y);

            // Determine the width and height of the triangle by finding the distance between the start and end coordinates
            width = Math.Abs(_currentPoint.X - initialPoint.X);
            height = Math.Abs(_currentPoint.Y - initialPoint.Y);

            // DEBUG ONLY
            //Console.WriteLine("Width = " + width + ", height = " + height);
            //Console.WriteLine("\r\n");

            // Take the largest value to determine the width of the circle
            width = Math.Max(width, height);
        }

        // Draw the Circle
        public override void Draw(Graphics g)
        {
            // Make the circle render smooth
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Create a pen object (safely)
            using (Pen pen = new Pen(colour, thickness))
            {
                // Draw the updated version of this circle
                g.DrawEllipse(pen, startPoint.X, startPoint.Y, width, width);
            } 
        }
    }
}
