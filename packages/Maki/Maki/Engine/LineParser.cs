//-----------------------------------------------------------------------
// <copyright file="LineParser.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine
{
    using System.Collections.Generic;
    using System.Linq;
    using ChessBot.Logic;
    using ChessBot.Logic.Moves;
    using Maki.Model;

    /// <summary>
    /// Class to parse UCI output into lines.
    /// </summary>
    public class LineParser
    {
        /// <summary>
        /// The UCI output keyword indicating the values that follow refer to the depth.
        /// </summary>
        public const string Depth = "depth";

        /// <summary>
        /// The UCI output keyword indicating the values that follow refer to line number, if multiple lines are requested.
        /// </summary>
        public const string MultiPV = "multipv";

        /// <summary>
        /// The UCI output keyword indicating the values that follow refer to evaluation of the position.
        /// </summary>
        public const string Score = "score";

        /// <summary>
        /// The UCI output keyword indicating the values that follow refer to the principal variation.
        /// </summary>
        public const string PV = "pv";

        /// <summary>
        /// The UCI output keyword indicating the values that follow refer to the centipawn eval.
        /// </summary>
        public const string ScoreCentipawn = "cp";

        /// <summary>
        /// The UCI output keyword indicating the values that follow refer to the number of moves until mate..
        /// </summary>
        public const string ScoreMate = "mate";

        private static readonly string[] LineKeywords = new string[]
        {
            Depth, MultiPV, Score, PV,
        };

        /// <summary>
        /// Parse a single UCI evaluation output line.
        /// </summary>
        /// <param name="fenPosition">The starting FEN (used for SAN context).</param>
        /// <param name="text">The UCI output.</param>
        /// <returns>The parsed line.</returns>
        public EvaluatedLine ParseLineText(string fenPosition, string text)
        {
            var iterator = new WordIterator(text);
            var line = new EvaluatedLine();
            while (iterator.HasRemainingWords)
            {
                string nextKeyword = iterator.SeekAny(LineKeywords);
                if (nextKeyword == null)
                {
                    break;
                }

                switch (nextKeyword)
                {
                    case Depth:
                        line.Depth = iterator.NextWordAsInt();
                        break;
                    case MultiPV:
                        line.LineRank = iterator.NextWordAsInt();
                        break;
                    case PV:
                        var words = new List<string>();
                        while (iterator.HasRemainingWords)
                        {
                            words.Add(iterator.NextWord());
                        }

                        line.LineUci = string.Join(' ', words);
                        break;
                    case Score:
                        var scoreType = iterator.NextWord();
                        if (scoreType == ScoreMate)
                        {
                            line.Evaluation = new EvaluationScore()
                            {
                                MovesUntilMate = iterator.NextWordAsInt(),
                            };
                        }
                        else if (scoreType == ScoreCentipawn)
                        {
                            line.Evaluation = new EvaluationScore()
                            {
                                Centipawns = iterator.NextWordAsInt(),
                            };
                        }

                        break;
                }
            }

            if (line.Depth == 0 || line.LineRank == 0 ||
                line.LineUci == null || line.Evaluation == null)
            {
                return null;
            }

            var lineParts = line.LineUci.Split(' ').ToList();
            var lineSan = this.UciToSan(fenPosition, lineParts);
            line.LineSan = string.Join(' ', lineSan);
            return line;
        }

        private List<string> UciToSan(string startingFen, List<string> uciMoves)
        {
            var san = new List<string>();
            var board = Board.NewFromFen(startingFen);
            foreach (var m in uciMoves)
            {
                (Board nextBoard, Move cbMove) = board.PerformUciMove(m);
                san.Add(cbMove.ToContextualSANString(board));
                board = nextBoard;
            }

            return san;
        }

        /// <summary>
        /// Mini class to iterate over words.
        /// </summary>
        public class WordIterator
        {
            private readonly string[] words;
            private int index = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="WordIterator"/> class.
            /// </summary>
            /// <param name="line">The UCI output line.</param>
            public WordIterator(string line)
            {
                this.words = line.Split(' ');
            }

            /// <summary>
            /// Gets a value indicating whether or not there are more words left to parse.
            /// </summary>
            public bool HasRemainingWords => this.index < this.words.Length;

            /// <summary>
            /// Gets the next word in the sequence.
            /// </summary>
            /// <returns>The next word.</returns>
            public string NextWord()
            {
                string word = this.words[this.index];
                this.index++;
                return word;
            }

            /// <summary>
            /// Gets the next word as an integer.
            /// </summary>
            /// <returns>The integer value of the next word.</returns>
            public int NextWordAsInt()
            {
                string word = this.NextWord();
                return int.Parse(word);
            }

            /// <summary>
            /// Keep skipping to the next word until the word matches a known keyword.
            /// </summary>
            /// <param name="words">The array of keywords.</param>
            /// <returns>The next matching keyword.</returns>
            public string SeekAny(string[] words)
            {
                while (this.HasRemainingWords)
                {
                    string nextWord = this.NextWord();
                    if (words.Contains(nextWord))
                    {
                        return nextWord;
                    }
                }

                return null;
            }
        }
    }
}