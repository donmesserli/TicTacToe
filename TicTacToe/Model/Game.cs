using System;
namespace TicTacToe.Model
{
    public class Game
    {
        public enum GameState
        {
            // The order of these matters, do not reorder them
            Playing,
            WinRow0,
            WinRow1,
            WinRow2,
            WinCol0,
            WinCol1,
            WinCol2,
            WinDiagLTR,
            WinDiagRTL,
            Draw
        }

        ///////////////////////       
        // public properties
        ///////////////////////
        // These values can't be 1 and 2 because checkIfPlayerCanWin()
        // and checkForWin() use sums can will get incorrect results
        public const int PlayerX = 1;
        public const int PlayerO = 5;
        public GameState State { get; private set; }
        public int CurrentPlayer { get; private set; }

        const int EMPTY = 0;
        // By changing these two constants, the board can be made any size.
        // They must, however, be equal.
        const int ROWS = 3;
        const int COLS = 3;
        int[,] board = new int[COLS, ROWS];
        int move = 0;
        int firstPlayer = 0; // who moves first and on even moves
        int secondPlayer = 0;
        Random random = new Random();

        public Game()
        {
            if (ROWS != COLS)
            {
                throw new Exception("Game.cs - ROWS and COLS must be equal!");
            }

            restart();
        }

        // restart (or just start) the game 
        public void restart()
        {
            move = 0;

            // clear the board
            for (int col = 0; col < COLS; col++)
            {
                for (int row = 0; row < ROWS; row++)
                {
                    board[col, row] = EMPTY;
                }
            }

            // The player to go first is random
            // I don't think generating a random int of 1 or 2 would end up being very random
            // Generate a large random number and then make the decision based on whether
            // the number is even or odd.
            int num = random.Next(100000);
            firstPlayer = num % 2 == 0 ? PlayerX : PlayerO;
            secondPlayer = firstPlayer == PlayerX ? PlayerO : PlayerX;
            CurrentPlayer = firstPlayer;

            State = GameState.Playing;
        }

        // Returns whether or now a move can be made
        public bool canMakeMove(int col, int row)
        {
            // Make sure we're playing
            // make sure there isn't a piece in that spot
            if (State == GameState.Playing && board[col, row] == EMPTY)
            {
                return true;
            }

            return false;
        }

        // Make the move
        // Returns the new state of the game
        public GameState makeMove(int col, int row)
        {
            if (canMakeMove(col, row))
            {
                board[col, row] = CurrentPlayer;

                // Check for win
                GameState state = checkForWin();

                if (state != GameState.Playing)
                {
                    State = state;
                }
                else
                {
                    // Check for a draw
                    int empties = countEmptySquares();
                    switch (empties)
                    {
                        case 0:
                            // No moves left, must be a draw
                            State = GameState.Draw;
                            break;
                        case 1:
                            // Check if other player can win
                            if (!checkIfPlayerCanWin(CurrentPlayer == PlayerX ? PlayerO : PlayerX))
                            {
                                State = GameState.Draw;
                            }

                            break;
                        default:
                            // Detecting a Draw with more than 1 empty
                            // square would require iterating all possible
                            // future moves and is beyond the scope of this exercise.
                            break;
                    }
                }
            }

            move++;
            if (State == GameState.Playing)
            {
                // It's now the other player's turn
                CurrentPlayer = move % 2 == 0 ? firstPlayer : secondPlayer;
            }            

            return State;
        }

        // Returns the number of empty squares on the board
        private int countEmptySquares()
        {
            int empties = 0;

            for (int col = 0; col < COLS; col++)
            {
                for (int row = 0; row < ROWS; row++)
                {
                    if (board[row, col] == EMPTY) { empties++; }
                }
            }

            return empties;
        }

        // Checks to see if the current player has won with the move
        // they just made.
        // Returns GameState.Playing if the moved didn't win the game.
        // Else, returns the GameState representing the column,  row, or diagonal
        // that won the game.
        private GameState checkForWin()
        {
            // target is met if there are three pieces in a row or column
            // occupied by the player
            int target = CurrentPlayer * ROWS;

            GameState state = GameState.WinCol0;
            // Check columns
            for (int col = 0; col < COLS; col++)
            {
                int sumofcol = board[col, 0] + board[col, 1] + board[col, 2];
                if (sumofcol == target)
                {
                    return state;
                }
                state++;
            }

            // Check rows
            state = GameState.WinRow0;
            for (int row = 0; row < ROWS; row++)
            {
                int sumofrow = board[0, row] + board[1, row] + board[2, row];
                if (sumofrow == target)
                {
                    return state;
                }
                state++;
            }

            // Check diagonals
            int sum = board[0, 0] + board[1, 1] + board[2, 2];
            if (sum == target)
            {
                return GameState.WinDiagLTR;
            }

            sum = board[2, 0] + board[1, 1] + board[0, 2];
            if (sum == target)
            {
                return GameState.WinDiagRTL;
            }

            return GameState.Playing;
        }

        // Checks to see it the given player can win with their next move.
        // Used to determine whether there is a draw when there is only
        // one empty square left on the board.
        // Returns true if the player can win.
        private bool checkIfPlayerCanWin(int player)
        {
            // target is met if there are two pieces in a row or column
            // occupied by the player
            int target = player * 2;

            // Check columns
            for (int col = 0; col < COLS; col++)
            {
                int sumofcol = board[col, 0] + board[col, 1] + board[col, 2];
                if (sumofcol == target)
                {
                    for (int row = 0; row < ROWS; row++)
                    {
                        if (board[col, row] == EMPTY)
                        {
                            return true;
                        }
                    }
                }
            }

            // Check rows
            for (int row = 0; row < ROWS; row++)
            {
                int sumofrow = board[0, row] + board[1, row] + board[2, row];
                if (sumofrow == target)
                {
                    // look for an empty spot in the column
                    for (int col = 0; col < COLS; col++)
                    {
                        if (board[col, row] == EMPTY)
                        {
                            return true;
                        }
                    }
                }
            }

            // Check diagonals
            int sum = board[0, 0] + board[1, 1] + board[2, 2];
            if (sum == target)
            {
                if (board[0, 0] == EMPTY || board[1, 1] == EMPTY || board[2, 2] == EMPTY)
                {
                    return true;
                }
            }

            sum = board[2, 0] + board[1, 1] + board[0, 2];
            if (sum == target)
            {
                if (board[2, 0] == EMPTY || board[1, 1] == EMPTY || board[0, 2] == EMPTY)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
