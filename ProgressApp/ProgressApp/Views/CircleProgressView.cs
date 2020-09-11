using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ProgressApp.Views
{
    public class CircleProgressView : View
    {

        #region Radius
        public static readonly BindableProperty RadiusProperty =
   BindableProperty.Create(nameof(Radius), typeof(double), typeof(CircleProgressView), 15d, propertyChanged: (obj, o, n) =>
     {
         (obj as CircleProgressView).RadiusPropertyChanged();
     });
        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        void RadiusPropertyChanged() 
        {
            //MeasureSizeByFontsize = false;
            //InvalidateMeasure(); 
        }
        #endregion

        #region ProgressBarWidth
        public static readonly BindableProperty ProgressBarWidthProperty =
BindableProperty.Create(nameof(ProgressBarWidth), typeof(double), typeof(CircleProgressView), 3d);
        public double ProgressBarWidth
        {
            get => (double)GetValue(ProgressBarWidthProperty);
            set => SetValue(ProgressBarWidthProperty, value);
        }
        #endregion

        #region BackgroundCircleColor
        public static readonly BindableProperty BackgroundCircleColorProperty =
BindableProperty.Create(nameof(BackgroundCircleColor), typeof(Color), typeof(CircleProgressView), Color.White);
        public Color BackgroundCircleColor
        {
            get => (Color)GetValue(BackgroundCircleColorProperty);
            set => SetValue(BackgroundCircleColorProperty, value);
        }
        #endregion

        #region ProgressBarColor
        public static readonly BindableProperty ProgressBarColorProperty =
BindableProperty.Create(nameof(ProgressBarColor), typeof(Color), typeof(CircleProgressView), Color.Blue);
        public Color ProgressBarColor
        {
            get => (Color)GetValue(ProgressBarColorProperty);
            set => SetValue(ProgressBarColorProperty, value);
        }
        #endregion

        #region ProgressTrackBarColor
        public static readonly BindableProperty ProgressTrackBarColorProperty =
BindableProperty.Create(nameof(ProgressTrackBarColor), typeof(Color), typeof(CircleProgressView), Color.Transparent);
        public Color ProgressTrackBarColor
        {
            get => (Color)GetValue(ProgressTrackBarColorProperty);
            set => SetValue(ProgressTrackBarColorProperty, value);
        }
        #endregion

        #region Progress
        public static readonly BindableProperty ProgressProperty =
BindableProperty.Create(nameof(Progress), typeof(float), typeof(CircleProgressView), default(float));
        /// <summary>
        /// 0-1
        /// </summary>
        public float Progress
        {
            get => (float)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
        #endregion

        #region Text
        public static readonly BindableProperty TextProperty =
   BindableProperty.Create(nameof(Text), typeof(string), typeof(CircleProgressView), default(string
       ));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        /// <summary>
        /// 文字的Margin，尽在MeasureSizeByFontsize=true时出现
        /// </summary>
        public Thickness TextMargin { get; set; }

        public float CharacterSpace { get; set; }

        public bool MeasureSizeByFontsize { get; set; }

        public string MinWidthText { get; set; }

        #region RightHalfAngle
        /// <summary>
        /// 右半边角度，即为半角模式
        /// </summary>
        public static readonly BindableProperty RightHalfAngleProperty =
   BindableProperty.Create(nameof(RightHalfAngle), typeof(float), typeof(CircleProgressView), default(float), propertyChanged: (obj, o, n) =>
   {
       (obj as CircleProgressView).InvalidateMeasure();
   });
        public float RightHalfAngle
        {
            get => (float)GetValue(RightHalfAngleProperty);
            set => SetValue(RightHalfAngleProperty, value);
        }
        #endregion



        public double FontSize { get; set; } = 14;

        public Color TextColor { get; set; } = Color.Black;

        public List<Color> GradientColors { get; set; }

        public CircleProgressView()
        {
            GradientColors = new List<Color>();

        }

        public Action<float, int> SmoothToProgressAction;
        public void SmoothToProgress(float targetProgress, int duration = 300)
        {
            SmoothToProgressAction?.Invoke(targetProgress, duration);
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return base.OnMeasure(widthConstraint, heightConstraint);
            //if (RightHalfAngle == 0)
            //{
            //    return new SizeRequest(new Size(Radius * 2, Radius * 2));
            //}
            //else if (RightHalfAngle <= 90)
            //{
            //    double d = ((90 - RightHalfAngle) * Math.PI) / 180;
            //    var halfWidth = Radius * Math.Cos(d);
            //    return new SizeRequest(new Size(halfWidth * 2, Radius));

            //}
            //else
            //{
            //    double d = ((180 - RightHalfAngle) * Math.PI) / 180;
            //    var extHeight = Radius * Math.Cos(d);
            //    return new SizeRequest(new Size(Radius * 2, Radius + extHeight));
            //}
        }
    }
}