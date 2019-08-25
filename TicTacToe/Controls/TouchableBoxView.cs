using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace TicTacToe.Controls
{
    class TouchableBoxView : BoxView
    {
        public event EventHandler Pressed;
        public event EventHandler Released;
        public event EventHandler Clicked;        

        public static readonly BindableProperty CommandProperty =
              BindableProperty.Create("Command", typeof(ICommand), typeof(TouchableBoxView), null);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter", typeof(object), typeof(TouchableBoxView), null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public virtual async void OnPressed()
        {
            Pressed?.Invoke(this, EventArgs.Empty);
        }

        public virtual async void OnReleased()
        {
            Released?.Invoke(this, EventArgs.Empty);
            Clicked?.Invoke(this, EventArgs.Empty);
            Command?.Execute(CommandParameter);
        }
    }
}
