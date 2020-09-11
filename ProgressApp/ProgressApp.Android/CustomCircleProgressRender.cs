using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ProgressApp.Views;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CustomCircleProgress), typeof(ProgressApp.Droid.CustomCircleProgressRender))]
namespace ProgressApp.Droid
{
    public class CustomCircleProgressRender : ViewRenderer
    {

        CustomCircleProgress circleProgress;
        Paint _mPaint;
        double _des = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
        const float StartAngle = -90;
        int _circleRadius = 0;
        int _progressBarWidth = 0;
        public CustomCircleProgressRender(Context context) : base(context)
        {
            this.SetClipChildren(false);
            SetWillNotDraw(false);
        }


        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {

            }
            if (e.NewElement != null)
            {
                if (circleProgress == null)
                {
                    circleProgress = Element as CustomCircleProgress;
                    SetProgressBarWidthData();
                }

            }
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

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (_mPaint == null)
            {
                _mPaint = new Paint(PaintFlags.AntiAlias);
            }
            DrawProgressBarTrack(canvas);
            DrawProgressBar(canvas);
            DrawBarBackgroundColor(canvas);
        }

        protected virtual void DrawProgressBarTrack(Canvas canvas)
        {
            if (circleProgress.ProgressTrackBarColor == Xamarin.Forms.Color.Transparent)
            {
                return;
            }
            RectF oval = new RectF();
            oval.Left = 0 - (_circleRadius * 2 - Width) / 2;
            oval.Right = Width + (_circleRadius * 2 - Width) / 2;
            oval.Top = 0;
            oval.Bottom = _circleRadius * 2;
            _mPaint.Color = circleProgress.ProgressTrackBarColor.ToAndroid();
            canvas.DrawArc(oval, -90 - circleProgress.RightHalfAngle, circleProgress.RightHalfAngle * 2, true, _mPaint);
        }

        protected virtual void DrawProgressBar(Canvas canvas)
        {
            RectF oval = new RectF();
            oval.Left = 0 - (_circleRadius * 2 - Width) / 2;
            oval.Right = Width + (_circleRadius * 2 - Width) / 2;
            oval.Top = 0;
            oval.Bottom = _circleRadius * 2;
            _mPaint.Color = circleProgress.ProgressBarColor.ToAndroid();
            var startAngle = StartAngle - circleProgress.RightHalfAngle;
            var nowAngle = circleProgress.RightHalfAngle * 2 * circleProgress.Progress;
            canvas.DrawArc(oval, startAngle, nowAngle, true, _mPaint);
        }


        int _cacheRectWidth = -1;
        protected virtual void DrawBarBackgroundColor(Canvas canvas)
        {
            _mPaint.Color = circleProgress.BackgroundCircleColor.ToAndroid();
            if (circleProgress.RightHalfAngle <= 90)
            {
                RectF oval = new RectF();
                oval.Left = 0 - (_circleRadius * 2 - Width) / 2 + _progressBarWidth;
                oval.Right = Width + (_circleRadius * 2 - Width) / 2 - _progressBarWidth;
                oval.Top = 0 + _progressBarWidth;
                oval.Bottom = _circleRadius * 2 - _progressBarWidth;
                canvas.DrawArc(oval, -90 - circleProgress.RightHalfAngle, circleProgress.RightHalfAngle * 2, true, _mPaint);
            }
            else
            {
                Path path = new Path();
                path.MoveTo(Width / 2, _circleRadius);
                if (_cacheRectWidth == -1)
                {
                    double d = ((180 - circleProgress.RightHalfAngle) * Math.PI) / 180;
                    _cacheRectWidth = (int)Math.Ceiling(_circleRadius * Math.Sin(d));
                }
                path.LineTo(Width / 2 - _cacheRectWidth, Height);
                path.LineTo(Width / 2 + _cacheRectWidth, Height);
                path.Close();
                canvas.DrawPath(path, _mPaint);
                Path acrPath = new Path();
                RectF oval = new RectF();
                oval.Left = 0 - (_circleRadius * 2 - Width) / 2 + _progressBarWidth;
                oval.Right = Width + (_circleRadius * 2 - Width) / 2 - _progressBarWidth;
                oval.Top = 0 + _progressBarWidth;
                oval.Bottom = _circleRadius * 2 - _progressBarWidth;
                acrPath.AddArc(oval, -90 - circleProgress.RightHalfAngle, circleProgress.RightHalfAngle * 2);
                canvas.DrawPath(acrPath, _mPaint);
            }



        }
    }
}