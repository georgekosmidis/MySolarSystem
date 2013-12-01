using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolarSystem {
    public partial class Form1 : Form {

        private int SUNS_NUM = 1;
        public int NUM_OBJECTS = 500;
        public double ROTATE_RATE = 0.001;
        public int FPS = 20;//frames per second

        bool IsMouseDown = false;
        System.Timers.Timer aTimer;
        _3d _center = new _3d();
        List<obj> _objects = new List<obj>();

        public Form1() {
            InitializeComponent();

            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.DoubleBuffered = true;

            aTimer = new System.Timers.Timer( 1000 / FPS );
            aTimer.Elapsed += aTimer_Elapsed;

            Random r = new Random( (int)DateTime.Now.Ticks );
            for (var i = 0; i < SUNS_NUM; i++) {
                var o = new obj( i, this.Width, this.Height, r, true );
                _objects.Add( o );
            }
            for (var i = SUNS_NUM; i < NUM_OBJECTS + SUNS_NUM; i++) {
                var o = new obj( i, this.Width, this.Height, r );
                _objects.Add( o );
            }

            aTimer.Enabled = true;

            _center.set( this.Width / 2, this.Height / 2, (this.Width + this.Height) / 2 );
            FindSunAndCenter();
        }

        void aTimer_Elapsed( object sender, System.Timers.ElapsedEventArgs e ) {
            Invalidate();
            WriteStats();
        }
        private void Form1_Resize( object sender, EventArgs e ) {
            var _newCenter = new _3d();
            _newCenter.set( this.Width / 2, this.Height / 2, (this.Width + this.Height) / 2 );

            for (var i = 1; i < _objects.Count; i++) {
                var oi = _objects[i];
                oi.p.x -= _center.x - _newCenter.x;
                oi.p.y -= _center.y - _newCenter.y;
                oi.p.z -= _center.z - _newCenter.z;
            }

            _center.set( this.Width / 2, this.Height / 2, (this.Width + this.Height) / 2 );

            textBox1.SetBounds( textBox1.Location.X, this.Height - 100, textBox1.Width, textBox1.Height );
        }
        private void Form1_Paint( object sender, PaintEventArgs e ) {
            var m = new List<obj>();
            if (IsMouseDown)
                return;

            for (var i = 0; i < _objects.Count; i++) {
                var oi = _objects[i];
                if (m.Contains( oi ))
                    continue;

                if (i > 0 && oi.isStray()) {
                    m.Add( oi );
                }
                else {
                    /*acc*/
                    for (var j = i + 1; j < _objects.Count; j++) {
                        var oj = _objects[j];
                        if (!oi.isCollided( oj )) {
                            oi.acc( oj );
                        }
                        else {
                            var oo = (oi.m < oj.m) ? oi : oj;//smallest goes
                            m.Add( oo );
                            var d = (float)(oo.r * 10);//smallest to 10, size of explosion
                            oo.Ellipse( e, (new SolidBrush( Color.Red )), (new Pen( Color.Yellow )), (float)oo.p.x - d / 2, (float)oo.p.y - d / 2, d );
                        }
                    }

                    oi.move();
                    oi.friction();
                }

                oi.draw( e );
            }
            foreach (var o in m) {
                _objects.Remove( o );
            }
            FindSunAndCenter();
        }

        /************************************/

        private void FindSunAndCenter() {
            double x = 0, y = 0, z = 0, m = 0;

            //gather mass 
            for (var i = 0; i < _objects.Count; i++) {
                var o = _objects[i];
                m += o.m;
                x += o.m * o.p.x;
                y += o.m * o.p.y;
                z += o.m * o.p.z;
            }
            x /= m;
            y /= m;
            z /= m;

            double md = double.PositiveInfinity;
            double mm = 0;

            //move screen to mass
            for (var i = 0; i < _objects.Count; i++) {
                var o = _objects[i];
                //move objects to center mass
                o.p.x -= x - this.Width / 2;
                o.p.y -= y - this.Height / 2;
                o.p.z -= z - (this.Width + this.Height) / 4;

                //find closest object to mass and set this as refernce
                double dx = x - o.p.x;
                double dy = y - o.p.y;
                double dz = z - o.p.z;
                var d = dx * dx + dy * dy + dz * dz;
                if (d < md) {
                    md = d;
                    obj.cntr = o;
                }
                if (m > mm) {
                    m = mm;
                    obj.sun = o;
                }
            }
        }

        public void WriteTextBox( string value ) {
            try {
                if (InvokeRequired) {
                    this.Invoke( new Action<string>( WriteTextBox ), new object[] { value } );
                    return;
                }
                textBox1.Text = value;
            }
            catch { }
        }

        #region Stats
        private void WriteStats() {
            double dco = double.MaxValue;//closest object to sun
            double dfo = double.MinValue;//farthest object
            double da = 0;//avg distance
            double sfo = double.MinValue;//fastest object
            double sso = double.MaxValue;//slowsest object
            double sa = 0;//avg speed
            double mbo = double.MinValue;//mass bigger object
            double mso = double.MaxValue;//mass smallest object
            double ma = 0;//avg mass speed

            var oh = _objects[0];
            for (var i = 1; i < _objects.Count; i++) {
                oh = oh.m > _objects[i].m ? oh : _objects[i];
                // oh.s = false;
            }

            for (var i = 0; i < _objects.Count; i++) {
                var oi = _objects[i];
                if (oi.id == oh.id)
                    continue;
                //distance
                var dx = oh.p.x - oi.p.x;
                var dy = oh.p.y - oi.p.y;
                var dz = oh.p.z - oi.p.z;
                var d = Math.Sqrt( dx * dx + dy * dy + dz * dz );//distance from sun
                dco = dco < d ? dco : d;
                dfo = dfo > d ? dfo : d;
                da += d;
                //speed
                var s = Math.Sqrt( oi.v.x * oi.v.x + oi.v.y * oi.v.y + oi.v.z * oi.v.z ); //speed
                sfo = sfo > s ? sfo : s;
                sso = sso < s ? sso : s;
                sa += s;
                //mass
                mbo = mbo > oi.m ? mbo : oi.m;
                mso = mso < oi.m ? mso : oi.m;
                ma += oi.m;
            }
            da /= _objects.Count - 1;
            sa /= _objects.Count - 1;
            ma /= _objects.Count - 1;

            var txt = " Objects: " + _objects.Count.ToString();
            txt += Environment.NewLine + " Speed Min/Max/Avg: " + sso.ToString( "#.00" ) + " / " + sfo.ToString( "#.00" ) + " / " + sa.ToString( "#.00" );
            txt += Environment.NewLine + " Distance Min/Max/Avg: " + dco.ToString( "#.00" ) + " / " + dfo.ToString( "#.00" ) + " / " + da.ToString( "#.00" );
            txt += Environment.NewLine + " Mass Sun/Min/Max/Avg: " + oh.m.ToString( "#.00" ) + " / " + mso.ToString( "#.00" ) + " / " + mbo.ToString( "#.00" ) + " / " + ma.ToString( "#.00" );
            WriteTextBox( txt );
        }

        #endregion

        private void Form1_MouseDown( object sender, MouseEventArgs e ) {
            aTimer.Enabled = false;
            IsMouseDown = true;
        }

        private void Form1_MouseUp( object sender, MouseEventArgs e ) {
            aTimer.Enabled = true;
            IsMouseDown = false;
        }

        _3d pm = new _3d();
        private void Form1_MouseMove( object sender, MouseEventArgs e ) {
            //if (IsMouseDown) {
            //    rotateY3D( -(pm.x - e.X) * ROTATE_RATE );
            //    rotateX3D( (pm.y - e.Y) * ROTATE_RATE );
            //    this.Invalidate();
            //}
            //pm.set( e.X, e.Y, 0 );
        }

        private void rotateY3D( double theta ) {
            var ct = Math.Cos( theta );
            var st = Math.Sin( theta );
            double x = 0, z = 0;

            for (int i = 0; i < _objects.Count; i += 1) {
                var oi = _objects[i];
                x = oi.p.x;
                z = oi.p.z;
                oi.p.x = ct * x + st * z;
                oi.p.z = -st * x + ct * z;

                x = oi.v.x;
                z = oi.v.z;

                oi.v.x = ct * x + st * z;
                oi.v.z = -st * x + ct * z;

            }
        }
        private void rotateX3D( double theta ) {
            var ct = Math.Cos( theta );
            var st = Math.Sin( theta );
            double y = 0, z = 0;

            for (int i = 0; i < _objects.Count; i++) {
                var oi = _objects[i];
                y = oi.p.y;
                z = oi.p.z;
                oi.p.y = ct * y - st * z;
                oi.p.z = st * y + ct * z;

                y = oi.v.y;
                z = oi.v.z;

                oi.v.y = ct * y - st * z;
                oi.v.z = st * y + ct * z;

            }
        }

    }
}
