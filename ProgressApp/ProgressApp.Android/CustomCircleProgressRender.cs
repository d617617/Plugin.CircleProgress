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

            //RectF oval = new RectF();
            //oval.Left = 0;
            //oval.Right = this.Width;
            //oval.Top = 0;
            //oval.Bottom = _circleRadius * 2;
            //_mPaint.Color = Color.Red;
            //canvas.DrawArc(oval, -90 - circleProgress.RightHalfAngle, circleProgress.RightHalfAngle * 2, true, _mPaint);
            //mPaint.Color = Color.Blue;
            //canvas.DrawCircle(this.Width / 2, this.Width / 2, this.Width / 2, mPaint);
            DrawOutsideProgressBarArc(canvas);
        }

        protected virtual void DrawOutsideProgressBarArc(Canvas canvas)
        {
            RectF oval = new RectF();
            oval.Left = 0 - (_circleRadius * 2 - Width) / 2;
            oval.Right = Width + (_circleRadius * 2 - Width) / 2;
            oval.Top = 0;
            oval.Bottom = _circleRadius * 2;
            _mPaint.Color = Color.Red;
            canvas.DrawArc(oval, -90 - circleProgress.RightHalfAngle, circleProgress.RightHalfAngle * 2, true, _mPaint);
        }
    }
}