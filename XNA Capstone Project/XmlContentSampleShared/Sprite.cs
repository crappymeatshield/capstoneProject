//Builds map (needs updating badly)
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XmlContentSampleShared
{
    public class Sprite
    {
        int floorsquare;
        int boxnum;

        int rows=59, cols=64;
        int height = 0;
        int width = 0;
        static int i=0;
        static float[] x = new float[7240];
        static float[] y = new float[7240];
        float wallradius = 0.2f;

        //public int Rows
        //{
        //    get { return rows; }
        //    set { rows = value; }
        //}

        //public int Cols
        //{
        //    get { return cols; }
        //    set { cols = value; }
        //}

        public int Floorsquare
        {
            get { return floorsquare; }
            set { floorsquare = value; }
        }

        public int Boxnum
        {
            get { return boxnum; }
            set { boxnum = value; }
        }
        

        
        public VertexPositionColorTexture[] initializewalls(VertexPositionColorTexture[] surfaceVertex)
        {
            Vector2 uv;
            Vector3 pos;
            Color color = Color.White;
            
             uv.X = 1.0f;
             uv.Y = 1.0f;
             pos.X = -1.0f;
             pos.Y = 2.0f;
             pos.Z = 0.0f;
             surfaceVertex[0] = new VertexPositionColorTexture(pos, color, uv);

             uv.X = 1.0f;
             uv.Y = 0.0f;
             pos.X = -1.0f;
             pos.Y = 0.0f;
             pos.Z = 0.0f;
             surfaceVertex[1] = new VertexPositionColorTexture(pos, color, uv);

             uv.X = 0.0f;
             uv.Y = 1.0f;
             pos.X = 0.0f;
             pos.Y = 2.0f;
             pos.Z = 0.0f;
             surfaceVertex[2] = new VertexPositionColorTexture(pos, color, uv);

             uv.X = 0.0f;
             uv.Y = 0.0f;
             pos.X = 0.0f;
             pos.Y = 0.0f;
             pos.Z = 0.0f;
             surfaceVertex[3] = new VertexPositionColorTexture(pos, color, uv);

             return surfaceVertex;
        }
        public Matrix draw(Matrix matrix, List<List<int>> boxlist)
        {
            int bnum = Boxnum;
            Matrix translation, rotationY;
            translation = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);
            rotationY = Matrix.CreateRotationY(0.0f);
            int fnum = Floorsquare;
            List<int> floorbox = new List<int>();
            floorbox.Add(floorsquare);
            floorbox.Add(boxnum);
            boxlist.Add(floorbox);

            height = (fnum / cols)+1;

#region old code
            /*if (fnum <= 64)
                height = 1;
            else if (fnum <= 128)
                height = 2;
            else if (fnum <= 192)
                height = 3;
            else if (fnum <= 256)
                height = 4;
            else if (fnum <= 320)
                height = 5;
            else if (fnum <= 384)
                height = 6;
            else if (fnum <= 448)
                height = 7;
            else if (fnum <= 512)
                height = 8;
            else if (fnum <= 576)
                height = 9;
            else if (fnum <= 640)
                height = 10;
            else if (fnum <= 704)
                height = 11;
            else if (fnum <= 768)
                height = 12;
            else if (fnum <= 832)
                height = 13;
            else if (fnum <= 896)
                height = 14;
            else if (fnum <= 960)
                height = 15;
            else if (fnum <= 1024)
                height = 16;
            else if (fnum <= 1088)
                height = 17;
            else if (fnum <= 1152)
                height = 18;
            else if (fnum <= 1216)
                height = 19;
            else if (fnum <= 1280)
                height = 20;
            else if (fnum <= 1344)
                height = 21;
            else if (fnum <= 1408)
                height = 22;
            else if (fnum <= 1472)
                height = 23;
            else if (fnum <= 1536)
                height = 24;
            else if (fnum <= 1600)
                height = 25;
            else if (fnum <= 1664)
                height = 26;
            else if (fnum <= 1728)
                height = 27;
            else if (fnum <= 1792)
                height = 28;
            else if (fnum <= 1856)
                height = 29;
            else if (fnum <= 1920)
                height = 30;
            else if (fnum <= 1984)
                height = 31;
            else if (fnum <= 2048)
                height = 32;
            else if (fnum <= 2112)
                height = 33;
            else if (fnum <= 2176)
                height = 34;
            else if (fnum <= 2240)
                height = 35;
            else if (fnum <= 2304)
                height = 36;
            else if (fnum <= 2368)
                height = 37;
            else if (fnum <= 2432)
                height = 38;
            else if (fnum <= 2496)
                height = 39;
            else if (fnum <= 2560)
                height = 40;
            else if (fnum <= 2624)
                height = 41;
            else if (fnum <= 2688)
                height = 42;
            else if (fnum <= 2752)
                height = 43;
            else if (fnum <= 2816)
                height = 44;
            else if (fnum <= 2880)
                height = 45;
            else if (fnum <= 2944)
                height = 46;
            else if (fnum <= 3008)
                height = 47;
            else if (fnum <= 3072)
                height = 48;
            else if (fnum <= 3136)
                height = 49;
            else if (fnum <= 3200)
                height = 50;
            else if (fnum <= 3264)
                height = 51;
            else if (fnum <= 3328)
                height = 52;
            else if (fnum <= 3392)
                height = 53;
            else if (fnum <= 3456)
                height = 54;
            else if (fnum <= 3520)
                height = 55;
            else if (fnum <= 3584)
                height = 56;
            else if (fnum <= 3648)
                height = 57;
            else if (fnum <= 3712)
                height = 58;
            else if (fnum <= 3776)
                height = 59;*/
#endregion
            width = (fnum - (64 * (height - 1)));
            width = width - 33;
            height = height - 33;

            switch (bnum)
            {
                case 1:
                    translation = Matrix.CreateTranslation(width + 1.0f, 0.0f, height);
                    x[i] = width + 0.5f;
                    y[i] = height;
                    i++;
                    x[i] = width;
                    y[i] = height;
                    i++;
                    break;
                case 2:
                    rotationY = Matrix.CreateRotationY(MathHelper.Pi / 2.0f);
                    translation = Matrix.CreateTranslation(width + 1.0f, 0.0f, height);
                    x[i] = width + 1.0f;
                    y[i] = height + 0.5f;
                    i++;
                    x[i] = width + 1.0f;
                    y[i] = height;
                    i++;
                    break;
                case 4:
                    translation = Matrix.CreateTranslation(width + 1.0f, 0.0f, height + 1.0f);
                    x[i] = width + 0.5f;
                    y[i] = height + 1.0f;
                    i++;
                    x[i] = width + 1.0f;
                    y[i] = height + 1.0f;
                    i++;
                    break;
                case 8:
                    rotationY = Matrix.CreateRotationY(MathHelper.Pi / 2.0f);
                    translation = Matrix.CreateTranslation(width, 0.0f, height);
                    x[i] = width;
                    y[i] = height + 0.5f;
                    i++;
                    x[i] = width;
                    y[i] = height + 1.0f;
                    i++;
                    break;
                case 0:
                    x[i] = -50.0f;
                    y[i] = -50.0f;
                    i++;
                    break;
            }
            matrix = rotationY * translation;
            if (i >= 7238)
            {
                i = 0;
            }
            return matrix;
            
        }
        // distance formula:  d= ((x2-x1)^2+(y2-y1)^2)^(1/2)
        public bool collision(float x1, float y1, float radius)
        {
            float d;
            float totradius;

            for (int q=0; q<=7238; q++)
            {
                d = (float)Math.Sqrt(((x[q] - x1)*(x[q] - x1)) + ((y[q] -y1)*(y[q] -y1)));
                totradius = radius + wallradius;
            if (d<totradius)
                return true;
            }
            return false;
        }
            
        public class SpriteContentReader : ContentTypeReader<Sprite>
        {
            protected override Sprite Read(ContentReader input, Sprite existingInstance)
            {
                Sprite sprite = new Sprite();

                /*<Rows>59</Rows>
                <Cols>64</Cols>*/
                //sprite.Rows = input.ReadInt32();
                //sprite.Cols = input.ReadInt32();
                sprite.Floorsquare = input.ReadInt32();
                sprite.Boxnum = input.ReadInt32();

                return sprite;
            }

        }
    }
}
