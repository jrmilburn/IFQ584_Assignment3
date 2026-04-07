namespace BoardGames
{
    public class NotaktoHelp : IHelp
    {
        public string ShowHelp()
        {
            return "Welcome to Notakto. This game is played over 3 boards and both players play with 'X' pieces. If a line is completed on one of the boards, " +
                "that board will be considered dead for the rest of the game and no pieces can be played on it. Your goal is to not be the last player to place an X on the last live board. \n\n" +
                "Moves can be played by typing 'm' or 'move' followed by the column and row you want to place your piece and the board you want to play it on. (e.g. 'm 0 1 2')\n\n" +
                "Other commands include: 'Undo', 'Redo', 'Save [filename]', 'Load [filename]', 'Help' and 'Quit'\n";
        }
    }
    public class GomokuHelp : IHelp
    {
        public string ShowHelp()
        {
            return "Welcome to Gomoku. In this game you will compete to make the first line of 5 on a 15x15 size board.\n\n Moves can be played by typing 'm' or 'move' followed by the column and row " +
                "you want to place your piece. (e.g. 'm 6 3')\n\n" +
                "Other commands include: 'Undo', 'Redo', 'Save [filename]', 'Load [filename]', 'Help' and 'Quit'\n";
        }
    }
    public class NumTTTHelp : IHelp
    {
        public string ShowHelp()
        {
            return "Welcome to NumericalTicTacToe. In this game, instead of playing X and O, you will instead be using numbers (Player 1 is odds, Player 2 is evens). The total numbers available is exual to the amount of" +
                "spaces available on a board (e.g. 3x3 board with have 9 numbers with P1: 1, 3, 5, 7, 9 and P2: 2, 4 ,6 ,8). You aim is to complete a line that equals the magic square number of your boards size (e.g. On a 3x3 board" +
                "a line will need to equal 15" +
                "\n\nMoves can be played by typing 'm' or 'move' followed by the column and row and the number you would like to play (e.g. P1: 'm 2 0 9')\n\n" +
                "Other commands include: 'Undo', 'Redo', 'Save [filename]', 'Load [filename]', 'Help' and 'Quit'\n";
        }
    }
}

