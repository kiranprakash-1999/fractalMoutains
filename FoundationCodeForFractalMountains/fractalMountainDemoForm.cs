using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Windows.;

namespace FoundationCodeForFractalMountains
{
    public partial class fractalMountainDemoForm : Form
    {

        
        #region Data Members of "fractalMountainDemo"

        private int _screenWidth, _screenHeight;

        /*
         * Co-ordinates of initial points on "pointDraggingPictureBox." 
         * The user can drag these points to select a custom initial triangle.
         * 
         * initialA -> initial point on z-axis
         * initialB -> initial point on x-axis
         * initialC -> initial point on y-axis
         * 
         * Points on x-axis: (50, 260), (119, 189) -> Equation Y = (-71/69)*X + (21490/69) (relative to "pointDraggingPictureBox")
         */

        private Point initialA = new Point(119, 55), initialB = new Point(51, 261), initialC = new Point(250, 190);

        // Some constants required for mapping points on "pointDraggingPictureBox" to 3D space.
        private const int ORIGIN_X = 119, ORIGIN_Y = 189; //Co-ordinates of origin relative to "pointDraggingPictureBox"
        private const int A_INDEX = 0, B_INDEX = 1, C_INDEX = 2;

        // Required for the feature that allows the user to select the initial vertices simply by dragging.
        private Bitmap pointDraggingBitmap = new Bitmap(300, 300);
        private Point[] dotsLocationRelativeToPicBox = new Point[3]; //Stores the co-ordinates of the centres of the dragging dots relative to picture box 
        private PictureBox[] dotPic = new PictureBox[3]; //Stores references to the picture boxes uses for the dot pictures.

        Random colourCodeGenerator = new Random();

        // Objects needed for GDI+ graphics.
        private Bitmap fractalMountainBitmap;
        private Graphics fractalMountainDrawingSurface;
        private Pen drawMountainPen = new Pen(Color.Black);

        private Pen drawInitialTrianglePen = new Pen(Color.Blue, 2); // The second argument specifies the line width.
        private Brush drawInitialTriangleBrush = new SolidBrush(Color.FromArgb(100, Color.LightSteelBlue));
      

        // Create an empty list of triangles
        private List<Triangle> triangleList = new List<Triangle>();

        // creates a vector for the light source
        private Vector3 lightSource = new Vector3(1,1,1);

        // Needed to traverse the list of triangles when drawing  the triangles
        private System.Collections.IEnumerator triangleListEnumerator;

        //Vertices and edges of initial triangle.
        private Vertex A, B, C;
        private Edge AB, BC, CA;

        // stores previous coordinates during rotation
        private int lastX_Angle, lastY_Angle, lastZ_Angle;

        



        /**     A
                /\
               /  \
              /    \
             /      \
            /        \  
           /          \
         B ------------ C
         
        */


        // Required for perspective projection
        private Camera camera;

        

        #endregion

        #region Constructor

        public fractalMountainDemoForm()
        {
            InitializeComponent();

            /**
             * Initializations
             */

            _screenWidth = mountainPictureBox.Width;
            _screenHeight = mountainPictureBox.Height;
            fractalMountainBitmap = new Bitmap(_screenWidth, _screenHeight);

            fractalMountainDrawingSurface = Graphics.FromImage(fractalMountainBitmap);

            triangleListEnumerator = triangleList.GetEnumerator();

            Graphics pointDraggingSurface = Graphics.FromImage(pointDraggingBitmap);

            dotPic[0] = movePointDotPictureBoxA;
            dotPic[1] = movePointDotPictureBoxB;
            dotPic[2] = movePointDotPictureBoxC;

            //Initialize "dotLocation," the array of "Point" objects.
            dotsLocationRelativeToPicBox[0] = initialA;
            dotsLocationRelativeToPicBox[1] = initialB;
            dotsLocationRelativeToPicBox[2] = initialC;

            //Draw initial triangle.
            pointDraggingSurface.DrawPolygon(drawInitialTrianglePen, new Point[] { initialA, initialB, initialC });


            //Fill triangle
            pointDraggingSurface.FillPolygon(drawInitialTriangleBrush, new Point[] { initialA, initialB, initialC });


            //Place the "dragging" dots at their initial positions
            for (int i = 0; i < dotPic.Length; i++)
            {
                dotPic[i].Location = new Point(dotsLocationRelativeToPicBox[i].X + pointDraggingPictureBox.Left - dotPic[i].Width / 2,
                                        dotsLocationRelativeToPicBox[i].Y + pointDraggingPictureBox.Top - dotPic[i].Height / 2);
            }

            // Fire the "Paint" event on "pointDraggingPictureBox"
            pointDraggingPictureBox.Refresh();

            //Set vertices of initial triangle.
            A = transformTo3D(initialA, A_INDEX);
            B = transformTo3D(initialB, B_INDEX);
            C = transformTo3D(initialC, C_INDEX);

            //Set edges of initial triangle.
            AB = new Edge(A, B);
            BC = new Edge(B, C);
            CA = new Edge(C, A);

            //Create the camera (direction, right, cameraPosition)
            camera = new Camera(new Vector3(-1, 0, -1), new Vector3(0, 1, 0), new Vector3(1, 0.2, 1), 90, new Point(_screenWidth, _screenHeight));

        }

        

        #endregion

        #region Event Handlers


        private void yRotationTrackBar_Scroll(object sender, EventArgs e)
        {
            // rotates all triangles within list
            triangleList = rotateMountain(triangleList, yRotationTrackBar.Value - lastY_Angle, 'y');
            // sets current value to previous value for the next time scrolling occurs
            lastY_Angle = yRotationTrackBar.Value;
            drawTriangles();
            mountainPictureBox.Refresh();
        }

        private void zRotationTrackBar_Scroll(object sender, EventArgs e)
        {
            triangleList = rotateMountain(triangleList, zRotationTrackBar.Value - lastZ_Angle, 'z');
            lastZ_Angle = zRotationTrackBar.Value;
            drawTriangles();
            mountainPictureBox.Refresh();
        }

        private void xrotationTrackBar_Scroll(object sender, EventArgs e)
        {
            triangleList = rotateMountain(triangleList, xRotationTrackBar.Value - lastX_Angle, 'x');
            lastX_Angle = xRotationTrackBar.Value;
            drawTriangles();
            mountainPictureBox.Refresh();
        }

        // change coordinates of light source / / / / / / / / / / / / / / / / / / / / / / / / / / / / // 
                                                                                                      //
        private void xNumericUpDown_ValueChanged(object sender, EventArgs e)                          //
        {                                                                                             //
            lightSource.X = (double)xNumericUpDown.Value;                                             //
            drawTriangles();                                                                          //
            mountainPictureBox.Refresh();                                                             //
        }                                                                                             //
                                                                                                      //
        private void yNumericUpDown_ValueChanged(object sender, EventArgs e)                          //
        {                                                                                             //
            lightSource.Y = (double)yNumericUpDown.Value;                                             // 
            drawTriangles();                                                                          //
            mountainPictureBox.Refresh();                                                             //
        }                                                                                             //
                                                                                                      //
        private void zNumericUpDown_ValueChanged(object sender, EventArgs e)                          // 
        {                                                                                             //
            lightSource.Z = (double)zNumericUpDown.Value;                                             //
            drawTriangles();                                                                          //
            mountainPictureBox.Refresh();                                                             //
        }                                                                                             //
                                                                                                      //
        // / / / / / / / / / / / / / / / / / / / //  / / / / / / / / // /  / / // / / /  // / / / / / // 



        private void animationTimer_Tick(object sender, EventArgs e)
        {
            if(xAnimationCheckBox.Checked == true)
            {
                triangleList = rotateMountain(triangleList, 1, 'x');
                drawTriangles();
                
            }

            if (yAnimationCheckBox.Checked == true)
            {
                triangleList = rotateMountain(triangleList, 1, 'y');
                drawTriangles();
               
            }

            if (zAnimationCheckBox.Checked == true)
            {
                triangleList = rotateMountain(triangleList, 1, 'z');
                drawTriangles();
                
            }
            mountainPictureBox.Refresh();
        }

        private void stopAnimationButton_Click(object sender, EventArgs e)
        {
            animationTimer.Enabled = false;
        }

        private void timerIntervalTrackBar_Scroll(object sender, EventArgs e)
        {
            animationTimer.Interval = timerIntervalTrackBar.Value;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            triangleList.Clear();
            drawTriangles();
            mountainPictureBox.Refresh();
        }

        private void timerIntervalTrackBar_ValueChanged(object sender, EventArgs e)
        {
            animationTimer.Interval = timerIntervalTrackBar.Value;
        }

        private void animateButton_Click(object sender, EventArgs e)
        {
            animationTimer.Enabled = true;
        }

        private void fractalMountainDemoForm_Load(object sender, EventArgs e)
        {
            // set default settings

            lastX_Angle = xRotationTrackBar.Value;
            lastY_Angle = yRotationTrackBar.Value;
            lastZ_Angle = zRotationTrackBar.Value;
            normalColouringRadioButton.Checked = true;

            xNumericUpDown.Value = 10;
            yNumericUpDown.Value = 10;
            zNumericUpDown.Value = 10;

            pointDraggingPictureBox.Image = Properties.Resources.xyz_3D_coordinate_axes;
        }

       

        private void drawMountainButton_Click(object sender, EventArgs e)
        {
            lightSource.X = (double)xNumericUpDown.Value;
            lightSource.Y = (double)yNumericUpDown.Value;
            lightSource.Z = (double)zNumericUpDown.Value;

            int maxIterations = Convert.ToInt32(iterationsNumericUpDown.Value);

            xRotationTrackBar.Value = 0;
            yRotationTrackBar.Value = 0;
            zRotationTrackBar.Value = 0;

            triangleList.Clear();
            fractalMountainDrawingSurface.Clear(mountainPictureBox.BackColor);
            mountainPictureBox.Refresh();

            //Add the initial Triangle to the list "TriangleList."
            triangleList.Add(new Triangle(AB, BC, CA));

            //Draw the initial triangle
            drawTriangles();
            mountainPictureBox.Refresh();

            System.Threading.Thread.Sleep(100); //Pause for 1000 ms = 1 s

            for (int iteration = 1; iteration <= maxIterations; iteration++)
            {
               
                    fractalMountainDrawingSurface.Clear(mountainPictureBox.BackColor);
                    mountainPictureBox.Refresh();

                    triangleList = subdivideTriangles();

                drawTriangles();

                mountainPictureBox.Refresh();

                    System.Threading.Thread.Sleep(100); //Pause for 1000 ms = 1 s
                
                
            }
        }

      
        private void mountainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(fractalMountainBitmap, 0, 0);
        }

        private void fieldOfViewTrackBar_ValueChanged(object sender, EventArgs e)
        {
            fieldOfViewLabel.Text = fieldOfViewTrackBar.Value.ToString();
            //Application.DoEvents();
            camera.FieldOfView = fieldOfViewTrackBar.Value;
            //fractalMountainDrawingSurface.Clear(mountainPictureBox.BackColor);
            drawTriangles();
            mountainPictureBox.Refresh();
        }

        private void pointDraggingPictureBox_Paint(object sender, PaintEventArgs e)
        {
           
           e.Graphics.DrawImage(pointDraggingBitmap, 0, 0);
        }

        // Update the location of the point being dragged on "pointDraggingPictureBox"
        public void movePointDotPictureBox_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                PictureBox picBox = ((PictureBox)sender);
                Point newLocationRelativeToClientRectangle = new Point(picBox.Left + e.X, picBox.Top + e.Y);
                Point originalLocationRelativeToClientRectangle = new Point(picBox.Left, picBox.Top);

                // If the new location of the "dot" picture box is within the boundaries of "pointDraggingPictureBox,"
                // move the picture box to the new location. Otherwise, do nothing.
                if (newLocationRelativeToClientRectangle.X >= pointDraggingPictureBox.Left
                                && newLocationRelativeToClientRectangle.X <= pointDraggingPictureBox.Right - picBox.Width
                                && newLocationRelativeToClientRectangle.Y >= pointDraggingPictureBox.Top
                                && newLocationRelativeToClientRectangle.Y <= pointDraggingPictureBox.Bottom - picBox.Height)
                {
                    for (int i = 0; i < dotPic.Length; i++)
                    {
                        if (sender.Equals(dotPic[i]))
                        {
                            //Update location of dot being dragged

                            if (i == A_INDEX) //point on z-axis 
                            {
                                picBox.Location = new Point(originalLocationRelativeToClientRectangle.X, newLocationRelativeToClientRectangle.Y);
                                A_Label.Location = new Point(picBox.Location.X - A_Label.Width, picBox.Location.Y);
                            }
                            else if (i == B_INDEX) //points on x-axis have equation Y = (-71.0/69.0)*X + (21490/69) relative to "pointDraggingPictureBox"
                            {
                                picBox.Location = new Point(newLocationRelativeToClientRectangle.X,
                                                        (int)((-71.0 / 69.0) * (newLocationRelativeToClientRectangle.X - pointDraggingPictureBox.Left)
                                                                        + (21490.0 / 69.0) + pointDraggingPictureBox.Top - picBox.Height)); //
                                B_Label.Location = new Point(picBox.Location.X + B_Label.Width / 2, picBox.Location.Y + B_Label.Height / 2);
                            }
                            else //point on y-axis
                            {
                                picBox.Location = new Point(newLocationRelativeToClientRectangle.X, originalLocationRelativeToClientRectangle.Y);
                                C_Label.Location = new Point(picBox.Location.X, picBox.Location.Y + 2 * C_Label.Height / 3);
                            }//end if


                            dotsLocationRelativeToPicBox[i] = new Point(picBox.Left - pointDraggingPictureBox.Left + picBox.Width / 2, picBox.Top - pointDraggingPictureBox.Top + picBox.Height / 2);


                            /***********
                             * Redraw
                             ***********/
                            Graphics pointDraggingSurface = Graphics.FromImage(pointDraggingBitmap);

                            //erase
                            pointDraggingSurface.Clear(pointDraggingPictureBox.BackColor);

                            //draw triangle
                            pointDraggingSurface.DrawPolygon(drawInitialTrianglePen, new Point[] { dotsLocationRelativeToPicBox[0], dotsLocationRelativeToPicBox[1], dotsLocationRelativeToPicBox[2] });

                            //Fill triangle 
                            pointDraggingSurface.FillPolygon(drawInitialTriangleBrush, new Point[] { dotsLocationRelativeToPicBox[0], dotsLocationRelativeToPicBox[1], dotsLocationRelativeToPicBox[2] });

                            //Fire the "Paint" event on "pointDraggingPictureBox"
                            pointDraggingPictureBox.Refresh();

                        }//end if

                    }//end for

                    // Set co-ordinates of vertices of initial triangle in 3D space based on location of dots. 
                    // "A" lies on the z-axis, "B" lies on the x-axis, "C" lies on the y-axis

                    A = transformTo3D(dotsLocationRelativeToPicBox[A_INDEX], A_INDEX);
                    B = transformTo3D(dotsLocationRelativeToPicBox[B_INDEX], B_INDEX);
                    C = transformTo3D(dotsLocationRelativeToPicBox[C_INDEX], C_INDEX);

                    pointA_3D_Label.Text = "A(" + Math.Round(A.X, 2).ToString() + ", " + Math.Round(A.Y, 2).ToString()
                                                                                    + ", " + Math.Round(A.Z, 2).ToString() + ")";
                    pointC_3D_Label.Text = "C(" + Math.Round(C.X, 2).ToString() + ", " + Math.Round(C.Y, 2).ToString()
                                                                                    + ", " + Math.Round(C.Z, 2).ToString() + ")";
                    //Set edges of initial triangles.
                    AB = new Edge(A, B);
                    BC = new Edge(B, C);
                    CA = new Edge(C, A);

                } //end if
            }// end if

        }//end movePointDotPictureBox_MouseMove


        #endregion

        #region Other Methods


        private List<Triangle> rotateMountain(List<Triangle> triangleList, int angle, char axis)
        {
            // local list to store new rotated triangles
            List<Triangle> rotatedTriangleList = new List<Triangle>();

            
            for (int x = 0; x < triangleList.Count; x++)
            {
                // within the triangles properties, find the verices, and creates vectors with the same coordinates
                Vector3 ABVector = new Vector3(triangleList[x].AB.V1.X, triangleList[x].AB.V1.Y, triangleList[x].AB.V1.Z);
                Vector3 BCVector = new Vector3(triangleList[x].BC.V1.X, triangleList[x].BC.V1.Y, triangleList[x].BC.V1.Z);
                Vector3 ACVector = new Vector3(triangleList[x].CA.V1.X, triangleList[x].CA.V1.Y, triangleList[x].CA.V1.Z);

                if (axis == 'x')
                {
                    ABVector = ABVector.rotateDegreesX(angle);
                    BCVector = BCVector.rotateDegreesX(angle);
                    ACVector = ACVector.rotateDegreesX(angle);
                }
                else if (axis == 'y')
                {
                    ABVector = ABVector.rotateDegreesY(angle);
                    BCVector = BCVector.rotateDegreesY(angle);
                    ACVector = ACVector.rotateDegreesY(angle);
                }
                else
                {
                    ABVector = ABVector.rotateDegreesZ(angle);
                    BCVector = BCVector.rotateDegreesZ(angle);
                    ACVector = ACVector.rotateDegreesZ(angle);
                }

                // convert vectors back into vertices
                Vertex firstVertex = new Vertex(ABVector.X, ABVector.Y, ABVector.Z);
                Vertex secondVertex = new Vertex(BCVector.X, BCVector.Y, BCVector.Z);
                Vertex thirdVertex = new Vertex(ACVector.X, ACVector.Y, ACVector.Z);

                Triangle rotatedTriangle = new Triangle(firstVertex, secondVertex, thirdVertex);
                rotatedTriangleList.Add(rotatedTriangle);
            }



            return rotatedTriangleList;
        }

        // Subdivide each triangle currently in "trianglelList."
        // Return a new list containing the subdivided triangles.
        private List<Triangle> subdivideTriangles()
        {
            List<Triangle> newTriangleList = new List<Triangle>();
            System.Collections.IEnumerator TriangleListEnumerator = triangleList.GetEnumerator();

            while (TriangleListEnumerator.MoveNext())
            {
                Triangle t = (Triangle)TriangleListEnumerator.Current;
                t.subdivide(newTriangleList);
            }

            return newTriangleList;

        }//end subdivideTriangle


        // Draw the triangles currently in "triangleList."
        private void drawTriangles()
        {
            
            fractalMountainDrawingSurface.Clear(mountainPictureBox.BackColor);
            mountainPictureBox.Refresh();
            System.Collections.IEnumerator TriangleListEnumerator = triangleList.GetEnumerator();
            Graphics g = Graphics.FromImage(fractalMountainBitmap);

            while (TriangleListEnumerator.MoveNext())
            {
                Triangle t = (Triangle)TriangleListEnumerator.Current;

                Edge AB = new Edge(t.AB);

                Vector3 ABVector = new Vector3(t.AB.V1.X, t.AB.V1.Y, t.AB.V1.Z);
                Vector3 BCVector = new Vector3(t.BC.V1.X, t.BC.V1.Y, t.BC.V1.Z);
                Vector3 ACVector = new Vector3(t.CA.V1.X, t.CA.V1.Y, t.CA.V1.Z);

                Brush mountainColour = determineBrushColour(ABVector, BCVector, ACVector); // gives brush colour depending on what radiobutton is selected

                Vertex A = new Vertex(AB.V1);
                Vertex B = new Vertex(AB.V2);
                Vertex C = new Vertex(t.BC.V2);

                Vector3 OA = new Vector3(A.X, A.Y, A.Z);
                Vector3 OB = new Vector3(B.X, B.Y, B.Z);
                Vector3 OC = new Vector3(C.X, C.Y, C.Z);

                Point A_Screen = camera.toScreen(OA);
                Point B_Screen = camera.toScreen(OB);
                Point C_Screen = camera.toScreen(OC);

                if (mountainColour == null) // signifies normal drawing, no colouring/shading
                {
                    g.DrawPolygon(Pens.Black, new Point[] { A_Screen, B_Screen, C_Screen });
                }
                else
                {
                    g.FillPolygon(mountainColour, new Point[] { A_Screen, B_Screen, C_Screen });
                }

            }

        }//end drawTriangles

        
        // Transform the 2D co-ordinates of the initial triangle (in "pointDraggingPictureBox") to 3D co-ordinates.
        private Vertex transformTo3D(Point p, int index)
        {
            Vertex pointIn3D = new Vertex(0, 0, 0);

            if (index == A_INDEX) //Point is on the y-axis
            {
                pointIn3D.Y = ((double)(ORIGIN_Y - p.Y)) / (ORIGIN_Y - initialA.Y);
            }

            else if (index == B_INDEX) //Point is on the z-axis
            {
                //Not yet implemented. Use default value of 1 for now.
                pointIn3D.Z = 1;
            }

            else if (index == C_INDEX) //Point is on the x-axis
            {
                pointIn3D.X = ((double)(p.X - ORIGIN_X)) / (initialC.X - ORIGIN_X);
                
            }

            return pointIn3D;
        }
  

        private Brush determineBrushColour(Vector3 AB, Vector3 BC, Vector3 AC)
        {
            if (shadingRadioButton.Checked == true)
            {
                Vector3 planeVector1 = AB.subtract(BC);
                Vector3 planeVector2 = BC.subtract(AC);

                Vector3 normalVector = planeVector1.crossProduct(planeVector2);

                // intensity factor depends on the angle between them, the cos of the angle is the factor
                double intensityFactor = normalVector.normalize().dotProduct(lightSource.normalize());

                                                        // scaling a range of [-1,1] to [0,1] with a linear function
                return new SolidBrush(Color.FromArgb((int)(((intensityFactor + 1) / 2) * 255), (int)(((intensityFactor + 1) / 2) * 255), (int)(((intensityFactor + 1) / 2) * 255)));
            }
            else if (negativeShadingRadioButton.Checked == true)
            {
                Vector3 planeVector1 = AB.subtract(BC);
                Vector3 planeVector2 = BC.subtract(AC);

                Vector3 normalVector = planeVector1.crossProduct(planeVector2);

                // slightly different algorithm, find the angle and divide by max angle (pi), which gives an interval of [0,1]
                double intensityFactor = Math.Acos(normalVector.normalize().dotProduct(lightSource.normalize())) / Math.PI;

                return new SolidBrush(Color.FromArgb((int)(intensityFactor * 255), (int)(intensityFactor * 255), (int)(intensityFactor * 255)));
            }
            else if (randomColoursRadioButton.Checked == true)
            {
                // generate random numbers between [0,255] and determine brush colour using RGB code
                return new SolidBrush(Color.FromArgb(colourCodeGenerator.Next(0, 256), colourCodeGenerator.Next(0, 256), colourCodeGenerator.Next(0, 256)));
            }
            else if (normalColouringRadioButton.Checked == true)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        #endregion



    }// end class
}//end namespace
