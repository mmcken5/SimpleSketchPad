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
        private Point mouseSelect; 

        private Line currentLine;

        private bool justStarting;
        private bool isSelected;


        public Polygon()
        {
            lines = new List<Line>();

            currentLine = new Line(startPoint, colour, thickness, id);
        }

        public Polygon(Point _startPoint, Color _colour, int _thickness, int _id)
        {
            id = _id; 

            colour = _colour;
            origColour = colour;
            thickness = _thickness;
            startPoint = _startPoint;
            initialStartPoint = _startPoint;
            justStarting = true;
            isSelected = false; 

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
            foreach (Line l in lines)
            {
                if (l.IsGraphicAtMousePoint(p))
                {
                    return true; 
                }
            }

            return false; 
        }

        // Redraw the graphic during and after being selected
        public override void SelectGraphic(Color c)
        {
            isSelected = true;

            // Change the colour
            colour = c;

            foreach (Line l in lines)
            {
                l.SelectGraphic(c);
            }
        }

        // Set the point where the mouse has clicked the object
        public override void SetMouseClickDragPoint(Point p)
        {
            mouseSelect = p;

            foreach (Line l in lines)
            {
                l.SetMouseClickDragPoint(p);
            }
        }

        // Update the point (as the mouse moves) while the graphic is being dragged
        public override void UpdateMouseClickDragPoint(Point p)
        {
            foreach (Line l in lines)
            {
                l.UpdateMouseClickDragPoint(p);
            }
        }

        // Set the graphic's colour back to the original colour
        public override void DeselectGraphic()
        {
            colour = origColour;
            isSelected = false;

            foreach (Line l in lines)
            {
                l.DeselectGraphic();
            }
        }

        // Return true if the graphic is currently selected
        public override bool IsGraphicSelected()
        {
            return isSelected;
        }

        // Return a copy of the graphic
        public override GraphicObject Copy(int _id)
        {
            Polygon p = new Polygon(startPoint, origColour, thickness, _id);

            List<Line> temp = new List<Line>();
            
            foreach (Line l in lines)
            {
                Line line = (Line)l.Copy(_id);
                temp.Add(line);
            }

            p.SetLinesObj(temp);

            return p; 
        }

        // Set the lines that draws the polygon
        public void SetLinesObj(List<Line> l)
        {
            lines = l; 
        }

        // Set the paste offset (how much to shift the object by)
        public override void PasteOffset(Point p)
        {
            foreach (Line l in lines)
            {
                l.PasteOffset(p);
            }
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
            return "Polygon " + id;
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
            string s_startPoint = JsonFormatPoint("startPoint", startPoint.X, startPoint.Y);
            string s_endPoint = JsonFormatPoint("endPoint", endPoint.X, endPoint.Y);
            string s_mouseSelect = JsonFormatPoint("mouseSelect", mouseSelect.X, mouseSelect.Y);
            string s_initialStartPoint = JsonFormatPoint("initialStartPoint", initialStartPoint.X, initialStartPoint.Y);
            string s_isSelected = JsonFormat("isSelected", isSelected.ToString());
            string s_justStarting = JsonFormat("justStarting", justStarting.ToString());

            // Encode all the lines
            string s_lines = "";
            foreach (Line l in lines)
            {
                s_lines += l.Encode();
                s_lines += "#";
            }
            s_lines = s_lines.TrimEnd('#');
            s_lines = JsonFormat("lines", s_lines, false);

            string json = jsonStart + s_objectType + s_id + s_colour + s_origColour + s_thickness + s_startPoint + s_endPoint + s_mouseSelect + s_initialStartPoint + s_isSelected + s_justStarting + s_lines + jsonEnd;
            return json;
        }

        public override void Decode(string s)
        {
            try
            {
                // Decode and set the graphics properties
                string[] jsonArr = s.TrimStart('{').TrimEnd('}').Split(',');

                id = JsonGetIntValue(jsonArr[1]);
                colour = JsonGetColorValue(jsonArr[2]);
                origColour = JsonGetColorValue(jsonArr[3]);
                thickness = JsonGetIntValue(jsonArr[4]);
                startPoint = JsonGetPointValue(jsonArr[5] + "," + jsonArr[6]);
                endPoint = JsonGetPointValue(jsonArr[7] + "," + jsonArr[8]);
                mouseSelect = JsonGetPointValue(jsonArr[9] + "," + jsonArr[10]);
                initialStartPoint = JsonGetPointValue(jsonArr[11] + "," + jsonArr[12]);
                isSelected = JsonGetBooleanValue(jsonArr[13]);
                justStarting = JsonGetBooleanValue(jsonArr[14]);

                string s_lines = s.Substring(s.IndexOf("lines") + 8);
                string[] s_lines_arr = s_lines.Split('#'); 

                // Get all the lines
                for (int i = 0; i < s_lines_arr.Length; i++)
                {
                    Line l = DecodeLine(s_lines_arr[i]);
                    lines.Add(l);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured while decoding the graphic.\r\n" + exc.Message);
            }
        }

        public override string ObjType()
        {
            return "polygon";
        }

        // Decode the JSON object and then set the graphic's properties
        private Line DecodeLine(string s)
        {
            int l_id = 0;
            int l_thickness = 0;
            Color l_origColour = new Color();
            Point l_startPoint = new Point();
            Point l_endPoint = new Point();

            try
            {
                // Decode and set the graphics properties
                string[] jsonArr = s.TrimStart('{').TrimEnd('}').Split(',');

                l_id = JsonGetIntValue(jsonArr[1]);
                l_origColour = JsonGetColorValue(jsonArr[3]);
                l_thickness = JsonGetIntValue(jsonArr[4]);
                l_startPoint = JsonGetPointValue(jsonArr[5] + "," + jsonArr[6]);
                l_endPoint = JsonGetPointValue(jsonArr[7] + "," + jsonArr[8]);
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured while decoding the graphic.\r\n" + exc.Message);
            }

            Line l = new Line(l_startPoint, l_origColour, l_thickness, l_id);
            l.Update(l_endPoint);

            return l;
        }
    }
}
