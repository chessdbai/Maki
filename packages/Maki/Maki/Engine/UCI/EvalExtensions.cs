//-----------------------------------------------------------------------
// <copyright file="EvalExtensions.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine.UCI
{
    using System;
    using System.Linq;
    using System.Text;
    using ChessBot.Logic;
    using ChessBot.Logic.Moves;
    using Maki.Model;

    /// <summary>
    /// A class with extension methods around creating the required
    /// "go" string.
    /// </summary>
    public static class EvalExtensions
    {
        /// <summary>
        /// Creates the UCI "go" string based on the start eval request.
        /// </summary>
        /// <param name="req">The evaluation request.</param>
        /// <returns>The "go" string.</returns>
        public static string ToGoString(this StartEvaluationRequest req)
        {
            var sb = new StringBuilder();
            sb.Append("go ");

            if (req!.CompletionCriteria?.MaxDepth != null)
            {
                sb.Append($"depth {req.CompletionCriteria.MaxDepth.Value} ");
            }

            if (req!.CompletionCriteria?.MaxNodes != null)
            {
                sb.Append($"nodes {req.CompletionCriteria.MaxNodes.Value} ");
            }

            if (req!.CompletionCriteria?.Mate != null)
            {
                sb.Append($"mate {req.CompletionCriteria.Mate.Value} ");
            }

            if (req!.CompletionCriteria?.PonderTime != null)
            {
                sb.Append($"movetime {req.CompletionCriteria.PonderTime.Value.TotalMilliseconds} ");
            }

            if (req!.CompletionCriteria?.Infinite ?? false)
            {
                sb.Append($"infinite ");
            }

            if (req!.Clock != null)
            {
                var clock = req.Clock;
                if (clock.WhiteTime != null)
                {
                    sb.Append($"wtime {(long)clock.WhiteTime.Value.TotalMilliseconds} ");
                }

                if (clock.BlackTime != null)
                {
                    sb.Append($"btime {(long)clock.BlackTime.Value.TotalMilliseconds} ");
                }

                if (clock.WhiteIncrement != null)
                {
                    sb.Append($"winc {(long)clock.WhiteIncrement.Value.TotalMilliseconds} ");
                }

                if (clock.BlackIncrement != null)
                {
                    sb.Append($"binc {(long)clock.BlackIncrement.Value.TotalMilliseconds} ");
                }

                if (clock.MovesToGo != null)
                {
                    sb.Append($"movestogo {clock.MovesToGo} ");
                }
            }

            if (req.SearchMoves != null && req.SearchMoves.Count > 0)
            {
                string searchMoveString = GetMoveFilterString(req);
                sb.Append($"{searchMoveString} ");
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Gets the move filter string for "searchmoves".
        /// </summary>
        /// <param name="req">The evaluation request.</param>
        /// <returns>The "searchmoves" string.</returns>
        /// <exception cref="ArgumentException">If an invalid SAN move is given.</exception>
        internal static string GetMoveFilterString(this StartEvaluationRequest req)
        {
            var sb = new StringBuilder();
            sb.Append("searchmoves ");
            var startFen = StartPositionExtrapolator.ExtrapolateStartingPosition(req.Position).Fen;
            var board = Board.NewFromFen(startFen);
            var moves = board.GetPossibleMoves(board.MoveHistory.Count % 2 == 0 ? PieceColor.White : PieceColor.Black);
            foreach (var m in req.SearchMoves)
            {
                if (!string.IsNullOrEmpty(m.UCI))
                {
                    sb.Append($"{m.UCI} ");
                }
                else if (!string.IsNullOrEmpty(m.SAN))
                {
                    var correspondingMove = moves.FirstOrDefault(mov => mov.ToContextualSANString(board) == m.SAN);
                    if (correspondingMove == default(Move))
                    {
                        throw new ArgumentException($"Cannot create go string with move filter with SAN '{m.SAN}' as no corresponding UCI move can be found.");
                    }

                    sb.Append($"{correspondingMove.ToUciString()} ");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}