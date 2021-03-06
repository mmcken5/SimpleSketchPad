﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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
        private Stack<List<GraphicObject>> state;
        private Stack<List<GraphicObject>> prevState;
        private List<GraphicObject> currentState;

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
            state = new Stack<List<GraphicObject>>();
            prevState = new Stack<List<GraphicObject>>();

            // Initialize state with blank canvas
            List<GraphicObject> blankCanvas = new List<GraphicObject>();
            Line blankLine = new Line(new Point(0, 0), Color.White, 1, -1);
            blankCanvas.Add(blankLine);
            //state.Push(blankCanvas);
            currentState = blankCanvas;

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

            // Set the default thickness to be 5
            comboBox2.SelectedIndex = 4;
            thickness = Int32.Parse(comboBox2.Text);

            // Set line as the default drawing mode
            button2.Select();
            drawMode = DrawMode.line;

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

                // Set line as the default drawing mode
                button2.Select();
                drawMode = DrawMode.line;
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

                        // If the graphic is part of a group, set the mouse point for all graphics in the group
                        List<GraphicObject> group = GetGroup(graphic);
                        if (group.Count > 0)
                        {
                            foreach (GraphicObject gO in group)
                            {
                                gO.SetMouseClickDragPoint(e.Location);
                            }
                        }

                        beginGraphicMove = true;
                        
                        break;
                    }
                }
            }
            // Redraw
            Redraw();
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
                    List<GraphicObject> group = new List<GraphicObject>();

                    // Check to see if selected graphic is part of a group
                    group = GetGroup(graphic);

                    // If selected graphic belongs to a group, apply move to all graphics in group
                    if (group.Count > 0)
                    {
                        foreach (GraphicObject g in group)
                        {
                            // Update the graphic to reflect dragging
                            g.UpdateMouseClickDragPoint(e.Location);
                        }
                    }                

                    // Update the graphic to reflect dragging
                    graphic.UpdateMouseClickDragPoint(e.Location);

                    isGraphicDragging = true; 
                }
            }

            // Refresh the picturebox (invoke the Paint event)
            Redraw();
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
                            MessageBox.Show("An error has occured while attempting to draw a Polygon.\r\n" + exc.Message);
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

                    if (graphic != null)
                    {

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

                            // Set the graphic reference to null (as nothing is selected)
                            graphic = null;
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
            }
            
            List<GraphicObject> copy = new List<GraphicObject>();
            GraphicObject newG; 

            // Loop through current graphics and create a copy
            foreach (GraphicObject g in graphicObjectList)
            {
                newG = g.Copy(id++);

                // Add the graphic to the list
                copy.Add(newG);
            }

            // Save the current state
            state.Push(currentState);
            currentState = copy;

            // Cause the picture box to re-paint (i.e. invoke Paint method)
            Redraw();
        }

        // If the graphic is part of a group return the group
        private List<GraphicObject> GetGroup(GraphicObject _go)
        {
            List<GraphicObject> list = new List<GraphicObject>();
            GraphicObject g = _go;

            // Check that a graphic is selected
            if (graphic != null)
            {
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
            else // Draw a blank canvas
            {
                e.Graphics.DrawLine(new Pen(Color.White, 2), new Point(0, 0), new Point(0, 0));
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
            Redraw();
        }

        // Copy the currently selected graphic(s)
        private void button9_Click(object sender, EventArgs e)
        {
            // Check if the selected graphic is part of a group
            List<GraphicObject> group = GetGroup(graphic);

            // Copy all the selected graphics to the copied list
            foreach (GraphicObject g in selectedGraphics)
            {
                // Create a copy of the graphic
                GraphicObject copy = g.Copy(id++);

                // Add the graphic to the copied graphics list
                copiedGraphics.Add(copy);
            }

            // If selected graphic part of a group, create a new group for the copied graphics
            if (group.Count > 0)
            {
                // Create a new group
                group = new List<GraphicObject>();

                // Add copied graphics to the new group
                foreach (GraphicObject g in copiedGraphics)
                {
                    // Add selected grapic
                    group.Add(g);
                }

                // Add that list to the group list
                groups.Add(group);
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

            // Set graphic reference to null (as nothing is selected)
            graphic = null;

            // Redraw
            Redraw();
        }

        // Group the selected graphics together
        private void button11_Click(object sender, EventArgs e)
        {
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

        // Ungroup the selected group of graphics
        private void button12_Click(object sender, EventArgs e)
        {
            // Find group that contains the selected graphic
            List<GraphicObject> group = GetGroup(graphic);

            // Delete the group
            if (group.Count > 0)
            {
                groups.Remove(group);
            }
        }

        // Undo the last drawing-related action
        private void btn_undo_Click(object sender, EventArgs e)
        {
            // Get the previous state
            if (state.Count > 0)
            {
                // Copy the current state to the list to be drawn
                List<GraphicObject> temp = new List<GraphicObject>();

                // Store current state in the redo stack
                foreach (GraphicObject g in currentState)
                {
                    temp.Add(g.Copy(id++));
                }
                prevState.Push(temp);


                temp = new List<GraphicObject>();

                // Get the previous state
                currentState = state.Pop();

                foreach (GraphicObject g in currentState)
                {
                    temp.Add(g.Copy(id++));
                }
                graphicObjectList = temp;
            }

            // Redraw
            Redraw();
        }

        // Redraw
        private void Redraw()
        {
            pictureBox1.Invalidate();
        }

        // Redo the last drawing-related action
        private void btn_redo_Click(object sender, EventArgs e)
        {
            if (prevState.Count == 0)
                return;

            // Copy the current state to the list to be drawn
            List<GraphicObject> temp = new List<GraphicObject>();

            // Store current state in the undo stack
            foreach (GraphicObject g in currentState)
            {
                temp.Add(g.Copy(id++));
            }
            state.Push(temp);

            temp = new List<GraphicObject>();

            // Get the previous state
            currentState = prevState.Pop();

            foreach (GraphicObject g in currentState)
            {
                temp.Add(g.Copy(id++));
            }
            graphicObjectList = temp;

            // Redraw
            Redraw();
        }

        // Save the drawings
        private void button14_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Get the file path as indicated by the user
                    string filePath = saveFileDialog1.FileName;

                    // Write to the new file
                    using (StreamWriter outputFile = new StreamWriter(filePath))
                    {
                        foreach (GraphicObject g in graphicObjectList)
                        {
                            outputFile.WriteLine(g.Encode());
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show("An error has occured while attempting to save the file./r/n" + exc.Message);
                }
            }
        }

        // Load an existing file
        private void button13_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Open the text file using a stream reader
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        // Clear the old graphics
                        graphicObjectList.Clear();

                        string textLine;

                        while ((textLine = sr.ReadLine()) != null)
                        {
                            // Decode the object type
                            string type = textLine.TrimStart('{').Split(':')[1].Replace("\"", "").Split(',')[0];

                            // Determine type of object and then create the object and add it to the list of objects to be drawn
                            if (type.ToLower() == "line")
                            {
                                Line obj = new Line();
                                obj.Decode(textLine);

                                graphicObjectList.Add(obj);
                            }
                            else if (type.ToLower() == "freehandline")
                            {
                                FreehandLine obj = new FreehandLine();
                                obj.Decode(textLine);

                                graphicObjectList.Add(obj);
                            }
                            else if (type.ToLower() == "rectangle")
                            {
                                Rectangle obj = new Rectangle();
                                obj.Decode(textLine);

                                graphicObjectList.Add(obj);
                            }
                            else if (type.ToLower() == "square")
                            {
                                Square obj = new Square();
                                obj.Decode(textLine);

                                graphicObjectList.Add(obj);
                            }
                            else if (type.ToLower() == "ellipse")
                            {
                                Ellipse l = new Ellipse();
                                l.Decode(textLine);

                                graphicObjectList.Add(l);
                            }
                            else if (type.ToLower() == "circle")
                            {
                                Circle obj = new Circle();
                                obj.Decode(textLine);

                                graphicObjectList.Add(obj);
                            }
                            else if (type.ToLower() == "polygon")
                            {
                                Polygon obj = new Polygon();
                                obj.Decode(textLine);

                                graphicObjectList.Add(obj);
                            }
                        }
                    }

                    Redraw();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("An error has occured./r/n" + exc.Message);
                }
            }
        }
    }
}
