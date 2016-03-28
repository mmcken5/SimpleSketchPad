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

    }
}
