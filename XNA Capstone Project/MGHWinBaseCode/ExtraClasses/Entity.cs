using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Projectiles;
using MGHGame;

namespace enimies
{
    public class Entity
    {
        private const float speed = 0.075f;
        public Vector3 Position = Vector3.Zero;
        public bool alive = false;
        public bool normMove = true;
        public bool cancharge=true;
        public int chargecooldown=0;
        public int waittime=0;
        public bool enemycanfire=true;

        public Entity(Vector3 position)
        {
            alive = true;
            Position = position;
        }

        public void die()
        {
            alive = false;
        }

        public void move(string direction, int min, int max)
        {
            Vector3 tempPosition = Position;
            switch (direction)
            {
                case "X":
                    if (normMove)
                    {
                        if (tempPosition.X + speed >= max)
                            normMove = false;
                        else
                            Position.X += speed;
                    }
                    else
                    {
                        if (tempPosition.X - speed <= min)
                            normMove = true;
                        else
                            Position.X -= speed;
                    }
                    break;
                case "Z":
                    if (normMove)
                    {
                        if (tempPosition.Z + speed >= max)
                            normMove = false;
                        else
                            Position.Z += speed;
                    }
                    else
                    {
                        if (tempPosition.Z - speed <= min)
                            normMove = true;
                        else
                            Position.Z -= speed;
                    }
                    break;
            }
        }

        
        #region oldcode
        // Matrix RotationMatrix = Matrix.Identity;

       // const int NUM_ROCKETS = 1000;
       // private Projectile[] rocket = new Projectile[NUM_ROCKETS];
       // Model rocketModel; Matrix[] rocketMatrix;
       // const int TOTAL_SPHERE_GROUPS = 2;
       // Sphere[] sphereGroup = new Sphere[TOTAL_SPHERE_GROUPS];
       // const bool SHOW = true; // set to false during release
       // public enum Group
       // {
       //     mini, rockets
       // }

       // public Entity(ContentManager Content, Vector3 inPosition, Vector3 inVelocity, string ModelPath, string texturePath)
       // {
       //     this.Position = inPosition;
       //     this.Velocity = inVelocity;
       //     shot = false;

       //     Model = Content.Load<Model>(ModelPath);
       //     texture = Content.Load<Texture2D>(texturePath);

       //     //ExtractBoundingSphere(Model, Color.Blue, (int)Group.mini);
       // }

       // public void Move(GameTime gameTime, float xAmount, float zAmount)
       // {
       //     if (!shot)
       //     {
       //         this.Position.X += xAmount * gameTime.ElapsedGameTime.Milliseconds;
       //         this.Position.Z += zAmount * gameTime.ElapsedGameTime.Milliseconds;
       //     }
       // }

       // public bool Shot
       // {
       //     get
       //     {
       //         return shot;
       //     }
       //     set { 
       //         shot = value; 
       //     }
       // }
       // protected bool shot = false;

       //public Matrix  worldMatrix()
       // {
       //     Matrix scale, translate;
       //     Matrix world = new Matrix();
       //     scale = Matrix.CreateScale(0.03f, 0.03f, 0.03f);
       //     translate = Matrix.CreateTranslation(Position);

       //     return world = scale * DirectMatrix() * translate;
       // }

       // public void Draw(ref Matrix ViewMatrix, ref Matrix ProjectionMatrix)
       // {
       //         foreach (ModelMesh mesh in this.Model.Meshes)
       //         {
       //             foreach (BasicEffect effect in mesh.Effects)
       //             {
       //                 effect.World = worldMatrix();
       //                 effect.View = ViewMatrix;
       //                 effect.Projection = ProjectionMatrix;
       //                 effect.TextureEnabled = true;
       //                 effect.Texture = texture;
       //                 effect.EnableDefaultLighting();
       //                 effect.PreferPerPixelLighting = true;

       //                 // Set the fog to match the black background color
       //                 effect.FogEnabled = true;
       //                 effect.FogColor = Vector3.Zero;
       //                 effect.FogStart = 1000;
       //                 effect.FogEnd = 3200;
       //             }
       //             mesh.Draw();
       //         }
       // }

       // public void Update(GameTime gameTime, float Boundary)
       // {
       //         Move(gameTime, Velocity.X, Velocity.Z);
       //         CheckForWallCollision((Boundary - 1.0f) * 0.5f, (Boundary - 1.0f) * 0.5f);
       //         //for (int i = 0; i < NUM_ROCKETS; i++)
       //         //    if (rocket[i].active)
       //         //    {
       //         //        if (ShotCollsionMini((int)Group.rockets, (int)Group.mini))
       //         //        {
       //         //            //health -= 1.5f;
       //         //            // mCurrentHealth -= 1;
       //         //            //contact = true;
       //         //            thisEnemy.Shot = true;

       //         //        }
       //         //    }
       // }
       // Matrix DirectMatrix()
       // {
       //     Vector3 L = Velocity;
       //     L.Normalize();
       //     Vector3 U = new Vector3(0.0f, 0.1f, 0.0f);
       //     U.Normalize();
       //     Vector3 R = Vector3.Cross(U, L);
       //     R.Normalize();

       //     Matrix M = new Matrix();
       //     M.M11 = -L.X; M.M12 = -L.Y; M.M13 = -L.Z; M.M14 = 0.0f;
       //     M.M21 = U.X; M.M22 = U.Y; M.M23 = U.Z; M.M24 = 0.0f;
       //     M.M31 = -R.X; M.M32 = -R.Y; M.M33 = -R.Z; M.M34 = 0.0f;
       //     M.M41 = 0.0f; M.M42 = 0.0f; M.M43 = 0.0f; M.M44 = 1.0f;
       //     return M;
       // }

       // // Checks if entity has passed or hit a wall, and if so, sets their position properly and then reflects them
       // // off of the wall.
       //  public void CheckForWallCollision(float width, float height)
       //  {
       //      if (Position.X >= width)
       //      {
       //          Velocity.X *= -1;
       //          Position.X = width;
       //      }
       //      else if (Position.X <= -width)
       //      {
       //          Velocity.X *= -1;
       //          Position.X = -width;
       //      }

       //      if (Position.Z >= height)
       //      {
       //          Velocity.Z *= -1;
       //          Position.Z = height;
       //      }
       //      else if (Position.Z <= -height)
       //      {
       //          Velocity.Z *= -1;
       //          Position.Z = -height;
       //      }
       //  }
       //  //bool Collision(BoundingSphere A, BoundingSphere B)
       //  //{
       //  //    if (A.Intersects(B))
       //  //        return true;
       //  //    return false;
       //  //}
       //  //void ExtractBoundingSphere(Model tempModel, Color color, int groupNum)
       //  //{
       //  //    // set up model temporarily
       //  //    Matrix[] tempMatrix = new Matrix[tempModel.Bones.Count];
       //  //    tempModel.CopyAbsoluteBoneTransformsTo(tempMatrix);

       //  //    // generate new sphere group
       //  //    BoundingSphere sphere = new BoundingSphere();
       //  //    sphereGroup[groupNum] = new Sphere(SHOW);

       //  //    // store radius, position, and color information for each sphere
       //  //    foreach (ModelMesh mesh in tempModel.Meshes)
       //  //    {
       //  //        sphere = mesh.BoundingSphere;
       //  //        Vector3 newCenter = sphere.Center;
       //  //        Matrix transformMatrix = Matrix.CreateScale(1.0f, 1.0f, 1.0f); //ScaleModel();
       //  //        sphereGroup[groupNum].AddSphere(sphere.Radius, sphere.Center,
       //  //        color);
       //  //    }
       //  //}

       //  //public bool ShotCollsionMini(int rocketGroup, int miniGroup)
       //  //{
       //  //    // check selected car and selected wall spheres for collisions
       //  //    for (int i = 0; i < sphereGroup[miniGroup].sphere.Count; i++)
       //  //    {
       //  //        for (int j = 0; j < sphereGroup[rocketGroup].sphere.Count; j++)
       //  //        {
       //  //            // 3: build cumulative matrix using I.S.R.O.T. sequence

       //  //            Matrix transform = worldMatrix();


       //  //            Matrix world = bulletworldMatrix();

       //  //            BoundingSphere rocketSphere = sphereGroup[rocketGroup].sphere[j].boundingSphere.Transform
       //  //                (world);

       //  //            // rocketSphere.Radius = sphereGroup[rocketGroup].sphere[j].boundingSphere.Radius;

       //  //            // generate temp bounding sphere with transformed sphere
       //  //            BoundingSphere tempSphere = sphereGroup[miniGroup].sphere[i].boundingSphere.Transform
       //  //                                       (transform);
       //  //            tempSphere.Radius = sphereGroup[miniGroup].sphere[i].boundingSphere.Radius;

       //  //            if (Collision(rocketSphere, tempSphere))
       //  //            {
       //  //                return true;
       //  //            }
       //  //        }
       //  //    }
        //  //}
        #endregion
    }
}
