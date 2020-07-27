using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gameLogic
{
    internal class Cell
    {
        private int m_Row;
        private int m_Column;
        private Card m_HoldsCard;

        // first constructor
        public Cell(Tuple<int, int> i_CellLocation)
        {
            this.m_Row = i_CellLocation.Item1;
            this.m_Column = i_CellLocation.Item2;
        }

        // second constructor
        public Cell(Tuple<int, int> i_CellLocation, Card i_HoldsCard)    
        {
            this.m_Row = i_CellLocation.Item1;
            this.m_Column = i_CellLocation.Item2;
            this.m_HoldsCard = i_HoldsCard;
            this.m_HoldsCard.CardLocation = this;
        }

        internal int Row
        {
            get
            {
                return this.m_Row;
            }
        }

        internal int Column
        {
            get
            {
                return this.m_Column;
            }
        }

        // get 'holdsCard'
        internal Card HoldsCard
        {
            get
            {
                return this.m_HoldsCard;
            }

            set
            {
                this.m_HoldsCard = value;
            }
        }

        internal Tuple<int, int> getCellIndexes()
        {
            int cellRow = this.m_Row;
            int cellColumn = this.m_Column;

            return new Tuple<int, int>(cellRow, cellColumn);
        }
    }
}
