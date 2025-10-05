using PokerCardGame.models;
using System;
using System.Numerics;

namespace PokerCardGame
{
    internal class Program
    {
        static void Main(string[] args)
        {

            SQLitePCL.Batteries.Init();
            
            using (var db = new Data.GameDbContext())
            {
                try
                {
                    db.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database creation error: {ex.Message}");
                }
            }


            Console.WriteLine("Welcome to the Poker Card Game!");
            Console.WriteLine("What's your name?");
            string playerName = Console.ReadLine();

            Player player = new Player(playerName);
            player.Wallet = 500;

            LogicAI AI = new LogicAI("AI");

            bool continuePlaying = true;

            while (continuePlaying && player.Wallet > 0)
            {
                Console.Clear(); // Clear screen before a new round

                // Base rate
                int baseRate = 10;
                int totalRate = baseRate;

                Console.WriteLine("Your wallet: {0} $", player.Wallet);
                Console.WriteLine("Basic rate: {0} $", baseRate);
                Console.WriteLine($"\n--- New Deal ---\n");
                Deck deck = new Deck();
                deck.Shuffle();

                // First deal cards to the table
                deck.DealToTable();

                // Copy cards from table to player's hand
                deck.CopyCardsFromTable(AI.Hand);
                deck.CopyCardsFromTable(player.Hand);

                // Add 2 cards from deck to player's hand
                deck.DealCardsToPlayer(player.Hand);
                deck.DealCardsToPlayer(AI.Hand);

                // Display player's hand
                player.DisplayHand();

                // Start the betting phase
                bool continueRaising = true;
                while (continueRaising)
                {
                    // Player raises
                    int playerRaise = player.RaisingTheStakes();
                    if (playerRaise > 0)
                    {
                        totalRate += playerRaise;

                        // AI raises
                        int aiRaise = AI.RaisingTheStakes();
                        if (aiRaise > 0)
                        {
                            totalRate += aiRaise;
                            Console.WriteLine("{0} raises by {1}$.\nTotal rate: {2}$", AI.Name, aiRaise, totalRate);
                        }
                        else
                        {
                            Console.WriteLine("{0} doesn't raise. Betting ends.", AI.Name);
                            continueRaising = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0} doesn't raise. Betting ends.", player.Name);
                        continueRaising = false;
                    }
                }

                Console.WriteLine("Final rate: {0}$", totalRate);

                // Check who won
                int playerHandValue = GameLogic.CheckHand(player.Hand);
                int aiHandValue = GameLogic.CheckHand(AI.Hand);

                // Get hand names
                string playerHandName = GameLogic.GetHandName(playerHandValue);
                string aiHandName = GameLogic.GetHandName(aiHandValue);

                // Display AI's hand (only last 2 cards)
                AI.DisplayHand();

                bool playerWon = false;

                if (playerHandValue > aiHandValue)
                {
                    Console.WriteLine("You won! Your hand is better: {0}", playerHandName);
                    Console.WriteLine("AI had: {0}", aiHandName);
                    playerWon = true;
                }
                else if (playerHandValue < aiHandValue)
                {
                    Console.WriteLine("You lost. AI has a better hand: {0}", aiHandName);
                    Console.WriteLine("You had: {0}", playerHandName);
                    playerWon = false;
                }
                else // If hands are equivalent, check kickers
                {
                    Console.WriteLine("Both have the same hand: {0}", playerHandName);

                    // Compare kickers - sort cards by descending value
                    var playerSortedCards = player.Hand.OrderByDescending(c => c.rank).ToList();
                    var aiSortedCards = AI.Hand.OrderByDescending(c => c.rank).ToList();

                    bool isDraw = true;

                    // Compare cards from highest
                    for (int i = 0; i < playerSortedCards.Count; i++)
                    {
                        if (playerSortedCards[i].rank > aiSortedCards[i].rank)
                        {
                            Console.WriteLine("You have a higher card: {0}", GameLogic.GetCardName(playerSortedCards[i].rank));
                            isDraw = false;
                            playerWon = true;
                            break;
                        }
                        else if (playerSortedCards[i].rank < aiSortedCards[i].rank)
                        {
                            Console.WriteLine("AI has a higher card: {0}", GameLogic.GetCardName(aiSortedCards[i].rank));
                            isDraw = false;
                            playerWon = false;
                            break;
                        }
                    }

                    if (isDraw)
                    {
                        Console.WriteLine("It's a tie! Your hands and highest cards are identical.");
                        playerWon = true; // In a tie, player doesn't lose money
                    }
                }

                // Update wallet
                if (!playerWon)
                {
                    player.Wallet -= totalRate;
                    Console.WriteLine("You lost {0} $. Your wallet: {1} $", totalRate, player.Wallet);
                }
                else
                {
                    player.Wallet += totalRate;
                    Console.WriteLine("You won {0} $! Your wallet: {1} $", totalRate, player.Wallet);
                }

                // Clear hands before new game
                player.Hand.Clear();
                AI.Hand.Clear();

                // Check if player wants to continue
                if (player.Wallet <= 0)
                {
                    Console.WriteLine("Your wallet is empty! Game over.");
                    break;
                }

                Console.WriteLine("Continue playing? y/n");
                string response = Console.ReadLine().ToLower();
                continuePlaying = response != "n";
            }

            Console.WriteLine("Thank you for playing!");
        }
    }
}