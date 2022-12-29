using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FoundationCodeForFractalMountains
{
    public class Triangle
    {
    
        #region Fields

        //    a 
        //    /\
        //   /  \
        //  /    \
        // b -----c

        //Fields

        //Edges of Triangle abcd
        private Edge _ab, _bc, _ca;

        #endregion


        #region Properties

        //Properties
        public Edge AB
        {
            get
            {
                return _ab;
            }

            set
            {
                _ab = value;
            }
        }// End property AB

        public Edge BC
        {
            get
            {
                return _bc;
            }

            set
            {
                _bc = value;
            }
        }// End property BC

        public Edge CA
        {
            get
            {
                return _ca;
            }

            set
            {
                _ca = value;
            }
        }// End property CA

        #endregion


        #region Constructors
        /**
         * Constructor Methods
         */

        //Set the edges of this Triangle


        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            _ab = new Edge(a, b);
            _bc = new Edge(b, c);
            _ca = new Edge(c, a);
      
        }//end constructor

        public Triangle(Edge ab, Edge bc, Edge ca)
        {
            this._ab = ab;
            this._bc = bc;
            this._ca = ca;
            
        }//end constructor

        #endregion


        #region Instance Methods
        /**
         * Other methods, all of which are instance methods.
         */

        

        // Subdivide one 'Triangle' into four. Then add to the given list.
        //            a 
        //            /\
        //           /  \
        //    mid_ab/....\ mid_ca
        //         / .   .\
        //        /    .   \
        //       b----------c
        //          mid_bc   
        public void subdivide(List<Triangle> triangleList)
        {
            Vertex mid_ab = _ab.subdivideEdge();
            Vertex mid_bc = _bc.subdivideEdge();
            Vertex mid_ca = _ca.subdivideEdge();
     
            //Create edges joining midpoints of the sides to the appropriate vertex
            Edge midAB_midBC = new Edge(mid_ab, mid_bc);
            Edge midBC_midCA = new Edge(mid_bc, mid_ca);
            Edge midCA_midAB = new Edge(mid_ca, mid_ab);
          
            // Create the new triangles and add them to the list.
            // Pay attention to the direction of the edges when creating the triangles.
            triangleList.Add(new Triangle(_ab.V1_Mid, midCA_midAB.reverse(),_ca.Mid_V2));
            triangleList.Add(new Triangle(_ab.Mid_V2, _bc.V1_Mid, midAB_midBC.reverse()));

            triangleList.Add(new Triangle(midBC_midCA.reverse(), _bc.Mid_V2, _ca.V1_Mid));
            triangleList.Add(new Triangle(midAB_midBC, midBC_midCA, midCA_midAB));
         

        }//end subdivide

        #endregion

    }//end of class Triangle


}//end namespace

