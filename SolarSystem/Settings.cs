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
    public partial class Settings : Form {
        private bool isLoaded = false;
        public Settings() {
            InitializeComponent();

            numSuns.Value = GLOBALS.SUNS_NUM;
            numObjects.Value = GLOBALS.NUM_OBJECTS;
            numRotate.Value = (decimal)GLOBALS.ROTATE_RATE;
            numFPS.Value = GLOBALS.FPS;
            numSunMass.Value = (decimal)GLOBALS.SUN_MASS_MULTIPLIER;
            numObjectSpeed.Value = (decimal)GLOBALS.PLANET_INITIAL_SPEED;
            numObjectMass.Value = (decimal)GLOBALS.PLANET_MASS_MULTIPLIER;
            numSphere.Value = (decimal)GLOBALS.SPHERE_SIZE;
            chkTail.Checked = GLOBALS.SHOW_TAIL;
            numTail.Value = GLOBALS.TAIL_SIZE;
            numStray.Value = GLOBALS.STRAY_LIMIT;
            numCollision.Value = (decimal)GLOBALS.COLLISION_THRESHOLD;
            numGravity.Value = (decimal)GLOBALS.GRAVITY;
            numFriction.Value = (decimal)GLOBALS.FRICTION;
            numFrictionSpeed.Value = (decimal)GLOBALS.FRICTION_FROM_SPEED;

            btnSun.BackColor = GLOBALS.SUN_INNER.Color;
            btnSunRing.BackColor = GLOBALS.SUN_RING.Color;
            btnObject.BackColor = GLOBALS.PLANET_INNER.Color;
            btnObjectRing.BackColor = GLOBALS.PLANET_RING.Color;
            btnExplosion.BackColor = GLOBALS.EXPLOSION_INNER.Color;
            btnExplosionRing.BackColor = GLOBALS.EXPLOSION_RING.Color;

            chkRespawn.Checked = GLOBALS.RESPAWN;

            numRandom.Value = GLOBALS.SEED;
            isLoaded = true;
            CalcData();
        }

        private void StoreData() {
            if ( !isLoaded ) return;
            GLOBALS.SUNS_NUM = (int)numSuns.Value;
            GLOBALS.NUM_OBJECTS = (int)numObjects.Value;
            GLOBALS.ROTATE_RATE = (double)numRotate.Value;
            GLOBALS.FPS = (int)numFPS.Value;
            GLOBALS.SUN_MASS_MULTIPLIER = (double)numSunMass.Value;
            GLOBALS.PLANET_INITIAL_SPEED = (double)numObjectSpeed.Value;
            GLOBALS.PLANET_MASS_MULTIPLIER = (double)numObjectMass.Value;
            GLOBALS.SPHERE_SIZE = (double)numSphere.Value;
            GLOBALS.SHOW_TAIL = chkTail.Checked;
            GLOBALS.TAIL_SIZE = (int)numTail.Value;
            GLOBALS.STRAY_LIMIT = (int)numStray.Value;
            GLOBALS.COLLISION_THRESHOLD = (double)numCollision.Value;
            GLOBALS.GRAVITY = (double)numGravity.Value;
            GLOBALS.FRICTION = (double)numFriction.Value;
            GLOBALS.FRICTION_FROM_SPEED = (double)numFrictionSpeed.Value;

            GLOBALS.SUN_INNER.Color = btnSun.BackColor;
            GLOBALS.SUN_RING.Color = btnSunRing.BackColor;
            GLOBALS.PLANET_INNER.Color = btnObject.BackColor;
            GLOBALS.PLANET_RING.Color = btnObjectRing.BackColor;
            GLOBALS.EXPLOSION_INNER.Color = btnExplosion.BackColor;
            GLOBALS.EXPLOSION_RING.Color = btnExplosionRing.BackColor;

            GLOBALS.RESPAWN = chkRespawn.Checked;

            GLOBALS.SEED = (int)numRandom.Value;
        }
        private void CalcData() {
            if ( !isLoaded ) return;
            Random r = new Random( GLOBALS.SEED );
            double sm = 0;
            double ss = 0;
            for ( var i = 0; i < GLOBALS.SUNS_NUM; i++ ) {
                var o = new obj( i, this.Width, this.Height, r, true );
                sm += o.m;
                ss += o.speed();
            }
            double mom = 0;
            for ( var i = GLOBALS.SUNS_NUM; i < GLOBALS.NUM_OBJECTS + GLOBALS.SUNS_NUM; i++ ) {
                var o = new obj( i, this.Width, this.Height, r );
                sm += o.m;
                mom = o.m > mom ? o.m : mom;
                ss += o.speed();
            }

            lblMass.Text = sm.ToString( "0,#.###" );
            lblMomentum.Text = (sm * ss).ToString( "0,#.###" );
            lblObjMass.Text = mom.ToString( "0,#.###" );
        }

        #region events
        private void btnStart_Click( object sender, EventArgs e ) {
            StoreData();
            CalcData();
            var f = new SolarSystem();
            f.Show();
        }
        private void btnColor( object sender ) {
            var d = new ColorDialog();
            d.AllowFullOpen = true;
            d.ShowHelp = true;
            d.Color = ((Button)sender).BackColor;
            d.AnyColor = true;
            d.SolidColorOnly = false;

            int i = (d.Color.B << 16) | (d.Color.G << 8) | d.Color.R;
            d.CustomColors = new[] { i };

            if ( d.ShowDialog() == DialogResult.OK )
                ((Button)sender).BackColor = d.Color;
            StoreData();
        }
        private void btnSun_Click( object sender, EventArgs e ) {
            btnColor( sender );
        }
        private void btnSunRing_Click( object sender, EventArgs e ) {
            btnColor( sender );
        }
        private void btnObject_Click( object sender, EventArgs e ) {
            btnColor( sender );
        }
        private void btnObjectRing_Click( object sender, EventArgs e ) {
            btnColor( sender );
        }
        private void btnExplosion_Click( object sender, EventArgs e ) {
            btnColor( sender );
        }
        private void btnExplosionRing_Click( object sender, EventArgs e ) {
            btnColor( sender );
        }
        private void numSuns_ValueChanged( object sender, EventArgs e ) {
            StoreData();
            CalcData();
        }
        private void numSunMass_ValueChanged( object sender, EventArgs e ) {
            StoreData();
            CalcData();
        }

        private void numObjects_ValueChanged( object sender, EventArgs e ) {
            StoreData();
            CalcData();
        }

        private void numObjectMass_ValueChanged( object sender, EventArgs e ) {
            StoreData();
            CalcData();
        }

        private void numObjectSpeed_ValueChanged( object sender, EventArgs e ) {
            StoreData();
            CalcData();
        }

        private void numGravity_ValueChanged( object sender, EventArgs e ) {
            StoreData();
            CalcData();
        }

        private void numSphere_ValueChanged( object sender, EventArgs e ) {
            StoreData();
            CalcData();
        }

        private void numRandom_ValueChanged( object sender, EventArgs e ) {
            StoreData();
            CalcData();
        }

        private void numFriction_ValueChanged( object sender, EventArgs e ) {
            StoreData();
        }

        private void numStray_ValueChanged( object sender, EventArgs e ) {
            StoreData();
        }

        private void numCollision_ValueChanged( object sender, EventArgs e ) {
            StoreData();
        }

        private void numFrictionSpeed_ValueChanged( object sender, EventArgs e ) {
            StoreData();
        }

        private void numTail_ValueChanged( object sender, EventArgs e ) {
            StoreData();
        }

        private void numFPS_ValueChanged( object sender, EventArgs e ) {
            StoreData();
        }

        private void numRotate_ValueChanged( object sender, EventArgs e ) {
            StoreData();
        }

        private void chkTail_CheckedChanged( object sender, EventArgs e ) {
            StoreData();
        }
        #endregion
    }
}
