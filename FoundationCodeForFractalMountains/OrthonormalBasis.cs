using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoundationCodeForFractalMountains
{
    public class OrthonormalBasis
    {

        #region Fields

        // private fields
        private Vector3 _position, _forward, _right, _up;

        #endregion


        #region Properties

        /******************************
         * 
         * DECLARATION OF PROPERTIES
         * 
         *****************************/

        // The property 'Position' represents the relative to the scene's co-ordinate system          
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }


        // The property 'Forward' represents the "forward" direction of the orthonormal basis.          
        public Vector3 Forward
        {
            get
            {
                return _forward;
            }
            set
            {
                _forward = value;
            }
        }


        // The property 'Right' represents the "right" direction of the orthonormal basis.          
        public Vector3 Right
        {
            get
            {
                return _right;
            }
            set
            {
                _right = value;
            }
        }


        // The property 'Up' represents the "up" direction of the orthonormal basis.          
        public Vector3 Up
        {
            get
            {
                return _up;
            }
            set
            {
                _up = value;
            }
        }


        #endregion


        #region Constructor

        // The orthonormal basis is determined once the vectors that define the "forward" and "right" directions are given.
        public OrthonormalBasis(Vector3 forward, Vector3 up)
        {
            generateBasis(forward, up);
        }

        #endregion


        #region Instance Methods

        /*************************************************************************
         * Generate an orthonormal basis given a direction vector "forward" and 
         * an "up" direction (both relative to the original co-ordinate system)
         * 
         *      _forward -> the "forward" direction (of the camera for e.g.)
         *      _right   -> the direction right of "forward"
         *      _up      -> the direction up from "forward"
         *      
         *************************************************************************/

        public void generateBasis(Vector3 forward, Vector3 up)
        {
            _forward = forward.normalize();
            _right = forward.crossProduct(up).normalize(); // This fails if 
            _up = _right.crossProduct(_forward);
        }

        public Vector3 projectOntoAxes(Vector3 v, bool isPosition)
        {
            if (isPosition) //For position vectors, use the position relative to the basis
                v = v.subtract(_position);

            Vector3 projection = new Vector3();

            //Project the vector onto each axis
            projection.X = _forward.dotProduct(v);
            projection.Y = _right.dotProduct(v);
            projection.Z = _up.dotProduct(v);

            return projection;
        }

        #endregion

    }
}

