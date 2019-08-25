using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using TicTacToe.Controls;
using TicTacToe.Droid.Renderers;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TouchableBoxView), typeof(TouchableBoxViewRenderer))]
namespace TicTacToe.Droid.Renderers
{
    class TouchableBoxViewRenderer : BoxRenderer
    {
        public TouchableBoxViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.BoxView> e)
        {
            base.OnElementChanged(e);

            var touchableBox = e.NewElement as TouchableBoxView;

            var viewGroup = (global::Android.Views.ViewGroup)ViewGroup;

            if (viewGroup != null)
            {
                viewGroup.Touch += (object sender, TouchEventArgs args) =>
                {
                    if (args.Event.Action == MotionEventActions.Down)
                    {
                        touchableBox.OnPressed();
                    }
                    else if (args.Event.Action == MotionEventActions.Up)
                    {
                        touchableBox.OnReleased();
                    }
                };
            }
        }
    }
}
