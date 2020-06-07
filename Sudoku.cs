using System;
using System.Collections.Generic;
using System.IO;

namespace sudoku
{
    public class Sudoku
    {

        private static Random rng = new Random();

        private byte[,] puzzle = new byte[9,9];
        private List<byte[,]> solutions = new List<byte[,]>(10);
        private int limitSolutions = -1;

        private string filename = @"puzzle.txt";

        public int Completed { get; private set; } = 0;

        public void SolvePuzzle(string fileName)
        {
            filename = fileName;
            if (File.Exists(fileName))
            {
                using (StreamReader sr = File.OpenText(filename))
                {
                    while( ! sr.EndOfStream )
                    {
                        var puzzleString = sr.ReadLine();
                        if (puzzleString.Length >= 81)
                        {
                            byte[,] p = StringToPuzzle(puzzleString);
                            Console.WriteLine($"Puzzle {Completed+1}:");
                            Console.WriteLine("");
                            SolvePuzzle(p);
                            Completed++;
                        }
                    }
                }
            }
        }

        public void SolvePuzzle( byte[,] inputPuzzle )
        {
            puzzle = inputPuzzle;
            solutions.Clear();
            limitSolutions = 1;

            // Console.WriteLine( $"Clues: {CountClues()}");
            OutputPuzzle( puzzle );
            Solve();
            if (solutions.Count > 0 )
            {
                Console.WriteLine("");
                OutputPuzzle( solutions[0] );
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine( "There is no solution for this puzzle.");
                Console.WriteLine("");
            }
        }

        private void Solve()
        {
            byte[] numbers = new byte[9];
            for( int i = 0; i < 9; i++ ) numbers[i] = (byte)(i+1);

            for( int y = 0; y < 9; y++ )
            {
                for( int x = 0; x < 9; x++ )
                {
                    if( puzzle[y,x] == 0 ){
                        Shuffle( numbers );
                        for( int v = 0; v < 9; v++ ){
                            if( Possible(y,x,numbers[v]) )
                            {
                                puzzle[y,x] = numbers[v];
                                Solve();
                                if ( solutions.Count == limitSolutions ) return;
                                puzzle[y,x] = 0;
                            } 
                        }
                        return;
                    }
                }
            }    
            solutions.Add( (byte[,])puzzle.Clone() );
            return;
        }

        private bool Possible( int y, int x, byte v )
        {
            for ( int i = 0; i < 9; i++)
            {
                if( puzzle[y,i] == v ){
                    return false;
                } 
                if( puzzle[i,x] == v){
                    return false;
                }
            }
            int top = (y/3) * 3;
            int left = (x/3) * 3;
            for (int i = top; i< top+3; i++ )
            {
                for(int j = left; j < left+3; j++)
                {
                    if( puzzle[i,j] == v ){
                        return false;
                    }
                }
            }
            return true;
        }
        private int CountClues()
        {
            int clues = 0;
            for( int y = 0; y < 9; y++ )
            {
                for( int x = 0; x < 9; x++ )
                {
                    if (puzzle[y,x] != 0) clues++;
                }
            }    
            return clues;
        }

        private void Clear()
        {
            for( int y = 0; y < 9; y++ )
            {
                for( int x = 0; x < 9; x++ )
                {
                    puzzle[y,x] = 0;
                }
            }    
            solutions.Clear();
        }

        public void GeneratePuzzle(int count, string fileName)
        {
            this.filename = fileName;
            GeneratePuzzle(count);
        }

        public void GeneratePuzzle(int count)
        {
            while ( count-- > 0 ){
                GeneratePuzzle();
                Console.WriteLine();
                Console.WriteLine( $"Generating: {count}");
                // Console.WriteLine( $"Clues={CountClues()}");
                OutputPuzzle( puzzle );
                // Console.WriteLine( $"Difficulty={Difficulty()}");
                // OutputDifficulty( puzzle );
                SavePuzzle();
                Completed++;
            }
        }

        public void GeneratePuzzle()
        {
            Clear();
            limitSolutions = 1;
            Solve();

            byte[,] newPuzzle = solutions[0];

            byte[] removeList = new byte[81];
            for( int i = 0; i < 81; i++ ) removeList[i] = (byte)(i);
            Shuffle( removeList );

            limitSolutions = 2;
            for( int removeItem = 0; removeItem < 81; removeItem++ ){
                int y = removeList[removeItem] / 9;
                int x = removeList[removeItem] % 9;
                byte saveItem = newPuzzle[y,x];
                newPuzzle[y,x] = 0;

                puzzle = (byte[,])newPuzzle.Clone();
                solutions.Clear();
                Solve();

                if ( solutions.Count != 1 ){
                    newPuzzle[y,x] = saveItem;
                }
            }

            puzzle = newPuzzle;
        }

        private void Shuffle( byte[] array )
        {
            int n = array.Length;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                byte value = array[k];  
                array[k] = array[n];  
                array[n] = value;  
            }
        }

        private void OutputPuzzle( byte[,] p )
        {
            Console.WriteLine( " +-----------+-----------+-----------+ " );
            for( int y = 0; y < 9; y++ )
            {
                if( y > 0 && y % 3 == 0 ) Console.WriteLine( " |-----------+-----------+-----------| " );
                for( int x = 0; x < 9; x++ )
                {
                    if( x % 3 == 0 ) Console.Write( " | " );
                    if (p[y,x] == 0) Console.Write( " . " );
                    else Console.Write( " " + Convert.ToString(p[y,x]) + " " );
                }
                Console.WriteLine( " | " );
            }    
            Console.WriteLine( " +-----------+-----------+-----------+ " );
        }

        private void OutputDifficulty( byte[,] p )
        {
            Console.WriteLine( " +-----------+-----------+-----------+ " );
            for( int y = 0; y < 9; y++ )
            {
                if( y > 0 && y % 3 == 0 ) Console.WriteLine( " |-----------+-----------+-----------| " );
                for( int x = 0; x < 9; x++ )
                {
                    if( x % 3 == 0 ) Console.Write( " | " );
                    if (p[y,x] == 0) {
                        int d = Difficulty(y,x);
                        Console.Write( " "+char.ConvertFromUtf32(d+64).ToString()+" " );
                    }
                    else Console.Write( " " + Convert.ToString(p[y,x]) + " " );
                }
                Console.WriteLine( " | " );
            }    
            Console.WriteLine( " +-----------+-----------+-----------+ " );
        }

        private int Difficulty(int y, int x)
        {
            int difficulty = 0;
            for(byte v = 1; v < 10; v++ )
            {
                if( Possible(y,x,v)) difficulty++;

            }
            return difficulty;
        }
        private int Difficulty()
        {
            int difficulty = 0;
            for( int y = 0; y < 9; y++ )
            {
                for( int x = 0; x < 9; x++ )
                {
                    if (puzzle[y,x] == 0) {
                        difficulty+= Difficulty(y,x);
                    }
                }
            }
            return difficulty;
        }

        private byte[,] StringToPuzzle( string puzzleString )
        {
           byte[,] p = new byte[9,9];
           int i = 0;
           for( int y = 0; y < 9; y++ )
            {
                for( int x = 0; x < 9; x++ )
                {
                    p[y,x] = Convert.ToByte(puzzleString[i++] - 48) ;
                }
            }    
            return p;
        }

        private string PuzzleToString( byte[,] p )
        {
           char[] c = new char[81];
           int i = 0;
           for( int y = 0; y < 9; y++ )
            {
                for( int x = 0; x < 9; x++ )
                {
                    c[i++] = Convert.ToChar(p[y,x]+48);
                }
            }    
            return new String(c);
        }

        public void SavePuzzle()
        {
            using (StreamWriter sw = File.AppendText(filename))
            {
                sw.WriteLine($"{PuzzleToString(puzzle)}");
            }	
        }
   }
}