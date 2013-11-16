using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolarSystem {
    class obj {

        private double SUN_MASS_MULTIPLIER = 10000;
        private double PLANET_MASS_MULTIPLIER = 0.2;
        private bool SHOW_TAIL = true;
        private int TAIL_SIZE = 20;
        //private int STRAY_LIMIT = 10000;
        private double STRAY_LIMIT_FORCE = 0.0001;
        private int COLLISION_DISTANCE = 50;
        private double GRAVITY = 0.6674;
        private double FRICTION = 0.001;
        private double FRICTION_FROM_SPEED = 20;

        private SolidBrush SUN_INNER = new SolidBrush( Color.Yellow );
        private SolidBrush SUN_RING = new SolidBrush( Color.Orange );

        private SolidBrush PLANET_INNER = new SolidBrush( Color.SkyBlue );
        private SolidBrush PLANET_RING = new SolidBrush( Color.LightSkyBlue );

        public _3d p = new _3d();
        public _3d v = new _3d();
        private List<_3d> t = new List<_3d>();
        public double m = 0;//mass
        public double r = 0;//radius
        // public bool s = false;//is sun
        public int id = 0;
        public static obj sun;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">obj id</param>
        /// <param name="w">form width</param>
        public obj( int i, int w, int h, Random r, bool isSun = false ) {
            this.id = i;
            if (isSun) {
                this.m = r.Next( 1, 2 ) * SUN_MASS_MULTIPLIER;
                this.r = Math.Log10( m );
                if (i == 0) {
                    this.p.x = w / 2;
                    this.p.y = h / 2;
                    this.p.z = (w + h) / 4;
                }
                else {
                    this.p.x = r.Next( 0, w );
                    this.p.y = r.Next( 0, h );
                    this.p.z = r.Next( 0, (w + h) / 2 );
                }
                this.v.x = 0;
                this.v.y = 0;
                this.v.z = 0;
            }
            else {
                this.p.x = r.Next( 0, w );
                this.p.y = r.Next( 0, h );
                this.p.z = r.Next( 0, (w + h) / 2 );
                this.v.x = r.Next( -4, 4 );
                this.v.y = r.Next( -4, 4 );
                this.v.z = r.Next( -4, 4 );

               
                this.m = r.Next( 2, 50 ) * PLANET_MASS_MULTIPLIER;
                this.r = Math.Log10( m / PLANET_MASS_MULTIPLIER );
            }
        }
        public void resetMass( double nm ) {
            this.m = nm;
            this.r = Math.Log10( m );
        }
        public void draw( PaintEventArgs e ) {
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            /*sun is bigger*/
            if (this.id == sun.id) {
                var ssb = new SolidBrush( Color.FromArgb( 255, SUN_INNER.Color.R, SUN_INNER.Color.G, SUN_INNER.Color.B ) );
                var sp = new Pen( Color.FromArgb( 255, SUN_RING.Color.R, SUN_RING.Color.G, SUN_RING.Color.B ), 1 );

                var d = (float)this.r * 2;
                var x = (float)(this.p.x - (d / 2));
                var y = (float)(this.p.y - (d / 2));
                e.Graphics.FillEllipse( ssb, x, y, d, d );
                e.Graphics.DrawEllipse( sp, x, y, d, d );

            }
            else {
                var d = (float)(Math.Max( 0, this.p.z ) * (this.r * 2) / (sun.p.z * 2));//size: actual size to distance
                var a = (int)Math.Round( Math.Min( Math.Max( 0, this.p.z ), sun.p.z ) * 255 / sun.p.z );//transarent when away

                var sb = new SolidBrush( Color.FromArgb( a, PLANET_INNER.Color.R, PLANET_INNER.Color.G, PLANET_INNER.Color.B ) );
                var p = new Pen( Color.FromArgb( a, PLANET_RING.Color.R, PLANET_RING.Color.G, PLANET_RING.Color.B ), 1 );
                var x = (float)(this.p.x - (d / 2));
                var y = (float)(this.p.y - (d / 2));

                Ellipse( e, sb, p, x, y, d );

                if (SHOW_TAIL)
                    for (var i = 0; i < this.t.Count; i++) {
                        var na = (int)Math.Round( a * ((1 - ((double)i / TAIL_SIZE)) * 0.5) );
                        var sbt = new SolidBrush( Color.FromArgb( na, PLANET_INNER.Color.R, PLANET_INNER.Color.G, PLANET_INNER.Color.B ) );
                        var np = new Pen( Color.FromArgb( na, PLANET_INNER.Color.R, PLANET_INNER.Color.G, PLANET_INNER.Color.B ), 1 );
                        var nd = d * (1 - ((float)i / TAIL_SIZE));

                        Ellipse( e, sbt, np, (float)this.t[i].x - (nd / 2), (float)this.t[i].y - (nd / 2), nd );

                    }
            }
        }
        public void Ellipse( PaintEventArgs e, SolidBrush pb, Pen p, float x, float y, float d ) {
            //don't draw outside screen
            if (x < 0
                || x > e.ClipRectangle.Width
                || y < 0
                || y > e.ClipRectangle.Height)
                return;

            //Behind sun?
            if (this.p.z > sun.p.z
                    || !(this.p.z < sun.p.z && x > sun.p.x - sun.r && x < sun.p.x + sun.r)
                    || !(this.p.z < sun.p.z && (y > sun.p.y - sun.r && y < sun.p.y + sun.r))
                    ) {
                e.Graphics.FillEllipse( pb, x, y, d, d );
                e.Graphics.DrawEllipse( p, x, y, d, d );
            }
        }
        public bool isCollided( obj o ) {
            double dx = this.p.x - o.p.x;
            double dy = this.p.y - o.p.y;
            double dz = this.p.z - o.p.z;
            var d = dx * dx + dy * dy + dz * dz;
            if (Math.Abs( d ) <= this.r + o.r + COLLISION_DISTANCE) {
                var oo = (this.m > o.m) ? this : o;//biggest takes mass
                oo.m += o.m;
                oo.r += 1;
                return true;
            }
            return false;
        }
        private double speed() {
            var s = Math.Sqrt( this.v.x * this.v.x + this.v.y * this.v.y + this.v.z * this.v.z );
            return Math.Round( s );
        }
        public void acc( obj o ) {
            double dx = this.p.x - o.p.x;
            double dy = this.p.y - o.p.y;
            double dz = this.p.z - o.p.z;
            var d = dx * dx + dy * dy + dz * dz;

            var force = GRAVITY * this.m * o.m / d;
            var accel1 = force / this.m;
            var accel2 = force / o.m;

            dx /= Math.Sqrt( d );
            dy /= Math.Sqrt( d );
            dz /= Math.Sqrt( d );
            this.v.x -= accel1 * dx;
            this.v.y -= accel1 * dy;
            this.v.z -= accel1 * dz;
            o.v.x += accel2 * dx;
            o.v.y += accel2 * dy;
            o.v.z += accel2 * dz;

        }
        public bool isStray(  ) {
            //var o0 = sun;
            //var dx = o0.p.x - this.p.x;
            //var dy = o0.p.y - this.p.y;
            //var dz = o0.p.z - this.p.z;
            //var d = Math.Sqrt( dx * dx + dy * dy + dz * dz );
            //return (Math.Abs( d ) > STRAY_LIMIT);

            double dx = this.p.x - sun.p.x;
            double dy = this.p.y - sun.p.y;
            double dz = this.p.z - sun.p.z;
            var d = dx * dx + dy * dy + dz * dz;

            var force = GRAVITY * this.m * sun.m / d;
            return( force / this.m < STRAY_LIMIT_FORCE);
            
        }
        public void move() {

            this.p.x += this.v.x;
            this.p.y += this.v.y;
            this.p.z += this.v.z;
            var tmp = new _3d();
            tmp.set( this.p.x, this.p.y, this.p.z );

            //TAIL
            this.t.Reverse();
            this.t.Add( tmp );
            this.t.Reverse();

            if (this.t.Count > TAIL_SIZE)
                this.t.RemoveRange( TAIL_SIZE, this.t.Count - TAIL_SIZE );

        }
        public void friction() {
            var friction = this.speed() * FRICTION;
            if (this.speed() > FRICTION_FROM_SPEED) {
                this.v.x *= (1 - friction);
                this.v.y *= (1 - friction);
                this.v.z *= (1 - friction);
            }
        }
    }
}
