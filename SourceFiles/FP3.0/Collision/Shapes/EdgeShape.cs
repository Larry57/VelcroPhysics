/*
* Farseer Physics Engine based on Box2D.XNA port:
* Copyright (c) 2010 Ian Qvist
* 
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes
{
    /// <summary>
    /// A line segment (edge) Shape. These can be connected in chains or loops
    /// to other edge Shapes. The connectivity information is used to ensure
    /// correct contact normals.
    /// </summary>
    public class EdgeShape : Shape
    {
        public bool HasVertex0, HasVertex3;

        /// <summary>
        /// Optional adjacent vertices. These are used for smooth collision.
        /// </summary>
        public Vector2 Vertex0;

        /// <summary>
        /// These are the edge vertices
        /// </summary>
        public Vector2 Vertex1, Vertex2;

        /// <summary>
        /// Optional adjacent vertices. These are used for smooth collision.
        /// </summary>
        public Vector2 Vertex3;

        public EdgeShape()
        {
            ShapeType = ShapeType.Edge;
            Radius = Settings.PolygonRadius;
            HasVertex0 = false;
            HasVertex3 = false;
        }

        /// <summary>
        /// Set this as an isolated edge.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void Set(Vector2 start, Vector2 end)
        {
            Vertex1 = start;
            Vertex2 = end;
            HasVertex0 = false;
            HasVertex3 = false;
        }

        public override Shape Clone()
        {
            var edge = new EdgeShape();
            edge.HasVertex0 = HasVertex0;
            edge.HasVertex3 = HasVertex3;
            edge.Radius = Radius;
            edge.Vertex0 = Vertex0;
            edge.Vertex1 = Vertex1;
            edge.Vertex2 = Vertex2;
            edge.Vertex3 = Vertex3;
            return edge;
        }

        public override int GetChildCount()
        {
            return 1;
        }

        /// <summary>
        /// Test a point for containment in this shape. This only works for convex shapes.
        /// </summary>
        /// <param name="transform">The shape world transform.</param>
        /// <param name="point">a point in world coordinates.</param>
        /// <returns>True if the point is inside the shape</returns>
        public override bool TestPoint(ref Transform transform, ref Vector2 point)
        {
            return false;
        }

        /// <summary>
        /// Cast a ray against a child shape.
        /// </summary>
        /// <param name="output">The ray-cast results.</param>
        /// <param name="input">The ray-cast input parameters.</param>
        /// <param name="transform">The transform to be applied to the shape.</param>
        /// <param name="childIndex">The child shape index.</param>
        /// <returns>True if the ray-cast hits the shape</returns>
        /// Implement Shape.
        public override bool RayCast(out RayCastOutput output, ref RayCastInput input,
                                     ref Transform transform, int childIndex)
        {
            // p = p1 + t * d
            // v = v1 + s * e
            // p1 + t * d = v1 + s * e
            // s * e - t * d = p1 - v1

            output = new RayCastOutput();

            // Put the ray into the edge's frame of reference.
            Vector2 p1 = MathUtils.MultiplyT(ref transform.R, input.Point1 - transform.Position);
            Vector2 p2 = MathUtils.MultiplyT(ref transform.R, input.Point2 - transform.Position);
            Vector2 d = p2 - p1;

            Vector2 v1 = Vertex1;
            Vector2 v2 = Vertex2;
            Vector2 e = v2 - v1;
            Vector2 normal = new Vector2(e.Y, -e.X);
            normal.Normalize();

            // q = p1 + t * d
            // dot(normal, q - v1) = 0
            // dot(normal, p1 - v1) + t * dot(normal, d) = 0
            float numerator = Vector2.Dot(normal, v1 - p1);
            float denominator = Vector2.Dot(normal, d);

            if (denominator == 0.0f)
            {
                return false;
            }

            float t = numerator / denominator;
            if (t < 0.0f || 1.0f < t)
            {
                return false;
            }

            Vector2 q = p1 + t * d;

            // q = v1 + s * r
            // s = dot(q - v1, r) / dot(r, r)
            Vector2 r = v2 - v1;
            float rr = Vector2.Dot(r, r);
            if (rr == 0.0f)
            {
                return false;
            }

            float s = Vector2.Dot(q - v1, r) / rr;
            if (s < 0.0f || 1.0f < s)
            {
                return false;
            }

            output.Fraction = t;
            if (numerator > 0.0f)
            {
                output.Normal = -normal;
            }
            else
            {
                output.Normal = normal;
            }
            return true;
        }

        /// <summary>
        /// Given a transform, compute the associated axis aligned bounding box for a child shape.
        /// </summary>
        /// <param name="aabb">The aabb results.</param>
        /// <param name="transform">The world transform of the shape.</param>
        /// <param name="childIndex">The child shape index.</param>
        public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
        {
            aabb = new AABB();
            Vector2 v1 = MathUtils.Multiply(ref transform, Vertex1);
            Vector2 v2 = MathUtils.Multiply(ref transform, Vertex2);

            Vector2 lower = Vector2.Min(v1, v2);
            Vector2 upper = Vector2.Max(v1, v2);

            Vector2 r = new Vector2(Radius, Radius);
            aabb.LowerBound = lower - r;
            aabb.UpperBound = upper + r;
        }

        /// <summary>
        /// Compute the mass properties of this shape using its dimensions and density.
        /// The inertia tensor is computed about the local origin, not the centroid.
        /// </summary>
        /// <param name="massData">Returns the mass data for this shape.</param>
        /// <param name="density">The density in kilograms per meter squared.</param>
        public override void ComputeMass(out MassData massData, float density)
        {
            massData = new MassData();
            massData.Mass = 0.0f;
            massData.Center = 0.5f * (Vertex1 + Vertex2);
            massData.Inertia = 0.0f;
        }
    }
}