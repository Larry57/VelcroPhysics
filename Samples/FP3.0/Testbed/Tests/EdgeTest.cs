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

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.TestBed.Tests
{
    public class EdgeTest : Test
    {
        private Fixture _circleFixture;

        public EdgeTest()
        {
            {
                Body ground = BodyFactory.CreateBody(World);

                Vector2 v1 = new Vector2(-10.0f, 0.0f);
                Vector2 v2 = new Vector2(-7.0f, -1.0f);
                Vector2 v3 = new Vector2(-4.0f, 0.0f);
                Vector2 v4 = new Vector2(0.0f, 0.0f);
                Vector2 v5 = new Vector2(4.0f, 0.0f);
                Vector2 v6 = new Vector2(7.0f, 1.0f);
                Vector2 v7 = new Vector2(10.0f, 0.0f);

                EdgeShape shape = new EdgeShape();

                shape.Set(v1, v2);
                //shape._index1 = 0;
                //shape._index2 = 1;
                shape.HasVertex3 = true;
                shape.Vertex3 = v3;
                ground.CreateFixture(shape);

                shape.Set(v2, v3);
                //shape._index1 = 1;
                //shape._index2 = 2;
                shape.HasVertex0 = true;
                shape.HasVertex3 = true;
                shape.Vertex0 = v1;
                shape.Vertex3 = v4;
                ground.CreateFixture(shape);

                shape.Set(v3, v4);
                //shape._index1 = 2;
                //shape._index2 = 3;
                shape.HasVertex0 = true;
                shape.HasVertex3 = true;
                shape.Vertex0 = v2;
                shape.Vertex3 = v5;
                ground.CreateFixture(shape);

                shape.Set(v4, v5);
                //shape._index1 = 3;
                //shape._index2 = 4;
                shape.HasVertex0 = true;
                shape.HasVertex3 = true;
                shape.Vertex0 = v3;
                shape.Vertex3 = v6;
                ground.CreateFixture(shape);

                shape.Set(v5, v6);
                //shape._index1 = 4;
                //shape._index2 = 5;
                shape.HasVertex0 = true;
                shape.HasVertex3 = true;
                shape.Vertex0 = v4;
                shape.Vertex3 = v7;
                ground.CreateFixture(shape);

                shape.Set(v6, v7);
                //shape._index1 = 5;
                //shape._index2 = 6;
                shape.HasVertex0 = true;
                shape.Vertex0 = v5;
                ground.CreateFixture(shape);
            }

            {
                Body body = BodyFactory.CreateBody(World, new Vector2(-0.5f, 0.5f));
                body.BodyType = BodyType.Dynamic;
                body.SleepingAllowed = false;

                CircleShape shape = new CircleShape(0.5f);
                _circleFixture = body.CreateFixture(shape, 1);
            }

            {
                Body body = BodyFactory.CreateBody(World, new Vector2(0.5f, 0.5f));
                body.BodyType = BodyType.Dynamic;
                body.SleepingAllowed = false;

                PolygonShape shape = new PolygonShape();
                shape.SetAsBox(0.5f, 0.5f);

                body.CreateFixture(shape, 1);
            }
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            DebugView.DrawString(50, TextLine, "Rotation: " + _circleFixture.Body.Rotation);
            TextLine += 15;
            DebugView.DrawString(50, TextLine, "Revolutions: " + _circleFixture.Body.Revolutions);

            base.Update(settings, gameTime);
        }

        internal static Test Create()
        {
            return new EdgeTest();
        }
    }
}