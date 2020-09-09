using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ProgressApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.progress.GradientColors.Add(Color.Blue) ;
            this.progress.GradientColors.Add(Color.DarkBlue);
            this.progress.GradientColors.Add(Color.Red);
           
        }

        double x;
        double y;
        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    x += e.TotalX;
                    y += e.TotalY;
                    box.TranslationX = x;
                    box.TranslationY = y;
                    //box.TranslationY = e.TotalY;

                    break;

                case GestureStatus.Completed:
                    // Store the translation applied during the pan

                    break;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Random r = new Random();
            //lbl.Text = r.Next(1, 1000).ToString();
            this.progress.SmoothToProgress((float)r.NextDouble());
        }
    }
}
