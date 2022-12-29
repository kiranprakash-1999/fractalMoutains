using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FoundationCodeForFractalMountains
{

    public class Camera
    {
        #region Fields

        //Screen data
        private const double DEG_TO_RAD = 0.0174532925199433;
        private Point _screenSize, _halfScreenSize;
        private double _aspectRatio;

        //Orthonormal basis for the camera
        private OrthonormalBasis _basis;

        //Horizontal field of view
        private double _fieldOfView;

        // Other camera data
        private Vector3 _lightSourcePosition = new Vector3(); // Not yet implemented.

        //Store reciprocals since multiplication is a faster operation than division
        private double invYTan; //1 / Tan(horizontal field of view / 2)
        private double invZTan; //1 / Tan(vertical field of view / 2)

        #endregion

        #region Properties

        /************************
         * PROPERTY DECLARATIONS
         ************************/


        // READ-ONLY PROPERTY: The orthonormal basis defines both the position of the camera and the 
        // direction in which it is pointed. Both of these are 3D vectors defined in the "world" space.
        public OrthonormalBasis Basis
        {
            get
            {
                return _basis;
            }
            
        }

        public Vector3 LightSourcePosition
        {
            get
            {
                return _lightSourcePosition;
            }
            set
            {
                _lightSourcePosition = value;
            }
        }

        public Point ScreenSize
        {
            get
            {
                return _screenSize;
            }
            set
            {
                _screenSize = value;
                _halfScreenSize = new Point(_screenSize.X / 2, _screenSize.Y / 2);
            }
        }

        public double FieldOfView
        {
            get
            {
                return _fieldOfView;
            }
            set
            {
                _fieldOfView = value;
                setFieldOfView(_fieldOfView);
            }
        }

        #endregion

        #region Constructors

        /******************
         * CONSTRUCTORS
         ******************/

        public Camera(Vector3 forward, Vector3 up, Vector3 position, Point screenSize)
        {
            initializeCamera(forward, up, position, 70,screenSize); 
        }


        public Camera(Vector3 forward, Vector3 up, Vector3 position, Vector3 lightSourcePosition, Point screenSize)
        {
            initializeCamera(forward, up, position, 70, screenSize);

            _lightSourcePosition = lightSourcePosition;
        }

        public Camera(Vector3 forward, Vector3 up, Vector3 position, double fieldOfView, Point screenSize)
        {
            initializeCamera(forward, up, position, fieldOfView,screenSize); 
        }

        public Camera(Vector3 forward, Vector3 right, Vector3 position, double fieldOfView, Vector3 lightSourcePosition, Point screenSize)
        {
            initializeCamera(forward, right, position, fieldOfView, screenSize); 

            _lightSourcePosition = lightSourcePosition;
        }

        #endregion

        #region Instance Methods

        public void initializeCamera(Vector3 forward, Vector3 up, Vector3 position, double fieldOfView, Point screenSize)
        {
            _basis = new OrthonormalBasis(forward, up);
            _basis.Position = position;
            _basis.generateBasis(forward, up);
            _screenSize = screenSize;
            _aspectRatio = ((double)_screenSize.X) / _screenSize.Y;
            _halfScreenSize = new Point(_screenSize.X / 2, _screenSize.Y / 2);

            setFieldOfView(fieldOfView);
         }

        public void setFieldOfView(double fieldOfView)
        {
            double fieldOfViewRad = fieldOfView * DEG_TO_RAD * 0.5; //(FOV in radians) / 2

            _fieldOfView = fieldOfView;

            //Calculate tangent reciprocals for quick computation of quotients (via multiplication)
            invYTan = 1.0 / Math.Tan(fieldOfViewRad);
            invZTan = 1.0 / Math.Tan(fieldOfViewRad /_aspectRatio);
        }

        //Projects a vector onto the camera in pixel coordinates
        public Point toScreen(Vector3 v)
        {
            Point screen = new Point();

            double x, y, z; // Coordinates relative to camera

            Vector3 proj = _basis.projectOntoAxes(v, true);

            x = proj.X;
            y = proj.Y;
            z = proj.Z;

            if (x != 0)
            {
                screen.X = (int)(y / x * invYTan * _halfScreenSize.X + _halfScreenSize.X);//
                screen.Y = (int)(-z / x * invZTan * _halfScreenSize.Y + _halfScreenSize.Y);
            }
            
            return screen;
        }

        #endregion
    }

}
