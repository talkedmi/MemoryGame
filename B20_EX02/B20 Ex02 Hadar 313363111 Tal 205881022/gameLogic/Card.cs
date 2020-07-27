using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gameLogic
{
    internal class Card
    {
        private eCardLetter m_Letter;
        private bool m_Open;
        private Cell m_CardLocation;

        internal static bool CompareCards(Card i_FirstCard, Card i_SecondCard, Player i_Player)
        {
            bool flag = false;

            if (i_FirstCard.m_Letter == i_SecondCard.m_Letter)
            {
                i_FirstCard.m_Open = true;
                i_SecondCard.m_Open = true;
                i_Player.UpdateScore(i_Player);
                flag = true;
            }

            return flag;
        }

        public Card(eCardLetter i_CardLetter)
        {
            this.m_Letter = i_CardLetter;
            this.m_Open = false;
        }

        // get the letter on the card
        internal eCardLetter Letter
        {
            get
            {
                return this.m_Letter;
            }
        }

        // get and set this.open
        internal bool Open
        {
            get
            {
                return this.m_Open;
            }

            set
            {
                this.m_Open = value;
            }
        }

        // get card location in the matrix
        internal Cell CardLocation
        {
            get
            {
                return this.m_CardLocation;
            }

            set
            {
                this.m_CardLocation = value;
            }
        }

        // if the card is already open - the pleyer should choose another one
        internal bool IsOpen()
        {
            return this.Open;
        }
    }
}
