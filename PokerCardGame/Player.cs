using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerCardGame
{
    internal class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; }
        public int Wallet { get; set; } // Added Wallet property

        public Player(string name)
        {
            Name = name;
            Hand = new List<Card>();
            Wallet = 0; // Default value, will be overwritten in Main
        }

        public void DisplayHand()
        {
            Console.WriteLine($"\n{Name}'s Hand:");

            // Display only the last 2 cards (or fewer if there are less than 2 cards)
            int startIndex = Math.Max(0, Hand.Count - 2);
            for (int i = startIndex; i < Hand.Count; i++)
            {
                Card card = Hand[i];
                string rankName = GetCardRankName(card.rank);
                Console.WriteLine($"{rankName} of {card.suit}");
            }
        }

        public int RaisingTheStakes()
        {
            Console.WriteLine("\nDo you want to up the ante? (y/n)");
            string answer = Console.ReadLine();
            int additionalRate = 0;

            if (answer.ToLower() == "y")
            {
                Console.WriteLine("\nHow much do you want to raise the stakes?");
                if (int.TryParse(Console.ReadLine(), out additionalRate))
                {
                    return additionalRate;
                }
                else
                {
                    Console.WriteLine("Invalid value. No additional stakes added.");
                    return 0;
                }
            }

            return additionalRate;
        }

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
    }
}
