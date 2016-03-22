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
        private Color colour; 
        private int thickness; 

        private List<Point> lines;

        public FreehandLine()
        {

        }

        public FreehandLine(Color _colour, int _thickness)
        {
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
    }
}
