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
        private int id; 

        private Color colour;
        private Color origColour; 
        private int thickness;

        private Point startPoint;
        private Point endPoint;
        private Point mouseSelect;

        private bool isSelected;

        public Line()
        {

        }

        public Line(Point _startPoint, Color _colour, int _thickness, int _id)
        {
            id = _id; 
            colour = _colour;
            origColour = colour; 
            thickness = _thickness;

            startPoint = _startPoint;

            isSelected = false; 
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


        // Return true if the object contains the point passed as a parameter
        public override bool IsGraphicAtMousePoint(Point p)
        {
            // Create a box around the object

            /* Check if the point is contained in the object */
            // Check to see if the point is inbetween the X coord of the start point and the end point
            if ((p.X >= Math.Min(startPoint.X, endPoint.X)) && (p.X <= Math.Max(startPoint.X, endPoint.X)))
            {
                // Check to see if the point is inbetween the Y coord of the start point and the end point
                if ((p.Y >= Math.Min(startPoint.Y, endPoint.Y)) && (p.Y <= Math.Max(startPoint.Y, endPoint.Y)))
                {
                    return true; 
                }
            }

            // If so draw a box around it (or change (darken) the colour by redrawing the object)

            return false;
        }

        // Change the colour of the graphic and return a box to be drawn around it
        public override void SelectGraphic(Color c)
        {
            isSelected = true; 

            // Change the colour
            colour = c;

            /*
            Point rectStartPoint = new Point();
            rectStartPoint.X = Math.Min(startPoint.X, endPoint.X);
            rectStartPoint.Y = Math.Min(startPoint.Y, endPoint.Y);

            Point rectEndPoint = new Point();
            rectEndPoint.X = Math.Max(startPoint.X, endPoint.X);
            rectEndPoint.Y = Math.Max(startPoint.Y, endPoint.Y);

            // Return a rectangle to be drawn around the graphic
            Rectangle r = new Rectangle(rectStartPoint, rectEndPoint, colour, thickness);

            return r; 
            */ 
        }

        // Set the point where the mouse has clicked the object
        public override void SetMouseClickDragPoint(Point p)
        {
            mouseSelect = p;
        }

        // Update the point (as the mouse moves) while the graphic is being dragged
        public override void UpdateMouseClickDragPoint(Point p)
        {
            // Determine how much the mouse has moved relative to the previous mouse point
            int xChange = p.X - mouseSelect.X;
            int yChange = p.Y - mouseSelect.Y;

            // Update the previous mouse point to be equal to the current mouse point
            mouseSelect.X = p.X;
            mouseSelect.Y = p.Y; 

            // Apply the same movement to the start point of the line
            startPoint.X += xChange;
            startPoint.Y += yChange; 
            
            // Apply the mouse movement to the end point of the line
            endPoint.X += xChange;
            endPoint.Y += yChange; 
        }

        // Set the graphic's colour back to the original colour
        public override void DeselectGraphic()
        {
            colour = origColour;
            isSelected = false; 
        }

        // Return true if the graphic is currently selected
        public override bool IsGraphicSelected()
        {
            return isSelected;
        }

        // Return a copy of the graphic
        public override GraphicObject Copy(int _id)
        {
            // Create a new line with the same attributes
            Line l = new Line(startPoint, origColour, thickness, _id);

            // Update the end point attribute
            l.Update(endPoint);

            return l; 
        }

        // Set the paste offset (how much to shift the object by)
        public override void PasteOffset(Point p)
        {
            // Shift the start point by the offset
            startPoint.X += p.X;
            startPoint.Y += p.Y;

            // Shift the end point by the offset
            endPoint.X += p.X;
            endPoint.Y += p.Y; 
        }

        public override bool Equals(object obj)
        {
            GraphicObject g = (GraphicObject)obj;
            return (id == g.GetId());
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "Line " + id;
        }

        public override int GetId()
        {
            return id; 
        }
    }
}
