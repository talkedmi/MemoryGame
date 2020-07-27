using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gameLogic
{
    internal class Matrix                 // change the name to board
    {
        private Cell[,] m_Board;
        private int m_NumOfCells;
        private int m_RowLength;
        private int m_ColLength;

        internal Matrix(int i_NumOfRows, int i_NumOfColumns)
        {
            this.m_RowLength = i_NumOfRows;
            this.m_ColLength = i_NumOfColumns;
            this.m_NumOfCells = i_NumOfRows * i_NumOfColumns;           
            this.m_Board = new Cell[i_NumOfRows, i_NumOfColumns];

            for (int i = 0; i < i_NumOfRows; i++)
            {
              for(int j = 0; j < i_NumOfColumns; j++)
                {
                    this.m_Board[i, j] = new Cell(new Tuple<int, int>(i, j));             
                }
            }           

            this.RandomBoard();
        }
         
        internal Cell[,] Board
        {
            get
            {
                return this.m_Board;
            }
        }

        internal int RowLength
        {
            get
            {
                return this.m_RowLength;
            }
        }

        internal int ColLength
        {
            get
            {
                return this.m_ColLength;
            }
        }

        internal int NumOfCells
        {
            get
            {
                return this.m_NumOfCells;
            }
        }

        // put random letters (pairs of A, B, ....) in the board
        internal void RandomBoard()
        {
            int amountOfLetter = this.m_NumOfCells / 2;
            Array arr = Enum.GetValues(typeof(eCardLetter));
            int[] cardLettersCounter = new int[amountOfLetter]; 
            Random rnd = new Random();
            int nextLetter = 0;

            for(int i = 0; i < this.RowLength; i++)
            {
                for(int j = 0; j < this.ColLength; j++)
                {
                    nextLetter = rnd.Next(0, amountOfLetter);
                    while (cardLettersCounter[nextLetter] == 2)
                    {
                        nextLetter = rnd.Next(0, amountOfLetter);
                    }

                    cardLettersCounter[nextLetter]++;
                    Card card = new Card((eCardLetter)arr.GetValue(nextLetter));
                    this.m_Board[i, j].HoldsCard = card;
                    card.CardLocation = this.m_Board[i, j];
                } 
            }
        }

        // choose card for the computer moves
        internal Cell RandomCell()
        {
            Random rnd = new Random();
            int randomRow = rnd.Next(0, this.RowLength);
            int randomCol = rnd.Next(0, this.ColLength);

            while (this.m_Board[randomRow, randomCol].HoldsCard.IsOpen())
            {
                randomRow = rnd.Next(0, this.RowLength);
                randomCol = rnd.Next(0, this.ColLength);
            }

            return this.m_Board[randomRow, randomCol];
        }
    }
}
