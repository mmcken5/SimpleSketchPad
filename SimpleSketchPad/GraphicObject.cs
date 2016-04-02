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
    abstract class GraphicObject
    {
        // Draw methods
        public abstract void Update(Point _currentPoint);

        public abstract void Draw(Graphics g);

        // Select methods
        public abstract void SelectGraphic(Color c);
        public abstract void DeselectGraphic();

        public abstract bool IsGraphicAtMousePoint(Point p);
        public abstract bool IsGraphicSelected();

        public abstract void SetMouseClickDragPoint(Point p);
        public abstract void UpdateMouseClickDragPoint(Point p);

        public abstract GraphicObject Copy(int _id);

        public abstract void PasteOffset(Point p);

        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
        public abstract override string ToString();

        public abstract int GetId();

        public abstract string Encode();
        public abstract void Decode(string s);

        public abstract string ObjType();

        protected string JsonFormat(string key, string value, bool comma = true)
        {
            if (comma)
                return "\"" + key + "\":\"" + value + "\",";

            return "\"" + key + "\":\"" + value + "\"";
        }

        protected string JsonFormatPoint(string key, int x, int y, bool comma = true)
        {
            if (comma)
                return "\"" + key + "\":{" + JsonFormat("x", x.ToString()) + JsonFormat("y", y.ToString(), false) + "},";

            return "\"" + key + "\":{" + JsonFormat("x", x.ToString()) + JsonFormat("y", y.ToString(), false) + "}";
        }

        protected string JsonGetValue(string keyVal)
        {
            // Remove the quotes and split the key apart from the value
            return keyVal.Replace("\"","").Split(':')[1];
        }

        protected int JsonGetIntValue(string keyVal)
        {
            int retVal = 0; 
            
            try
            {
                retVal = Int32.Parse(JsonGetValue(keyVal));
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error occured while attempting to convert a the value of a JSON key-value pair into an int.\r\n" + exc.Message);
            }
            
            return retVal;  
        }

        protected Color JsonGetColorValue(string keyVal)
        {
            Color c = new Color();

            try
            {
                int arbg = JsonGetIntValue(keyVal);
                c = Color.FromArgb(arbg);
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error occured while attempting to convert a the value of a JSON key-value pair into a Color object.\r\n" + exc.Message);
            }

            return c; 
        }

        protected Point JsonGetPointValue(string keyVal)
        {
            Point p = new Point();

            try
            {
                string val = keyVal.Split('{')[1].Replace("\"","");

                int xI = val.IndexOf('x') + 2;
                int yI = val.IndexOf('y') + 2;

                string s_x = val.Substring(xI, (val.IndexOf(',') - xI));
                string s_y = val.Substring(yI, (val.IndexOf('}') - yI));

                int x = Int32.Parse(s_x);
                int y = Int32.Parse(s_y);

                p.X = x;
                p.Y = y;
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error occured while attempting to convert a the value of a JSON key-value pair into a Point object.\r\n" + exc.Message);
            }

            return p; 
        }

        protected bool JsonGetBooleanValue(string keyVal)
        {
            try
            {
                string val = JsonGetValue(keyVal);

                if (val.ToLower() == "true")
                    return true;
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error occured while attempting to convert a the value of a JSON key-value pair into a bool.\r\n" + exc.Message);
            }

            return false; 
        }


    }
}
