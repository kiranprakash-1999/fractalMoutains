using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoundationCodeForFractalMountains
{
    public class Vertex
    {
        #region Fields

        /**Instance Data Fields
         * x, y: x-, y- and z-co-ordinates of this vertex.
         * 
         * These fields are NOT exposed to the user of the class.
         * Their values are accessible through the properties 'X,' 'Y' and 'Z."
         */
        private double _x, _y, _z;

        #endregion

        #region Properties

        // Static object used for generating pseudo-random numbers.
        private static Random randomGenerator = new Random();

        //The property 'X' exposes the field 'x'
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
        }//end property

        //The property 'Y' exposes the field 'y'
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

        }//end property

        //The property 'Z' exposes the field 'z'
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

        }//end property


        #endregion

        #region Constructors

        //Set x-, y- and z-co-ordinates of this vertex to 0
        public Vertex()
        {
            _x = 0;
            _y = 0;
            _z = 0;
        }

        //Set x-, y- and z-co-ordinates of this vertex to x, y and z respectively
        public Vertex(double x, double y, double z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }



        //Set this vertex to "v"
        public Vertex(Vertex v)
        {
            _x = v._x;
            _y = v._y;
            _z = v._z;
        }//end constructor

        #endregion

        #region Instance Methods

        // Adjust the height by an amount proportional to the distance between the two given vertices.
        // 
        public void adjustHeight(Vertex v1, Vertex v2)
        {
            double adjustmentFactor = randomGenerator.NextDouble();
            double distance = new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z).magnitude();
            
            if (double.IsNaN(distance))
                throw new Exception("distance is NaN");

            // ??? Must experiment with this
            _y = (randomGenerator.Next(0, 2) == 1) ? _y + adjustmentFactor * distance / 6 : _y - adjustmentFactor * distance /6;

            if (double.IsNaN(_y))
                throw new Exception("_y is NaN");

        }//end adjustHeight

        #endregion

    }//end of class Vertex
}
