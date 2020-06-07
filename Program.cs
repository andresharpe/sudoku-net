using System;
using System.Diagnostics;
using CommandLine;
using CommandLine.Text;

namespace sudoku
{

    class Options
    {
        [Option('s', "solve", Required = false, HelpText = "Solves puzzles in a text file")]
        public bool Solve { get; set; }

        [Option('g', "generate", Required = false, HelpText = "Generates puzzles and appends them to a text file")]
        public bool Generate { get; set; }

        [Option('f', "file", Default = @".\puzzles.txt", HelpText = @"A file containing puzzles, one per line. Defaults to .\puzzles.txt")]
        public string File { get; set; }

        [Option('n', "number", Default = 10, HelpText = @"The number of puzzles to generate and append to file")]
        public int Number { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Sudoku puzzle = new Sudoku();

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    if (o.Generate)
                    {
                        puzzle.GeneratePuzzle(o.Number,o.File);
                    }
                    else
                    {
                        puzzle.SolvePuzzle(o.File);
                    }
                });


            sw.Stop();
            Console.WriteLine();
            var secs = sw.ElapsedMilliseconds / 1000.0f;
            var speed = puzzle.Completed / secs;
            Console.WriteLine($"Elapsed time: {secs:0.###} seconds. Puzzles completed: {puzzle.Completed}. Speed: {speed:0.###} puzzles / second.");
        }

    }
}
