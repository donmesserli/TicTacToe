using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Xamarin.Forms;
using TicTacToe.Controls;
using TicTacToe.iOS.Renderers;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TouchableBoxView), typeof(TouchableBoxViewRenderer))]
namespace TicTacToe.iOS.Renderers
{
    class TouchableBoxViewRenderer : BoxRenderer
    {
        UILongPressGestureRecognizer uiTap;
        TouchableBoxView touchableBox;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.BoxView> e)
        {
            base.OnElementChanged(e);

            touchableBox = e.NewElement as TouchableBoxView;

            uiTap = new UILongPressGestureRecognizer(Tapped);
            uiTap.MinimumPressDuration = 0;
            AddGestureRecognizer(uiTap);
        }

        private void Tapped(object sender)
        {
            if (uiTap.State == UIGestureRecognizerState.Began)
            {
                touchableBox.OnPressed();
            }
            else if (uiTap.State == UIGestureRecognizerState.Ended)
            {
                touchableBox.OnReleased();
            }
        }
    }
}
