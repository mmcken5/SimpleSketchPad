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
    class Rectangle : GraphicObject
    {
        private Color colour;
        private int thickness;

        private Point initialPoint;
        private Point startPoint;
        private Point endPoint;

        private int width;
        private int height;

        public Rectangle()
        {

        }

        public Rectangle(Point _startPoint, Color _colour, int _thickness)
        {
            colour = _colour;
            thickness = _thickness;

            initialPoint = _startPoint;
        }

        // Update the end point of the line
        public override void Update(Point _currentPoint)
        {
            // The start point will be the lesser value between the initial (mouse down) point and the current mouse point
            startPoint.X = Math.Min(initialPoint.X, _currentPoint.X);
            startPoint.Y = Math.Min(initialPoint.Y, _currentPoint.Y);

            // The end point will be the greater value between the initial (mouse down) point and the current mouse point
            endPoint.X = Math.Max(initialPoint.X, _currentPoint.X);
            endPoint.Y = Math.Max(initialPoint.Y, _currentPoint.Y);

            // Determine the width and height of the triangle by finding the distance between the start and end coordinates
            width = Math.Abs(endPoint.X - startPoint.X);
            height = Math.Abs(endPoint.Y - startPoint.Y);
        }

        // Draw the Rectangle
        public override void Draw(Graphics g)
        {
            // Make the Rectangle render smooth
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Create a pen object (safely)
            using (Pen pen = new Pen(colour, thickness))
            {
                // Draw the updated version of this Rectangle
                g.DrawRectangle(pen, startPoint.X, startPoint.Y, width, height);
            } 
        }
    }
}
