using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoundationCodeForFractalMountains
{
    /******************************************************
   * The class "Vector3" models a 3-dimensional vector
   ******************************************************/
    public class Vector3
    {

        #region Fields

        private static readonly Vector3 ZERO_VECTOR = new Vector3(0, 0, 0);
        private static readonly Vector3 I = new Vector3(1, 0, 0), J = new Vector3(0, 1, 0), K = new Vector3(0, 0, 1);

        /**********************************************************************
         * FIELDS of the 'Vector3' class
         * x, y, z: x, y and z components of this vector.
         * These fields are NOT exposed to the user of the class.
         * Their values are accessible through the properties 'X,' 'Y' and 'Z'
         **********************************************************************/
        private double _x, _y, _z;

        #endregion

        #region Properties

        /******************************
         * 
         * DECLARATION OF PROPERTIES
         * 
         *****************************/

        //The property 'X' represents the "x-component" of this vector.          
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        //The property 'Y' represents the "y-component" of this vector.
        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }

        //The property 'Y' represents the "y-component" of this vector.
        public double Z
        {
            get
            {
                return _z;
            }

            set
            {
                _z = value;
            }
        }

        #endregion

        #region Constructors

        //Set x, y and z components of this vector to 0
        public Vector3()
        {
            _x = 0;
            _y = 0;
            _z = 0;
        }

        //Set x, y and z components of this vector to x and y respectively
        public Vector3(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        //Set this vector equal to the vector "v"
        public Vector3(Vector3 v)
        {
            _x = v._x;
            _y = v._y;
            _z = v._z;

         }

        #endregion

        #region Instance Methods

        /*********************
         * INSTANCE METHODS
         ********************/

        //Copy the vector "v" to this vector
        public void copy(Vector3 v)
        {
            _x = v._x;
            _y = v._y;
            _z = v._z;
        }

        //Return the sum of this vector and "v"
        public Vector3 add(Vector3 v)
        {
            return new Vector3(_x + v._x, _y + v._y, _z + v._z);
        }

        //Return this vector subtract "v"
        public Vector3 subtract(Vector3 v)
        {
            return new Vector3(_x - v._x, _y - v._y, _z - v._z);
        }


        //Return this vector multiplied by the scalar "a"
        public Vector3 timesScalar(double a)
        {
            return new Vector3(a * _x, a * _y, a * _z);
        }

        //Return the dot product of this vector with the vector "v"	
        public double dotProduct(Vector3 v)
        {
            return _x * v._x + _y * v._y + _z * v._z;
        }


        public Vector3 crossProduct(Vector3 v)
        {
            return new Vector3(_y * v._z - _z * v._y, _z * v._x - _x * v._z, _x * v._y - _y * v._x);
        }

        //Return the square of the magnitude of this vector
        public double squareOfMagnitude()
        {
            return _x * _x + _y * _y + _z * _z;
        }

        //Return the magnitude of this vector
        public double magnitude()
        {
            return Math.Sqrt(_x * _x + _y * _y + _z * _z);
        }

        //Normalize this vector
        public Vector3 normalize()
        {
            double normalizingFactor = 1 / Math.Sqrt(_x * _x + _y * _y + _z * _z);

            if (double.IsNaN(normalizingFactor))
                throw new InvalidOperationException("Division by zero.");

            return new Vector3(_x * normalizingFactor, _y * normalizingFactor, _z * normalizingFactor);
        }

        //Is this vector parallel to the given vector?
        public bool isParallelTo(Vector3 v)
        {
            if (this.crossProduct(v).add(new Vector3(0, 0, 0)) == ZERO_VECTOR)
            {
                return true;
            }

            return false;
        }


        //Is this vector perpendicular to the given vector?
        public bool isPerpendicularTo(Vector3 v)
        {
            if (this.dotProduct(v) == 0)
            {
                return true;
            }

            return false;
        }

        //Rotate this vector about the x-axis
        public Vector3 rotateX(double thetaRadians)
        {
            double x = _x;
            double y = Math.Cos(thetaRadians) * _y - Math.Sin(thetaRadians) * _z;
            double z = Math.Sin(thetaRadians) * _y + Math.Cos(thetaRadians) * _z;

            return new Vector3(x, y, z);
        }

        //Rotate this vector about the x-axis
        public Vector3 rotateDegreesX(double thetaDegrees)
        {
            double thetaRadians = thetaDegrees * Math.PI / 180;
            return rotateX(thetaRadians);

        }

        public Vector3 rotateY(double thetaRadians)
        {
            double x = Math.Sin(thetaRadians) * _z + Math.Cos(thetaRadians) * _x;
            double y = _y;
            double z = Math.Cos(thetaRadians) * _z - Math.Sin(thetaRadians) * _x;

            return new Vector3(x, y, z);
        }

        public Vector3 rotateDegreesY(double thetaDegrees)
        {
            double thetaRadians = thetaDegrees * Math.PI / 180;
            return rotateY(thetaRadians);
        }

        public Vector3 rotateZ(double thetaRadians)
        {
            double x = Math.Cos(thetaRadians) * _x - Math.Sin(thetaRadians) * _y;
            double y = Math.Sin(thetaRadians) * _x + Math.Cos(thetaRadians) * _y;
            double z = _z;

            return new Vector3(x, y, z);
        }

        public Vector3 rotateDegreesZ(double thetaDegrees)
        {
            double thetaRadians = thetaDegrees * Math.PI / 180;
            return rotateZ(thetaRadians);

        }

        public string toStringOrderedTripleNotation()
        {
            return "(" + _x + ", " + _y + ", " + _z + ")";
        }

        //Return a string representing this vector in the format xi + yj
        public string toStringUnitVectorNotation()
        {
            return _x + "i " + ((_y < 0) ? "- " : "+ ") + Math.Abs(_y) + "j"
                                 + ((_z < 0) ? "- " : "+ ") + Math.Abs(_z) + "k";
        }

        #endregion

        #region Static Methods

        /**********************
         * STATIC METHODS
         *********************/

        //Return the sum of the vectors "v1" and "v2"
        public static Vector3 sum(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1._x + v2._x, v1._y + v2._y, v1._z + v2._z);
        }

        //Return the difference of the vectors "v1" and "v2"
        public static Vector3 difference(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1._x - v2._x, v1._y - v2._y, v1._z - v2._z);
        }

        //Return the vector "v" multiplied by the scalar "a"
        public static Vector3 product(double a, Vector3 v)
        {
            return new Vector3(a * v._x, a * v._y, a * v._z);
        }

        #endregion
    }
}
