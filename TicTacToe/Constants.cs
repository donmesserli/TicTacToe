using System;

namespace TicTacToe
{
    internal static class Constants
    {
        public const string dingsound = "ding.wav";
        public const string badsound = "bad.wav";
        public const string winsound = "dingdingding.wav";
        public const string drawsound = "sigh.wav";

#if __IOS__
        public const string resourcePrefix = "TicTacToe.iOS.Embedded_Resources.";
#else
        public const string resourcePrefix = "TicTacToe.Droid.Embedded_Resources.";
#endif

    }
}