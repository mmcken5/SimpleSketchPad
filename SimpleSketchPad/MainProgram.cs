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
        private List<GraphicObject> selectedGraphics;
        private List<GraphicObject> copiedGraphics;

        private bool polygonInProgress;
        private bool beginGraphicMove;
        private bool isGraphicDragging;

        private List<List<GraphicObject>> groups;

        private Point defaultPasteOffset;

        private int copySize;

        private int id; 

        public MainProgram()
        {
            InitializeComponent();

            // Initialize variables
            drawButtons = new List<Button>();
            selectButtons = new List<Button>();
            graphicObjectList = new List<GraphicObject>();
            selectedGraphics = new List<GraphicObject>();
            copiedGraphics = new List<GraphicObject>();
            groups = new List<List<GraphicObject>>();

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

            // Set the default paste offset
            defaultPasteOffset = new Point(50, 50);

            polygonInProgress = false;
            beginGraphicMove = false;
            isGraphicDragging = false;

            copySize = 0;
            id = 0;
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

            // Check to see if in draw mode
            if (mode == Mode.draw)
            {
                // Create a new GraphicsObject, depending on which draw mode is selected
                switch (drawMode)
                {
                    case DrawMode.freehand:
                        graphic = new FreehandLine(colour, thickness, id++);
                        break;
                
                    case DrawMode.line:
                        graphic = new Line(startPoint, colour, thickness, id++);
                        break;

                    case DrawMode.rectangle:
                        graphic = new Rectangle(startPoint, colour, thickness, id++);
                        break;

                    case DrawMode.ellipse:
                        graphic = new Ellipse(startPoint, colour, thickness, id++);
                        break;

                    case DrawMode.square:
                        graphic = new Square(startPoint, colour, thickness, id++);
                        break;

                    case DrawMode.circle:
                        graphic = new Circle(startPoint, colour, thickness, id++);
                        break;
                }

                if (drawMode == DrawMode.polygon)
                {
                    // Check if a polygon is being drawn
                    // If one is not, create one
                    if (!polygonInProgress)
                    {
                        polygonInProgress = true;

                        // Create a new polygon
                        graphic = new Polygon(e.Location, colour, thickness, id++);
                    }
                }

                // Add the graphic to the list of existing (already drawn) graphics
                graphicObjectList.Add(graphic);
            }

            // Must be in select mode
            else
            {
                // See if a graphic is under the mouse
                foreach (GraphicObject g in graphicObjectList)
                {
                    // Select the graphic (if there is more than one match only select the top layer graphic (i.e. the first match))
                    if (g.IsGraphicAtMousePoint(e.Location))
                    {
                        // Store a reference to the graphic
                        graphic = g;

                        // Set the point on the graphic where the mouse has clicked
                        g.SetMouseClickDragPoint(e.Location);

                        beginGraphicMove = true;
                        
                        break;
                    }
                }
            }
            // Redraw
            pictureBox1.Invalidate();
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

            else
            {
                if (beginGraphicMove)
                {
                    // Update the graphic to reflect dragging
                    graphic.UpdateMouseClickDragPoint(e.Location);

                    isGraphicDragging = true; 
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

            if (mode == Mode.draw)
            {
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
            }

            else
            {
                if (isGraphicDragging)
                {
                    // Finish dragging the graphic
                    beginGraphicMove = false;
                    isGraphicDragging = false; 
                }
                else
                {
                    beginGraphicMove = false;

                    // If graphic is part of a group, get the group
                    List<GraphicObject> selectGroup = GetGroup(graphic); 

                    // Check if graphic is selected
                    if (graphic.IsGraphicSelected())
                    {
                        // Check if graphic is part of a group
                        // If part of group, deselect entire group

                        if (selectGroup.Count > 0)
                        {
                            foreach (GraphicObject g in selectGroup)
                            {
                                // Deselect the graphic
                                g.DeselectGraphic();

                                // Remove from list of selected graphics
                                selectedGraphics.Remove(g);
                            }
                        }
                        else
                        {
                            // Deselect the graphic
                            graphic.DeselectGraphic();

                            // Remove from list of selected graphics
                            selectedGraphics.Remove(graphic);
                        }
                    }
                    else
                    {
                        // Check if graphic is part of a group
                        // If part of group, select entire group

                        // Loop through group and select each graphic
                        if (selectGroup.Count > 0)
                        {
                            foreach (GraphicObject g in selectGroup)
                            {
                                // Select the graphic and show this by changing the colour
                                g.SelectGraphic(Color.Red);

                                // Add the selected graphic to the list of selected graphics
                                selectedGraphics.Add(g);
                            }
                        }
                        else
                        {
                            // Select the graphic and show this by changing the colour
                            graphic.SelectGraphic(Color.Red);

                            // Add the selected graphic to the list of selected graphics
                            selectedGraphics.Add(graphic);
                        }
                    }
                }
            }

            // Cause the picture box to re-paint (i.e. invoke Paint method)
            pictureBox1.Invalidate();
        }

        // If the graphic is part of a group return the group
        private List<GraphicObject> GetGroup(GraphicObject _go)
        {
            List<GraphicObject> list = new List<GraphicObject>();
            GraphicObject g = _go;

            // Loop through list of groups
            foreach (List<GraphicObject> group in groups)
            {
                // Loop through each graphic in the current group
                foreach (GraphicObject gObj in group)
                {
                    // Check if selected graphic is eqaul to graphic in group
                    if (g.Equals(gObj))
                    {
                        list = group;
                        return list; 
                    }
                }
            }

            return list; 
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

        // Delete all the selected graphics
        private void button8_Click(object sender, EventArgs e)
        {
            // Convert the list of selected graphics to an array
            GraphicObject[] gArr = selectedGraphics.ToArray();

            // Remove each one from the original list
            foreach (GraphicObject g in gArr)
            {
                graphicObjectList.Remove(g);
            }

            // Clear the selected graphics list
            selectedGraphics.Clear();

            // Redraw
            pictureBox1.Invalidate();
        }

        // Copy the currently selected graphic(s)
        private void button9_Click(object sender, EventArgs e)
        {
            // Copy all the selected graphics to the copied list
            foreach (GraphicObject g in selectedGraphics)
            {
                // Create a copy of the graphic
                GraphicObject copy = g.Copy(id++);

                // Add the graphic to the copied graphics list
                copiedGraphics.Add(copy);
            }
        }

        // Paste the copied graphics
        private void button10_Click(object sender, EventArgs e)
        {
            // Add the copied graphics to the list of graphics to be drawn and include a paste offset so they are not directly on top of current graphic
            foreach (GraphicObject g in copiedGraphics)
            {
                // Set the point where to paste
                g.PasteOffset(defaultPasteOffset);

                // Add to the list of graphics that are to be drawn
                graphicObjectList.Add(g);
            }

            // Clear the copied graphics
            copiedGraphics.Clear();

            // Deselect the selected graphics
            foreach (GraphicObject g in selectedGraphics)
            {
                g.DeselectGraphic();
            }
            
            // Clear the selected graphics
            selectedGraphics.Clear();

            // Redraw
            pictureBox1.Invalidate();
        }

        // Group the selected graphics together
        private void button11_Click(object sender, EventArgs e)
        {
            // IN PROGRESS:
            List<GraphicObject> group = new List<GraphicObject>();

            // Check if any of selected graphics are part of group
            foreach (GraphicObject g in selectedGraphics)
            {
                // If the current graphic belongs to a group, get a reference to that group
                group = GetGroup(g);

                // If found a group, break out of the loop
                if (group.Count > 0)
                {
                    break; 
                }
            }

            // If a group exists, add graphics to that group
            if (group.Count > 0)
            {
                foreach (GraphicObject g in selectedGraphics)
                {
                    // Add selected grapic if not already in group
                    if (!group.Contains(g))
                    {
                        group.Add(g);
                    }
                }
            }
            // Create a new group if one does not exist and add selected graphics to it
            else
            {
                foreach (GraphicObject g in selectedGraphics)
                {
                    // Add selected grapic
                    group.Add(g);
                }

                // Add that list to the group list
                groups.Add(group);
            }
        }

        // TODO: Ungroup the selected group of graphics
        private void button12_Click(object sender, EventArgs e)
        {
            // TODO: Find the list in the list of groups that contains any one of the selected graphics

            // TODO: Loop through the selected graphics and check to see if in a group

            // TODO: If one in a group is found, delete that list from the groups list

        }
    }
}
