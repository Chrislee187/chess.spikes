﻿using chess.engine.Actions;
using chess.engine.Board;
using chess.engine.Chess;
using chess.engine.Game;
using chess.engine.Movement;
using chess.engine.Movement.SimpleValidators;
using chess.engine.tests.Builders;
using NUnit.Framework;

namespace chess.engine.tests.Movement
{
    [TestFixture]
    public class DestinationContainsEnemyValidationTests
    {
        private EasyBoardBuilder _board;
        private BoardState _boardState;

        [SetUp]
        public void SetUp()
        {
            _board = new EasyBoardBuilder()
                .Board("r  qk  r" +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "        " +
                       "R   K  R"
                );
            var game = new ChessGame(_board.ToGameSetup());
            _boardState = game.BoardState;
        }

        [Test]
        public void Should_return_true_for_valid_take()
        {
            var validator = new DestinationContainsEnemyMoveValidator();

            var containsEnemy = ChessMove.Create("A1", "A8", ChessMoveType.MoveOrTake);
            Assert.True(validator.ValidateMove(containsEnemy, _boardState));

        }
        [Test]
        public void Should_return_false_for_invalid_take()
        {
            var validator = new DestinationContainsEnemyMoveValidator();

            var noEnemy = ChessMove.Create("E8", "G8", ChessMoveType.MoveOrTake);
            Assert.False(validator.ValidateMove(noEnemy, _boardState));

        }
    }
    [TestFixture]
    public class ChessPathsValidatorTests
    {

        [Test]
        public void Should_find_move_that_leaves_king_in_check()
        {
            var board = new EasyBoardBuilder()
                .Board("r   k  r" +
                       "        " +
                       "        " +
                       "    p   " +
                       "   PQ   " +
                       "        " +
                       "        " +
                       "R   K  R"
                );
            var game = new ChessGame(board.ToGameSetup());

            var validator = new ChessPathsValidator(new ChessPathValidator(new MoveValidationFactory()), new BoardActionFactory());

            var moveOrTake = ChessMove.CreateMoveOrTake(BoardLocation.At("E5"), BoardLocation.At("D4"));
            var doesMoveLeaveMovingPlayersKingInCheck = validator.DoesMoveLeaveMovingPlayersKingInCheck(moveOrTake, game.BoardState);

            Assert.That(doesMoveLeaveMovingPlayersKingInCheck, Is.True);
        }

    }
}