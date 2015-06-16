using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace Projectiles
{
    public class Projectile
    {
        public  Vector3 rocketPosition, previousPosition; // rocket position
        private Vector3 speed;                      // relative change in X,Y,Z
        public  Matrix  directionMatrix;            // direction transformations
        public  bool    active;                     // visibility
        private float   boundary;                   // edge of world on X and Z
        public  int     box;
        public bool canfoward = true;
        public bool canbackward = true;
        public bool canleft = true;
        public bool canright = true;
       // private float   seconds;                    // seconds since launch

        private Vector3 startPosition;              // launch position

        public Projectile()
        {
            active = false;
        }

        public void Launch(Vector3 look, Vector3 start)
        {
            rocketPosition = startPosition = start;   // start at camera
            speed    = Vector3.Normalize(look); // unitize direction
            active   = true;                    // make visible
            //seconds  = 0.0f;                    // used with gravity only
        }

        private void SetDirectionMatrix()
        {
            Vector3 Look  = rocketPosition - previousPosition;
            Look.Normalize();

            Vector3 Up    = new Vector3(0.0f, 1.0f, 0.0f); // fake Up to get
            Vector3 Right = Vector3.Cross(Up, Look);
            Right.Normalize();

            Up = Vector3.Cross(Look, Right);               // calculate Up with
            Up.Normalize();                                // correct vectors
            
            Matrix matrix  = new Matrix();                 // compute direction matrix
            matrix.Right   = Right;
            matrix.Up      = Up;
            matrix.Forward = Look;
            matrix.M44     = 1.0f;                         // W is set to 1 to enable transforms
            directionMatrix = matrix;
        }

        public void Updatebox(Vector3 tempRP)
        {
            int row = (int)tempRP.Z + 32;
            int col = (int)tempRP.X + 32;
            box = ((row - 1) * 64) + (col);
        }

        public void UpdateProjectile(GameTime gameTime, List<List<int>> boxnumlist)
        {
            Vector3 tempRP = rocketPosition;
            previousPosition = rocketPosition;    // archive last position
            tempRP        += speed        // update current position
                            * (float)gameTime.ElapsedGameTime.Milliseconds/20.0f;
            
            Updatebox(tempRP);
            int x2 = (int)rocketPosition.X, y2 = (int)rocketPosition.Z;

            foreach (List<int> squareNboxnum in boxnumlist)
            {
                if (box == squareNboxnum[0])
                {
                    switch (squareNboxnum[1])
                    {
                        case 1:
                            canfoward = false;
                            break;
                        case 2:
                            canright = false;
                            break;
                        case 4:
                            canbackward = false;
                            break;
                        case 8:
                            canleft = false;
                            break;
                    }
                }
                else if (box < squareNboxnum[0])
                    break;
            }
            if (!canfoward)
                if (tempRP.Z < (float)(y2-1))
                    active = false;
            if (!canbackward)
                if (tempRP.Z > (float)(y2))
                    active = false;
            if (!canleft)
                if (tempRP.X < (float)(x2-1))
                    active = false;
            if (!canright)
                if (tempRP.X > (float)(x2))
                    active = false;
            // deactivate if outer border exceeded on X or Z
            if (rocketPosition.Z > 36.0f || rocketPosition.X > 36.0f ||
                rocketPosition.Z < -36.0f|| rocketPosition.X < -36.0f ||
                rocketPosition.Y < 0.0f || rocketPosition.Y > 2.0f)
                    active = false;
            rocketPosition += speed        // update current position
                                * (float)gameTime.ElapsedGameTime.Milliseconds / 20.0f;
                SetDirectionMatrix();
                canfoward = true;
                canbackward = true;
                canleft = true;
                canright = true;
        }
        public void LoSUpdateProjectile(GameTime gameTime, List<List<int>> boxnumlist)
        {
            Vector3 tempRP = rocketPosition;
            previousPosition = rocketPosition;    // archive last position
            tempRP += speed;       // update current position
                            

            Updatebox(tempRP);
            int x2 = (int)rocketPosition.X, y2 = (int)rocketPosition.Z;

            foreach (List<int> squareNboxnum in boxnumlist)
            {
                if (box == squareNboxnum[0])
                {
                    switch (squareNboxnum[1])
                    {
                        case 1:
                            canfoward = false;
                            break;
                        case 2:
                            canright = false;
                            break;
                        case 4:
                            canbackward = false;
                            break;
                        case 8:
                            canleft = false;
                            break;
                    }
                }
                else if (box < squareNboxnum[0])
                    break;
            }
            if (!canfoward)
                if (tempRP.Z < (float)(y2 - 0.5))
                    active = false;
            if (!canbackward)
                if (tempRP.Z > (float)(y2-.5))
                    active = false;
            if (!canleft)
                if (tempRP.X < (float)(x2 - 0.5))
                    active = false;
            if (!canright)
                if (tempRP.X > (float)(x2-.5))
                    active = false;
            // deactivate if outer border exceeded on X or Z
            if (rocketPosition.Z > 36.0f || rocketPosition.X > 36.0f ||
                rocketPosition.Z < -36.0f || rocketPosition.X < -36.0f ||
                rocketPosition.Y < 0.0f || rocketPosition.Y > 2.0f)
                active = false;
            rocketPosition += speed;        // update current position
                                
            SetDirectionMatrix();
            canfoward = true;
            canbackward = true;
            canleft = true;
            canright = true;
        }
    }
}