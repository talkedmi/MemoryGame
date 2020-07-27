using System;

namespace gameLogic
{
    public class GameBoard
    {
        private Matrix m_GameMatrix;
        private Player m_PlayerOne;
        private Player m_playerTwo;
        
        public GameBoard(int i_rowNumber, int i_colNumber) // does it have to be public?
        {
            this.m_GameMatrix = new Matrix(i_rowNumber, i_colNumber);
            this.m_PlayerOne = new Player(null, false, true);
            this.m_playerTwo = new Player(null, false, false);   
        }

        public GameBoard()
        {
            this.m_GameMatrix = new Matrix(4, 4); // default
            this.m_PlayerOne = new Player(null, false, true);
            this.m_playerTwo = new Player(null, false, false);
        }

        internal Matrix GameMatrix
        {
            get
            {
                return this.m_GameMatrix;
            }
        }

        internal Player FirstPlayer
        {
            get
            {
                return this.m_PlayerOne;
            }
        }

        internal Player SecondPlayer
        {
            get
            {
                return this.m_playerTwo;
            }
        }

        private static bool validCharacters(string i_Cell)
        {
            return i_Cell.Length == 2 && char.IsLetter(i_Cell[0]) && char.IsUpper(i_Cell[0]) && char.IsNumber(i_Cell[1]);
        }

        internal void StartGame() // check access modifier
        {
            Console.WriteLine("Hello, please enter your name");
            this.FirstPlayer.Name = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Hello " + this.FirstPlayer.Name + "! \n" +
                "press 1 if you want to play against your friend or press 2 if you want to play against the computer");
            Console.WriteLine();
            string gameType = Console.ReadLine();
            while (gameType != "1" && gameType != "2")
            {
                Console.WriteLine("invalid input please, \n" +
                    "press 1 if you want to play against your friend or press 2 if you want to play against the computer");
                gameType = Console.ReadLine();
            }

            if (gameType == "1")
            {
                Console.WriteLine("please enter your player 2 name");
                this.SecondPlayer.Name = Console.ReadLine();
            }
            else
            {
                this.SecondPlayer.IsComputer = true;
                this.SecondPlayer.Name = "Computer";
            }

            Console.WriteLine();
            Console.WriteLine("\n" + "Alright!!! Lets play!");
            Console.WriteLine();
            this.initGameBoardDisplay();
            Console.WriteLine("\n" + "You can press 'Q' anytime you want to quit the game");
        }

        private Tuple<int, int> pickACell()
        {
            Console.WriteLine("\n" + "Please pick a cell you would like to flip on the board in the current format: \n" +
                "column number, row number. for example: B3");
            string cellChoice = Console.ReadLine();

            if (cellChoice != "Q")
            {
                cellChoice = this.isValidCell(cellChoice);
                char columnChar = cellChoice[0];
                char rowChar = cellChoice[1];
                int columnInteger = cellChoice[0] - 65;
                int rowInteger = cellChoice[1] - 48 - 1;
                Ex02.ConsoleUtils.Screen.Clear();
                this.openPlayersCell(this, rowInteger, columnInteger);

                return new Tuple<int, int>(rowInteger, columnInteger);
            }
            else
            {
                System.Environment.Exit(0);
            }

            return null;
        }

        private string isValidCell(string io_CellChoice)
        {
            bool open = false;
            while (!(validCharacters(io_CellChoice) && this.isInsideBoard(io_CellChoice) && !(open = this.isOpen(io_CellChoice))))
            {
                if (!open)
                {
                    Console.WriteLine("Invalid input, try again: Please pick a cell you would like to flip on the board in the current format: \n" +
                                           "column number, row number. for example: B3");
                    io_CellChoice = Console.ReadLine();
                    continue;
                }
                else
                {
                    Console.WriteLine("Card is already open, Please pick a cell you would like to flip on the board in the current format: \n" +
                                    "column number, row number. for example: B3");
                    io_CellChoice = Console.ReadLine();
                    continue;
                }
            }

            return io_CellChoice;
        }

        private bool isInsideBoard(string io_Cell)
        {
            int col = (int)(io_Cell[0] - 65);
            int row = int.Parse(io_Cell.Substring(1)) - 1;

            return col < this.GameMatrix.ColLength && row < this.GameMatrix.RowLength && col >= 0 && row >= 0;
        }

        private bool isOpen(string i_Cell)
        {
            return this.GameMatrix.Board[i_Cell[1] - 48 - 1, i_Cell[0] - 65].HoldsCard.IsOpen();
        }

        private bool playerCurrTurn(Player i_Player)
        {
            bool cardsMatch = false;

            if (!i_Player.IsComputer)
            {
                Tuple<int, int> cellOneIndices = this.pickACell();
                Tuple<int, int> cellTwoIndices = this.pickACell();

                cardsMatch = Card.CompareCards(this.GameMatrix.Board[cellOneIndices.Item1, cellOneIndices.Item2].HoldsCard, this.GameMatrix.Board[cellTwoIndices.Item1, cellTwoIndices.Item2].HoldsCard, i_Player);

                if (!cardsMatch)
                {
                    System.Threading.Thread.Sleep(2000);
                    this.GameMatrix.Board[cellOneIndices.Item1, cellOneIndices.Item2].HoldsCard.Open = false;
                    this.GameMatrix.Board[cellTwoIndices.Item1, cellTwoIndices.Item2].HoldsCard.Open = false;
                    Ex02.ConsoleUtils.Screen.Clear();
                    this.printBoard(this);
                }
            }

            Ex02.ConsoleUtils.Screen.Clear();
            this.printBoard(this);
            return cardsMatch;
        }

        private bool computerCurrTurn(Player i_Player)
        {
            bool cardsMatch = false;

            if (i_Player.IsComputer)
            {
                Card computerFirstPick = i_Player.ComputerMove(this.GameMatrix);
                i_Player.insertCardData(computerFirstPick);
                Tuple<int, char, Card> alreadyOpenedCardWithSameLetter = i_Player.doesComputerSawOpenedCellLetter(computerFirstPick);

                Ex02.ConsoleUtils.Screen.Clear();
                this.printBoard(this);
                System.Threading.Thread.Sleep(2000);
                Ex02.ConsoleUtils.Screen.Clear();
                Card computerSecondPick;

                // added for ai 
                if (alreadyOpenedCardWithSameLetter == null) 
                {
                    computerSecondPick = i_Player.ComputerMove(this.GameMatrix);
                    i_Player.insertCardData(computerSecondPick);
                }
                else
                {
                    computerSecondPick = this.m_GameMatrix.Board[alreadyOpenedCardWithSameLetter.Item1, alreadyOpenedCardWithSameLetter.Item2 - 65].HoldsCard;
                    computerSecondPick.Open = true;
                }

                this.printBoard(this);
                
                cardsMatch = Card.CompareCards(computerFirstPick, computerSecondPick, i_Player);

                if (!cardsMatch)
                {
                    Ex02.ConsoleUtils.Screen.Clear();
                    this.printBoard(this);
                    System.Threading.Thread.Sleep(2000);
                    computerFirstPick.Open = false;
                    computerSecondPick.Open = false;
                    Ex02.ConsoleUtils.Screen.Clear();
                    this.printBoard(this);
                }
            }
            
            return cardsMatch;
        }

        private void initGameBoardDisplay()
        {
            this.GameMatrix.RandomBoard();
            this.columnMarkUp(this.GameMatrix.Board.GetLength(1));
            for (int i = 0; i < this.GameMatrix.Board.GetLength(0); i++)
            {
                this.lineBreaker(this.GameMatrix.Board.GetLength(1));
                Console.Write(i + 1 + " ");
                for (int j = 0; j < this.m_GameMatrix.Board.GetLength(1); j++)
                {
                    Console.Write("|   ");
                }

                Console.Write("|");
                Console.WriteLine();
            }

            this.lineBreaker(this.GameMatrix.Board.GetLength(1));
        }

        private void openPlayersCell(GameBoard io_board, int i_row, int i_col)
        {
            this.columnMarkUp(io_board.GameMatrix.Board.GetLength(1));
            for (int i = 0; i < io_board.m_GameMatrix.Board.GetLength(0); i++)
            {
                this.lineBreaker(io_board.m_GameMatrix.Board.GetLength(1));
                Console.Write(i + 1 + " ");
                for (int j = 0; j < io_board.m_GameMatrix.Board.GetLength(1); j++)
                {
                    if (io_board.GameMatrix.Board[i, j].HoldsCard.Open == true)
                    {
                        Console.Write("| " + io_board.GameMatrix.Board[i, j].HoldsCard.Letter + " ");
                    }
                    else
                    {
                        if (i == i_row && j == i_col)
                        {
                            io_board.GameMatrix.Board[i_row, i_col].HoldsCard.Open = true;
                            Console.Write("| " + io_board.GameMatrix.Board[i_row, i_col].HoldsCard.Letter + " ");
                        }
                        else
                        {
                            Console.Write("|   ");
                        }
                    }
                }

                Console.Write("|");
                Console.WriteLine();
            }

            this.lineBreaker(io_board.m_GameMatrix.Board.GetLength(1));
        }

        private void printBoard(GameBoard io_board)
        {
            this.columnMarkUp(io_board.GameMatrix.Board.GetLength(1));
            for (int i = 0; i < io_board.GameMatrix.Board.GetLength(0); i++)
            {
                this.lineBreaker(io_board.GameMatrix.Board.GetLength(1));
                Console.Write(i + 1 + " ");
                for (int j = 0; j < io_board.GameMatrix.Board.GetLength(1); j++)
                {
                    if (io_board.GameMatrix.Board[i, j].HoldsCard.Open == true)
                    {
                        Console.Write("| " + io_board.GameMatrix.Board[i, j].HoldsCard.Letter + " ");
                    }
                    else
                    {
                        Console.Write("|   ");
                    }
                }

                Console.Write("|");
                Console.WriteLine();
            }

            this.lineBreaker(io_board.GameMatrix.Board.GetLength(1));
        }

        private void lineBreaker(int i_length)
        {
            Console.Write("  ");
            for (int i = 0; i < ((i_length * 4) + 1); i++)
            {
                Console.Write("=");
            }

            Console.WriteLine();
        }

        private void columnMarkUp(int i_length)
        {
            Console.Write("    ");
            for (int i = 0; i < i_length; i++)
            {
                Console.Write((char)(i + 65) + "   ");
            }

            Console.WriteLine();
        }

        internal int gameGridInputValidity(string io_userInput)
        {
            bool validInput = false;
            while (!validInput)
            {
                if (io_userInput.Length != 1)
                {
                    Console.WriteLine("Invalid input, Please Enter Two integer numbers Between 4-6 to for the game board number of rows and numer of columns , and the number of cells must be even");
                    io_userInput = Console.ReadLine();
                    continue;
                }
                else
                {
                    if (io_userInput[0] - 48 < 4 || io_userInput[0] - 48 > 6)
                    {
                        Console.WriteLine("Invalid input, Please Enter Two integer numbers Between 4-6 to for the game board number of rows and numer of columns , and the number of cells must be even");
                        io_userInput = Console.ReadLine();
                        continue;
                    }
                }

                validInput = true;
            }

            return io_userInput[0] - 48;
        }

        public void gameFlow()
        {
            while ((this.FirstPlayer.Score + this.SecondPlayer.Score) != (this.GameMatrix.NumOfCells / 2))
            {
                bool anotherTurn = false;

                if (this.FirstPlayer.Turn)
                {
                    Console.WriteLine("\n" + this.FirstPlayer.Name + "'s Turn");
                    anotherTurn = this.playerCurrTurn(this.FirstPlayer);
                    if (!anotherTurn)
                    {
                        this.FirstPlayer.Turn = false;
                        this.SecondPlayer.Turn = true;
                    }
                }
                else
                {
                    if (this.SecondPlayer.Turn)
                    {
                        Console.WriteLine("\n" + this.SecondPlayer.Name + "'s Turn");
                        System.Threading.Thread.Sleep(1250);
                        if (this.SecondPlayer.IsComputer)
                        {
                            anotherTurn = this.computerCurrTurn(this.SecondPlayer);
                        }
                        else
                        {
                            anotherTurn = this.playerCurrTurn(this.SecondPlayer);
                        }

                        if (!anotherTurn)
                        {
                            this.SecondPlayer.Turn = false;
                            this.FirstPlayer.Turn = true;
                        }
                    }
                }

                this.showScoreBoard();
            }
           
            Console.WriteLine("Game over: \n \n" + this.FirstPlayer.Name + "'s points: " + this.FirstPlayer.Score +
                "\n" + this.SecondPlayer.Name + "'s points: " + this.SecondPlayer.Score);
            Console.WriteLine("Would you like to play another game?");
            Console.WriteLine("Press Y if you want to keep playing");
            Console.WriteLine("Press N if you hwold like to quit game");
            string startNewGameUserInput = Console.ReadLine();
            startNewGameUserInput = this.newGameInputValidty(startNewGameUserInput);
            this.playAnotherGame(startNewGameUserInput);
        }

        public void playAnotherGame(string io_userInput)
        {
            if (io_userInput == "Y")
            {
                Ex02.ConsoleUtils.Screen.Clear();
                GameBoard gameBoard = new GameBoard(4, 4);
                gameBoard.StartGame();
                gameBoard.gameFlow();
                Console.ReadLine();
            }

            if (io_userInput == "N")
            {
                // terminate
                Console.WriteLine("Bye-Bye");
                System.Threading.Thread.Sleep(2000);
                System.Environment.Exit(0);
            }
        }

        public string newGameInputValidty(string io_userInput)
        {
            while (io_userInput != "N" && io_userInput != "Y")
            {
                Console.WriteLine("invalid input");
                Console.WriteLine("Press Y if you want to keep playing");
                Console.WriteLine("Press N if you hwold like to quit game");
                io_userInput = Console.ReadLine();
            }

            return io_userInput;
        }

        public GameBoard gameBoardInit(int i_rowNumber, int colNumber)
        {
            GameBoard gameBoard = new GameBoard(i_rowNumber, i_rowNumber);

            return gameBoard;
        }

        internal Tuple<int, int> InputSizeOfBoard()
        {
            Console.WriteLine("Please Enter Two integer numbers Between 4-6 to for the game board number of rows and numer of columns , and the number of cells must be even");
            int inputNumberOfRows = 0;
            int inputNumberOfColumns = 0;
            string inputNumberOfRowsString = Console.ReadLine();
            inputNumberOfRows = this.gameGridInputValidity(inputNumberOfRowsString);

            string inputNumberOfColumnsString = Console.ReadLine();
            inputNumberOfColumns = this.gameGridInputValidity(inputNumberOfColumnsString);

            return new Tuple<int, int>(inputNumberOfRows, inputNumberOfColumns);
        }

        private Tuple<int, int> sizeInputValidity(string io_RowInput, string io_ColumnInput)
        {
            bool validInput = false;

            while (!validInput)
            {
                if (io_RowInput.Length != 1 || io_ColumnInput.Length != 1)
                {
                    Console.WriteLine("Invalid input, please Enter Two integer numbers Between 4-6 to for the game board number of rows and numer of columns," +
                        " and the number of cells must be even");
                    io_RowInput = Console.ReadLine();
                    io_ColumnInput = Console.ReadLine();
                    continue;
                }
                else
                {
                    if (io_RowInput[0] - 48 < 4 || io_RowInput[0] - 48 > 6 || io_ColumnInput[0] - 48 < 4 || io_ColumnInput[0] - 48 > 6)
                    {
                        Console.WriteLine("Invalid input, please Enter Two integer numbers Between 4-6 to for the game board number of rows and numer of columns," +
                            " and the number of cells must be even");
                        io_RowInput = Console.ReadLine();
                        io_ColumnInput = Console.ReadLine();
                        continue;
                    }
                }

                validInput = true;
            }

            return new Tuple<int, int>(io_RowInput[0] - 48, io_ColumnInput[0] - 48);
        }

        private void showScoreBoard()
        {
            Console.WriteLine("Current Score: \n \n" + this.FirstPlayer.Name + "'s points: " + this.FirstPlayer.Score +
                "\n" + this.SecondPlayer.Name + "'s points: " + this.SecondPlayer.Score);
        }
    }
}
