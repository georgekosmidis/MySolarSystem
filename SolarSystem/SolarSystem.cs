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
    public partial class SolarSystem : Form {

        private bool IsMouseDown = false;
        private System.Timers.Timer aTimer;
        private _3d _center = new _3d();
        private List<obj> _objects = new List<obj>();

        private obj objBigger;
        private obj objFaster;

        public SolarSystem() {
            InitializeComponent();
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.DoubleBuffered = true;

            aTimer = new System.Timers.Timer( 1000 / GLOBALS.FPS );
            aTimer.Elapsed += aTimer_Elapsed;

            Random r = new Random( GLOBALS.SEED );
            for ( var i = 0; i < GLOBALS.SUNS_NUM; i++ ) {
                var o = new obj( i, this.Width, this.Height, r, true );
                _objects.Add( o );
            }
            for ( var i = GLOBALS.SUNS_NUM; i < GLOBALS.NUM_OBJECTS + GLOBALS.SUNS_NUM; i++ ) {
                var o = new obj( i, this.Width, this.Height, r );
                _objects.Add( o );
            }

            aTimer.Enabled = true;

            _center.set( this.Width / 2, this.Height / 2, (this.Width + this.Height) / 2 );
        }

        void aTimer_Elapsed( object sender, System.Timers.ElapsedEventArgs e ) {
            Invalidate();
        }
        private void Form1_Resize( object sender, EventArgs e ) {
            var _newCenter = new _3d();
            _newCenter.set( this.Width / 2, this.Height / 2, (this.Width + this.Height) / 2 );

            for ( var i = 1; i < _objects.Count; i++ ) {
                var oi = _objects[i];
                oi.p.x -= _center.x - _newCenter.x;
                oi.p.y -= _center.y - _newCenter.y;
                oi.p.z -= _center.z - _newCenter.z;
            }

            _center.set( this.Width / 2, this.Height / 2, (this.Width + this.Height) / 2 );

            textBox1.SetBounds( textBox1.Location.X, this.Height - 100, textBox1.Width, textBox1.Height );
        }
        private int iYears = 0;
        private void Form1_Paint( object sender, PaintEventArgs e ) {
            var m = new List<obj>();
            if ( IsMouseDown )
                return;
            FindAndCenter( e );
            iYears++;
            for ( var i = 0; i < _objects.Count; i++ ) {
                var oi = _objects[i];
                if ( m.Contains( oi ) )
                    continue;

                //if ( i != 0 && Math.Abs( _objects[i].v.x ) < 0.1 )
                //    MessageBox.Show( "why?" );
                if ( i > 0 && oi.isStray() ) {
                    if ( !GLOBALS.RESPAWN )
                        m.Add( oi );
                    else {
                        oi.v.x *= -1 / 10;
                        oi.v.y *= -1 / 10;
                        oi.v.z *= -1 / 10;
                        oi.p.x /= 10;
                        oi.p.y /= 10;
                        oi.p.z /= 10;
                    }
                }
                else {

                    /*acc*/
                    for ( var j = i + 1; j < _objects.Count; j++ ) {

                        var oj = _objects[j];
                        if ( !oi.isCollided( oj ) ) {
                            oi.acc( oj );
                        }
                        else {
                            var oo = (oi.m < oj.m) ? oi : oj;//smallest goes
                            m.Add( oo );
                            var d = (float)(oo.r * 10);//smallest times 10, size of explosion
                            var np = new Pen( Color.FromArgb( 255, GLOBALS.EXPLOSION_RING.Color.R, GLOBALS.EXPLOSION_RING.Color.G, GLOBALS.EXPLOSION_RING.Color.B ), 1 );
                            oo.Ellipse( e, GLOBALS.EXPLOSION_INNER, np, (float)oo.p.x - d / 2, (float)oo.p.y - d / 2, d );
                        }
                    }

                    oi.move();
                    oi.friction();
                }

                oi.draw( e );
            }
            foreach ( var o in m ) {
                _objects.Remove( o );
            }

            WriteStats();
        }

        /************************************/

        private void FindAndCenter( PaintEventArgs e ) {

            var suns = _objects.Where( o => o.s == true ).OrderByDescending( o => o.m );
            if ( suns.Count() > 0 ) {
                var o = suns.First();
                obj.cntr = o;
                for ( var i = 0; i < _objects.Count; i++ ) {
                    _objects[i].p.x -= o.p.x;
                    _objects[i].p.y -= o.p.y;
                    _objects[i].p.z -= o.p.z;
                }

                o.p.x = this.Width / 2;
                o.p.y = this.Height / 2;
                o.p.z = (this.Width + this.Height) / 4;                
            }
            else {
                double am = 0;//_objects.Where( o => !o.s ).Average( o => o.m );
                double m = _objects.Where( o => o.m >= am ).Sum( o => o.m );
                double x = _objects.Where( o => o.m >= am ).Sum( o => o.m * o.p.x ) / m;
                double y = _objects.Where( o => o.m >= am ).Sum( o => o.m * o.p.y ) / m;
                double z = _objects.Where( o => o.m >= am ).Sum( o => o.m * o.p.z ) / m;

                //  var p = new Pen( Color.WhiteSmoke );
                //  e.Graphics.DrawEllipse( p, (float)x, (float)y, 10, 10 );

                double md = double.PositiveInfinity;
                //move screen to mass
                for ( var i = 0; i < _objects.Count; i++ ) {
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
                    if ( d < md ) {
                        md = d;
                        obj.cntr = o;
                    }
                }
            }
        }

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

            for ( var i = 0; i < _objects.Count; i++ ) {
                var oi = _objects[i];
                if ( oi.s )
                    continue;
                //distance
                var d = oi.distance( obj.cntr );
                dco = dco < d ? dco : d;
                dfo = dfo > d ? dfo : d;
                da += d;
                //speed
                var s = oi.speed();
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

            //write stats
            var txt = " Objects: " + _objects.Count.ToString();
            txt += Environment.NewLine + " Speed Min/Max/Avg: " + sso.ToString( "#.00" ) + " / " + sfo.ToString( "#.00" ) + " / " + sa.ToString( "#.00" );
            txt += Environment.NewLine + " Distance Min/Max/Avg: " + dco.ToString( "#.00" ) + " / " + dfo.ToString( "#.00" ) + " / " + da.ToString( "#.00" );
            try {
                txt += Environment.NewLine + " Mass Sun/Min/Max/Avg: " + _objects.Where( o => o.s ).FirstOrDefault().m.ToString( "#.00" ) + " / " + mso.ToString( "#.00" ) + " / " + mbo.ToString( "#.00" ) + " / " + ma.ToString( "#.00" );
            }
            catch {
                txt += Environment.NewLine + " Mass Min/Max/Avg: " + mso.ToString( "#.00" ) + " / " + mbo.ToString( "#.00" ) + " / " + ma.ToString( "#.00" );

            }
            WriteTextBox( txt );
        }
        public void WriteTextBox( string value ) {
            try {
                if ( InvokeRequired ) {
                    this.Invoke( new Action<string>( WriteTextBox ), new object[] { value } );
                    return;
                }
                textBox1.Text = value;
            }
            catch { }
        }


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

            for ( int i = 0; i < _objects.Count; i += 1 ) {
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

            for ( int i = 0; i < _objects.Count; i++ ) {
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
