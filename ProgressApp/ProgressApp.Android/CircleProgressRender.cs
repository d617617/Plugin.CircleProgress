using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using ProgressApp.Views;
using Xamarin.Forms.Platform.Android;
using static Android.Text.Layout;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CircleProgressView), typeof(ProgressApp.Droid.CircleProgressRender))]
namespace ProgressApp.Droid
{
    public class CircleProgressRender : ViewRenderer
    {
        CircleProgressView circleProgress;
        double _des = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
        Paint _mPaint;

        const float StartAngle = -90;
        int _circleRadius = 0;
        int _progressBarWidth = 0;
        SweepGradient mSweepGradient;
        public CircleProgressRender(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            if (circleProgress == null)
            {
                circleProgress = Element as CircleProgressView;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var propName = e.PropertyName;
            if (propName == nameof(CircleProgressView.Progress)
                || propName == nameof(CircleProgressView.Text)
                || propName == nameof(CircleProgressView.BackgroundCircleColor)
                || propName == nameof(CircleProgressView.ProgressBarColor)
                || propName == nameof(CircleProgressView.ProgressTrackBarColor)
                )
            {
                Invalidate();
            }
        }


        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            SetProgressData();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            SetProgressData();
        }



        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (_mPaint == null)
            {
                _mPaint = new Paint();
            }
            DrawProgressBarTrack(canvas);
            DrawProgressBar(canvas);
            DrawBackgroundCircle(canvas);
            DrawText(canvas);
            //drawDial(canvas);
        }


        private void drawDial(Canvas canvas)
        {
            int total = 1;
            canvas.Save();
            var centerX = Width / 2;
            var centerY = Height / 2;
            //canvas.Rotate(0, centerX, centerY);
            _mPaint.Color = Color.Red;
            _mPaint.StrokeWidth = 15;
            for (int i = 0; i < total; i++)
            {
                canvas.DrawLine(centerX, centerY, centerX + _circleRadius, centerY + _circleRadius, _mPaint);
                canvas.Rotate(20, centerX, centerY);
            }
            canvas.Restore();
        }


        void SetProgressData()
        {
            _circleRadius = (int)Math.Ceiling(_des * circleProgress.Radius);
            if (circleProgress.ProgressBarWidth < 1)
            {
                _progressBarWidth = (int)Math.Ceiling(_circleRadius * circleProgress.ProgressBarWidth);
            }
            else
            {
                _progressBarWidth = (int)Math.Ceiling(_des * circleProgress.ProgressBarWidth);
            }

        }


        protected virtual void DrawBackgroundCircle(Canvas canvas)
        {
            var centerX = Width / 2;
            var centerY = Height / 2;
            var lastRadius = _circleRadius - _progressBarWidth;
            _mPaint.Color = circleProgress.BackgroundCircleColor.ToAndroid();
            _mPaint.SetShader(null);
            canvas.DrawCircle(centerX, centerY, lastRadius, _mPaint);
        }

        protected virtual void DrawProgressBarTrack(Canvas canvas)
        {
            if (circleProgress.ProgressTrackBarColor == Xamarin.Forms.Color.Transparent)
            {
                return;
            }
            var centerX = Width / 2;
            var centerY = Height / 2;
            _mPaint.Color = circleProgress.ProgressTrackBarColor.ToAndroid();
            var lastRadius = _circleRadius;
            _mPaint.SetShader(null);
            canvas.DrawCircle(centerX, centerY, lastRadius, _mPaint);
        }

        protected virtual void DrawProgressBar(Canvas canvas)
        {

            //绘制完全
            RectF oval = new RectF();
            oval.Left = 0;
            oval.Right = this.Width;
            oval.Top = 0;
            oval.Bottom = this.Height;
            if (circleProgress.GradientColors.Count != 0)
            {
                var centerX = Width / 2;
                var centerY = Height / 2;
                if (mSweepGradient == null)
                {
                    var colors = new int[circleProgress.GradientColors.Count];
                    int count = 0;
                    foreach (var item in circleProgress.GradientColors)
                    {
                        colors[count] = item.ToAndroid();
                        count++;
                    }
                    mSweepGradient = new SweepGradient(centerX, centerY, colors: colors, positions: null);
                }
                _mPaint.SetShader(mSweepGradient);
            }
            else
            {
                _mPaint.Color = circleProgress.ProgressBarColor.ToAndroid();
            }
            var nowAngle = 360 * circleProgress.Progress;
            if (nowAngle > 360)
            {
                nowAngle = 360;
            }
            canvas.DrawArc(oval, StartAngle, nowAngle, true, _mPaint);
        }

        protected virtual void DrawText(Canvas canvas)
        {
            if (string.IsNullOrWhiteSpace(circleProgress.Text))
            {
                return;
            }
            _mPaint.Color = circleProgress.TextColor.ToAndroid();
            _mPaint.TextSize = circleProgress.FontSize <= 1 ? (int)Math.Ceiling(_circleRadius * 2 * circleProgress.FontSize) :
                (int)Math.Ceiling(circleProgress.FontSize * _des);
            var textWidth = _mPaint.MeasureText(circleProgress.Text);
            var fontMetrics = _mPaint.GetFontMetrics();
            var textX = (_circleRadius * 2 - textWidth) / 2;
            var textY = _circleRadius + (fontMetrics.Bottom - fontMetrics.Top) / 2 - fontMetrics.Bottom;
            canvas.DrawText(circleProgress.Text, textX, textY, _mPaint);

        }
    }
}