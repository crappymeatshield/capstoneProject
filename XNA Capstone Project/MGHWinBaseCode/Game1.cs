//------------------------------------------------------------
// distance formula:  d= ((x2-x1)^2+(y2-y1)^2)^(1/2)
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using CameraViewer;
using Projectiles;
using XmlContentSampleShared;

namespace MGHGame
{
    /// <summary>
    /// This is the driving class for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region variables
        //------------------------------------------------------------
        // C L A S S   L E V E L   D E C L A R A T I O N S
        //------------------------------------------------------------
        // constant definitions
        private const float BOUNDARY = 35.0f;
        const float BASE_HEIGHT = 0.35f;

        public static int level = 1;

        // accesses drawing methods and properties
        GraphicsDeviceManager graphics;

        // handle mouse on the PC
#if !XBOX
        MouseState mouse, mouseCurrent, mousePrevious;
#endif
        GamePadState gpCurrent, gpPrevious;
        GamePadState gamepad, gamepadPrevious;

        // for loading and drawing 2D images on the game window
        SpriteBatch spriteBatch;
        SpriteBatch spriteBatch2;
        SpriteFont spriteFont;

        //wall variables
        BasicEffect basicEffect;
        List<Sprite> sprites = new List<Sprite>();
        List<List<int>> boxnumlist = new List<List<int>>();

        const float FOV = MathHelper.PiOver4;
        const float FOVPlusPadding = FOV * 1.1f;

        const float ViewRange = 3200.0f;
        const float ViewRangeSquared = ViewRange * ViewRange;

        //number of and array of rockets
        int num_rockets = 0;
        const int MAX_ROCKETS = 5;
        //private Projectile[] rocket = new Projectile[NUM_ROCKETS];
        List<Projectile> bullets = new List<Projectile>();

        Matrix projectionMatrix2 = Matrix.Identity;
        Matrix viewMatrix2 = Matrix.Identity;
                
        //3d models
        Model  playerGunModel;
        Texture2D playerGunTexture;
        Matrix[] playerGunMatrix;

        Model rocketModel; 
        Matrix[] rocketMatrix;

        Vector3 timemachinepostion=new Vector3(8.5f, 0.0f, -4.5f);
        bool touchtimemachine = false;
        Model timemachine;
        Matrix[] timeMatrix;

        struct shootingenemies
        {
            public Vector3 badguyp;
            public enimies.Entity badguys;
            //public Projectile losBullet;
            public Projectile enemyBullet;
            public Matrix[] shootingmatrix;
            public bool los;
            public float spotlightAngle;
            public string direction;
            public int min;
            public int max;
        }

        struct chargingenemies
        {
            public Vector3 badguyp;
            public enimies.Entity badguys;
            //public Projectile losBullet;
            public bool los;
            public Matrix[] chargingmatrix;
            public float spotlightAngle;
        }

        struct bothenemies
        {
            public Vector3 badguyp;
            public enimies.Entity badguys;
            public Projectile enemyBullet;
            public bool los;
            public float spotlightAngle;
            public int minX;
            public int maxX;
            public int minZ;
            public int maxZ;
            public Matrix[] bossmatrix;
            public int health;
        }

        public struct nonmovingchar
        {
            public Vector3 badguyp;
            public enimies.Entity badguys;
        }

        public nonmovingchar hitler;
        Model hitlerModel;
        Matrix[] hitlerMatrix;
        public nonmovingchar scientist;
        Model scientistModel;
        Matrix[] scientistMatrix;

        chargingenemies[] charge = new chargingenemies[5];
        shootingenemies[] shoot = new shootingenemies[5];
        bothenemies boss = new bothenemies();



        //static Vector3 badguyp=new Vector3(0.0f, 0.6f, -12.0f);
        Model human;
        Model soilder;
        Model bigboss;
        Model powercell;
        Matrix[] powerMatrix;
        Vector3 powerposition;
        //Matrix[] humanMatrix;
        //enimies.Entity badguys = new enimies.Entity(badguyp);
        //Projectile losBullet;
        //Projectile enemyBullet=new Projectile();
        //int waittime=0;
        //bool enemycanfire=true;
        //bool los;
        //bool cancharge = true;
        //int chargecooldown = 0;

        int PlayerHealth = 1000;
        // load and access PositionColor.fx shader
        private Effect positionColorEffect;    // shader object
        private EffectParameter positionColorEffectWVP; // to set display matrix for window

        // load and access Texture.fx shader
        private Effect textureEffect;          // shader object                 
        private EffectParameter textureEffectWVP;       // cumulative matrix w*v*p 
        private EffectParameter textureEffectImage;     // texture parameter

        // camera 
        private Camera cam = new Camera();

        // vertex types and buffers
        private VertexDeclaration positionColor;
        private VertexDeclaration positionColorTexture;

        // ground and wall vertices and texture
        VertexPositionColorTexture[]
            groundVertices = new VertexPositionColorTexture[4];
        private Texture2D grassTexture;
        VertexPositionColorTexture[]
            wallVertices = new VertexPositionColorTexture[4];
        private Texture2D walltexture;
        private Texture2D ceilingtexture;

        //sound variables
        private static SoundBank soundBank2;
        private static AudioEngine soundEngine2;

        Vector2 fontPos;

        #endregion 
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #region initialize
        /// <summary>
        /// This method is called when the program begins to set game application
        /// properties such as status bar title and draw mode.  It initializes the  
        /// camera viewer projection, vertex types, and shaders.
        /// </summary>
        private void InitializeBaseCode()
        {
            // set status bar in PC Window (there is none for the Xbox 360)
            Window.Title = "Commuzi Advanced 3D";

            fontPos = new Vector2(1.0f, 1.0f);

            // see both sides of objects drawn
            graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            // set camera matrix
            cam.SetProjection(Window.ClientBounds.Width,
                              Window.ClientBounds.Height);

            // initialize vertex types
            positionColor = new VertexDeclaration(graphics.GraphicsDevice,
                                          VertexPositionColor.VertexElements);
            positionColorTexture = new VertexDeclaration(graphics.GraphicsDevice,
                                          VertexPositionColorTexture.VertexElements);

            // load PositionColor.fx and set global params
            positionColorEffect = Content.Load<Effect>("Shaders\\PositionColor");
            positionColorEffectWVP = positionColorEffect.Parameters["wvpMatrix"];

            // load Texture.fx and set global params
            textureEffect = Content.Load<Effect>("Shaders\\Texture");
            textureEffectWVP = textureEffect.Parameters["wvpMatrix"];
            textureEffectImage = textureEffect.Parameters["textureImage"];
            powerposition = new Vector3(40.0f, 40.0f, 40.0f);
            //sprite = Content.Load<Sprite>("walllist");
            switch (level)
            {
                case 1:
                    sprites = Content.Load<List<Sprite>>("walllist");
                    cam.position = new Vector3(-27.5f, 0.9f, -25.0f);
                    cam.view = new Vector3(-27.5f, 0.9f, -0.5f);
                    timemachinepostion = new Vector3(8.5f, 0.0f, -4.5f);
                    charge[0].badguyp = new Vector3(-15.0f, 0.6f, 5.0f);
                    charge[0].badguys = new enimies.Entity(charge[0].badguyp);
                    charge[0].spotlightAngle = 0.0f;
                    charge[1].badguyp = new Vector3(-15.0f, 0.6f, 8.0f);
                    charge[1].badguys = new enimies.Entity(charge[1].badguyp);
                    charge[1].spotlightAngle = 0.0f;
                    charge[2].badguyp = new Vector3(-15.0f, 0.6f, 11.0f);
                    charge[2].badguys = new enimies.Entity(charge[2].badguyp);
                    charge[2].spotlightAngle = 0.0f;
                    charge[3].badguyp = new Vector3(-15.0f, 0.6f, 14.0f);
                    charge[3].badguys = new enimies.Entity(charge[3].badguyp);
                    charge[3].spotlightAngle = 0.0f;
                    charge[4].badguyp = new Vector3(-15.0f, 0.6f, 17.0f);
                    charge[4].badguys = new enimies.Entity(charge[4].badguyp);
                    charge[4].spotlightAngle = 0.0f;
                    shoot[0].badguyp = new Vector3(7.0f, 0.6f, -12.0f);
                    shoot[0].badguys = new enimies.Entity(shoot[0].badguyp);
                    shoot[0].direction = "X";
                    shoot[0].max = 8;
                    shoot[0].min = -12;
                    shoot[0].spotlightAngle = 0.0f;
                    shoot[0].enemyBullet = new Projectile();
                    shoot[1].badguyp = new Vector3(-17.0f, 0.6f, -15.5f);
                    shoot[1].badguys = new enimies.Entity(shoot[1].badguyp);
                    shoot[1].direction = "X";
                    shoot[1].max = 8;
                    shoot[1].min = -20;
                    shoot[1].spotlightAngle = 0.0f;
                    shoot[1].enemyBullet = new Projectile();
                    shoot[2].badguyp = new Vector3(26.5f, 0.6f, 0.0f);
                    shoot[2].badguys = new enimies.Entity(shoot[2].badguyp);
                    shoot[2].direction = "Z";
                    shoot[2].max = 3;
                    shoot[2].min = -21;
                    shoot[2].spotlightAngle = 0.0f;
                    shoot[2].enemyBullet = new Projectile();
                    shoot[3].badguyp = new Vector3(18.5f, 0.6f, 14.5f);
                    shoot[3].badguys = new enimies.Entity(shoot[3].badguyp);
                    shoot[3].direction = "Z";
                    shoot[3].max = 23;
                    shoot[3].min = 3;
                    shoot[3].spotlightAngle = 0.0f;
                    shoot[3].enemyBullet = new Projectile();
                    shoot[4].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    shoot[4].badguys = new enimies.Entity(shoot[4].badguyp);
                    shoot[4].badguys.die();
                    shoot[4].enemyBullet = new Projectile();
                    shoot[4].enemyBullet.active = false;
                    hitler.badguyp = new Vector3(17.0f, 0.6f, 13.0f);
                    hitler.badguys = new enimies.Entity(hitler.badguyp);
                    scientist.badguyp = new Vector3(17.0f, 0.6f, 16.0f);
                    scientist.badguys = new enimies.Entity(scientist.badguyp);
                    break;
                case 2:
                    sprites = Content.Load<List<Sprite>>("communistlevel");
                    cam.position = new Vector3(-28.5f, 0.9f, -25.0f);
                    cam.view = new Vector3(-28.5f, 0.9f, -0.5f);
                    timemachinepostion=new Vector3(-28.5f, 0.0f, -27.5f);
                    charge[0].badguyp = new Vector3(-21.0f, 0.75f, 17.0f);
                    charge[0].badguys = new enimies.Entity(charge[0].badguyp);
                    charge[0].spotlightAngle = 0.0f;
                    charge[1].badguyp = new Vector3(-4.0f, 0.75f, 17.0f);
                    charge[1].badguys = new enimies.Entity(charge[1].badguyp);
                    charge[1].spotlightAngle = 0.0f;
                    charge[2].badguyp = new Vector3(-20.0f, 0.75f, -8.5f);
                    charge[2].badguys = new enimies.Entity(charge[2].badguyp);
                    charge[2].spotlightAngle = 0.0f;
                    charge[3].badguyp = new Vector3(5.5f, 0.75f, -10.5f);
                    charge[3].badguys = new enimies.Entity(charge[3].badguyp);
                    charge[3].spotlightAngle = 0.0f;
                    charge[4].badguyp = new Vector3(-11.0f, 0.75f, -19.5f);
                    charge[4].badguys = new enimies.Entity(charge[4].badguyp);
                    charge[4].spotlightAngle = 0.0f;
                    shoot[0].badguyp = new Vector3(0.0f, 0.6f, 23.5f);
                    shoot[0].badguys = new enimies.Entity(shoot[0].badguyp);
                    shoot[0].direction = "X";
                    shoot[0].max = 29;
                    shoot[0].min = -26;
                    shoot[0].spotlightAngle = 0.0f;
                    shoot[0].enemyBullet = new Projectile();
                    shoot[1].badguyp = new Vector3(0.0f, 0.6f, -3.0f);
                    shoot[1].badguys = new enimies.Entity(shoot[1].badguyp);
                    shoot[1].direction = "X";
                    shoot[1].max = 29;
                    shoot[1].min = -26;
                    shoot[1].spotlightAngle = 0.0f;
                    shoot[1].enemyBullet = new Projectile();
                    shoot[2].badguyp = new Vector3(0.0f, 0.6f, -30.0f);
                    shoot[2].badguys = new enimies.Entity(shoot[2].badguyp);
                    shoot[2].direction = "X";
                    shoot[2].max = 29;
                    shoot[2].min = -21;
                    shoot[2].spotlightAngle = 0.0f;
                    shoot[2].enemyBullet = new Projectile();
                    shoot[3].badguyp = new Vector3(-7.0f, 0.6f, 2.5f);
                    shoot[3].badguys = new enimies.Entity(shoot[3].badguyp);
                    shoot[3].direction = "X";
                    shoot[3].max = -1;
                    shoot[3].min = -25;
                    shoot[3].spotlightAngle = 0.0f;
                    shoot[3].enemyBullet = new Projectile();
                    shoot[4].badguyp = new Vector3(4.0f, 0.6f, 0.0f);
                    shoot[4].badguys = new enimies.Entity(shoot[4].badguyp);
                    shoot[4].direction = "Z";
                    shoot[4].max = 23;
                    shoot[4].min = -3;
                    shoot[4].spotlightAngle = 0.0f;
                    shoot[4].enemyBullet = new Projectile();
                    boss.badguyp = new Vector3(19.0f, 0.75f, 11.0f);
                    boss.badguys = new enimies.Entity(boss.badguyp);
                    boss.maxX = 30;
                    boss.maxZ = 19;
                    boss.minX = 8;
                    boss.minZ = 1;
                    boss.spotlightAngle = 0.0f;
                    boss.enemyBullet = new Projectile();
                    boss.health = 5;
                    break;
                case 3:
                    if (hitler.badguys.alive)
                    {
                        hitler.badguys.die();
                        hitler.badguys.Position.X = -40.0f;
                    }
                    sprites = Content.Load<List<Sprite>>("nazilevel");
                    cam.position = new Vector3(-28.5f, 0.9f, -25.0f);
                    cam.view = new Vector3(-28.5f, 0.9f, -0.5f);
                    timemachinepostion = new Vector3(-28.5f, 0.0f, -27.5f);
                    charge[0].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    charge[0].badguys = new enimies.Entity(shoot[4].badguyp);
                    charge[0].badguys.die();
                    charge[1].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    charge[1].badguys = new enimies.Entity(shoot[4].badguyp);
                    charge[1].badguys.die();
                    charge[2].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    charge[2].badguys = new enimies.Entity(shoot[4].badguyp);
                    charge[2].badguys.die();
                    charge[3].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    charge[3].badguys = new enimies.Entity(shoot[4].badguyp);
                    charge[3].badguys.die();
                    charge[4].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    charge[4].badguys = new enimies.Entity(shoot[4].badguyp);
                    charge[4].badguys.die();
                    shoot[0].badguyp = new Vector3(0.0f, 0.6f, 12.0f);
                    shoot[0].badguys = new enimies.Entity(shoot[0].badguyp);
                    shoot[0].direction = "X";
                    shoot[0].max = 29;
                    shoot[0].min = -16;
                    shoot[0].spotlightAngle = 0.0f;
                    shoot[0].enemyBullet = new Projectile();
                    shoot[1].badguyp = new Vector3(0.0f, 0.6f, 17.0f);
                    shoot[1].badguys = new enimies.Entity(shoot[1].badguyp);
                    shoot[1].direction = "X";
                    shoot[1].max = 29;
                    shoot[1].min = -21;
                    shoot[1].spotlightAngle = 0.0f;
                    shoot[1].enemyBullet = new Projectile();
                    shoot[2].badguyp = new Vector3(0.0f, 0.6f, 23.0f);
                    shoot[2].badguys = new enimies.Entity(shoot[2].badguyp);
                    shoot[2].direction = "X";
                    shoot[2].max = 29;
                    shoot[2].min = -26;
                    shoot[2].spotlightAngle = 0.0f;
                    shoot[2].enemyBullet = new Projectile();
                    shoot[3].badguyp = new Vector3(0.0f, 0.6f, -29.0f);
                    shoot[3].badguys = new enimies.Entity(shoot[3].badguyp);
                    shoot[3].direction = "X";
                    shoot[3].max = 29;
                    shoot[3].min = -21;
                    shoot[3].spotlightAngle = 0.0f;
                    shoot[3].enemyBullet = new Projectile();
                    shoot[4].badguyp = new Vector3(0.0f, 0.6f, -24.0f);
                    shoot[4].badguys = new enimies.Entity(shoot[4].badguyp);
                    shoot[4].direction = "X";
                    shoot[4].max = 29;
                    shoot[4].min = -16;
                    shoot[4].spotlightAngle = 0.0f;
                    shoot[4].enemyBullet = new Projectile();
                    boss.badguyp = new Vector3(-6.0f, 0.6f, 3.5f);
                    boss.badguys = new enimies.Entity(boss.badguyp);
                    boss.maxX = 19;
                    boss.maxZ = 7;
                    boss.minX = -16;
                    boss.minZ = -13;
                    boss.spotlightAngle = 0.0f;
                    boss.enemyBullet = new Projectile();
                    boss.health = 5;
                    powerposition = new Vector3(-6.0f, 0.3f, 3.5f);
                    break;
                case 4:
                    if (scientist.badguys.alive)
                    {
                        scientist.badguys.die();
                        scientist.badguys.Position.X = -40.0f;
                    }
                    sprites = Content.Load<List<Sprite>>("zombielevel");
                    cam.position = new Vector3(-19.5f, 0.9f, -18.0f);
                    cam.view = new Vector3(-36.0f, 0.9f, -36.0f);
                    timemachinepostion = new Vector3(-20.5f, 0.0f, -16.5f);
                    charge[0].badguyp = new Vector3(-17.0f, 0.6f, -15.0f);
                    charge[0].badguys = new enimies.Entity(charge[0].badguyp);
                    charge[0].spotlightAngle = 0.0f;
                    charge[1].badguyp = new Vector3(-17.0f, 0.6f, 6.0f);
                    charge[1].badguys = new enimies.Entity(charge[1].badguyp);
                    charge[1].spotlightAngle = 0.0f;
                    charge[2].badguyp = new Vector3(-17.0f, 0.6f, 17.0f);
                    charge[2].badguys = new enimies.Entity(charge[2].badguyp);
                    charge[2].spotlightAngle = 0.0f;
                    charge[3].badguyp = new Vector3(8.0f, 0.6f, -15.0f);
                    charge[3].badguys = new enimies.Entity(charge[3].badguyp);
                    charge[3].spotlightAngle = 0.0f;
                    charge[4].badguyp = new Vector3(8.0f, 0.6f, 6.0f);
                    charge[4].badguys = new enimies.Entity(charge[4].badguyp);
                    charge[4].spotlightAngle = 0.0f;
                    shoot[0].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    shoot[0].badguys = new enimies.Entity(shoot[4].badguyp);
                    shoot[0].badguys.die();
                    shoot[0].enemyBullet = new Projectile();
                    shoot[0].enemyBullet.active = false;
                    shoot[1].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    shoot[1].badguys = new enimies.Entity(shoot[4].badguyp);
                    shoot[1].badguys.die();
                    shoot[1].enemyBullet = new Projectile();
                    shoot[1].enemyBullet.active = false;
                    shoot[2].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    shoot[2].badguys = new enimies.Entity(shoot[4].badguyp);
                    shoot[2].badguys.die();
                    shoot[2].enemyBullet = new Projectile();
                    shoot[2].enemyBullet.active = false;
                    shoot[3].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    shoot[3].badguys = new enimies.Entity(shoot[4].badguyp);
                    shoot[3].badguys.die();
                    shoot[3].enemyBullet = new Projectile();
                    shoot[3].enemyBullet.active = false;
                    shoot[4].badguyp = new Vector3(40.0f, 40.0f, 40.0f);
                    shoot[4].badguys = new enimies.Entity(shoot[4].badguyp);
                    shoot[4].badguys.die();
                    shoot[4].enemyBullet = new Projectile();
                    shoot[4].enemyBullet.active = false;
                    boss.badguyp = new Vector3(25.0f, 0.6f, 17.0f);
                    boss.badguys = new enimies.Entity(boss.badguyp);
                    boss.maxX = 30;
                    boss.maxZ = 24;
                    boss.minX = 4;
                    boss.minZ = 10;
                    boss.spotlightAngle = 0.0f;
                    boss.enemyBullet = new Projectile();
                    boss.health = 5;
                    break;
            }

            soundEngine2 = new AudioEngine("Content\\Audio\\Background2.xgs");
            soundBank2 = new SoundBank(soundEngine2, "Content\\Audio\\Sound Bank.xsb");
        }

        /// <summary>
        /// Set vertices for rectangular surface that is drawn using a triangle strip.
        /// </summary>
        private void InitializeGround()
        {
            const float BORDER = BOUNDARY;
            Vector2 uv = new Vector2(0.0f, 0.0f);
            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            Color color = Color.White;

            // top left
            uv.X = 0.0f; uv.Y = 0.0f;
            pos.X = -BORDER; pos.Y = 0.0f; pos.Z = BORDER;
            groundVertices[0] = new VertexPositionColorTexture(pos, color, uv);

            // bottom left
            uv.X = 0.0f; uv.Y = 10.0f;
            pos.X = -BORDER; pos.Y = 0.0f; pos.Z = -BORDER;
            groundVertices[1] = new VertexPositionColorTexture(pos, color, uv);

            // top right
            uv.X = 10.0f; uv.Y = 0.0f;
            pos.X = BORDER; pos.Y = 0.0f; pos.Z = BORDER;
            groundVertices[2] = new VertexPositionColorTexture(pos, color, uv);

            // bottom right
            uv.X = 10.0f; uv.Y = 10.0f;
            pos.X = BORDER; pos.Y = 0.0f; pos.Z = -BORDER;
            groundVertices[3] = new VertexPositionColorTexture(pos, color, uv);
        }
        private void Initializewall()
        {
            Vector2 uv = new Vector2(0.0f, 0.0f);
            Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
            Color color = Color.White;

            // top left
            uv.X = 0.0f; uv.Y = 0.0f;
            pos.X = 0.0f; pos.Y = 0.0f; pos.Z = -5.0f;
            wallVertices[0] = new VertexPositionColorTexture(pos, color, uv);

            // bottom left
            uv.X = 0.0f; uv.Y = 10.0f;
            pos.X = 0.0f; pos.Y = 2.5f; pos.Z = -5.0f;
            wallVertices[1] = new VertexPositionColorTexture(pos, color, uv);

            // top right
            uv.X = 10.0f; uv.Y = 0.0f;
            pos.X = 5.0f; pos.Y = 0.0f; pos.Z = -5.0f;
            wallVertices[2] = new VertexPositionColorTexture(pos, color, uv);

            // bottom right
            uv.X = 10.0f; uv.Y = 10.0f;
            pos.X = 5.0f; pos.Y = 2.5f; pos.Z = -5.0f;
            wallVertices[3] = new VertexPositionColorTexture(pos, color, uv);
        }

        void InitializeModels()
        {
            playerGunModel = Content.Load<Model>("Models\\weapon");
            playerGunMatrix = new Matrix[playerGunModel.Bones.Count];
            playerGunModel.CopyAbsoluteBoneTransformsTo(playerGunMatrix);

            rocketModel = Content.Load<Model>("Models\\rocket");
            rocketMatrix = new Matrix[rocketModel.Bones.Count];
            rocketModel.CopyAbsoluteBoneTransformsTo(rocketMatrix);

            timemachine = Content.Load<Model>("Models\\TimeMachine");
            timeMatrix = new Matrix[timemachine.Bones.Count];
            timemachine.CopyAbsoluteBoneTransformsTo(timeMatrix);

            powercell = Content.Load<Model>("Models\\PowerCells");
            powerMatrix = new Matrix[powercell.Bones.Count];
            powercell.CopyAbsoluteBoneTransformsTo(powerMatrix);

            if (level == 1 || level == 3 || level == 4)
            {
                human = Content.Load<Model>("Models\\Zombie");
                for (int i = 0; i < 5; i++)
                {
                    charge[i].chargingmatrix = new Matrix[human.Bones.Count];
                    human.CopyAbsoluteBoneTransformsTo(charge[i].chargingmatrix);
                }
            }
            else if (level == 2)
            {
                human = Content.Load<Model>("Models\\Bear");
                for (int i = 0; i < 5; i++)
                {
                    charge[i].chargingmatrix = new Matrix[human.Bones.Count];
                    human.CopyAbsoluteBoneTransformsTo(charge[i].chargingmatrix);
                }
            }
            if (level == 1 || level == 3 || level == 4)
            {
                soilder = Content.Load<Model>("Models\\Nazisoldier");
                for (int i = 0; i < 5; i++)
                {
                    shoot[i].shootingmatrix = new Matrix[soilder.Bones.Count];
                    soilder.CopyAbsoluteBoneTransformsTo(shoot[i].shootingmatrix);
                }
            }
            else if (level == 2)
            {
                soilder = Content.Load<Model>("Models\\Commisoldier");
                for (int i = 0; i < 5; i++)
                {
                    shoot[i].shootingmatrix = new Matrix[soilder.Bones.Count];
                    soilder.CopyAbsoluteBoneTransformsTo(shoot[i].shootingmatrix);
                }
            }
            if (level == 2)
            {
                bigboss = Content.Load<Model>("Models\\CommiBoss");
                boss.bossmatrix = new Matrix[bigboss.Bones.Count];
                bigboss.CopyAbsoluteBoneTransformsTo(boss.bossmatrix);
            }
            if (level == 3)
            {
                bigboss = Content.Load<Model>("Models\\NaziBoss");
                boss.bossmatrix = new Matrix[bigboss.Bones.Count];
                bigboss.CopyAbsoluteBoneTransformsTo(boss.bossmatrix);
            }
            if (level == 4)
            {
                bigboss = Content.Load<Model>("Models\\Zombie");
                boss.bossmatrix = new Matrix[bigboss.Bones.Count];
                bigboss.CopyAbsoluteBoneTransformsTo(boss.bossmatrix);
            }

            //humanMatrix = new Matrix[human.Bones.Count];
            //human.CopyAbsoluteBoneTransformsTo(humanMatrix);

            hitlerModel = Content.Load<Model>("Models\\Hitler");
            hitlerMatrix = new Matrix[hitlerModel.Bones.Count];
            hitlerModel.CopyAbsoluteBoneTransformsTo(hitlerMatrix);
            
            scientistModel = Content.Load<Model>("Models\\Scientist");
            scientistMatrix = new Matrix[scientistModel.Bones.Count];
            scientistModel.CopyAbsoluteBoneTransformsTo(scientistMatrix);
        }
        /// <summary>
        /// Executes set-up routines when program begins. 
        /// </summary>
        protected override void Initialize()
        {
            basicEffect = new BasicEffect(graphics.GraphicsDevice, null);
            InitializeBaseCode();
            InitializeGround();
            InitializeModels();
            //Initializewall();            
            //sprite.initializewalls(wallVertices);
            /*for (int i = 0; i < NUM_ROCKETS; i++)
                rocket[i] = new Projectile(BOUNDARY / 2.0f);//(WorldMatrix("ship1"));*/
            // now that the GraphicsDevice has been created, we can calculate the
            // aspect ratio ...
            float aspectRatio = graphics.GraphicsDevice.Viewport.Width /
                (float)graphics.GraphicsDevice.Viewport.Height;

            // ... and use that value to create the projection matrix.
            projectionMatrix2 = Matrix.CreatePerspectiveFieldOfView(
                FOV, aspectRatio, 1f, ViewRange); 
            base.Initialize();
        }
        #endregion
        #region turntoface
        
        const float SpotlightTurnSpeed = 0.05f;

        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        private static float TurnToFace(Vector3 position, Vector3 faceThis,
            float currentAngle, float turnSpeed)
        {
            // consider this diagram:
            //         C 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // S--------
            //     x
            // 
            // where S is the position of the spot light, C is the position of the cat,
            // and "o" is the angle that the spot light should be facing in order to 
            // point at the cat. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = faceThis.X - position.X;
            float z = faceThis.Z - position.Z;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            float desiredAngle = (float)Math.Atan2(z, x);

            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            float difference = WrapAngle(desiredAngle + currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        #endregion
        #region updateAndMove
        Matrix ScaleModel()
        {
            const float SCALAR = 1.0f;
            return Matrix.CreateScale(SCALAR, SCALAR, SCALAR);
        }

        /// <summary>
        /// Draws colored surfaces with PositionColor.fx shader. 
        /// </summary>
        /// <param name="primitiveType">Object type drawn with vertex data.</param>
        /// <param name="vertexData">Array of vertices.</param>
        /// <param name="numPrimitives">Total primitives drawn.</param>
        private void PositionColorShader(PrimitiveType primitiveType,
                                         VertexPositionColor[] vertexData,
                                         int numPrimitives)
        {
            positionColorEffect.Begin(); // begin using PositionColor.fx
            positionColorEffect.Techniques[0].Passes[0].Begin();

            // set drawing format and vertex data then draw primitive surface
            graphics.GraphicsDevice.VertexDeclaration = positionColor;
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                                    primitiveType, vertexData, 0, numPrimitives);

            positionColorEffect.Techniques[0].Passes[0].End();
            positionColorEffect.End();  // stop using PositionColor.fx
        }

        /// <summary>
        /// Draws textured primitive objects using Texture.fx shader. 
        /// </summary>
        /// <param name="primitiveType">Object type drawn with vertex data.</param>
        /// <param name="vertexData">Array of vertices.</param>
        /// <param name="numPrimitives">Total primitives drawn.</param>
        private void TextureShader(PrimitiveType primitiveType,
                                   VertexPositionColorTexture[] vertexData,
                                   int numPrimitives)
        {
            textureEffect.Begin(); // begin using Texture.fx
            textureEffect.Techniques[0].Passes[0].Begin();

            // set drawing format and vertex data then draw surface
            graphics.GraphicsDevice.VertexDeclaration = positionColorTexture;
            graphics.GraphicsDevice.DrawUserPrimitives
                                    <VertexPositionColorTexture>(
                                    primitiveType, vertexData, 0, numPrimitives);

            textureEffect.Techniques[0].Passes[0].End();
            textureEffect.End(); // stop using Textured.fx
        }

        Matrix WorldMatrix(string modelName, Vector3 position, int j)
        {
            Matrix rotationYOrbit, translation, translationOrbit, scale, rotationX;
            Matrix world = new Matrix();
            switch (modelName)
            {
                case "playerGun":
                    scale = Matrix.CreateScale(0.01f, 0.01f, 0.01f);

                    translation = Matrix.CreateTranslation(
                                      cam.position.X, BASE_HEIGHT * 2, cam.position.Z);
                    translationOrbit
                                   = Matrix.CreateTranslation(0.15f, 0.0f, -0.65f);
                    Vector3 look = cam.view - cam.position;
                    rotationX = Matrix.CreateRotationX(look.Y / (MathHelper.Pi *(MathHelper.Pi+MathHelper.Pi)));
                    rotationYOrbit = Matrix.CreateRotationY((float)
                                     (Math.Atan2(look.X, look.Z)+MathHelper.Pi));
                    world = scale * translationOrbit * rotationX * rotationYOrbit * translation;
                    break;
                case "timemachine":
                    scale = Matrix.CreateScale(0.15f, 0.15f, 0.15f);
                    translation = Matrix.CreateTranslation(timemachinepostion);
                    rotationYOrbit = Matrix.CreateRotationY(3.0f*(MathHelper.PiOver2));
                    world = scale *rotationYOrbit * translation;
                    break;
                case "hitler":
                    scale = Matrix.CreateScale(0.012f, 0.012f, 0.012f);
                    translation = Matrix.CreateTranslation(hitler.badguys.Position);
                    rotationYOrbit = Matrix.CreateRotationY(3.0f * MathHelper.PiOver2);
                    world = scale * rotationYOrbit * translation;
                    break;
                case "scientist":
                    scale = Matrix.CreateScale(0.0035f, 0.0035f, 0.0035f);
                    translation = Matrix.CreateTranslation(scientist.badguys.Position);
                    rotationYOrbit = Matrix.CreateRotationY(3.0f * MathHelper.PiOver2);
                    world = scale * rotationYOrbit * translation;
                    break;
                case "charge":
                    scale = Matrix.CreateScale(0.0035f, 0.0035f, 0.0035f);
                    translation = Matrix.CreateTranslation(position);       //Matrix.CreateTranslation(8.5f, 0.0f, -4.5f);
                    /*if(badguys.normMove)
                        rotationYOrbit = Matrix.CreateRotationY((MathHelper.PiOver2));
                    else
                        rotationYOrbit = Matrix.CreateRotationY(3.0f*MathHelper.PiOver2);*/
                    rotationYOrbit = Matrix.CreateRotationY(charge[j].spotlightAngle+(3*MathHelper.PiOver2));
                    world = scale *rotationYOrbit * translation;
                    break;
                case "shoot":
                    scale = Matrix.CreateScale(0.0035f, 0.0035f, 0.0035f);
                    translation = Matrix.CreateTranslation(position);       //Matrix.CreateTranslation(8.5f, 0.0f, -4.5f);
                    /*if(badguys.normMove)
                        rotationYOrbit = Matrix.CreateRotationY((MathHelper.PiOver2));
                    else
                        rotationYOrbit = Matrix.CreateRotationY(3.0f*MathHelper.PiOver2);*/
                    rotationYOrbit = Matrix.CreateRotationY(shoot[j].spotlightAngle + (3 * MathHelper.PiOver2));
                    world = scale * rotationYOrbit * translation;
                    break;
                case "boss":
                    scale = Matrix.CreateScale(0.0035f, 0.0035f, 0.0035f);
                    translation = Matrix.CreateTranslation(position);       //Matrix.CreateTranslation(8.5f, 0.0f, -4.5f);
                    /*if(badguys.normMove)
                        rotationYOrbit = Matrix.CreateRotationY((MathHelper.PiOver2));
                    else
                        rotationYOrbit = Matrix.CreateRotationY(3.0f*MathHelper.PiOver2);*/
                    rotationYOrbit = Matrix.CreateRotationY(boss.spotlightAngle + (3 * MathHelper.PiOver2));
                    world = scale * rotationYOrbit * translation;
                    break;
                case "powercell":
                    scale = Matrix.CreateScale(0.15f, 0.15f, 0.15f);
                    translation = Matrix.CreateTranslation(position);       //Matrix.CreateTranslation(8.5f, 0.0f, -4.5f);
                    /*if(badguys.normMove)
                        rotationYOrbit = Matrix.CreateRotationY((MathHelper.PiOver2));
                    else
                        rotationYOrbit = Matrix.CreateRotationY(3.0f*MathHelper.PiOver2);*/
                    rotationYOrbit = Matrix.CreateRotationY(boss.spotlightAngle + (3 * MathHelper.PiOver2));
                    world = scale * rotationYOrbit * translation;
                    break;
                case "rocket":
                    int i = 0;
                    foreach (Projectile bullet in bullets)
                    {
                        if (bullet.active)
                        {
                            Matrix rotateX, translate;

                            // 2: initialize matrices
                            scale = Matrix.CreateScale(0.3f, 0.3f, 0.3f);
                            rotateX = Matrix.CreateRotationX(-MathHelper.Pi / 2.0f);
                            translate = Matrix.CreateTranslation(bullet.rocketPosition);

                            // 3: build cumulative matrix using I.S.R.O.T. sequence
                            world = scale * rotateX * bullet.directionMatrix * translate;
                        }
                        i++;
                    }
                    break;
            }
            return world;
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState kbState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || kbState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            mousePrevious = mouseCurrent;
            mouseCurrent = Mouse.GetState();

            // update camera
            cam.SetFrameInterval(gameTime);
            cam.Move(Move(), boxnumlist);
            cam.Strafe(Strafe(), boxnumlist);
            cam.SetView(ChangeView(gameTime));

            if (hitler.badguys.alive)
            {
                foreach (Projectile bullet in bullets)
                {
                    if (Math.Sqrt((Math.Pow((hitler.badguys.Position.X - bullet.rocketPosition.X), 2) + Math.Pow((hitler.badguys.Position.Z - bullet.rocketPosition.Z), 2))) <= 1)
                    {
                        hitler.badguys.die();
                        hitler.badguys.Position.X = -40.0f;
                        bullet.active = false;
                    }
                }
            }

            if (scientist.badguys.alive)
            {
                foreach (Projectile bullet in bullets)
                {
                    if (Math.Sqrt((Math.Pow((scientist.badguys.Position.X - bullet.rocketPosition.X), 2) + Math.Pow((scientist.badguys.Position.Z - bullet.rocketPosition.Z), 2))) <= 1)
                    {
                        scientist.badguys.die();
                        scientist.badguys.Position.X = -40.0f;
                        bullet.active = false;
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (charge[i].badguys.alive)
                {
                    charge[i].spotlightAngle = TurnToFace(charge[i].badguys.Position, cam.position, charge[i].spotlightAngle, SpotlightTurnSpeed);
                    foreach (Projectile bullet in bullets)
                    {
                        if (Math.Sqrt((Math.Pow((charge[i].badguys.Position.X - bullet.rocketPosition.X), 2) + Math.Pow((charge[i].badguys.Position.Z - bullet.rocketPosition.Z), 2))) <= 1)
                        {
                            charge[i].badguys.die();
                            charge[i].badguys.Position.X = -40.0f;
                            bullet.active = false;
                        }
                    }
                    charge[i].los = LoS(charge[i].badguys, gameTime);
                    //badguys.move("X", -14, 10);
                    if (charge[i].los && charge[i].badguys.cancharge)
                        enemycharge(charge[i].badguys);
                    if (!charge[i].badguys.cancharge)
                    {
                        charge[i].badguys.chargecooldown++;
                        if (charge[i].badguys.chargecooldown >= 100)
                        {
                            charge[i].badguys.cancharge = true;
                            charge[i].badguys.chargecooldown = 0;
                        }
                    }
                    /*if (los&& !enemyBullet.active && enemycanfire)
                    {
                        enemycanfire = false;
                        enemyshoot(badguys, enemyBullet);
                    }
                    if (!enemycanfire)
                    {
                        waittime++;
                        if (waittime >= 100)
                        {
                            enemycanfire = true;
                            waittime = 0;
                        }
                    }*/
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (shoot[i].badguys.alive)
                {
                    shoot[i].spotlightAngle = TurnToFace(shoot[i].badguys.Position, cam.position, shoot[i].spotlightAngle, SpotlightTurnSpeed);
                    foreach (Projectile bullet in bullets)
                    {
                        if (Math.Sqrt((Math.Pow((shoot[i].badguys.Position.X - bullet.rocketPosition.X), 2) + Math.Pow((shoot[i].badguys.Position.Z - bullet.rocketPosition.Z), 2))) <= 1)
                        {
                            shoot[i].badguys.die();
                            shoot[i].badguys.Position.X = -40.0f;
                            bullet.active = false;
                        }
                    }
                    shoot[i].los = LoS(shoot[i].badguys, gameTime);
                    shoot[i].badguys.move(shoot[i].direction, shoot[i].min, shoot[i].max);
                    /*if (charge[i].los)
                        enemycharge(charge[i].badguys);*/
                    if (shoot[i].los && !shoot[i].enemyBullet.active && shoot[i].badguys.enemycanfire)
                    {
                        shoot[i].badguys.enemycanfire = false;
                        enemyshoot(shoot[i].badguys, shoot[i].enemyBullet);
                    }
                    if (!shoot[i].badguys.enemycanfire)
                    {
                        shoot[i].badguys.waittime++;
                        if (shoot[i].badguys.waittime >= 100)
                        {
                            shoot[i].badguys.enemycanfire = true;
                            shoot[i].badguys.waittime = 0;
                        }
                    }
                }
                if (shoot[i].enemyBullet.active)
                {
                    shoot[i].enemyBullet.UpdateProjectile(gameTime, boxnumlist);
                    if (Math.Sqrt((Math.Pow((cam.position.X - shoot[i].enemyBullet.rocketPosition.X), 2) + Math.Pow((cam.position.Z - shoot[i].enemyBullet.rocketPosition.Z), 2))) <= 1)
                    {
                        PlayerHealth -= 5;
                        shoot[i].enemyBullet.active = false;
                        Vector3 temp = new Vector3(36.0f, 36.0f, 36.0f);
                        shoot[i].enemyBullet.rocketPosition = (temp);
                    }
                }
            }
            if (level == 2 && cam.position.X < boss.maxX && cam.position.X > boss.minX && cam.position.Z < boss.maxZ && cam.position.Z > boss.minZ || level == 3 && cam.position.X < boss.maxX && cam.position.X > boss.minX && cam.position.Z < boss.maxZ && cam.position.Z > boss.minZ || level == 4 && cam.position.X < boss.maxX && cam.position.X > boss.minX && cam.position.Z < boss.maxZ && cam.position.Z > boss.minZ)
            {
                if (boss.badguys.alive)
                {
                    boss.spotlightAngle = TurnToFace(boss.badguys.Position, cam.position, boss.spotlightAngle, SpotlightTurnSpeed);
                    foreach (Projectile bullet in bullets)
                    {
                        if (Math.Sqrt((Math.Pow((boss.badguys.Position.X - bullet.rocketPosition.X), 2) + Math.Pow((boss.badguys.Position.Z - bullet.rocketPosition.Z), 2))) <= 1)
                        {
                            boss.health--;
                            bullet.rocketPosition = new Vector3(40.0f, 40.0f, 40.0f);
                            if (boss.health <= 0)
                            {
                                boss.badguys.die();
                                boss.badguys.Position.X = -40.0f;
                            }
                            bullet.active = false;
                        }
                    }
                    boss.los = LoS(boss.badguys, gameTime);
                    //boss.badguys.move(boss.direction, shoot[i].min, shoot[i].max);
                    
                    if (boss.los && !boss.enemyBullet.active && boss.badguys.enemycanfire)
                    {
                        boss.badguys.enemycanfire = false;
                        enemyshoot(boss.badguys, boss.enemyBullet);
                    }
                    else if (boss.los && boss.badguys.cancharge && !boss.enemyBullet.active)
                        enemycharge(boss.badguys);
                    if (!boss.badguys.cancharge)
                    {
                        boss.badguys.chargecooldown++;
                        if (boss.badguys.chargecooldown >= 100)
                        {
                            boss.badguys.cancharge = true;
                            boss.badguys.chargecooldown = 0;
                        }
                    }
                    if (!boss.badguys.enemycanfire)
                    {
                        boss.badguys.waittime++;
                        if (boss.badguys.waittime >= 100)
                        {
                            boss.badguys.enemycanfire = true;
                            boss.badguys.waittime = 0;
                        }
                    }
                }
            }
            if (level == 2 || level == 3 || level == 4)
            {
                if (boss.enemyBullet.active)
                {
                    boss.enemyBullet.UpdateProjectile(gameTime, boxnumlist);
                    if (Math.Sqrt((Math.Pow((cam.position.X - boss.enemyBullet.rocketPosition.X), 2) + Math.Pow((cam.position.Z - boss.enemyBullet.rocketPosition.Z), 2))) <= 1)
                    {
                        PlayerHealth -= 5;
                        boss.enemyBullet.active = false;
                        Vector3 temp = new Vector3(36.0f, 36.0f, 36.0f);
                        boss.enemyBullet.rocketPosition = (temp);
                    }
                }
            }
            if (level == 3)
            {
                if (Math.Sqrt((Math.Pow((cam.position.X - powerposition.X), 2) + Math.Pow((cam.position.Z - powerposition.Z), 2))) <= 1)
                {
                    powerposition = new Vector3(40.0f, 40.0f, 40.0f);
                }
            }
            //if (!soundEngine2.IsDisposed)
            //{
            gpPrevious = gpCurrent;  // update gamepad states
            gpCurrent = GamePad.GetState(PlayerIndex.One);
            //Fire();                     // check input for fire
            // launch rocket for right trigger and left click events
            if (gamepad.Triggers.Right > 0 && gamepadPrevious.Triggers.Right == 0
#if !XBOX
 || mouseCurrent.LeftButton == ButtonState.Pressed
                && mousePrevious.LeftButton == ButtonState.Released
#endif
&& num_rockets<MAX_ROCKETS)
            {
                num_rockets++;
                Projectile bullet1=new Projectile();
                bullets.Add(bullet1);

                int i = 0;
                foreach (Projectile bullet in bullets)
                {
                    if (bullet.active == false)
                    {
                        LaunchRocket(i, bullet);
                        GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                        //vibrating = true;
                        //timeFrame = 10.0f;
                        break;
                    }
                    i++;
                }
            }
            //}
            int j = 0;
            foreach (Projectile bullet in bullets)
            {
                if (bullet.active)
                    bullet.UpdateProjectile(gameTime, boxnumlist);
                if (!bullet.active)
                {
                    bullets.RemoveAt(j);
                    num_rockets--;
                    break;
                }
                j++;
            }
            bullets.TrimExcess();

            // distance formula:  d= ((x2-x1)^2+(y2-y1)^2)^(1/2)
            if (Math.Sqrt((Math.Pow((timemachinepostion.X - (double)cam.x1), 2) + Math.Pow((timemachinepostion.Z - (double)cam.y1), 2))) <= 1)
                touchtimemachine = true;
            else
                touchtimemachine = false;

            if (touchtimemachine && !hitler.badguys.alive && !scientist.badguys.alive && level==1)
            {
                level = 2;
                Initialize();
                boxnumlist.Clear();
            }
            else if (touchtimemachine && !hitler.badguys.alive && level == 1)
            {
                level = 4;
                Initialize();
                boxnumlist.Clear();
            }
            else if (touchtimemachine && !scientist.badguys.alive && level == 1)
            {
                level = 3;
                Initialize();
                boxnumlist.Clear();
            }
            else if(touchtimemachine && !boss.badguys.alive && level!=1)
            {
                level = 1;
                Initialize();
                boxnumlist.Clear();
            }

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            // create SpriteBatch object for drawing animated 2D images
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch2 = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Courier New");
            // load texture
            if (level == 1)
            {
                grassTexture = Content.Load<Texture2D>("Images\\dirtfloor");
                walltexture = Content.Load<Texture2D>("Images\\concrete");
                ceilingtexture = Content.Load<Texture2D>("Images\\concrete");
            }
            if (level == 2)
            {
                grassTexture = Content.Load<Texture2D>("Images\\dirtfloor");
                walltexture = Content.Load<Texture2D>("Images\\commiconcrete");
                ceilingtexture = Content.Load<Texture2D>("Images\\concrete");
            }
            if (level == 3)
            {
                grassTexture = Content.Load<Texture2D>("Images\\dirtfloor");
                walltexture = Content.Load<Texture2D>("Images\\concrete");
                ceilingtexture = Content.Load<Texture2D>("Images\\concrete");
            }
            if (level == 4)
            {
                grassTexture = Content.Load<Texture2D>("Images\\road");
                walltexture = Content.Load<Texture2D>("Images\\brokenbuilding");
                ceilingtexture = Content.Load<Texture2D>("Images\\road");
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Updates camera viewer in forwards and backwards direction.
        /// </summary>
        float Move()
        {
            KeyboardState kb = Keyboard.GetState();
            GamePadState gp = GamePad.GetState(PlayerIndex.One);
            float move = 0.0f;
            const float SCALE = 1.50f;

            // gamepad in use
            if (gp.IsConnected)
            {
                // left stick shifted left/right
                if (gp.ThumbSticks.Left.Y != 0.0f)
                    move = (SCALE * gp.ThumbSticks.Left.Y);
            }
            // no gamepad - use UP&DOWN or W&S
            else
            {
#if !XBOX
                if (kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.W))
                    move = 1.0f;  // Up or W - move ahead
                else if (kb.IsKeyDown(Keys.Down) || kb.IsKeyDown(Keys.S))
                    move = -1.0f; // Down or S - move back
#endif
            }
            return move;
        }

        /// <summary>
        /// Updates camera viewer in sideways direction.
        /// </summary>
        float Strafe()
        {
            KeyboardState kb = Keyboard.GetState();
            GamePadState gp = GamePad.GetState(PlayerIndex.One);

            // using gamepad leftStick shifted left / right for strafe
            if (gp.IsConnected)
            {
                if (gp.ThumbSticks.Left.X != 0.0f)
                    return gp.ThumbSticks.Left.X;
            }
            // using keyboard - strafe with Left&Right or A&D
            else if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.A))
                return -1.0f; // strafe left
            else if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D))
                return 1.0f;  // strafe right
            return 0.0f;
        }

        /// <summary>
        /// Changes camera viewing angle.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        Vector2 ChangeView(GameTime gameTime)
        {
            const float SENSITIVITY = 250.0f;
            const float VERTICAL_INVERSION = -1.0f; // vertical view control
            // negate to reverse

            // handle change in view using right and left keys
            KeyboardState kbState = Keyboard.GetState();
            int widthMiddle = Window.ClientBounds.Width / 2;
            int heightMiddle = Window.ClientBounds.Height / 2;
            Vector2 change = Vector2.Zero;
            GamePadState gp = GamePad.GetState(PlayerIndex.One);

            if (gp.IsConnected == true) // gamepad on PC / Xbox
            {
                float scaleY = VERTICAL_INVERSION * (float)
                               gameTime.ElapsedGameTime.Milliseconds / 50.0f;
                change.Y = scaleY * gp.ThumbSticks.Right.Y * SENSITIVITY;
                change.X = gp.ThumbSticks.Right.X * SENSITIVITY;
            }
            else
            {
                // use mouse only (on PC)
#if !XBOX
                float scaleY = VERTICAL_INVERSION * (float)
                               gameTime.ElapsedGameTime.Milliseconds / 100.0f;
                float scaleX = (float)gameTime.ElapsedGameTime.Milliseconds / 400.0f;

                // get cursor position
                mouse = Mouse.GetState();

                // cursor not at center on X
                if (mouse.X != widthMiddle)
                {
                    change.X = mouse.X - widthMiddle;
                    change.X /= scaleX;
                }
                // cursor not at center on Y
                if (mouse.Y != heightMiddle)
                {
                    change.Y = mouse.Y - heightMiddle;
                    change.Y /= scaleY;
                }
                // reset cursor back to center
                Mouse.SetPosition(widthMiddle, heightMiddle);
#endif
            }
            return change;
        }

        void Fire()
        {
            if (gpCurrent.Triggers.Right != 0.0f
            && gpPrevious.Triggers.Right == 0.0f
#if !XBOX
 || mouseCurrent.LeftButton == ButtonState.Pressed &&
                mousePrevious.LeftButton != ButtonState.Pressed
#endif
)
                soundBank2.PlayCue("Gun Shot 01");
        }
        private void enemycharge(enimies.Entity person)
        {
            Vector3 look=cam.position-person.Position;
            Vector3 tempposition = person.Position;
            int box;
            bool canfoward = true;
            bool canbackward = true;
            bool canleft = true;
            bool canright = true;
            look.Normalize();
            tempposition=tempposition + look/3;
            if (Math.Sqrt((Math.Pow((cam.position.X - tempposition.X), 2) + Math.Pow((cam.position.Z - tempposition.Z), 2))) <= 1&&person.cancharge)
            {
                PlayerHealth -= 10;
                Vector3 temp = cam.position + look;
                int row = (int)cam.position.Z + 32;
                int col = (int)cam.position.X + 32;
                box = ((row - 1) * 64) + (col);
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
                int x2 = (int)cam.position.X, y2 = (int)cam.position.Z;
                
                if (canfoward)
                    if (temp.Z < (float)(y2 - 0.25))
                    {
                        cam.position.Z = temp.Z;
                    }
                if (canbackward)
                    if (temp.Z > (float)(y2 - 0.75))
                    {
                        cam.position.Z = temp.Z;
                    }
                if (canleft)
                    if (temp.X < (float)(x2 - 0.25))
                    {
                        cam.position.X = temp.X;
                    }
                if (canright)
                    if (temp.X > (float)(x2 - 0.75))
                    {
                        cam.position.X = temp.X;
                    }

                canfoward = true;
                canbackward = true;
                canleft = true;
                canright = true;
                person.cancharge = false;
            }
            else if(person.cancharge)
                person.Position = tempposition;
            if (!person.cancharge)
            {
                person.chargecooldown++;
                if (person.chargecooldown >= 100)
                    person.cancharge = true;
            }

        }

        private void enemyshoot(enimies.Entity person, Projectile bullet)
        {
            Matrix orbitX, orbitY, translate, position;
            Vector3 look, start;
            start = person.Position;
            position = new Matrix(); // zero matrix
            position.M14 = 1.0f;         // set W to 1 so you can transform it

            translate = Matrix.CreateTranslation(start.X, start.Y, start.Z);

            // use same direction as launcher
            look = cam.position - start;

            // offset needed to rotate rocket about X to see it with camera
            //float offsetAngle = MathHelper.Pi*2.0;

            // adjust angle about X with changes in Look (Forward) direction
            orbitX = Matrix.CreateRotationX(look.Y / (-MathHelper.Pi * (MathHelper.Pi + MathHelper.Pi)));

            // rocket's Y direction is same as camera's at time of launch
            orbitY = Matrix.CreateRotationY((float)Math.Atan2(look.X, look.Z));

            // move rocket to camera position where launcher base is also located


            // use the I.S.R.O.T. sequence to get rocket start position
            position = position * orbitX * orbitY * translate;

            // convert from matrix back to vector so it can be used for updates
            start = new Vector3(position.M11, position.M12, position.M13);
            bullet.Launch(look, start);
        }

        private bool LoS(enimies.Entity person, GameTime gametime)
        {
            Projectile bullet = new Projectile();
            Matrix orbitX, orbitY, translate, position;
            Vector3 look, start;
            start = person.Position;
            position = new Matrix(); // zero matrix
            position.M14 = 1.0f;         // set W to 1 so you can transform it

            translate = Matrix.CreateTranslation(start.X, start.Y, start.Z);

            // use same direction as launcher
            look = cam.position - start;

            // offset needed to rotate rocket about X to see it with camera
            //float offsetAngle = MathHelper.Pi*2.0;

            // adjust angle about X with changes in Look (Forward) direction
            orbitX = Matrix.CreateRotationX(look.Y / (-MathHelper.Pi * (MathHelper.Pi + MathHelper.Pi)));

            // rocket's Y direction is same as camera's at time of launch
            orbitY = Matrix.CreateRotationY((float)Math.Atan2(look.X, look.Z));

            // move rocket to camera position where launcher base is also located


            // use the I.S.R.O.T. sequence to get rocket start position
            position = position * orbitX * orbitY * translate;

            // convert from matrix back to vector so it can be used for updates
            start = new Vector3(position.M11, position.M12, position.M13);
            bullet.Launch(look, start);
            do
            {
                if (bullet.active)
                {
                    bullet.LoSUpdateProjectile(gametime, boxnumlist);
                    if (Math.Sqrt((Math.Pow((cam.position.X - bullet.rocketPosition.X), 2) + Math.Pow((cam.position.Z - bullet.rocketPosition.Z), 2))) <= 1)
                        return true;
                }
            } while (bullet.active);

            return false;
        }

        private void LaunchRocket(int i, Projectile bullet)
        {
            Matrix orbitTranslate, orbitX, orbitY, translate, position;
            Vector3 look, start;

            /*translation = Matrix.CreateTranslation(
                                      cam.position.X, BASE_HEIGHT * 2, cam.position.Z);
            translationOrbit
                           = Matrix.CreateTranslation(-0.15f, 0.0f, 0.65f);
            Vector3 look = cam.view - cam.position;
            rotationX = Matrix.CreateRotationX(look.Y / (-MathHelper.Pi * (MathHelper.Pi + MathHelper.Pi)));
            rotationYOrbit = Matrix.CreateRotationY((float)
                             (Math.Atan2(look.X, look.Z)));*/

            // create matrix and store origin in first row
            position = new Matrix(); // zero matrix
            position.M14 = 1.0f;         // set W to 1 so you can transform it

            translate = Matrix.CreateTranslation(cam.position.X, BASE_HEIGHT * 2.0f, cam.position.Z);
            // move to tip of launcher
            orbitTranslate = Matrix.CreateTranslation(-0.15f, 0.0f, 1.69f);

            // use same direction as launcher
            look = cam.view - cam.position;

            // offset needed to rotate rocket about X to see it with camera
            //float offsetAngle = MathHelper.Pi*2.0;

            // adjust angle about X with changes in Look (Forward) direction
            orbitX = Matrix.CreateRotationX(look.Y / (-MathHelper.Pi * (MathHelper.Pi + MathHelper.Pi)));

            // rocket's Y direction is same as camera's at time of launch
            orbitY = Matrix.CreateRotationY((float)Math.Atan2(look.X, look.Z));

            // move rocket to camera position where launcher base is also located
            

            // use the I.S.R.O.T. sequence to get rocket start position
            position = position * orbitTranslate * orbitX * orbitY * translate;

            // convert from matrix back to vector so it can be used for updates
            start = new Vector3(position.M11, position.M12, position.M13);
            bullet.Launch(look, start);
        }

        void DeleteAudio2()
        {
            soundBank2.Dispose();
            soundEngine2.Dispose();
        }
#endregion
        #region draw
        /// <summary>
        /// Triggers drawing of ground with texture shader.
        /// </summary>
        private void DrawGround()
        {
            // 1: declare matrices
            Matrix world, translation;

            // 2: initialize matrices
            translation = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);

            // 3: build cumulative world matrix using I.S.R.O.T. sequence
            // identity, scale, rotate, orbit(translate & rotate), translate
            world = translation;

            // 4: set shader parameters
            basicEffect.World = world;
            basicEffect.View = cam.viewMatrix;
            basicEffect.Projection = cam.projectionMatrix;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = grassTexture;

            // 5: draw object - primitive type, vertex data, # primitives
            basicEffect.Begin();
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                graphics.GraphicsDevice.VertexDeclaration = positionColorTexture;
                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, groundVertices, 0, 2);
                pass.End();
            }
            basicEffect.End();
        }
        private void DrawCeiling()
        {
            // 1: declare matrices
            Matrix world, translation;

            // 2: initialize matrices
            translation = Matrix.CreateTranslation(0.0f, 2.0f, 0.0f);

            // 3: build cumulative world matrix using I.S.R.O.T. sequence
            // identity, scale, rotate, orbit(translate & rotate), translate
            world = translation;

            // 4: set shader parameters
            basicEffect.World = world;
            basicEffect.View = cam.viewMatrix;
            basicEffect.Projection = cam.projectionMatrix;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = ceilingtexture;

            // 5: draw object - primitive type, vertex data, # primitives
            basicEffect.Begin();
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                graphics.GraphicsDevice.VertexDeclaration = positionColorTexture;
                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, groundVertices, 0, 2);
                pass.End();
            }
            basicEffect.End();
        }
        private void Drawwall()
        {
            // 1: declare matrices
            Matrix world, translation, rotationY;

            // 2: initialize matrices
            translation = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);
            rotationY = Matrix.CreateRotationY(0.0f);
            world = translation * rotationY;
            // 3: build cumulative world matrix using I.S.R.O.T. sequence
            // identity, scale, rotate, orbit(translate & rotate), translate

            foreach (Sprite sprite in sprites)
            {
                
                    wallVertices = sprite.initializewalls(wallVertices);
                    world = sprite.draw(world, boxnumlist);
                    

                    // 4: set shader parameters
                    basicEffect.Begin();
                    foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                    {
                        basicEffect.World = world;
                        basicEffect.View = cam.viewMatrix;
                        basicEffect.Projection = cam.projectionMatrix;
                        basicEffect.TextureEnabled = true;
                        basicEffect.Texture = walltexture;

                        // 5: draw object - primitive type, vertex data, # primitives

                        pass.Begin();
                        graphics.GraphicsDevice.VertexDeclaration = positionColorTexture;
                        graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, wallVertices, 0, 2);
                        pass.End();
                    }
                    basicEffect.End();
            }
        }

        void DrawModel(Model model, string modelName, Vector3 position, int j)
        {
            // declare matrices
            Matrix world = WorldMatrix(modelName, position, j);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    switch(modelName)
                    {
                        case "playerGun":
                            // pass wvp to shader
                            effect.World = playerGunMatrix[mesh.ParentBone.Index] * world;
                            break;
                        case "timemachine":
                            effect.World = timeMatrix[mesh.ParentBone.Index] * world;
                            break;
                        case "charge":
                            effect.World = charge[j].chargingmatrix[mesh.ParentBone.Index] * world;
                            break;
                        case "shoot":
                            effect.World = shoot[j].shootingmatrix[mesh.ParentBone.Index] * world;
                            break;
                        case "hitler":
                            effect.World = hitlerMatrix[mesh.ParentBone.Index] * world;
                            break;
                        case "scientist":
                            effect.World = scientistMatrix[mesh.ParentBone.Index] * world;
                            break;
                        case "boss":
                            effect.World = boss.bossmatrix[mesh.ParentBone.Index] * world;
                            break;
                        case "powercell":
                            effect.World = powerMatrix[mesh.ParentBone.Index] * world;
                            break;
                    }
                    effect.View = cam.viewMatrix;
                    effect.Projection = cam.projectionMatrix;

                    // set lighting
                    effect.EnableDefaultLighting();
                }
                // draw object
                mesh.Draw();
            }
        }

        private void DrawRockets(Model model, int i)
        {
            Vector3 temp=new Vector3(0.0f,0.0f,0.0f);
            Matrix world = WorldMatrix("Rocket", temp, i);

            // 4: set shader parameters
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = rocketMatrix[mesh.ParentBone.Index]
                                 * world;
                    effect.View = cam.viewMatrix;
                    effect.Projection = cam.projectionMatrix;
                    effect.EnableDefaultLighting();
                    effect.SpecularPower = 16.5f;
                }
                // 5: draw object
                mesh.Draw();
            }
        }

        private void DrawText()
        {
            StringBuilder buffer = new StringBuilder();

                //buffer.AppendFormat("FPS: {0}\n", framesPerSecond);
                buffer.Append("Camera:  ");
                buffer.AppendFormat("level:{0}\n", level.ToString());
                buffer.AppendFormat("  Health:  {0}\n",
                    PlayerHealth.ToString("f2"));
                //Projectile[] rocket = bullets.ToArray();
                //if (bullets.Count >= 1)
                //{
                //    buffer.AppendFormat("  Bullet Position: x:{0} y:{1} z:{2}\n",
                //        rocket[0].rocketPosition.X.ToString("f2"),
                //        rocket[0].rocketPosition.Y.ToString("f2"),
                //        rocket[0].rocketPosition.Z.ToString("f2"));
                //    buffer.AppendFormat("  Bullet Box: {0}\n",
                //        rocket[0].box.ToString("f2"));
                //}
                //buffer.AppendFormat("  Orientation: heading:{0} pitch:{1}\n",
                //    camera.HeadingDegrees.ToString("f2"),
                //    camera.PitchDegrees.ToString("f2"));
                //buffer.AppendFormat("  Velocity: x:{0} y:{1} z:{2}\n",
                //    camera.CurrentVelocity.X.ToString("f2"),
                //    camera.CurrentVelocity.Y.ToString("f2"),
                //    camera.CurrentVelocity.Z.ToString("f2"));

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            spriteBatch.DrawString(spriteFont, buffer.ToString(), fontPos, Color.Yellow);
            spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            Vector3 temp = new Vector3(0.0f, 0.0f, 0.0f);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawGround();
            DrawCeiling();
            Drawwall();

            DrawModel(playerGunModel, "playerGun", temp, 0);
            DrawModel(timemachine, "timemachine", temp, 0);
            /*if(badguys.alive)
                DrawModel(human, "human", badguys.Position);*/
            if (hitler.badguys.alive)
                DrawModel(hitlerModel, "hitler", temp, 0);
            if (scientist.badguys.alive)
                DrawModel(scientistModel, "scientist", temp, 0);
            for (int i = 0; i < 5; i++)
                if (charge[i].badguys.alive)
                    DrawModel(human, "charge", charge[i].badguys.Position, i);
            for (int i = 0; i < 5; i++)
                if (shoot[i].badguys.alive)
                    DrawModel(soilder, "shoot", shoot[i].badguys.Position, i);
            if(level==2 || level==3 || level==4)
                DrawModel(bigboss, "boss", boss.badguys.Position, 0);
            if (level == 3)
                DrawModel(powercell, "powercell", powerposition, 0);
            /*int i = 0;
            foreach (Projectile bullet in bullets)
            {
                if (bullet.active)
                    DrawRockets(rocketModel, i);
                i++;
            }*/
            DrawText();
            base.Draw(gameTime);
        }
        #endregion
    }
}  