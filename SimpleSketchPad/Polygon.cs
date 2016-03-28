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
    class Polygon : GraphicObject
    {
        private int id; 

        private Color colour;
        private Color origColour;
        private int thickness;

        private List<Line> lines;

        private Point initialStartPoint;
        private Point startPoint;
        private Point endPoint;

        private Line currentLine;

        bool justStarting;

        public Polygon()
        {

        }

        public Polygon(Point _startPoint, Color _colour, int _thickness, int _id)
        {
            id = _id; 

            colour = _colour;
            thickness = _thickness;
            startPoint = _startPoint;
            initialStartPoint = _startPoint;
            justStarting = true;

            lines = new List<Line>();

            currentLine = new Line(startPoint, colour, thickness, id);
        }

        // Update the end point of the line
        public override void Update(Point _currentPoint)
        {
            currentLine.Update(_currentPoint);
        }

        public bool AddLine(Point _endPoint)
        {
            endPoint = _endPoint;

            if (!justStarting && IsNearStartPoint(endPoint))
            {
                currentLine.Update(initialStartPoint);
                lines.Add(currentLine);

                return false; 
            }
            justStarting = false; 
            
            currentLine.Update(endPoint);
            lines.Add(currentLine);

            startPoint = endPoint;

            currentLine = new Line(startPoint, colour, thickness, id);

            return true; 
        }

        // Draw the Polygon
        public override void Draw(Graphics g)
        {
            foreach (Line l in lines)
            {
                l.Draw(g);
            }

            currentLine.Draw(g);
        }

        // Close the polygon
        public void ClosePolygon()
        {

        }

        private bool IsNearStartPoint(Point p)
        {
            bool retVal = false;

            int x = Math.Abs(p.X - initialStartPoint.X);
            int y = Math.Abs(p.Y - initialStartPoint.Y);

            if ( (x < 10) && (y < 10) )
            {
                retVal = true; 
            }

            return retVal; 
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
            return "Polygon " + id;
        }

        public override int GetId()
        {
            return id;
        }
    }
}
