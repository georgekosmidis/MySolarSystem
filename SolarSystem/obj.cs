using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolarSystem
{
    class obj
    {

        public _3d p = new _3d();//position
        public _3d v = new _3d();//velocity
        private List<_3d> t = new List<_3d>();//tail
        public double m = 0;//mass
        public double r = 0;//radius
        public int id = 0;
        public static obj cntr;//centered object, everyone goes arround this one
        public bool s = false;//is sun
                              // private int sphereRadius = 600;

        public obj(int i, int w, int h, Random r, bool isSun = false)
        {
            this.id = i;
            s = isSun;
            //sphereRadius = w > h ? h : w;

            var phi = this.random(r) * 360;
            var theta = Math.Acos(this.random(r) * 2 - 1);
            var ra = (h / (2 / GLOBALS.SPHERE_SIZE) * Math.Pow(this.random(r), ((double)1) / 3));
            this.p.x = ra * Math.Sin(theta) * Math.Cos(phi) + w / 2;
            this.p.y = ra * Math.Sin(theta) * Math.Sin(phi) + h / 2;
            this.p.z = ra * Math.Cos(theta) * (Math.Cos(phi) * Math.Sin(phi) / 2) + ((w + h) / 4);

            var d = GLOBALS.PLANET_INITIAL_SPEED / Math.Sqrt(this.p.x * this.p.x + this.p.y * this.p.y);
            this.v.x = this.random(r) * d * (this.p.y - h / 2);
            this.v.y = this.random(r) * d * -(this.p.x - h / 2);
            this.v.z = this.random(r) * d * -(this.p.z - h / 2);

            this.m = random(r) * GLOBALS.PLANET_MASS_MULTIPLIER;

            if (isSun)
            {
                this.m = (random(r) + 0.5) * GLOBALS.SUN_MASS_MULTIPLIER;
                if (i == 0)
                {
                    this.p.x = w / 2;
                    this.p.y = h / 2;
                    this.p.z = (w + h) / 4;
                    this.v.x = 0;
                    this.v.y = 0;
                    this.v.z = 0;
                }
            }

            this.r = Math.Log10(m) * 1.5;

        }
        public void resetMass(double nm)
        {
            this.m = nm;
            this.r = Math.Log10(m);
        }
        public void draw(PaintEventArgs e)
        {
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            /*sun is bigger*/
            if (this.s)
            {
                var ssb = new SolidBrush(Color.FromArgb(255, GLOBALS.SUN_INNER.Color.R, GLOBALS.SUN_INNER.Color.G, GLOBALS.SUN_INNER.Color.B));
                var sp = new Pen(Color.FromArgb(255, GLOBALS.SUN_RING.Color.R, GLOBALS.SUN_RING.Color.G, GLOBALS.SUN_RING.Color.B), 1);

                var d = (float)this.r * 2;
                var x = (float)(this.p.x - (d / 2));
                var y = (float)(this.p.y - (d / 2));
                e.Graphics.FillEllipse(ssb, x, y, d, d);
                e.Graphics.DrawEllipse(sp, x, y, d, d);

            }
            else
            {
                var d = (float)(Math.Max(0, this.p.z) * (this.r * 2) / (cntr.p.z * 2));//size: actual size to distance
                var a = 255;
                if (this.p.z < cntr.p.z)
                    a = (int)Math.Round(this.p.z * 255 / cntr.p.z);
                a = a < 0 ? 0 : a;
                a = a > 255 ? 255 : a;

                var sb = new SolidBrush(Color.FromArgb(a, GLOBALS.PLANET_INNER.Color.R, GLOBALS.PLANET_INNER.Color.G, GLOBALS.PLANET_INNER.Color.B));

                var p = new Pen(Color.FromArgb(a, GLOBALS.PLANET_RING.Color.R, GLOBALS.PLANET_RING.Color.G, GLOBALS.PLANET_RING.Color.B), 1);
                var x = (float)(this.p.x - (d / 2));
                var y = (float)(this.p.y - (d / 2));

                Ellipse(e, sb, p, x, y, d);

                if (GLOBALS.SHOW_TAIL)
                    for (var i = 0; i < this.t.Count; i++)
                    {
                        var na = (int)Math.Round(a * ((1 - ((double)i / GLOBALS.TAIL_SIZE)) * 0.5));
                        var sbt = new SolidBrush(Color.FromArgb(na, GLOBALS.PLANET_INNER.Color.R, GLOBALS.PLANET_INNER.Color.G, GLOBALS.PLANET_INNER.Color.B));
                        var np = new Pen(Color.FromArgb(na, GLOBALS.PLANET_INNER.Color.R, GLOBALS.PLANET_INNER.Color.G, GLOBALS.PLANET_INNER.Color.B), 1);
                        var nd = d * (1 - ((float)i / GLOBALS.TAIL_SIZE));

                        Ellipse(e, sbt, np, (float)this.t[i].x - (nd / 2), (float)this.t[i].y - (nd / 2), nd);
                    }
            }
        }
        public void Ellipse(PaintEventArgs e, SolidBrush pb, Pen p, float x, float y, float d)
        {
            //don't draw outside screen
            if (x < 0
                || x > e.ClipRectangle.Width
                || y < 0
                || y > e.ClipRectangle.Height)
                return;

            //Behind sun?
            if (this.p.z > cntr.p.z
                    || !(this.p.z < cntr.p.z && x > cntr.p.x - cntr.r && x < cntr.p.x + cntr.r)
                    || !(this.p.z < cntr.p.z && (y > cntr.p.y - cntr.r && y < cntr.p.y + cntr.r))
                    )
            {
                e.Graphics.FillEllipse(pb, x, y, d, d);
                e.Graphics.DrawEllipse(p, x, y, d, d);
            }
        }
        public bool isCollided(obj o)
        {
            double dx = this.p.x - o.p.x;
            double dy = this.p.y - o.p.y;
            double dz = this.p.z - o.p.z;
            var d = dx * dx + dy * dy + dz * dz;
            if (Math.Abs(d) - (this.r + o.r) <= (this.r + o.r) * GLOBALS.COLLISION_THRESHOLD)
            {
                //new mass
                var mass = this.m + o.m;
                this.m += o.m;
                this.r = Math.Log10(m);
                //find biggest
                //var oo = (this.m > o.m) ? this : o;
                //biggest takes mass
                //oo.m = mass;
                //oo.r = Math.Log10( m );

                return true;
            }
            return false;
        }
        public double speed()
        {
            return Math.Sqrt(this.v.x * this.v.x + this.v.y * this.v.y + this.v.z * this.v.z);
            //return Math.Round( s );
        }
        public double distance(obj o)
        {
            double dx = this.p.x - o.p.x;
            double dy = this.p.y - o.p.y;
            double dz = this.p.z - o.p.z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public void acc(obj o)
        {

            var d = this.distance(o);

            var force = GLOBALS.GRAVITY * this.m * o.m / Math.Pow(d, 3);

            var accelThis = force / this.m;
            var accelThat = force / o.m;

            double dx = this.p.x - o.p.x;
            double dy = this.p.y - o.p.y;
            double dz = this.p.z - o.p.z;

            this.v.x -= accelThis * dx;
            this.v.y -= accelThis * dy;
            this.v.z -= accelThis * dz;

            o.v.x += accelThat * dx;
            o.v.y += accelThat * dy;
            o.v.z += accelThat * dz;

        }
        public bool isStray()
        {
            return (Math.Abs(this.distance(obj.cntr)) > GLOBALS.STRAY_LIMIT);
        }
        public void move()
        {

            this.p.x += this.v.x;
            this.p.y += this.v.y;
            this.p.z += this.v.z;
            var tmp = new _3d();
            tmp.set(this.p.x, this.p.y, this.p.z);

            //TAIL
            this.t.Reverse();
            this.t.Add(tmp);
            this.t.Reverse();

            if (this.t.Count > GLOBALS.TAIL_SIZE)
                this.t.RemoveRange(GLOBALS.TAIL_SIZE, this.t.Count - GLOBALS.TAIL_SIZE);

        }
        public void friction()
        {
            var friction = this.speed() * GLOBALS.FRICTION;
            if (this.speed() > GLOBALS.FRICTION_FROM_SPEED)
            {
                this.v.x *= (1 - friction);
                this.v.y *= (1 - friction);
                this.v.z *= (1 - friction);
            }
        }
        private double random(Random r)
        {
            return ((double)r.Next(1, 999)) / 1000;
        }
    }
}
