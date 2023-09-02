using System.Collections.Generic;

namespace DefaultNamespace
{
    public static class GameState
    {
        public static bool newGame = true;

        public static int gameMode = 0;

        public static List<List<int>> board = new List<List<int>>();
        public static List<List<int>> boardState = new List<List<int>>();

    }
}