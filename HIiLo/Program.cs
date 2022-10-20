namespace HiLo;

internal class Program
{
    static void Main(string[] args)
    {
        _ = new HiLoGame();
    }
}

internal class HiLoGame
{
    private static int RangeStart { get; set; } = 1;
    private static int RangeEnd { get; set; } = 100;
    private static int MysteryNumber { get; set; }

    private static List<Player> PlayerList { get; set; } = new();
    private static List<int> Attempts { get; set; } = new();
    private static List<MenuItem> StartMenuItems { get; set; } = new()
    {
        new MenuItem("Setup Mystery Number Range", () => SetupRange()),
        new MenuItem("Add Player To Match", () => AddPlayer()),
        new MenuItem("Remove Existing Player", () => RemovePlayer()),
        new MenuItem("Check Score Board", () => PrintPlayersScore()),
        new MenuItem("Start Match", () => StartGame()),
        new MenuItem("Exit Game", () => CloseGame())
    };

    public HiLoGame() => StartInitialMenu();

    private static void StartInitialMenu(int selectedItemIndex = 0)
    {
        Console.Clear();

        Console.WriteLine("****************************************************************************************************************");
        Console.WriteLine("Welcome to Hi-Lo! Please choose a range for your mystery number! or try to guess the number between 1 - 100. \n" +
                  "The goal of the game is to discover the mystery number in a minimum of iterations, lets start the game!");
        Console.WriteLine("**************************************************************************************************************** \n");
        Console.WriteLine("Please Choose one of the options below: \n");

        foreach (MenuItem item in StartMenuItems)
        {
            if (item == StartMenuItems[selectedItemIndex])
                Console.Write("> ");
            else
                Console.Write(" ");

            Console.WriteLine(item.Name);
        }

        ConsoleKeyInfo keyinfo = Console.ReadKey();

        do
        {
            switch (keyinfo.Key)
            {
                case ConsoleKey.DownArrow:
                    if (selectedItemIndex + 1 < StartMenuItems.Count)
                    {
                        selectedItemIndex++;
                        StartInitialMenu(selectedItemIndex);
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (selectedItemIndex - 1 >= 0)
                    {
                        selectedItemIndex--;
                        StartInitialMenu(selectedItemIndex);
                    }
                    break;
                case ConsoleKey.Enter:
                    StartMenuItems[selectedItemIndex].ItemAction.Invoke();
                    selectedItemIndex = 0;
                    break;
                default:
                    StartInitialMenu(selectedItemIndex);
                    break;
            }
        } while (keyinfo.Key != ConsoleKey.Escape);

        CloseGame();
    }
   
    public static void StartGame()
    {
        Console.Clear();
        Console.WriteLine("****************************************************************************************************************");
        Console.WriteLine($"Let's start this match! the mystery number is between { RangeStart } and { RangeEnd } try to guess the number to win the match!");
        Console.WriteLine("**************************************************************************************************************** \n\n");

        GenerateMysteryNumber();

        bool rightGuess = false;

        while (rightGuess == false)
        {
            foreach (var player in PlayerList)
            {
                Console.WriteLine($"{player.Name} what is your guess for the mystery number?");

                int guess;

                while (!int.TryParse(Console.ReadLine(), out guess))
                {
                    Console.WriteLine("\n");
                    Console.WriteLine($"{player.Name} please type a valid number between {RangeStart} and {RangeEnd} \n");
                }

                player.CurrentGuess = guess;
            }

            Console.WriteLine($"After a carefull evaluation the result is: \n");

            foreach (var player in PlayerList)
            {
                if (player.CurrentGuess > MysteryNumber)
                { 
                    Console.WriteLine($"{ player.Name } the number: { player.CurrentGuess } is bigger than the mystery number \n");
                }
                else if (player.CurrentGuess < MysteryNumber)
                {
                    Console.WriteLine($"{ player.Name } the number: { player.CurrentGuess } is smaller than the mystery number \n");
                }
                else
                {
                    Console.WriteLine($"Congratulations { player.Name } you are the winner and you guessed right! The mystery number is: { player.CurrentGuess } Keep up with the good work! \n");
                    Thread.Sleep(1300);
                    player.AddWin();
                    rightGuess = true;
                }
            }
        }

        PrintMessageReturnToInitialMenu("Let's get you back to the initial menu you can start another match from there!");
    }

    private static void CloseGame()
    {
        Console.Clear();
        Console.Write("Thank you for playing Hi-Lo The Guessing Game, I hope to see you soon again!");
        Thread.Sleep(1300);
        Environment.Exit(0);
    }

    private static void PrintPlayersScore()
    {
        Console.Clear();

        if (!PlayerList.Any())
            PrintMessageReturnToInitialMenu("There are no players added to this game!");

        if (PlayerList.Count > 1)
            PlayerList.Sort((x, y) => y.TotalWin.CompareTo(x.TotalWin));

        int PlayerRank = 1;

        PlayerList.ForEach(player =>
        {
            Console.WriteLine($"{ PlayerRank } - { player.Name } : Total Win { player.TotalWin }");
        });  

        Console.ReadKey();
        StartInitialMenu();
    }

    private static void PrintMessageReturnToInitialMenu(string message)
    {
        Console.Write($"{message} \n");
        Thread.Sleep(1300);
        StartInitialMenu();
    }

    #region << -- Manage Players -- >> 

    public static void AddPlayer()
    {
        string? playerName = string.Empty;

        while (string.IsNullOrWhiteSpace(playerName))
        {
            Console.Clear();
            Console.WriteLine("****************************************************************************************************************");
            Console.WriteLine("Please type the Player Name: ");
            Console.WriteLine("****************************************************************************************************************");

            playerName = Console.ReadLine();

            if (PlayerList.Where(e => e.Name.ToUpper() == playerName?.ToUpper()).Any())
            {
                playerName = string.Empty;
                Console.WriteLine("The Player Name must be unique, please type a new name! Press Enter to Continue.");
                Thread.Sleep(1300);
            }
        }

        PlayerList.Add(new Player(playerName));
        PrintMessageReturnToInitialMenu("The new player was added!");
    }

    public static void RemovePlayer(int selectedPlayerIndex = 0)
    {
        Console.Clear();

        if (!PlayerList.Any())
            PrintMessageReturnToInitialMenu("There are no players added to this game!");

        Console.WriteLine("****************************************************************************************************************");
        Console.WriteLine("Please choose the player you wish to remove: ");
        Console.WriteLine("****************************************************************************************************************");

        foreach (Player player in PlayerList)
        {
            if (player == PlayerList[selectedPlayerIndex])
                Console.Write("> ");
            else
                Console.Write(" ");

            Console.WriteLine(player.Name);
        }

        ConsoleKeyInfo keyinfo = Console.ReadKey();

        do
        {
            switch (keyinfo.Key)
            {
                case ConsoleKey.DownArrow:
                    if (selectedPlayerIndex + 1 < StartMenuItems.Count)
                    {
                        selectedPlayerIndex++;
                        RemovePlayer(selectedPlayerIndex);
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (selectedPlayerIndex - 1 >= 0)
                    {
                        selectedPlayerIndex--;
                        RemovePlayer(selectedPlayerIndex);
                    }
                    break;
                case ConsoleKey.Enter:
                    PlayerList.Remove(PlayerList[selectedPlayerIndex]);
                    selectedPlayerIndex = 0;
                    PrintMessageReturnToInitialMenu("The player was succesfully removed!");
                    break;
                default:
                    RemovePlayer(selectedPlayerIndex);
                    break;
            }
        } while (keyinfo.Key != ConsoleKey.Escape);

        StartInitialMenu();
    }

    #endregion

    #region << -- Mystery Number Setup -- >>

    private static void SetupRange()
    {
        Console.Clear();
        Console.Write($"Please type the number for the start of the range: \n");
        RangeStart = SetRangeNumber();
        Console.Write($"Please type the number for the end of the range: \n");
        RangeEnd = SetRangeNumber();

        while (RangeEnd <= RangeStart)
        {
            Console.Write($"Please type a number bigger than: {RangeStart} \n");
            RangeEnd = SetRangeNumber();
        }

        PrintMessageReturnToInitialMenu("The new range is setup!");
    }

    private static int SetRangeNumber()
    {
        int inputNumber = 0;

        while (inputNumber == 0)
            if (!int.TryParse(Console.ReadLine(), out inputNumber))
                Console.Write($"The number must be a valid integer! \n");

        return inputNumber;
    }

    public static void GenerateMysteryNumber()
    {
        Random rand = new Random();
        MysteryNumber = rand.Next(RangeStart, RangeEnd);
    }

    #endregion
}

internal class Player
{ 
    public string Name { get; private set; }
    public int TotalWin { get; private set; } = 0;
    public int CurrentGuess { get; set; }

    public Player(string name) => Name = name;

    public void AddWin() => TotalWin++;
}

internal class MenuItem
{
    public string Name { get; }
    public Action ItemAction { get; }

    public MenuItem(string name, Action selected)
    {
        Name = name;
        ItemAction = selected;
    }
}
