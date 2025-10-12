namespace PokerCardGame.Classes
{
    internal class GameLogic
    {
        public static int CheckHand(List<Card> hand)
        {
            if (royalFlush(hand))
                return 10;
            else if (straightFlush(hand))
                return 9;
            else if (fourOfAKind(hand))
                return 8;
            else if (fullHouse(hand))
                return 7;
            else if (flush(hand))
                return 6;
            else if (straight(hand))
                return 5;
            else if (threeOfAKind(hand))
                return 4;
            else if (twoPair(hand))
                return 3;
            else if (onePair(hand))
                return 2;
            else
                return 1; // high card
        }

        // Helper method to get the name of the hand
        public static string GetHandName(int handValue)
        {
            switch (handValue)
            {
                case 10: return "Royal Flush";
                case 9: return "Straight Flush";
                case 8: return "Four of a Kind";
                case 7: return "Full House";
                case 6: return "Flush";
                case 5: return "Straight";
                case 4: return "Three of a Kind";
                case 3: return "Two Pair";
                case 2: return "One Pair";
                default: return "High Card";
            }
        }

        // Helper method to get card name
        public static string GetCardName(int cardValue)
        {
            switch (cardValue)
            {
                case 14: return "Ace";
                case 13: return "King";
                case 12: return "Queen";
                case 11: return "Jack";
                default: return cardValue.ToString();
            }
        }

        public static bool royalFlush(List<Card> hand)
        {
            // Royal Flush: 10, J, Q, K, A of the same suit
            if (!flush(hand)) return false;

            var ranks = hand.Select(c => c.rank).OrderBy(r => r).ToList();
            return ranks.SequenceEqual(new[] { 10, 11, 12, 13, 14 });
        }

        public static bool straightFlush(List<Card> hand)
        {
            // Straight Flush: 5 consecutive cards of the same suit
            return flush(hand) && straight(hand);
        }

        public static bool fourOfAKind(List<Card> hand)
        {
            // Four of a Kind: 4 cards of the same rank
            return hand.GroupBy(c => c.rank)
                      .Any(g => g.Count() == 4);
        }

        public static bool fullHouse(List<Card> hand)
        {
            // Full House: 3 cards of one rank and 2 cards of another rank
            var groups = hand.GroupBy(c => c.rank).ToList();
            return groups.Count == 2 && groups.Any(g => g.Count() == 3);
        }

        public static bool flush(List<Card> hand)
        {
            // Flush: 5 cards of the same suit
            return hand.GroupBy(c => c.suit).Count() == 1;
        }

        public static bool straight(List<Card> hand)
        {
            // Straight: 5 consecutive cards
            var orderedRanks = hand.Select(c => c.rank).OrderBy(r => r).ToList();

            // Check for normal straight
            bool isNormalStraight = true;
            for (int i = 0; i < orderedRanks.Count - 1; i++)
            {
                if (orderedRanks[i] + 1 != orderedRanks[i + 1])
                {
                    isNormalStraight = false;
                    break;
                }
            }

            // Check for straight with Ace low (A-2-3-4-5)
            bool isAceLowStraight = orderedRanks.Contains(14) && // Ace
                                    orderedRanks.Contains(2) &&
                                    orderedRanks.Contains(3) &&
                                    orderedRanks.Contains(4) &&
                                    orderedRanks.Contains(5);

            return isNormalStraight || isAceLowStraight;
        }

        public static bool threeOfAKind(List<Card> hand)
        {
            // Three of a Kind: 3 cards of the same rank
            return hand.GroupBy(c => c.rank)
                      .Any(g => g.Count() == 3);
        }

        public static bool twoPair(List<Card> hand)
        {
            // Two Pair: 2 cards of one rank and 2 cards of another rank
            return hand.GroupBy(c => c.rank)
                      .Count(g => g.Count() == 2) == 2;
        }

        public static bool onePair(List<Card> hand)
        {
            // One Pair: 2 cards of the same rank
            return hand.GroupBy(c => c.rank)
                      .Any(g => g.Count() == 2);
        }

        public static int highCard(List<Card> hand)
        {
            // Returns the value of the highest card in hand
            if (hand == null || hand.Count == 0)
                return 0;

            return hand.Max(c => c.rank);
        }

        // Helper method to get the value of the highest card in any hand
        public int GetHighCardValue(List<Card> hand)
        {
            return highCard(hand);
        }
    }
}
