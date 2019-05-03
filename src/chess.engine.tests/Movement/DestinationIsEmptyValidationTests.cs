﻿using chess.engine.Board;
using chess.engine.Chess;
using chess.engine.Movement;
using chess.engine.Movement.SimpleValidators;
using chess.engine.tests.Builders;
using NUnit.Framework;

namespace chess.engine.tests.Movement
{
    [TestFixture]
    public class DestinationIsEmptyValidationTests
    {
        private EasyBoardBuilder _board;
        private BoardState _boardState;
        private DestinationIsEmptyValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _board = new EasyBoardBuilder()
                .Board("r   k  r" +
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
            _validator = new DestinationIsEmptyValidator();
        }

        [Test]
        public void Should_return_true_for_move_to_empty_space()
        {
            var empty = ChessMove.Create("E1", "E2", ChessMoveType.CastleKingSide);
            Assert.True(_validator.ValidateMove(empty, _boardState));
        }

        [Test]
        public void Should_return_false_for_move_to_non_empty_space()
        {
            var notEmpty = ChessMove.Create("A1", "A8", ChessMoveType.CastleQueenSide);
            Assert.False(_validator.ValidateMove(notEmpty, _boardState));
        }
    }

}