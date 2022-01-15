#nullable enable
using System;
using static System.Console;

namespace Bme121
{
    class Player
    {
		public string name;
		public string discColour;
		public bool isTurn;
		public string symbol;
		public int totalScore;
		public int discCount;
		public Player( string name, string discColour, bool isTurn, string symbol, int totalScore, int discCount )
		{
			this.name = name;
			this.discColour = discColour;
			this.isTurn = isTurn;
			this.symbol = symbol;
			this.totalScore = totalScore;
			this.discCount = discCount;
		}
	}
	
	
    static partial class Program
    {

        static void Main( )
        {
            //Setup: user information and game board size
            Console.Clear( );
            WriteLine( "Welcome to Othello!" );
            Write( "Please enter the play's name for white (<Enter> to set the name to 'White'): " );
            string whitePlayerName = ReadLine() !;
            if( whitePlayerName == "" )
            {
				whitePlayerName = "White";
			}
            Player whitePlayer = new Player ( whitePlayerName, "white", true, "O", 0, 0);
            Write( "Please enter the play's name for black (<Enter> to set the name to 'Black'): " );
            string blackPlayerName = ReadLine() !;
            if( blackPlayerName == "" )
            {
				blackPlayerName = "Black";
			}
            Player blackPlayer = new Player ( blackPlayerName, "black", false, "X", 0, 0);			
			string[ , ] gameBoard = new string [0, 0];
			while( gameBoard.GetLength(0) == 0 )
			{
				WriteLine( "Please enter the desired board size (rows and cols must be even numbers between 4 to 26): " );
				Write( "Row: " );
				int rowsCustomize = int.Parse( ReadLine() ! );
				Write( "Column: " );
				int columnsCustomize = int.Parse( ReadLine() ! );
				gameBoard = NewBoard(rowsCustomize,columnsCustomize) ;
			}
			Console.Clear( );
			WriteLine( );
			WriteLine( " Welcome to Othello!" );
			WriteLine( );
			DisplayBoard( gameBoard );
			WriteLine( );
			
			//The game continues when no players enter "quit" or valid moves are available
			while( ( whitePlayer.isTurn || blackPlayer.isTurn ) && ( !IfGameOver( whitePlayer, gameBoard ) || !IfGameOver( blackPlayer, gameBoard ) ) )
			{
				if ( !IfGameOver( whitePlayer, gameBoard ) ) 
				{
					gameBoard = PlayerTurn( whitePlayer, blackPlayer, gameBoard );
				}
				else
				{
					WriteLine( "Sorry {0}, there's no valid move for you to make. ", whitePlayer.name );
					SwitchTurn( whitePlayer, blackPlayer );
				}
				
				if ( !IfGameOver( blackPlayer, gameBoard ) ) 
				{
					gameBoard = PlayerTurn( blackPlayer, whitePlayer, gameBoard );
				}
				else 
				{
					WriteLine( "Sorry {0}, there's no valid move for you to make. ", blackPlayer.name );
					SwitchTurn( blackPlayer, whitePlayer );
				}
				
				//Display current score
				whitePlayer.discCount = TargetStringCount( whitePlayer.symbol, gameBoard );
				blackPlayer.discCount = TargetStringCount( blackPlayer.symbol, gameBoard );
				WriteLine( "Up until the current round: " );
				CheckStatus( whitePlayer, blackPlayer );
			}
			
			EndGame( whitePlayer, blackPlayer );		
			Write( "Thank you for playing Othello!" );
			
        }
        
        static bool IfValidMove (int rowIndex, int colIndex, Player player, string[ , ] gameBoard, bool ifPrintMessage )
        {
			
			bool ifValid = true;
			if( rowIndex < 0 || rowIndex > gameBoard.GetLength(0) - 1 ) 			
			{
				ifValid = false; 
				if ( ifPrintMessage ) WriteLine( "Out of the board range. Please re-enter." );
			}
			else if( colIndex < 0 || colIndex > gameBoard.GetLength(1) - 1 )		
			{
				ifValid = false;     
				if ( ifPrintMessage ) WriteLine( "Out of the board range. Please re-enter." );
			}
			else if( gameBoard[rowIndex, colIndex] != " ") 						
			{
				ifValid = false;	 
				if ( ifPrintMessage ) WriteLine("Space already been taken. Please re-enter.");
			}
			else if( !IfChange( gameBoard, MakeMove( rowIndex, colIndex, player, gameBoard ) ) )			
			{
				ifValid = false;     
				if ( ifPrintMessage ) WriteLine("Does not result in a flip. Please re-enter.");
			}
			return ifValid;
		}
		
		static string[ , ] MakeMove( int rowIndex, int colIndex, Player player, string[ , ] gameBoard )
		{
			//This method doesn't make change to the original game board, all changes are stored in gameCopy
			string[ , ] gameCopy = gameBoard.Clone() as string[ , ]; //store the flips
			string[ , ] gameSubCopy = gameCopy.Clone() as string[ , ]; //used to check through each of the 8 lines
			string playerSymbol = player.symbol;
			
			//check down
			int count = 0;
			//Go through the game board until hitting the wall
			for (int row = rowIndex + 1; row < gameCopy.GetLength(0) && gameCopy[row, colIndex] != playerSymbol; row++ )
			{
				// Hits an empty space before meeting the player's own symbol --> no flip
				// Hits the wall without meeting the player's own symbol --> no flip
				if ( ( gameCopy[row, colIndex] == " " ) || ( row == gameCopy.GetLength(0) - 1 && gameCopy[row, colIndex] != playerSymbol ) ) 
				{
					count = 0;
					gameSubCopy = gameCopy.Clone() as string[ , ];
					break;
				}
				else
				{
					count++;
					gameSubCopy[row, colIndex] = playerSymbol;
				}
			}
			if( count > 0 )	 gameCopy = gameSubCopy.Clone() as string[ , ]; 
			count = 0;
			
			//check up
			for (int row = rowIndex - 1; row >= 0 && gameCopy[row, colIndex] != playerSymbol; row-- )
			{
				if ( ( gameCopy[row, colIndex] == " " ) || ( row == 0 && gameCopy[row, colIndex] != playerSymbol ) ) 
				{
					count = 0;
					gameSubCopy = gameCopy.Clone() as string[ , ];
					break;
				}
				else
				{
					count++;
					gameSubCopy[row, colIndex] = playerSymbol;
				}
			}
			if( count > 0 ) gameCopy = gameSubCopy.Clone() as string[ , ];
			count = 0;
			
			//check right
			for (int col = colIndex + 1; col < gameCopy.GetLength(1) && gameCopy[rowIndex, col] != playerSymbol; col++ )
			{
				if ( ( gameCopy[rowIndex, col] == " " ) || ( col == gameCopy.GetLength(1) - 1 && gameCopy[rowIndex, col] != playerSymbol ) ) 
				{
					count = 0;
					gameSubCopy = gameCopy.Clone() as string[ , ];
					break;
				}
				else 
				{
					count++;
					gameSubCopy[rowIndex, col] = playerSymbol;
				}
			}
			if( count > 0 ) gameCopy = gameSubCopy.Clone() as string[ , ];
			count = 0;
			
			//check left
			for (int col = colIndex - 1; col >= 0 && gameCopy[rowIndex, col] != playerSymbol; col-- )
			{
				if ( gameCopy[rowIndex, col] == " " || ( col == 0 && gameCopy[rowIndex, col] != playerSymbol ) ) 
				{
					count = 0;
					gameSubCopy = gameCopy.Clone() as string[ , ];
					break;
				}
				else
				{
					count++;
					gameSubCopy[rowIndex, col] = playerSymbol;
				}
			}
			if( count > 0 ) gameCopy = gameSubCopy.Clone() as string[ , ];
			count = 0;
			
			//check upleft
			for (int move = 1; (rowIndex - move) >= 0 && (colIndex - move) >= 0 && gameCopy[rowIndex - move, colIndex - move] != playerSymbol; move++ )
			{
				if ( ( gameCopy[rowIndex - move, colIndex - move] == " " ) ||
					 ( (rowIndex - move) == 0 || (colIndex - move) == 0) && gameCopy[rowIndex - move, colIndex - move] != playerSymbol ) 
				{
					count = 0;
					gameSubCopy = gameCopy.Clone() as string[ , ];
					break;
				}
				else 
				{
					count++;
					gameSubCopy[rowIndex - move, colIndex - move] = playerSymbol;
				}
			}
			if( count > 0 ) gameCopy = gameSubCopy.Clone() as string[ , ];
			count = 0;
			
			//check upright
			for (int move = 1; (rowIndex - move) >= 0 && (colIndex + move) < gameCopy.GetLength(1) && gameCopy[rowIndex - move, colIndex + move] != playerSymbol; move++ )
			{
				if ( ( gameCopy[rowIndex - move, colIndex + move] == " " ) ||
					 ( ( (rowIndex - move) == 0 || (colIndex + move) == gameCopy.GetLength(1) - 1 ) && gameCopy[rowIndex - move, colIndex + move] != playerSymbol ) ) 
				{
					count = 0;
					gameSubCopy = gameCopy.Clone() as string[ , ];
					break;
				}
				else 
				{
					count++;
					gameSubCopy[rowIndex - move, colIndex + move] = playerSymbol;
				}
			}
			if( count > 0 ) gameCopy = gameSubCopy.Clone() as string[ , ];
			count = 0;
			
			//check downleft
			for (int move = 1; (rowIndex + move) < gameCopy.GetLength(0) && (colIndex - move) >= 0 && gameCopy[rowIndex + move, colIndex - move] != playerSymbol; move++ )
			{
				if ( ( gameCopy[rowIndex + move, colIndex - move] == " " ) ||
					 ( ( (rowIndex + move) == gameCopy.GetLength(0) - 1 || (colIndex - move) == 0 ) && gameCopy[rowIndex + move, colIndex - move] != playerSymbol ) )
				{
					count = 0;
					gameSubCopy = gameCopy.Clone() as string[ , ];
					break;
				}
				else {
					count++;
					gameSubCopy[rowIndex + move, colIndex - move] = playerSymbol;
				}
			}
			if( count > 0 ) gameCopy = gameSubCopy.Clone() as string[ , ];
			count = 0;
			
			//check downright
			for (int move = 1; (rowIndex + move) < gameCopy.GetLength(0) && (colIndex + move) < gameCopy.GetLength(1) && gameCopy[rowIndex + move, colIndex + move] != playerSymbol; move++ )
			{
				if ( ( gameCopy[rowIndex + move, colIndex + move] == " " ) || 
					 ( ( (rowIndex + move) == gameCopy.GetLength(0) - 1 && (colIndex + move) == gameCopy.GetLength(1) - 1 ) && gameCopy[rowIndex + move, colIndex + move] != playerSymbol ) ) 
				{
					count = 0;
					gameSubCopy = gameCopy.Clone() as string[ , ];
					break;
				}
				else 
				{
					count++;
					gameSubCopy[rowIndex + move, colIndex + move] = playerSymbol;
				}
			}
			if( count > 0 ) gameCopy = gameSubCopy.Clone() as string[ , ];
			count = 0;
			
			//return the updated game board
			return gameCopy;
		}
		
		static bool IfGameOver( Player player, string[ , ] gameBoard )
		{
			//Game is over when no valid move is available
			bool ifGameOver = true;
			for( int i = 0; i < gameBoard.GetLength(0); i++ )
			{
				for( int j = 0; j < gameBoard.GetLength(1); j++ )
				{
					if( IfValidMove(i, j, player, gameBoard, false) ) ifGameOver = false;
				}
			}
			return ifGameOver;
		}
		
		static string[ , ] PlayerTurn( Player currentPlayer, Player opponentPlayer, string[ , ] gameBoard ){
			bool ifValid = false;
			while ( currentPlayer.isTurn && !ifValid )
			{
				Write( "Hello {0} ({1} using symbol {2}). Please enter a move: ", currentPlayer.name, currentPlayer.discColour, currentPlayer.symbol );
				string response = ReadLine() !;
				if( response == "quit" )
				{
					ifValid = true;
					currentPlayer.isTurn = false;
				}
				else if( response == "skip" )
				{
					ifValid = true;
					currentPlayer.isTurn = false;
					opponentPlayer.isTurn = true;
				}
				else
				{
					if( response.Length == 2 ) 
					{
						string rowLetter = response.Substring( 0, 1 ); //response[0].toString
						string colLetter = response.Substring( 1, 1 ); //response[1].toString
						int rowIndex = IndexAtLetter( rowLetter );
						int colIndex = IndexAtLetter( colLetter );
						ifValid = IfValidMove( rowIndex, colIndex, currentPlayer, gameBoard, true );
						if( ifValid )
						{
							gameBoard = MakeMove( rowIndex, colIndex, currentPlayer, gameBoard );
							gameBoard[rowIndex, colIndex] = currentPlayer.symbol;
							currentPlayer.isTurn = false;
							opponentPlayer.isTurn = true;
						}
					}
					else WriteLine( "Entrance must be 2 letter. Please re-enter." );
				}
			}
			Console.Clear();
			DisplayBoard( gameBoard );
            WriteLine( );
            return gameBoard;
		}
		
		//This method is used to check if there's change made in game board and is used in ifValidMove method to detect changes
		static bool IfChange( string[ , ] originalGameBoard, string[ , ] copyGameBoard )
		{
			bool ifChange = false;
			for( int i = 0; i < originalGameBoard.GetLength(0); i++ )
			{
				for( int j = 0; j < originalGameBoard.GetLength(1); j++ )
				{
					if( originalGameBoard[i,j] != copyGameBoard[i,j] ) return true;
				}
			}
			return false;
		}
		
		//Used to count player's score
		static int TargetStringCount( string targetString, string[ , ]gameBoard )
		{
			int count = 0;
			for( int i = 0; i < gameBoard.GetLength(0); i++ )
			{
				for( int j = 0; j < gameBoard.GetLength(1); j++ )
				{
					if( gameBoard[i,j] == targetString ) count++;
				}
			}
			return count;
		}
		
		static void CheckStatus( Player player1, Player player2 )
		{
			Write( "{0}({1}) has {2} discs and {3}({4}) has {5} discs, ", player1.name, player1.symbol, player1.discCount, player2.name, player2.symbol, player2.discCount );
			if( player1.discCount > player2.discCount )
				WriteLine( "{0} wins by {1} discs", player1.name, ( player1.discCount - player2.discCount ) );
			else if ( player1.discCount < player2.discCount )
				WriteLine( "{0} wins by {1} discs", player2.name, ( player2.discCount - player1.discCount ) );
			else
				WriteLine( "there's a tie" );
		}
		
		static void SwitchTurn( Player player1, Player player2 )
		{
			player1.isTurn = false;
			player2.isTurn = true;
		}
		
		static void EndGame( Player player1, Player player2 )
		{
			WriteLine();
			WriteLine( "************************* Game over ************************** " );
			CheckStatus( player1, player2 );
			if( player1.discCount > player2.discCount ) player1.totalScore += player1.discCount - player2.discCount;
			else if ( player1.discCount < player2.discCount ) player2.totalScore += player2.discCount - player1.discCount;
			WriteLine( "{0}({1}) has a score of {2} and {3}({4}) has a score of {5}", player1.name, player1.symbol, player1.totalScore, player2.name, player2.symbol, player2.totalScore);
			if( Math.Abs(player1.discCount - player2.discCount) >= 2 && Math.Abs(player1.discCount - player2.discCount) <= 10 ) 
				WriteLine( "It is a close game!" );
			else if( Math.Abs(player1.discCount - player2.discCount) >= 12 && Math.Abs(player1.discCount - player2.discCount) <= 24 ) 
				WriteLine( "It is a hot game!" );
			else if( Math.Abs(player1.discCount - player2.discCount) >= 26 && Math.Abs(player1.discCount - player2.discCount) <= 38 ) 
				WriteLine( "It is a fight game!" );
			else if( Math.Abs(player1.discCount - player2.discCount) >= 40 && Math.Abs(player1.discCount - player2.discCount) <= 52 ) 
				WriteLine( "It is a walkway game!" );
			else if( Math.Abs(player1.discCount - player2.discCount) >= 54 && Math.Abs(player1.discCount - player2.discCount) <= 64 ) 
				WriteLine( "It is a perfect game!" );
			player1.discCount = 0;
			player2.discCount = 0;
		}
    }
    
    
}
