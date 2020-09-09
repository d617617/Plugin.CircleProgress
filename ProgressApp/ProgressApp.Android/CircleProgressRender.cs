using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android.Animation;
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
        float _tempMaxProgress;

        const float StartAngle = -90;
        int _circleRadius = 0;
        int _progressBarWidth = 0;
        SweepGradient mSweepGradient;
        ValueAnimator _progressAnimator;
        public CircleProgressRender(Context context) : base(context)
        {
            _progressAnimator = new ValueAnimator();
            SetWillNotDraw(false);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                _progressAnimator.Update -= UpdateProgress_AnimatorUpdate;
                circleProgress.SmoothToProgressAction = null;
            }
            if (e.NewElement != null)
            {
                if (circleProgress == null)
                {
                    circleProgress = Element as CircleProgressView;
                }
                _progressAnimator.Update += UpdateProgress_AnimatorUpdate;
                circleProgress.SmoothToProgressAction = (progress, duration) =>
                {
                    SmoothToProgress(circleProgress.Progress, progress, duration);
                };
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
            SetProgressBarWidthData();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            SetProgressBarWidthData();
        }



        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (_mPaint == null)
            {
                _mPaint = new Paint();
                _mPaint.Flags = PaintFlags.AntiAlias;
            }
            DrawProgressBarTrack(canvas);
            DrawProgressBar(canvas);
            DrawBackgroundCircle(canvas);
            DrawText(canvas);
            //DrawDial(canvas);
        }


        private void DrawDial(Canvas canvas)
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


        void SetProgressBarWidthData()
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


        void SmoothToProgress(float start, float end, int duration)
        {
            _tempMaxProgress = end;
            _progressAnimator.SetFloatValues(new float[] { start, end });
            _progressAnimator.SetDuration(duration);
            _progressAnimator.Start();
        }

        void UpdateProgress_AnimatorUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs e)
        {
            var percent = (float)e.Animation.AnimatedValue;
            circleProgress.Progress = _tempMaxProgress * percent;
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
                    Matrix matrix = new Matrix();
                    matrix.SetRotate(-90,centerX,centerY);
                    mSweepGradient.SetLocalMatrix(matrix);
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