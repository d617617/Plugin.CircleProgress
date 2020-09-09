using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ProgressApp.Views
{
    public class CustomCircleProgress : View
    {
        #region Radius
        public static readonly BindableProperty RadiusProperty =
   BindableProperty.Create(nameof(Radius), typeof(double), typeof(CustomCircleProgress), 15d, propertyChanged: (obj, o, n) =>
   {
       (obj as CustomCircleProgress).InvalidateMeasure();
   });
        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }
        #endregion

        #region ProgressBarWidth
        public static readonly BindableProperty ProgressBarWidthProperty =
BindableProperty.Create(nameof(ProgressBarWidth), typeof(double), typeof(CustomCircleProgress), 3d);
        public double ProgressBarWidth
        {
            get => (double)GetValue(ProgressBarWidthProperty);
            set => SetValue(ProgressBarWidthProperty, value);
        }
        #endregion

        #region BackgroundCircleColor
        public static readonly BindableProperty BackgroundCircleColorProperty =
BindableProperty.Create(nameof(BackgroundCircleColor), typeof(Color), typeof(CustomCircleProgress), Color.White);
        public Color BackgroundCircleColor
        {
            get => (Color)GetValue(BackgroundCircleColorProperty);
            set => SetValue(BackgroundCircleColorProperty, value);
        }
        #endregion

        #region ProgressBarColor
        public static readonly BindableProperty ProgressBarColorProperty =
BindableProperty.Create(nameof(ProgressBarColor), typeof(Color), typeof(CustomCircleProgress), Color.Blue);
        public Color ProgressBarColor
        {
            get => (Color)GetValue(ProgressBarColorProperty);
            set => SetValue(ProgressBarColorProperty, value);
        }
        #endregion

        #region ProgressTrackBarColor
        public static readonly BindableProperty ProgressTrackBarColorProperty =
BindableProperty.Create(nameof(ProgressTrackBarColor), typeof(Color), typeof(CustomCircleProgress), Color.Transparent);
        public Color ProgressTrackBarColor
        {
            get => (Color)GetValue(ProgressTrackBarColorProperty);
            set => SetValue(ProgressTrackBarColorProperty, value);
        }
        #endregion

        #region Progress
        public static readonly BindableProperty ProgressProperty =
BindableProperty.Create(nameof(Progress), typeof(float), typeof(CustomCircleProgress), default(float));
        /// <summary>
        /// 0-1
        /// </summary>
        public float Progress
        {
            get => (float)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
        #endregion


        #region RightHalfAngle
        public static readonly BindableProperty RightHalfAngleProperty =
   BindableProperty.Create(nameof(RightHalfAngle), typeof(float), typeof(CustomCircleProgress), 45f, propertyChanged: (obj, o, n) =>
   {
       (obj as CustomCircleProgress).InvalidateMeasure();
   });
        public float RightHalfAngle
        {
            get => (float)GetValue(RightHalfAngleProperty);
            set => SetValue(RightHalfAngleProperty, value);
        }
        #endregion

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (RightHalfAngle <= 90)
            {
                double d = ((90 - RightHalfAngle) * Math.PI) / 180;
                var halfWidth = Radius * Math.Cos(d);
                return new SizeRequest(new Size(halfWidth * 2, Radius));
            }
            else
            {                
                double d = ( (180 - RightHalfAngle) * Math.PI) / 180;
                var extHeight = Radius * Math.Cos(d);
                return new SizeRequest(new Size(Radius * 2, Radius + extHeight));
            }


        }
    }
}
