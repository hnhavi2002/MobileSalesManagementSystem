using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MINHTUANMOBILE
{
    public partial class SplashForm : SplashScreen
    {
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        double pbUnit;
        int pbWIDTH, pbHEIGHT, pbComplete;
        Bitmap bmp;
        Graphics g;
        public SplashForm()
        {
            InitializeComponent();
            this.labelCopyright.Text = "Copyright © 1998-" + DateTime.Now.Year.ToString();
            this.Load += SplashFormLoad;
        }
        public void SplashFormLoad(Object sender, EventArgs e)
        {
            pbWIDTH = pictureEdit1.Width;
            pbHEIGHT = pictureEdit1.Height;
            pbUnit = pbWIDTH / 100.0;
            pbComplete = 0;
            bmp = new Bitmap(pbWIDTH, pbHEIGHT);
            t.Interval = 50;
            t.Tick += new EventHandler(this.t_Tick);
            t.Start();
        }

        private void t_Tick(object sender, EventArgs e)
        {
            g = Graphics.FromImage(bmp);
            g.Clear(Color.LightGreen);
            g.FillRectangle(Brushes.CornflowerBlue, new Rectangle(0, 0, (int)(pbComplete * pbUnit), pbHEIGHT));
            g.DrawString(pbComplete + "%", new Font("Arial", pbHEIGHT / 2), Brushes.Black, new PointF(pbWIDTH / 2 - pbHEIGHT, pbHEIGHT / 10));
            // load bipmap
            pictureEdit1.Image = bmp;
            //update pb complete
            pbComplete++;
            if (pbComplete > 100)
            {
                //Dispose
                g.Dispose();
                t.Stop();
                openMain();
            }
        }
        public void openMain()
        {
            this.Hide();

            if (Application.OpenForms.OfType<FormMain>().Any())
            {
                Application.OpenForms.OfType<FormMain>().First().BringToFront();
            }
            else
            {
                FormMain mainForm = new FormMain();
                mainForm.Show();
            }

        }
        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

    }
}