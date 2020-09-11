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

        const float DefaultStartAngle = -90;
        int _circleRadius = 0;
        int _progressBarWidth = 0;
        int _cacheRectWidth = -1;
        float _fontSize = 0;
        SweepGradient mSweepGradient;
        ValueAnimator _progressAnimator;

        float HalfAngle => circleProgress.RightHalfAngle;

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
            //if (propName == nameof(CircleProgressView.Radius))
            //{
            //    if (circleProgress.MeasureSizeByFontsize)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        RequestLayout();
            //        Invalidate();
            //    }
            //}
        }


        /// <summary>
        /// 设置半径R和BarWidth的int值
        /// </summary>
        void SetFontSize()
        {
            _fontSize = (float)(circleProgress.FontSize * _des);
        }

        /// <summary>
        /// 设置BarWidth
        /// </summary>
        void SetBarWidth()
        {
            if (circleProgress.ProgressBarWidth < 1 && circleProgress.MeasureSizeByFontsize)
            {
                throw new Exception("when ProgressBarWidth<1,you radius must >0");
            }
            if (circleProgress.ProgressBarWidth < 1)
            {
                _progressBarWidth = (int)Math.Ceiling(_circleRadius * circleProgress.ProgressBarWidth);
            }
            else
            {
                _progressBarWidth = (int)Math.Ceiling(_des * circleProgress.ProgressBarWidth);
            }
        }

        #region OnMeasure,测算元素尺寸

        /// <summary>
        /// 当控件未设置Radius时，计算文本
        /// </summary>
        /// <returns></returns>
        protected int CalcuteRadius()
        {
            Paint lintTextPaint = new Paint();
            lintTextPaint.TextSize = _fontSize;
            lintTextPaint.LetterSpacing = circleProgress.CharacterSpace;
            var textWidth = lintTextPaint.MeasureText(circleProgress.MinWidthText);
            int minCircleWidth = (textWidth + circleProgress.TextMargin.HorizontalThickness.ToDroidInt()
                /*+ 2 * _progressBarWidth*/).ToInt();
            var text = circleProgress.Text.Replace("\\n", "\n");
            TextPaint tp = new TextPaint(lintTextPaint);
            StaticLayout staticLayout = new StaticLayout(source: text, tp,
               minCircleWidth, Alignment.AlignCenter, 1, 0, true);

            int minCircleHeight = staticLayout.Height + circleProgress.TextMargin.VerticalThickness.ToDroidInt()
                        /*+ 2 * _progressBarWidth*/;

            var centerPoint = new PointF((minCircleWidth / 2f), (minCircleHeight / 2f));
            var a = Math.Sqrt(centerPoint.X * centerPoint.X + centerPoint.Y * centerPoint.Y).ToInt();

            return a+_progressBarWidth;
        }

        /// <summary>
        /// 通过半径计算尺寸
        /// </summary>
        /// <param name="radius"></param>
        protected void MeasureSizeByRadius(int radius)
        {
            if (HalfAngle == 0) //360的圆
            {
                SetMeasuredDimension(radius * 2, radius * 2);
            }
            else if (HalfAngle <= 90)
            {
                double d = ((90 - HalfAngle) * Math.PI) / 180;
                var halfWidth = (int)Math.Ceiling(radius * Math.Cos(d));
                SetMeasuredDimension(halfWidth * 2, radius);
            }
            else
            {
                double d = ((180 - HalfAngle) * Math.PI) / 180;
                var extHeight = (int)Math.Ceiling(radius * Math.Cos(d));
                SetMeasuredDimension(radius * 2, radius + extHeight);
            }
        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            SetFontSize();
            SetBarWidth();
            _cacheRectWidth = -1;
            if (circleProgress.MeasureSizeByFontsize)
            {
                _circleRadius = CalcuteRadius();
                circleProgress.Radius = _circleRadius / _des;
                MeasureSizeByRadius(_circleRadius);
            }
            else
            {
                _circleRadius = (int)Math.Ceiling(circleProgress.Radius * _des);
                MeasureSizeByRadius(_circleRadius);
            }

        }

        #endregion


        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (_mPaint == null)
            {
                _mPaint = new Paint();
                _mPaint.Flags = PaintFlags.AntiAlias;
                _mPaint.StrokeCap = (Paint.Cap.Round);
            }
            var barRect = CalucuteBarRectRectF();
            DrawProgressBarTrack(canvas, barRect);
            DrawProgressBar(canvas, barRect);
            DrawBarBackgroundColor(canvas);
            DrawText(canvas);
            //DrawDial(canvas);
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

        /// <summary>
        /// 获取Bar的绘制区域
        /// </summary>
        /// <returns></returns>
        protected RectF CalucuteBarRectRectF()
        {
            RectF oval = new RectF();
            oval.Left = 0 - (_circleRadius * 2 - Width) / 2;
            oval.Right = Width + (_circleRadius * 2 - Width) / 2;
            oval.Top = 0;
            oval.Bottom = _circleRadius * 2;
            return oval;
        }


        #region 绘制Bar的背景色

        void DrawBarBackgroundByHalfAngle(Canvas canvas)
        {
            RectF oval = new RectF();
            oval.Left = 0 - (_circleRadius * 2 - Width) / 2 + _progressBarWidth;
            oval.Right = Width + (_circleRadius * 2 - Width) / 2 - _progressBarWidth;
            oval.Top = 0 + _progressBarWidth;
            oval.Bottom = _circleRadius * 2 - _progressBarWidth;
            if (circleProgress.RightHalfAngle <= 90)
            {
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
                acrPath.AddArc(oval, -90 - circleProgress.RightHalfAngle, circleProgress.RightHalfAngle * 2);
                canvas.DrawPath(acrPath, _mPaint);
            }

        }

        protected virtual void DrawBarBackgroundColor(Canvas canvas)
        {
            _mPaint.Color = circleProgress.BackgroundCircleColor.ToAndroid();
            _mPaint.SetShader(null);
            if (circleProgress.RightHalfAngle == 0)
            {
                var centerX = Width / 2;
                var centerY = Height / 2;
                var lastRadius = _circleRadius - _progressBarWidth;
                canvas.DrawCircle(centerX, centerY, lastRadius, _mPaint);
            }
            else
            {
                DrawBarBackgroundByHalfAngle(canvas);
            }

        }
        #endregion

        #region 绘制占位轨迹
        protected virtual void DrawProgressBarTrack(Canvas canvas, RectF barRect)
        {
            if (circleProgress.ProgressTrackBarColor == Xamarin.Forms.Color.Transparent
                || circleProgress.Progress >= 1)
            {
                return;
            }
            var centerX = barRect.Width() / 2;
            var centerY = barRect.Height() / 2;
            _mPaint.Color = circleProgress.ProgressTrackBarColor.ToAndroid();
            _mPaint.SetShader(null);
            if (circleProgress.RightHalfAngle == 0)
            {
                canvas.DrawCircle(centerX, centerY, _circleRadius, _mPaint);
            }
            else
            {
                var halfAngle = circleProgress.RightHalfAngle;
                canvas.DrawArc(barRect, DefaultStartAngle - halfAngle, halfAngle * 2, true, _mPaint);
            }
        }
        #endregion

        #region 绘制进度条
        /// <summary>
        /// 根据 RightHalfAngle 获取当前的扫描的角度
        /// </summary>
        /// <returns></returns>
        protected Tuple<float, float> GetEndAngle()
        {
            float startAngle, endAngle;
            if (circleProgress.RightHalfAngle == 0)
            {
                startAngle = DefaultStartAngle;
                endAngle = 360 * circleProgress.Progress;
            }
            else
            {
                startAngle = DefaultStartAngle - circleProgress.RightHalfAngle;
                endAngle = circleProgress.RightHalfAngle * 2 * circleProgress.Progress;
            }
            return new Tuple<float, float>(startAngle, endAngle);
        }



        /// <summary>
        /// 设置画笔的渐变
        /// </summary>
        protected void SetPaintGradient(RectF rectF)
        {
            if (circleProgress.GradientColors.Count == 0)
            {
                _mPaint.Color = circleProgress.ProgressBarColor.ToAndroid();
                return;

            }
            var centerX = Width / 2;
            var centerY = rectF.Height() / 2;
            if (mSweepGradient == null)
            {
                var colors = new int[circleProgress.GradientColors.Count];
                int count = 0;
                foreach (var item in circleProgress.GradientColors)
                {
                    colors[count] = item.ToAndroid();
                    count++;
                }
                float[] position = null;
                if (circleProgress.RightHalfAngle != 0)
                {
                    var maxCanSeeVal = circleProgress.RightHalfAngle * 2 / 360;
                    position = new float[colors.Length];
                    for (int i = 0; i < colors.Length; i++)
                    {
                        position[i] = i * (maxCanSeeVal / colors.Length);
                    }
                }
                mSweepGradient = new SweepGradient(centerX, centerY, colors: colors, positions: position);
                Matrix matrix = new Matrix();
                var offsetAngle = DefaultStartAngle;
                if (circleProgress.RightHalfAngle > 0)
                {
                    offsetAngle -= circleProgress.RightHalfAngle;
                }
                matrix.SetRotate(offsetAngle, centerX, centerY);
                mSweepGradient.SetLocalMatrix(matrix);
            }
            _mPaint.SetShader(mSweepGradient);

        }

        protected virtual void DrawProgressBar(Canvas canvas, RectF barRect)
        {
            var angleTump = GetEndAngle();
            SetPaintGradient(barRect);
            canvas.DrawArc(barRect, angleTump.Item1, angleTump.Item2, true, _mPaint);
        }


        #endregion

        #region 绘制文本
        protected virtual void DrawText(Canvas canvas)
        {
            if (string.IsNullOrWhiteSpace(circleProgress.Text))
            {
                return;
            }
            _mPaint.Color = circleProgress.TextColor.ToAndroid();
            _mPaint.TextSize = _fontSize;
            //var textWidth = _mPaint.MeasureText(circleProgress.Text);
            //var fontMetrics = _mPaint.GetFontMetrics();
            //var textX = (_circleRadius * 2 - textWidth) / 2;
            //var textY = Height / 2 + (fontMetrics.Bottom - fontMetrics.Top) / 2 - fontMetrics.Bottom;
            //canvas.DrawText(circleProgress.Text, textX, textY, _mPaint);
            TextPaint tp = new TextPaint(_mPaint);

            int textRangeWidth = Width;
            if (Width - 2 * _progressBarWidth >= 0)
            {
                textRangeWidth -= 2 * _progressBarWidth;
            }
            var text = circleProgress.Text.Replace("\\n", "\n");
            StaticLayout staticLayout = new StaticLayout(text, tp,
               textRangeWidth, Alignment.AlignCenter, 1, 0, true);
            var textHeight = staticLayout.Height;
            var centerY = (Height - textHeight) / 2;
            canvas.Save();
            canvas.Translate(_progressBarWidth, centerY);
            staticLayout.Draw(canvas);
            canvas.Restore();
        }
        #endregion

        #region 绘制刻度
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
        #endregion
    }

    public static class RenderHelper
    {
        static double des = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;

        public static int ToInt(this float val)
        {
            return (int)Math.Ceiling(val);
        }

        public static int ToInt(this double val)
        {
            return (int)Math.Ceiling(val);
        }

        public static int ToDroidInt(this float val)
        {

            return (int)Math.Ceiling(val * des);
        }

        public static int ToDroidInt(this double val)
        {
            return (int)Math.Ceiling(val * des);
        }
    }
}