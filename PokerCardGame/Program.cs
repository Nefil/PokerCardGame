using PokerCardGame.Classes;
using PokerCardGame.Data;
using PokerCardGame.Models;

namespace PokerCardGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Creating a database if it does not exist
            var connectionString = "Data Source=GameDB.db";
            using (var db = new GameDbContext(connectionString))
            {
                bool created = db.Database.EnsureCreated();
            }

            bool exitGame = false;
            while (!exitGame)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Poker Card Game! What are we gonna do?");
                Console.WriteLine("1. Play\n2. Exit");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1": // Play
                        Console.Clear(); 
                        PlayGame(connectionString);
                        break;

                    case "2":
                        Console.WriteLine("Thank you for visiting! Goodbye!");
                        exitGame = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please select 1 or 2.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void PlayGame(string connectionString)
        {
            Console.WriteLine("What's your name?");
            string _playerName = Console.ReadLine();
            Console.Clear(); 

            // Initialize player as null
            Player player = null;
            using (var db = new GameDbContext(connectionString))
            {
                var existingPlayer = db.Players.FirstOrDefault(p => p.PlayerName == _playerName);

                // Options for existing player
                if (existingPlayer != null)
                {
                    // If the player already exists
                    bool validChoice = false;
                    string choice;

                    while (!validChoice)
                    {
                        Console.WriteLine($"User with nickname '{_playerName}' already exists!");
                        Console.WriteLine("1. Load existing data");
                        Console.WriteLine("2. Start over (overwrite data)");
                        Console.WriteLine("3. Choose different nickname");
                        Console.WriteLine("4. Exit");

                        choice = Console.ReadLine();

                        switch (choice)
                        {
                            case "1": // Load existing data
                                player = new Player(_playerName);
                                player.Wallet = (int)existingPlayer.Money;
                                Console.WriteLine($"Welcome back, {_playerName}! Loaded wallet: {player.Wallet}$");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                                Console.Clear(); 
                                validChoice = true;
                                break;

                            case "2": // Overwrite data
                                player = new Player(_playerName);
                                player.Wallet = 500; // Default value
                                existingPlayer.Money = 500;
                                db.SaveChanges();
                                Console.WriteLine($"Hello, {_playerName}! Reset wallet to {player.Wallet}$");
                                validChoice = true;
                                break;

                            case "3": // Choose different nickname or exit
                                Console.WriteLine("Enter new nickname: ");
                                _playerName = Console.ReadLine();

                                // Check if new nickname already exists in the database
                                while (db.Players.Any(p => p.PlayerName == _playerName))
                                {
                                    Console.WriteLine("This nickname is also taken!");
                                    Console.WriteLine("Enter another nickname (or type 'exit' to quit the game): ");
                                    _playerName = Console.ReadLine();

                                    if (_playerName.ToLower() == "exit")
                                    {
                                        Console.WriteLine("Thanks for playing!");
                                        return; // Exit the game
                                    }
                                }

                                // New player - add to database
                                player = new Player(_playerName);
                                player.Wallet = 500;
                                db.Players.Add(new Players { PlayerName = _playerName, Money = 500 });
                                db.SaveChanges();
                                Console.WriteLine($"Welcome, {_playerName}! Your starting wallet: {player.Wallet}$");
                                validChoice = true;
                                break;

                            case "4": // Exit game
                                Console.WriteLine("Thanks for playing!");
                                return; // Exit the game

                            default:
                                Console.WriteLine("Invalid choice. Please select an option from 1 to 4.\n");
                                break;
                        }
                    }
                }
                else
                {
                    // New player - add to database
                    player = new Player(_playerName);
                    player.Wallet = 500;  // Set default value

                    db.Players.Add(new Players { PlayerName = _playerName, Money = 500 });
                    db.SaveChanges();
                    Console.WriteLine($"Welcome, {_playerName}! Your starting wallet: {player.Wallet}$");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear(); 
                }
            }

            LogicAI AI = new LogicAI("AI");

            bool continuePlaying = true;

            while (continuePlaying && player.Wallet > 0)
            {
                Console.Clear(); 

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

            // Update wallet balance in database at the end of the game
            using (var db = new GameDbContext(connectionString))
            {
                var playerToUpdate = db.Players.FirstOrDefault(p => p.PlayerName == player.Name);
                if (playerToUpdate != null)
                {
                    playerToUpdate.Money = player.Wallet;
                    db.SaveChanges();
                }
            }

            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }
    }
}