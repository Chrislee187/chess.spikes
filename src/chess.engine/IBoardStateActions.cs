﻿using chess.engine.Entities;
using chess.engine.Game;

namespace chess.engine
{
    public interface IBoardStateActions
    {
        ChessPieceEntity GetEntity(BoardLocation loc);
        void ClearLocation(BoardLocation loc);
        void SetEntity(BoardLocation loc, ChessPieceEntity entity);

    }
}