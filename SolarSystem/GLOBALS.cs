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
        public static int NUM_OBJECTS = 10;
        public static double ROTATE_RATE = 0.1;
        public static int FPS = 20;//frames per second
        public static int SEED = 1;//Environment.TickCount;

        //mass and speed
        public static double SUN_MASS_MULTIPLIER = 100000;//mass multiplier of sun
        public static double PLANET_INITIAL_SPEED = 81;//bigger sun, bigger speed to escape gravity
        public static double PLANET_MASS_MULTIPLIER = 50;
        public static double SPHERE_SIZE = 1;// times screen height

        //tail
        public static bool SHOW_TAIL = true;
        public static int TAIL_SIZE = 20;

        //remove planets
        public static int STRAY_LIMIT = 10000000;//remove objects far away
        public static double COLLISION_THRESHOLD = 0.5;//collide threshold
        public static double GRAVITY = 0.6674;
        public static bool RESPAWN = true;

        //friction
        public static double FRICTION = 0.01;//friction
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
