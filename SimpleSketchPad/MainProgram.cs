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
    public partial class MainProgram : Form
    {
        private List<Button> drawButtons;
        private List<Button> selectButtons;

        private enum Mode { draw, select };
        private Mode mode; 

        private enum DrawMode {freehand, line, rectangle, square, ellipse, circle, polygon};
        private DrawMode drawMode;

        private Point startPoint;
        private Point prevPoint;
        private Point currentPoint;
        private bool isMouseDown;

        private Color colour;
        private int thickness;

        private GraphicObject graphic;
        private List<GraphicObject> graphicObjectList;

        private bool polygonInProgress;

        //List<List<Point>> curves = new List<List<Point>>();
        //List<Point> currentLine = new List<Point>();

        public MainProgram()
        {
            InitializeComponent();

            // Initialize variables
            drawButtons = new List<Button>();
            selectButtons = new List<Button>();
            graphicObjectList = new List<GraphicObject>();

            // Add the draw buttons
            drawButtons.Add(button1);
            drawButtons.Add(button2);
            drawButtons.Add(button3);
            drawButtons.Add(button4);
            drawButtons.Add(button5);
            drawButtons.Add(button6);
            drawButtons.Add(button7);

            // Add the select buttons
            selectButtons.Add(button8);
            selectButtons.Add(button9);
            selectButtons.Add(button10);
            selectButtons.Add(button11);
            selectButtons.Add(button12);

            // Set the mode to the default of "draw"
            mode = Mode.draw;
            comboBox1.SelectedIndex = 0; 

            // Set the default colour to be black
            colorDialog1.Color = Color.Black;
            btn_colour.BackColor = colorDialog1.Color;
            colour = colorDialog1.Color;

            // Set the default thickness to be 2
            comboBox2.SelectedIndex = 1;
            thickness = Int32.Parse(comboBox2.Text);

            // Set freehand as the default drawing mode
            button1.Select();
            drawMode = DrawMode.freehand;

            polygonInProgress = false;
        }

        // Show the colour picker options
        private void btn_colour_Click(object sender, EventArgs e)
        {
            // Update the color
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btn_colour.BackColor = colorDialog1.Color;
                colour = colorDialog1.Color; 
            }
        }

        // Mode has been changed
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Mode has been set to "draw"
            if (comboBox1.SelectedIndex == 0)
            {
                DisplayDrawButtons();
                mode = Mode.draw;

                // Set freehand as the default drawing mode
                button1.Select();
                drawMode = DrawMode.freehand;
            }
            // Mode has been set to "select"
            else if (comboBox1.SelectedIndex == 1)
            {
                DisplaySelectButtons();
                mode = Mode.select;
            }
        }

        // Hide the buttons associated with the draw mode
        private void HideDrawButtons()
        {
            foreach (Button b in drawButtons)
            {
                b.Enabled = false; 
            }
        }

        // Display the buttons associated the draw mode
        private void DisplayDrawButtons()
        {
            HideSelectButtons();

            foreach (Button b in drawButtons)
            {
                b.Enabled = true;
            }
        }
        
        // Hide the buttons associated with the select mode
        private void HideSelectButtons()
        {
            foreach (Button b in selectButtons)
            {
                b.Enabled = false;
            }
        }

        // Display the buttons assocaited with the select mode
        private void DisplaySelectButtons()
        {
            HideDrawButtons();

            foreach (Button b in selectButtons)
            {
                b.Enabled = true;
            }
        }

        // Exit the application
        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Draw mode has been changed to "freehand"
        private void button1_Click(object sender, EventArgs e)
        {
            drawMode = DrawMode.freehand;
        }

        // Draw mode has been changed to "line"
        private void button2_Click(object sender, EventArgs e)
        {
            drawMode = DrawMode.line;
        }

        // Draw mode has been changed to "rectangle"
        private void button3_Click(object sender, EventArgs e)
        {
            drawMode = DrawMode.rectangle;
        }

        // Draw mode has been changed to "square"
        private void button4_Click(object sender, EventArgs e)
        {
            drawMode = DrawMode.square;
        }

        // Draw mode has been changed to "ellipse"
        private void button5_Click(object sender, EventArgs e)
        {
            drawMode = DrawMode.ellipse;
        }

        // Draw mode has been changed to "circle"
        private void button6_Click(object sender, EventArgs e)
        {
            drawMode = DrawMode.circle;
        }

        // Draw mode has been changed to "polygon"
        private void button7_Click(object sender, EventArgs e)
        {
            drawMode = DrawMode.polygon;
        }

        // Change the cursor when the mouse enters the draw area
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            // Change the cursor to a cross
            if (mode == Mode.draw)
            {
                pictureBox1.Cursor = Cursors.Cross;
            }
            // Change the cursor to a hand
            else if (mode == Mode.select)
            {
                pictureBox1.Cursor = Cursors.Hand;
            }
        }

        // Begin drawing if in draw mode
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Save the last point (assign it to the current mouse position)
            startPoint = e.Location;

            isMouseDown = true;

            // Create a new GraphicsObject, depending on which draw mode is selected
            switch (drawMode)
            {
                case DrawMode.freehand:
                    graphic = new FreehandLine(colour, thickness);
                    break;
                
                case DrawMode.line:
                    graphic = new Line(startPoint, colour, thickness);
                    break;

                case DrawMode.rectangle:
                    graphic = new Rectangle(startPoint, colour, thickness);
                    break;

                case DrawMode.ellipse:
                    graphic = new Ellipse(startPoint, colour, thickness);
                    break;

                case DrawMode.square:
                    graphic = new Square(startPoint, colour, thickness);
                    break;

                case DrawMode.circle:
                    graphic = new Circle(startPoint, colour, thickness);
                    break;
            }

            if (drawMode == DrawMode.polygon)
            {
                // Check if a polygon is being drawn
                // If one is not, create one
                if (!polygonInProgress)
                {
                    polygonInProgress = true;

                    graphic = new Polygon(e.Location, colour, thickness);
                }
            }

            // Add the graphic to the list of existing (already drawn) graphics
            graphicObjectList.Add(graphic);
        }

        // Draw if in draw mode
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Check to see if in draw mode
            if (mode == Mode.draw)
            {
                // Check to see if the mouse button is down
                if (isMouseDown == true)
                {
                    if (startPoint != null && graphic != null)
                    {
                        // If no available bitmap exists on the picturebox to draw on
                        if (pictureBox1.Image == null)
                        {
                            // Create a new bitmap
                            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

                            // Assign the picturebox.Image property to the bitmap created
                            pictureBox1.Image = bmp;
                        }

                        // Update the current graphic with the new mouse location (coordinates)
                        graphic.Update(e.Location);
                    }
                }

                else
                {
                    if (drawMode == DrawMode.polygon && polygonInProgress)
                    {
                        // Update the current graphic with the new mouse location (coordinates)
                        graphic.Update(e.Location);
                    }
                }
            }
            // Refresh the picturebox (invoke the Paint event)
            pictureBox1.Invalidate();
        }

        // Finish drawing the graphic
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            startPoint = Point.Empty;
            prevPoint = Point.Empty;
            currentPoint = Point.Empty;

            if (drawMode == DrawMode.polygon)
            {
                // Check if a polygon is being drawn
                // If it is, update the newest line
                if (polygonInProgress)
                {
                    try
                    {
                        graphic.Update(e.Location);
                        Polygon p = (Polygon)graphic;
                        polygonInProgress = p.AddLine(e.Location);
                    }
                    catch (Exception exc)
                    {

                    }
                }
            }

            // Cause the picture box to re-paint (i.e. invoke Paint method)
            pictureBox1.Invalidate();
        }

        private void clearButton_Click(object sender, EventArgs e)//our clearing button
        {

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = null;

                Invalidate();
            }
        }

        // Perform the drawing
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (graphicObjectList.Count > 0)
            {
                foreach (GraphicObject g in graphicObjectList)
                {
                    g.Draw(e.Graphics);
                }
            }
        }

        // Update the thickness value
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            thickness = Int32.Parse(comboBox2.Text);
        }

        // Mouse has left the box, stop drawing
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            // Stop drawing
            isMouseDown = false;
            startPoint = Point.Empty;
            prevPoint = Point.Empty;
            currentPoint = Point.Empty;
        }
    }
}
