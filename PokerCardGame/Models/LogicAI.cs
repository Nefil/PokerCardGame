
namespace PokerCardGame.models
{
    internal class LogicAI : Player
    {
        private Random random = new Random();

        public LogicAI(string name) : base(name)
        {
            // LogicAI class constructor calls the base Player class constructor
        }

        public new int RaisingTheStakes()
        {
            int additionalRate = 0;

            // Check the value of AI's hand
            int handValue = GameLogic.CheckHand(Hand);

            // Set chance to raise based on hand
            int chanceToRaise = 0;

            switch (handValue)
            {
                case 10: // Royal Flush
                    chanceToRaise = 90;
                    break;
                case 9: // Straight Flush
                    chanceToRaise = 80;
                    break;
                case 8: // Four of a Kind
                    chanceToRaise = 75;
                    break;
                case 7: // Full House
                    chanceToRaise = 70;
                    break;
                case 6: // Flush
                    chanceToRaise = 65;
                    break;
                case 5: // Straight
                    chanceToRaise = 65;
                    break;
                case 4: // Three of a Kind
                    chanceToRaise = 65;
                    break;
                case 3: // Two Pair
                    chanceToRaise = 60;
                    break;
                case 2: // One Pair
                    chanceToRaise = 55;
                    break;
                default: // High Card
                    chanceToRaise = 30;
                    break;
            }

            // Randomly determine if AI will raise
            int randomNumber = random.Next(100); // 0-99

            if (randomNumber < chanceToRaise)
            {
                // AI decides to raise
                // Better hand allows for a higher raise
                int maxRaise = handValue * 10; // Maximum additional rate depends on hand
                additionalRate = random.Next(5, maxRaise + 1);

                string handName = GameLogic.GetHandName(handValue);
            }

            return additionalRate;
        }
    }
}
