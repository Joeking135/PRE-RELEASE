//Skeleton Program code for the AQA A Level Paper 1 Summer 2024 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Puzzle
{
	class PuzzleCS
	{
		static Random Rng = new Random();

		static void Main(string[] args)
		{
			string Again = "y";
			int Score;
			while (Again == "y")
			{
				string Filename = null;
				while (!(File.Exists(Filename + ".txt") || Filename == ""))
				{
					Console.Write("Press Enter to start a standard puzzle or enter name of file to load: ");
					Filename = Console.ReadLine();
					if (!(File.Exists(Filename)||Filename == ""))
					{
						Console.WriteLine("Invalid File name, please try again.");
					}
				}
				Puzzle MyPuzzle;
				if (Filename.Length > 0)
				{
					MyPuzzle = new Puzzle(Filename + ".txt");
				}
				else
				{
					MyPuzzle = new Puzzle(8, Convert.ToInt32(8 * 8 * 0.6));
				}
				Score = MyPuzzle.AttemptPuzzle();
				Console.WriteLine("Puzzle finished. Your score was: " + Score);
				Console.Write("Do another puzzle? ");
				Again = Console.ReadLine().ToLower();
			}
			Console.ReadLine();
		}

		class Puzzle
		{
			private int Score;
			private int SymbolsLeft;
			private int GridSize;
			private List<Cell> Grid;
			private List<Pattern> AllowedPatterns;
			private List<string> AllowedSymbols;

			public Puzzle(string Filename)
			{
				Grid = new List<Cell>();
				AllowedPatterns = new List<Pattern>();
				AllowedSymbols = new List<string>();
				LoadPuzzle(Filename);
			}

			public Puzzle(int Size, int StartSymbols)
			{
				Score = 0;
				SymbolsLeft = StartSymbols;
				GridSize = Size;
				Grid = new List<Cell>();
				for (var Count = 1; Count <= GridSize * GridSize; Count++)
				{
					Cell C;
					if (Rng.Next(1, 101) < 90)
					{
						C = new Cell();
					}
					else
					{
						C = new BlockedCell();
					}
					Grid.Add(C);
				}
				AllowedPatterns = new List<Pattern>();
				AllowedSymbols = new List<string>();
				Pattern QPattern = new Pattern("Q", "QQ**Q**QQ");
				AllowedPatterns.Add(QPattern);
				AllowedSymbols.Add("Q");
				Pattern XPattern = new Pattern("X", "X*X*X*X*X");
				AllowedPatterns.Add(XPattern);
				AllowedSymbols.Add("X");
				Pattern TPattern = new Pattern("T", "TTT**T**T");
				AllowedPatterns.Add(TPattern);
				AllowedSymbols.Add("T");
			}

			private void LoadPuzzle(string Filename)
			{
				try
				{
					using (StreamReader MyStream = new StreamReader(Filename))
					{
						int NoOfSymbols = Convert.ToInt32(MyStream.ReadLine());
						for (var Count = 1; Count <= NoOfSymbols; Count++)
						{
							AllowedSymbols.Add(MyStream.ReadLine());
						}
						int NoOfPatterns = Convert.ToInt32(MyStream.ReadLine());
						for (var Count = 1; Count <= NoOfPatterns; Count++)
						{
							List<string> Items = MyStream.ReadLine().Split(',').ToList();
							Pattern P = new Pattern(Items[0], Items[1]);
							AllowedPatterns.Add(P);
						}
						GridSize = Convert.ToInt32(MyStream.ReadLine());
						for (var Count = 1; Count <= GridSize * GridSize; Count++)
						{
							Cell C;
							List<string> Items = MyStream.ReadLine().Split(',').ToList();
							if (Items[0] == "@")
							{
								C = new BlockedCell();
							}
							else
							{
								C = new Cell();
								C.ChangeSymbolInCell(Items[0]);
								for (var CurrentSymbol = 1; CurrentSymbol < Items.Count; CurrentSymbol++)
								{
									C.AddToNotAllowedSymbols(Items[CurrentSymbol]);
								}
							}
							Grid.Add(C);
						}
						Score = Convert.ToInt32(MyStream.ReadLine());
						SymbolsLeft = Convert.ToInt32(MyStream.ReadLine());
					}
				}
				catch
				{
					Console.WriteLine("Puzzle not loaded");
				}
			}

			public virtual int AttemptPuzzle()
			{
				bool Finished = false;
				while (!Finished)
				{
					DisplayPuzzle();
					Console.WriteLine("Current score: " + Score);
					bool Valid = false;
					int Row = -1;
					while (!Valid)
					{
						Console.Write("Enter row number: ");
						try
						{
							Row = Convert.ToInt32(Console.ReadLine());
							Valid = true;
						}
						catch
						{
						}
					}
					int Column = -1;
					Valid = false;
					while (!Valid)
					{
						Console.Write("Enter column number: ");
						try
						{
							Column = Convert.ToInt32(Console.ReadLine());
							Valid = true;
						}
						catch
						{
						}
					}
					string Symbol = GetSymbolFromUser();
					SymbolsLeft -= 1;
					Cell CurrentCell = GetCell(Row, Column);
					if (CurrentCell.CheckSymbolAllowed(Symbol))
					{
						CurrentCell.ChangeSymbolInCell(Symbol);
						int AmountToAddToScore = CheckForMatchWithPattern(Row, Column);
						if (AmountToAddToScore > 0)
						{
							Score += AmountToAddToScore;
						}
					}
					if (SymbolsLeft == 0)
					{
						Finished = true;
					}
				}
				Console.WriteLine();
				DisplayPuzzle();
				Console.WriteLine();
				return Score;
			}

			private Cell GetCell(int Row, int Column)
			{
				return Grid[(GridSize - Row) * GridSize + Column - 1];
			}

			public virtual int CheckForMatchWithPattern(int Row, int Column)
			{
				for (var StartRow = Row + 2; StartRow >= Row; StartRow--)
				{
					for (var StartColumn = Column - 2; StartColumn <= Column; StartColumn++)
					{
						try
						{
							string PatternString = "";
							PatternString += GetCell(StartRow, StartColumn).GetSymbol();
							PatternString += GetCell(StartRow, StartColumn + 1).GetSymbol();
							PatternString += GetCell(StartRow, StartColumn + 2).GetSymbol();
							PatternString += GetCell(StartRow - 1, StartColumn + 2).GetSymbol();
							PatternString += GetCell(StartRow - 2, StartColumn + 2).GetSymbol();
							PatternString += GetCell(StartRow - 2, StartColumn + 1).GetSymbol();
							PatternString += GetCell(StartRow - 2, StartColumn).GetSymbol();
							PatternString += GetCell(StartRow - 1, StartColumn).GetSymbol();
							PatternString += GetCell(StartRow - 1, StartColumn + 1).GetSymbol();
							foreach (var P in AllowedPatterns)
							{
								string CurrentSymbol = GetCell(Row, Column).GetSymbol();
								if (P.MatchesPattern(PatternString, CurrentSymbol))
								{
									GetCell(StartRow, StartColumn).AddToNotAllowedSymbols(CurrentSymbol);
									GetCell(StartRow, StartColumn + 1).AddToNotAllowedSymbols(CurrentSymbol);
									GetCell(StartRow, StartColumn + 2).AddToNotAllowedSymbols(CurrentSymbol);
									GetCell(StartRow - 1, StartColumn + 2).AddToNotAllowedSymbols(CurrentSymbol);
									GetCell(StartRow - 2, StartColumn + 2).AddToNotAllowedSymbols(CurrentSymbol);
									GetCell(StartRow - 2, StartColumn + 1).AddToNotAllowedSymbols(CurrentSymbol);
									GetCell(StartRow - 2, StartColumn).AddToNotAllowedSymbols(CurrentSymbol);
									GetCell(StartRow - 1, StartColumn).AddToNotAllowedSymbols(CurrentSymbol);
									GetCell(StartRow - 1, StartColumn + 1).AddToNotAllowedSymbols(CurrentSymbol);
									return 10;
								}
							}
						}
						catch
						{
						}
					}
				}
				return 0;
			}

			private string GetSymbolFromUser()
			{
				string Symbol = "";
				while (!AllowedSymbols.Contains(Symbol))
				{
					Console.Write("Enter symbol: ");
					Symbol = Console.ReadLine();
				}
				return Symbol;
			}

			private string CreateHorizontalLine()
			{
				string Line = "  ";
				for (var Count = 1; Count <= GridSize * 2 + 1; Count++)
				{
					Line += "-";
				}
				return Line;
			}

			public virtual void DisplayPuzzle()
			{
				Console.WriteLine();
				if (GridSize < 10)
				{
					Console.Write("  ");
					for (var Count = 1; Count <= GridSize; Count++)
					{
						Console.Write(" " + Count);
					}
				}
				Console.WriteLine();
				Console.WriteLine(CreateHorizontalLine());
				for (var Count = 0; Count < Grid.Count(); Count++)
				{
					if (Count % GridSize == 0 && GridSize < 10)
					{
						Console.Write((GridSize - ((Count + 1) / GridSize)) + " ");
					}
					Console.Write("|" + Grid[Count].GetSymbol());
					if ((Count + 1) % GridSize == 0)
					{
						Console.WriteLine("|");
						Console.WriteLine(CreateHorizontalLine());
					}
				}
			}
		}

		class Pattern
		{
			private string Symbol;
			private string PatternSequence;

			public Pattern(string SymbolToUse, string PatternString)
			{
				Symbol = SymbolToUse;
				PatternSequence = PatternString;
			}

			public virtual bool MatchesPattern(string PatternString, string SymbolPlaced)
			{
				if (SymbolPlaced != Symbol)
				{
					return false;
				}
				for (var Count = 0; Count < PatternSequence.Length; Count++)
				{
					if (PatternSequence[Count].ToString() == Symbol && PatternString[Count].ToString() != Symbol)
					{
						return false;
					}
				}
				return true;
			}

			public virtual string GetPatternSequence()
			{
				return PatternSequence;
			}
		}

		class Cell
		{
			protected string Symbol;
			private List<string> SymbolsNotAllowed;

			public Cell()
			{
				Symbol = "";
				SymbolsNotAllowed = new List<string>();
			}

			public virtual string GetSymbol()
			{
				if (IsEmpty())
				{
					return "-";
				}
				else
				{
					return Symbol;
				}
			}

			public bool IsEmpty()
			{
				if (Symbol.Length == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			public void ChangeSymbolInCell(string NewSymbol)
			{
				Symbol = NewSymbol;
			}

			public virtual bool CheckSymbolAllowed(string SymbolToCheck)
			{
				foreach (var Item in SymbolsNotAllowed)
				{
					if (Item == SymbolToCheck)
					{
						return false;
					}
				}
				return true;
			}

			public virtual void AddToNotAllowedSymbols(string SymbolToAdd)
			{
				SymbolsNotAllowed.Add(SymbolToAdd);
			}

			public virtual void UpdateCell()
			{
			}
		}

		class BlockedCell : Cell
		{
			public BlockedCell() : base()
			{
				Symbol = "@";
			}

			public override bool CheckSymbolAllowed(string SymbolToCheck)
			{
				return false;
			}
		}
	}
}
