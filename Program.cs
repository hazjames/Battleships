using System;
using System.Text.RegularExpressions;

namespace Battleships
{
    class Program
    {
        static void Main(string[] args)
        {
            int noOfBattleships = 1;
            int noOfDestroyers = 2;
            int boardSize = 10;

            //ships stored in format [[number of hits, size of ship]]
            int[,] ships = new int[noOfBattleships + noOfDestroyers, 2];

            for (int i = 0; i < ships.GetLength(0); i++)
            {
                ships[i, 0] = 0;

                if (i <= noOfBattleships - 1)
                {
                    ships[i, 1] = 5;
                }
                else
                {
                    ships[i, 1] = 4;
                }
            }

            char[,] board = CreateGame(boardSize, ships);

            int turnCount = 1;

            while (!hasWon(ships))
            {
                displayBoard(board);
                Console.Write("Select target (e.g A1): ");
                string input = Console.ReadLine();
                playMove(board, ships, input);
                turnCount++;
            }

            displayBoard(board);
            Console.WriteLine($"\nWell Done. You Won in {turnCount} turns!");
        }

        static char[,] CreateGame(int boardSize, int[,] ships)
        {
            char[,] board = new char[boardSize, boardSize];

            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    board[y, x] = '.';
                }
            }

            Random rand = new Random();

            //For each ship pick whether it is going horizontal and a random place withint he confines of the board to place it
            //Also check wether it is a valid placement (does not collide with another ship)
            for (int ship = 0; ship < ships.GetLength(0); ship++)
            {
                bool placed = false;
                while (!placed)
                {
                    bool isHorizontal = rand.Next(0, 2) == 1 ? true : false;
                    int start = rand.Next(0, (boardSize / 2) + 1);
                    int otherDim = rand.Next(0, boardSize);

                    if (IsValidPlacement(board, start, otherDim, isHorizontal, ships[ship, 1]))
                    {
                        for (int pos = start; pos < start + ships[ship, 1]; pos++)
                        {
                            if (isHorizontal)
                            {
                                board[otherDim, pos] = ship.ToString()[0];
                            }
                            else
                            {
                                board[pos, otherDim] = ship.ToString()[0];
                            }
                        }
                        placed = true;
                    }
                }
            }

            return board;
        }

        static bool IsValidPlacement(char[,] board, int start, int otherDim, bool isHorizontal, int shipSize)
        {
            for (int pos = start; pos < start + shipSize; pos++)
            {
                if ((isHorizontal && board[otherDim, pos] != '.') ||
                    (!isHorizontal && board[pos, otherDim] != '.'))
                {
                    return false;
                }
            }

            return true;
        }

        static void displayBoard(char[,] board)
        {
            Console.Write("\n   ");

            for (int width = 0; width < board.GetLength(0); width++)
            {
                Console.Write((char)(65 + width) + " ");
            }

            Console.Write("\n");

            for (int x = 0; x < board.GetLength(0); x++)
            {
                if (x < 9) Console.Write(" ");

                Console.Write((x + 1) + " ");

                for (int y = 0; y < board.GetLength(1); y++)
                {
                    switch (board[y, x])
                    {
                        case '.':
                        case 'H':
                        case 'M':
                            Console.Write(board[y, x] + " ");
                            break;
                        default:
                            Console.Write(". ");
                            break;
                    }
                }

                Console.Write("\n");
            }

            Console.Write("\n");
        }

        static bool isValidMove(char[,] board, string[] input)
        {
            if (input.Length != 2 || string.IsNullOrEmpty(input[1]))
            {
                return false;
            }

            int row = Int32.Parse(input[1]) - 1;
            int column = (input[0].ToCharArray()[0] - 65);

            if (row < 0 || row > board.GetLength(0) - 1
                || column < 0 || column > board.GetLength(1) - 1
                || board[column, row] == 'H' || board[column, row] == 'M')
            {
                return false;
            }

            return true;
        }

        static void playMove(char[,] board, int[,] ships, string playerInput)
        {
            Console.Clear();
            Console.Write("\nLast Move: ");

            playerInput = Regex.Replace(playerInput.ToUpper(), "[^A-Z0-9]", "");
            string[] input = Regex.Split(playerInput, "(?<=[A-Z])");

            if (!isValidMove(board, input))
            {
                Console.WriteLine("Invalid.");
                return;
            }

            Console.Write(playerInput);

            int row = Int32.Parse(input[1]) - 1;
            int column = (input[0].ToCharArray()[0] - 65);

            if (board[column, row] != '.')
            {
                int ship = (int)char.GetNumericValue(board[column, row]);
                board[column, row] = 'H';
                ships[ship, 0]++;
                Console.WriteLine(ships[ship, 0] == ships[ship, 1] ? " (Sunk!)" : " (Hit!)");
            }
            else
            {
                board[column, row] = 'M';
                Console.WriteLine(" (Miss!)");
            }

            return;
        }

        static bool hasWon(int[,] ships)
        {
            for (int ship = 0; ship < ships.GetLength(0); ship++)
            {
                if (ships[ship, 0] != ships[ship, 1])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
