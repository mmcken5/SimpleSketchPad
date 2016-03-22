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
        private Color colour;
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

        public Polygon(Point _startPoint, Color _colour, int _thickness)
        {
            colour = _colour;
            thickness = _thickness;
            startPoint = _startPoint;
            initialStartPoint = _startPoint;
            justStarting = true;

            lines = new List<Line>();

            currentLine = new Line(startPoint, colour, thickness);
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

            currentLine = new Line(startPoint, colour, thickness);

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
    }
}
