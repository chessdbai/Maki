//-----------------------------------------------------------------------
// <copyright file="StartPositionExtrapolator.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine
{
    using System;
    using ChessBot.Logic;
    using ChessBot.Pgn;
    using Maki.Model;

    /// <summary>
    /// Class to determines the true starting position based on a potentially incomplete starting position.
    /// </summary>
    public static class StartPositionExtrapolator
    {
        /// <summary>
        /// Determines the true starting position based on a potentially incomplete starting position.
        /// </summary>
        /// <param name="position">The incomplete position to extrapolate.</param>
        /// <returns>The completed starting position.</returns>
        /// <exception cref="MalformedStartingPositionException">On invalid starting position.</exception>
        public static StartingPosition ExtrapolateStartingPosition(StartingPosition position)
        {
            string pgn = position.Pgn;
            string fen = position.Fen;

            if (fen != null)
            {
                return position;
            }

            var parser = new PgnGameParser();
            try
            {
                var pgnGame = parser.ParsePgn(pgn);
                var ply = pgnGame.FirstMove;
                while (ply.NextMoveInMainLine != null)
                {
                    ply = ply.NextMoveInMainLine;
                }

                var board = ply.ToBoard();
                fen = board.ToFen();

                return new StartingPosition()
                {
                    Fen = fen,
                    Pgn = pgn,
                };
            }
            catch (Exception e)
            {
                if (e is AlienChessException || e is PgnFormatException)
                {
                    throw new MalformedStartingPositionException("The provided starting position could not be parsed from the PGN format.", e);
                }
                else
                {
                    throw new MalformedStartingPositionException("The provided starting position could not be parsed from the PGN format.");
                }
            }
        }
    }
}