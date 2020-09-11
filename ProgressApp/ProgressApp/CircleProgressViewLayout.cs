using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ProgressApp
{
    public class CircleProgressViewLayout : ContentView
    {
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var contentSize = this.Content.Measure(widthConstraint, heightConstraint).Request;
            return base.OnMeasure(widthConstraint, heightConstraint);
        }
    }
}
