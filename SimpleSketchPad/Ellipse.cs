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
    class Ellipse : GraphicObject
    {
        private int id; 

        private Color colour;
        private Color origColour;
        private int thickness;

        private Point initialPoint;
        private Point startPoint;
        private Point endPoint;
        private Point mouseSelect;

        private int width;
        private int height;

        private bool isSelected;

        public Ellipse()
        {

        }

        public Ellipse(Point _startPoint, Color _colour, int _thickness, int _id)
        {
            id = _id; 

            colour = _colour;
            origColour = colour;
            thickness = _thickness;

            initialPoint = _startPoint;
            isSelected = false; 
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

        // Draw the Ellipse
        public override void Draw(Graphics g)
        {
            // Make the Ellipse render smooth
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Create a pen object (safely)
            using (Pen pen = new Pen(colour, thickness))
            {
                // Draw the updated version of this Ellipse
                g.DrawEllipse(pen, startPoint.X, startPoint.Y, width, height);
            } 
        }

        // Return true if the object contains the point passed as a parameter
        public override bool IsGraphicAtMousePoint(Point p)
        {
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

            return false;
        }

        // Redraw the graphic during and after being selected
        public override void SelectGraphic(Color c)
        {
            isSelected = true;

            // Change the colour
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

        public override GraphicObject Copy(int _id)
        {
            // Create a new line with the same attributes
            Ellipse e = new Ellipse(startPoint, origColour, thickness, _id);

            // Update the end point attribute
            e.Update(endPoint);

            return e; 
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
            return "Ellipse " + id;
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
            string s_isSelected = JsonFormat("isSelected", isSelected.ToString(), false);

            string json = jsonStart + s_objectType + s_id + s_colour + s_origColour + s_thickness + s_startPoint + s_endPoint + s_mouseSelect + s_isSelected + jsonEnd;
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
                initialPoint = JsonGetPointValue(jsonArr[5] + "," + jsonArr[6]);
                endPoint = JsonGetPointValue(jsonArr[7] + "," + jsonArr[8]);
                mouseSelect = JsonGetPointValue(jsonArr[9] + "," + jsonArr[10]);
                isSelected = JsonGetBooleanValue(jsonArr[11]);

                // Call to make sure the width and height are calculated
                Update(endPoint);
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has occured while decoding the graphic.\r\n" + exc.Message);
            }
        }

        public override string ObjType()
        {
            return "ellipse";
        }
    }
}
