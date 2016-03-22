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
        public abstract void Update(Point _currentPoint);

        public abstract void Draw(Graphics g);
    }
}
