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
        private int id;

        private Color colour;
        private Color origColour;
        private int thickness;

        private Point initialPoint;
        private Point startPoint;
        private Point endPoint;

        private int width;
        private int height;

        public Rectangle()
        {

        }

        public Rectangle(Point _startPoint, Color _colour, int _thickness, int _id)
        {
            id = _id;

            colour = _colour;
            origColour = colour;
            thickness = _thickness;

            initialPoint = _startPoint;
        }

        public Rectangle(Point _startPoint, Point _endPoint, Color _colour, int _thickness, int _id)
        {
            // Set the Rectangle's properties
            id = _id;
            
            startPoint = _startPoint;
            endPoint = _endPoint;
            
            colour = _colour;
            thickness = _thickness;

            width = endPoint.X - startPoint.X;
            height = endPoint.Y - startPoint.Y;
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

        // Return true if the object contains the point passed as a parameter
        public override bool IsGraphicAtMousePoint(Point p)
        {
            throw new NotImplementedException();
        }

        // Redraw the graphic during and after being selected
        public override void SelectGraphic(Color c)
        {
            throw new NotImplementedException();
        }

        // Set the point where the mouse has clicked the object
        public override void SetMouseClickDragPoint(Point p)
        {
            throw new NotImplementedException();
        }

        // Update the point (as the mouse moves) while the graphic is being dragged
        public override void UpdateMouseClickDragPoint(Point p)
        {
            throw new NotImplementedException();
        }

        // Set the graphic's colour back to the original colour
        public override void DeselectGraphic()
        {
            colour = origColour;
        }

        // Return true if the graphic is currently selected
        public override bool IsGraphicSelected()
        {
            throw new NotImplementedException();
        }

        // Return a copy of the graphic
        public override GraphicObject Copy(int _id)
        {
            throw new NotImplementedException();
        }

        // Set the paste offset (how much to shift the object by)
        public override void PasteOffset(Point p)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "Rectangle " + id;
        }

        public override int GetId()
        {
            return id;
        }

        public override string Encode()
        {
            throw new NotImplementedException();
        }

        public override void Decode(string s)
        {
            throw new NotImplementedException();
        }

        public override string ObjType()
        {
            return "rectangle";
        }
    }
}
