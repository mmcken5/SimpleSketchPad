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
    class FreehandLine : Line
    {
        private int id; 

        private Color colour;
        private Color origColour;
        private int thickness; 

        private List<Point> lines;

        public FreehandLine()
        {

        }

        public FreehandLine(Color _colour, int _thickness, int _id)
        {
            id = _id; 

            colour = _colour;
            thickness = _thickness;

            lines = new List<Point>();
        }

        public override void Update(Point _currentPoint)
        {
            lines.Add(_currentPoint);
        }

        public override void Draw(Graphics g)
        {
            // Make the line render smooth
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw a line
            using (Pen pen = new Pen(colour, thickness))
            {
                if (lines.Count > 1)
                {
                    g.DrawCurve(pen, lines.ToArray());
                }
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
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "FreehandLine " + id;
        }

        public override int GetId()
        {
            return id;
        }
    }
}
