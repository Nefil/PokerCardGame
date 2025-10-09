
namespace PokerCardGame.models
{
    internal class Card
    {
        public int rank;
        public string suit;

        // Card Constructor
        public Card(int rank, string suit)
        {
            this.rank = rank;
            this.suit = suit;
        }
    }
}
