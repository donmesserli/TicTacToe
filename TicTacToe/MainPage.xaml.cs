using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.SimpleAudioPlayer;
using TicTacToe.Controls;
using TicTacToe.Model;
using TicTacToe.Services;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

// Implementation Note:
// Usually, a Xamarin.Forms application uses the MVVM pattern. Having a ViewModel for each Page
// and binding each control's properties (such as an Image's Source property) to public properties in
// the ViewModel.
// In the case of this app, with only one page, that would have added unnecessry complexity (i.e. 9
// extra private properties and 9 extra public properties with getter and setters. It would not have
// made the app any more readable, maintainable, or efficient. Therefore, I decided to use the MVC pattern
// and access the Images and their Source properties directly.
namespace TicTacToe
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        const string xsturn = "X's TURN";
        const string osturn = "O's TURN";
        const string xwins = "X WINS!!!!!!!";
        const string owins = "O WINS!!!!!!!";
        const string draw = "IT'S A DRAW";

        Game game;

        Image[,] images = new Image[3, 3];
        Image[] winningsquares = new Image[3];

        public ICommand SquareTouchedCommand { get; private set; }
        public ICommand NewGameCommand { get; private set; }

        // Creating an audio player just for the ding sound eliminates the lag that
        // happens the first time it's used. The ding sound is always the firt sound
        // the user will ever hear.
        ISimpleAudioPlayer dingPlayer = SoundService.Instance.CreateAudioPlayer(Constants.dingsound);

        public MainPage()
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            SquareTouchedCommand = new Command(HandleSquareTouched);
            NewGameCommand = new Command(HandleNewGame);

            BindingContext = this;
            
            fillImagesArray();            

            game = new Game();

            setMessage(game.CurrentPlayer == Game.PlayerX ? xsturn : osturn);
        }

        // Puts the Image objects in a 3x3 array so that we can
        // reference them by col,row.
        private void fillImagesArray()
        {
            images[0, 0] = image00;
            images[0, 1] = image01;
            images[0, 2] = image02;
            images[1, 0] = image10;
            images[1, 1] = image11;
            images[1, 2] = image12;
            images[2, 0] = image20;
            images[2, 1] = image21;
            images[2, 2] = image22;
        }       

        // Set the text of the message label at the top of the screen.
        private void setMessage(String message)
        {
            messageLabel.Text = message;
        }

        // Handles when the user touches a square on the board.
        public async void HandleSquareTouched(Object obj)
        {
            // Get the col, row from the CommandParameter of the Command
            string parameter = obj as string;
            int value = int.Parse(parameter);
            int col = value / 10;
            int row = value % 10;

            // If the game is over, don't do anything
            if (game.State != Game.GameState.Playing)
            {
                return;
            }

            // Interact with the Game object
            // Is the square empty?
            if (game.canMakeMove(col, row))
            {
                images[col, row].Source = game.CurrentPlayer == Game.PlayerX ? "x.png" : "o.png";
                dingPlayer.Play();
                Game.GameState state = game.makeMove(col, row);

                switch (state)
                {
                    // We're still playing
                    case Game.GameState.Playing:
                        setMessage(game.CurrentPlayer == Game.PlayerX ? xsturn : osturn);
                        break;

                    // This move won the game
                    case Game.GameState.WinCol0:
                    case Game.GameState.WinCol1:
                    case Game.GameState.WinCol2:
                    case Game.GameState.WinRow0:
                    case Game.GameState.WinRow1:
                    case Game.GameState.WinRow2:
                    case Game.GameState.WinDiagLTR:
                    case Game.GameState.WinDiagRTL:
                        setMessage(game.CurrentPlayer == Game.PlayerX ? xwins : owins);
                        getImagesForWinner(state);
                        SoundService.Instance.PlaySound(Constants.winsound);
                        await animateWinningSquares();
                        break;
                    case Game.GameState.Draw:
                        setMessage(draw);
                        // Play a sound to let them know something happened
                        SoundService.Instance.PlaySound(Constants.drawsound);
                        break;
                }
            }
            else
            {
                // The square wasn't empty. Play an annoying sound.
                SoundService.Instance.PlaySound(Constants.badsound);
            }
        }

        // Handles when the user touches the "New Game" button
        public async void HandleNewGame(Object obj)
        {
            if (game.State == Game.GameState.Playing)
            {
                bool result = await DisplayAlert("New Game", "Are you sure you want to start a new game?", "Yes", "No");
                if (result)
                {
                    startNewGame();
                }
            }
            else
            {
                startNewGame();
            }
                
        }

        private void startNewGame()
        {
            // Get rid of all the images
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    images[col, row].Source = null;
                }
            }

            // Tell the game to start over
            game.restart();

            // Figure out who's turn it is and display it
            setMessage(game.CurrentPlayer == Game.PlayerX ? xsturn : osturn);
        }

        // Given an array of the three images in the winning squares,
        // animate them by shrinking them to 80% and then back to 100%
        // three times.
        async Task animateWinningSquares()
        {
            Task[] tasks = new Task[3];

            for (int i = 0; i < 3; i++)
            {
                tasks[0] = winningsquares[0].ScaleTo(0.8, 50, Easing.Linear);
                tasks[1] = winningsquares[1].ScaleTo(0.8, 50, Easing.Linear);
                tasks[2] = winningsquares[2].ScaleTo(0.8, 50, Easing.Linear);
                await Task.WhenAll(tasks);
                await Task.Delay(500);
                tasks[0] = winningsquares[0].ScaleTo(1.0, 50, Easing.Linear);
                tasks[1] = winningsquares[1].ScaleTo(1.0, 50, Easing.Linear);
                tasks[2] = winningsquares[2].ScaleTo(1.0, 50, Easing.Linear);
                await Task.WhenAll(tasks);
            }            
        }

        // Given the winning row, column. or diagonal returned by the
        // Game object, fill an array with the images for those squares.
        // Note; squares is an instance object that keep getting reused.
        Image[] getImagesForWinner(Game.GameState state)
        {
            switch (state)
            {
                case Game.GameState.WinCol0:
                    winningsquares[0] = image00;
                    winningsquares[1] = image01;
                    winningsquares[2] = image02;
                    break;
                case Game.GameState.WinCol1:
                    winningsquares[0] = image10;
                    winningsquares[1] = image11;
                    winningsquares[2] = image12;
                    break;
                case Game.GameState.WinCol2:
                    winningsquares[0] = image20;
                    winningsquares[1] = image21;
                    winningsquares[2] = image22;
                    break;
                case Game.GameState.WinRow0:
                    winningsquares[0] = image00;
                    winningsquares[1] = image10;
                    winningsquares[2] = image20;
                    break;
                case Game.GameState.WinRow1:
                    winningsquares[0] = image01;
                    winningsquares[1] = image11;
                    winningsquares[2] = image21;
                    break;
                case Game.GameState.WinRow2:
                    winningsquares[0] = image02;
                    winningsquares[1] = image12;
                    winningsquares[2] = image22;
                    break;
                case Game.GameState.WinDiagLTR:
                    winningsquares[0] = image00;
                    winningsquares[1] = image11;
                    winningsquares[2] = image22;
                    break;
                case Game.GameState.WinDiagRTL:
                    winningsquares[0] = image02;
                    winningsquares[1] = image11;
                    winningsquares[2] = image20;
                    break;
            }

            return winningsquares;
        }
    }
}
