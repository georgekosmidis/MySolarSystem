using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem {
    public static class GLOBALS {
        //form
        public static int SUNS_NUM = 1;
        public static int NUM_OBJECTS = 501;
        public static double ROTATE_RATE = 0.001;
        public static int FPS = 20;//frames per second
        public static int SEED = Environment.TickCount;

        //mass and speed
        public static double SUN_MASS_MULTIPLIER = 1000;//mass multiplier of sun
        public static double PLANET_INITIAL_SPEED = 5;//bigger sun, bigger speed to escape gravity
        public static double PLANET_MASS_MULTIPLIER = 20;
        public static double SPHERE_SIZE = 1;// times screen height

        //tail
        public static bool SHOW_TAIL = false;
        public static int TAIL_SIZE = 20;

        //remove planets
        public static int STRAY_LIMIT = 1000000;//remove objects far away
        public static double COLLISION_THRESHOLD = 0.5;//collide threshold
        public static double GRAVITY = 0.6674;

        //friction
        public static double FRICTION = 0.001;//friction
        public static double FRICTION_FROM_SPEED = 200;//apply to super fast objects


        //colors
        public static SolidBrush SUN_INNER = new SolidBrush( Color.Yellow );
        public static SolidBrush SUN_RING = new SolidBrush( Color.Orange );
        public static SolidBrush PLANET_INNER = new SolidBrush( Color.SkyBlue );
        public static SolidBrush PLANET_RING = new SolidBrush( Color.LightSkyBlue );
        public static SolidBrush EXPLOSION_INNER = new SolidBrush( Color.Red );
        public static SolidBrush EXPLOSION_RING = new SolidBrush( Color.Yellow );
    }
}
