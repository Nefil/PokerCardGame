namespace PokerCardGame.Classes
{
    internal class Deck
    {
        public Random random = new Random();
        private List<Card> cards;
        public List<Card> Table;

        // Deck Constructor
        public Deck()
        {
            cards = new List<Card>();
            Table = new List<Card>();
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };

            foreach (string suit in suits)
            {
                for (int rank = 2; rank <= 14; rank++)
                {
                    cards.Add(new Card(rank, suit));
                }
            }
        }

        // Method for shuffling the deck
        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);

                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        public void DealToTable()
        {
            for (int i = 0; i < 3; i++)
            {
                Table.Add(cards[0]);
                cards.RemoveAt(0);
            }

            Console.WriteLine("Cards on table:");
            foreach (Card card in Table)
            {
                // Convert numeric value to card name
                string rankName = GetCardRankName(card.rank);
                Console.WriteLine($"{rankName} of {card.suit}");
            }
        }

        // Helper method to convert values to card names
        private string GetCardRankName(int rank)
        {
            switch (rank)
            {
                case 14: return "Ace";
                case 13: return "King";
                case 12: return "Queen";
                case 11: return "Jack";
                default: return rank.ToString();
            }
        }

        // Method for copying 3 cards from table to player's hand (without removing from table)
        public void CopyCardsFromTable(List<Card> playerHand)
        {
            if (Table == null || playerHand == null || Table.Count < 3)
            {
                Console.WriteLine("Cannot copy cards - not enough cards on the table or invalid lists.");
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                playerHand.Add(Table[i]);
            }

        }

        // Method to add 2 cards from deck to player's hand
        public void DealCardsToPlayer(List<Card> playerHand)
        {
            // Use AddCard method to add 2 cards from deck to player's hand
            AddCard(cards, cards, playerHand, 2);
        }

        public static void AddCard(List<Card> someList, List<Card> deckList, List<Card> Hand, int amount)
        {
            if (someList == null || Hand == null || someList.Count < amount)
            {
                Console.WriteLine("Cannot add cards - insufficient number of cards or invalid lists.");
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                Hand.Add(someList[0]);
                deckList.RemoveAt(0);
            }

        }
    }
}
