using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FoundationCodeForFractalMountains
{
    public class Edge
    {
        #region Fields

        //Fields
        private Vertex _v1 = new Vertex(), _v2 = new Vertex(); //Endpoints of this edge
        private Vertex _midpoint = new Vertex(); //Midpoint of this edge

        private Edge _reversed; //This edge "reversed" (i.e. this edge is v1_v2, the reversed edge is v2_v1)

        private Edge _v1_mid, _mid_v2; // Edges resulting from subdivision of this edge 
        // (i.e. v1-------------v2  ->   v1-----mid  &  mid-----v2)

        #endregion

        #region Properties

        //Properties

        //The property 'V1' exposes the field 'v1'.
        public Vertex V1
        {
            get
            {
                return _v1;
            }

            set
            {
                _v1 = value;
            }
        }// End property V1

        //The property 'V2' exposes the field 'v2'.
        public Vertex V2
        {
            get
            {
                return _v2;
            }

            set
            {
                _v2 = value;
            }
        }// End property V2

        //The property 'V1_Mid' exposes the field 'v1_mid.'          
        public Edge V1_Mid
        {
            get
            {
                return _v1_mid;
            }
            set
            {
                _v1_mid = value;
            }
        }//end property V1_Mid


        //The property 'V1_Mid' exposes the field 'Mid_V2.'          
        public Edge Mid_V2
        {
            get
            {
                return _mid_v2;
            }
            set
            {
                _mid_v2 = value;
            }
        }//end property Mid_V2

        #endregion

        #region Constructors

        /**
         * Constructor Method
         */

        //Set the vertices of this Edge
        public Edge(Vertex v1, Vertex v2)
        {
            _v1 = v1;
            _v2 = v2;

        }//end constructor

        public Edge(Edge AB)
        {
            _v1 = AB._v1;
            _v2 = AB._v2;

        }//end constructor


        #endregion

        #region Instance Methods
        /**
         * Other methods, all of which are instance methods.
         */


        // Return the "adjusted" midpoint of this edge. 
        // (i.e. edge with midpoint raised or lowered by a random amount -> see 'adjustHeight' method in 'Vertex' class)
        // Also set a reference to the common midpoint.
        // This is what keeps subdivided edges together.
        public Vertex subdivideEdge()
        {
            if ((_reversed != null) && (_reversed._v1_mid != null)) //Midpoint of "shared" edge already computed.
            {
                _v1_mid = _reversed._mid_v2.reverse();
                _mid_v2 = _reversed._v1_mid.reverse();
                _midpoint = _reversed._midpoint;
            }
            else
            {
                _midpoint = new Vertex((_v1.X + _v2.X) / 2, (_v1.Y + _v2.Y) / 2, (_v1.Z + _v2.Z) / 2);

                _midpoint.adjustHeight(_v1, _v2);

                _v1_mid = new Edge(_v1, _midpoint);
                _mid_v2 = new Edge(_midpoint, _v2);
            }

            return _midpoint;

        }//end subdivide

        /**
         * Return another Edge with vertices in the reverse order.
	     * (i.e. edge v2_v1 is returned)
	     * 
         * After this method is executed this edge and its reverse will have the following properties:
         * this edge-> v1_v2, thisEdgeReversed-> v2_v1
         * reverse  -> v2_v1, thisEdgeReversed-> v1_v2
	     */
        public Edge reverse()
        {
            Edge e = new Edge(_v2, _v1);

            _reversed = e;
            e._reversed = this;

            return e;
        }//end reverse

        #endregion

    }//end of class 'Edge'
}
