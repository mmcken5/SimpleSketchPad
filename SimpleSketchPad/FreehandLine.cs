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

        private bool isSelected;

        private Point mouseSelect;

        public FreehandLine()
        {
            lines = new List<Point>();
        }

        public FreehandLine(Color _colour, int _thickness, int _id)
        {
            id = _id; 

            colour = _colour;
            origColour = _colour;
            thickness = _thickness;

            lines = new List<Point>();

            isSelected = false; 
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
            foreach (Point lp in lines)
            {
                int x = Math.Abs(lp.X - p.X);
                int y = Math.Abs(lp.Y - p.Y);

                if ((x <= 10) && (y <= 10))
                    return true;
            }

            return false; 
        }

        // Redraw the graphic during and after being selected
        public override void SelectGraphic(Color c)
        {
            isSelected = true;
            colour = c; 
        }

        // Set the point where the mouse has clicked the object
        public override void SetMouseClickDragPoint(Point p)
        {
            mouseSelect = p; 
        }

        // Update the point (as the mouse moves) while the graphic is being dragged
        public override void UpdateMouseClickDragPoint(Point p)
        {
            // Convert the list of points to an array of points for easier modification
            Point[] lineArr = lines.ToArray();

            // Determine how much the mouse has moved relative to the previous mouse point
            int xChange = p.X - mouseSelect.X;
            int yChange = p.Y - mouseSelect.Y;

            // Update the previous mouse point to be equal to the current mouse point
            mouseSelect.X = p.X;
            mouseSelect.Y = p.Y; 

            // Apply the same movement to each point in the line
            for (int i = 0; i < lineArr.Length; i++)
            {
                lineArr[i].X += xChange;
                lineArr[i].Y += yChange;
            }

            // Convert the array of points back to a list of points
            lines = lineArr.ToList<Point>();
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
            // Create a new freehand line with the same attributes
            FreehandLine fhl = new FreehandLine(origColour, thickness, _id);

            // Add all the points to the new freehandline
            foreach (Point p in lines)
            {
                fhl.Update(p);
            }

            return fhl; 
        }

        // Set the paste offset (how much to shift the object by)
        public override void PasteOffset(Point p)
        {
            // Convert the list of points to an array of points for easier modification
            Point[] lineArr = lines.ToArray();

            // Apply the paste offset shift to each point
            for (int i = 0; i < lineArr.Length; i++)
            {
                lineArr[i].X += p.X;
                lineArr[i].Y += p.Y;
            }

            // Convert the array of points back to a list of points
            lines = lineArr.ToList<Point>();
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
            return "FreehandLine " + id;
        }

        public override int GetId()
        {
            return id;
        }

        public override string Encode()
        {
            string jsonStart = "{";
            string jsonEnd = "}";

            string s_objectType = JsonFormat("objectType", ObjType());
            string s_id = JsonFormat("id", id.ToString());
            string s_colour = JsonFormat("colour", colour.ToArgb().ToString());
            string s_origColour = JsonFormat("origColour", origColour.ToArgb().ToString());
            string s_thickness = JsonFormat("thickness", thickness.ToString());
            string s_isSelected = JsonFormat("isSelected", isSelected.ToString());
            string s_mouseSelect = JsonFormatPoint("mouseSelect", mouseSelect.X, mouseSelect.Y);
            
            string s_lines = "";
            foreach (Point p in lines)
            {
                s_lines += "{" + JsonFormat("x", p.X.ToString()) + JsonFormat("y", p.Y.ToString(), false) + "},";
            }
            s_lines = s_lines.TrimEnd(',');
            s_lines = JsonFormat("points", s_lines, false);

            string json = jsonStart + s_objectType + s_id + s_colour + s_origColour + s_thickness + s_isSelected + s_mouseSelect + s_lines + jsonEnd;
            return json;
        }

        public override void Decode(string s)
        {
            try
            {
                // Decode and set the graphics properties
                string[] jsonArr = s.Split('{');

                string[] arr = jsonArr[1].Split(',');

                id = JsonGetIntValue(arr[1]);
                colour = JsonGetColorValue(arr[2]);
                origColour = JsonGetColorValue(arr[3]);
                thickness = JsonGetIntValue(arr[4]);
                isSelected = JsonGetBooleanValue(arr[5]);
                mouseSelect = JsonGetPointValue("{" + jsonArr[2].Split('}')[0] + "}");
                
                // Get all the points and put it into the list "lines"
                for (int i = 3; i < jsonArr.Length; i++)
                {
                    string temp = "{";
                    temp += jsonArr[i].TrimEnd(',');
                    Point newP = new Point();
                    newP = JsonGetPointValue(temp);
                    lines.Add(newP);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured while decoding the graphic.\r\n" + exc.Message);
            }
        }

        public override string ObjType()
        {
            return "freehandline";
        }
    }
}
