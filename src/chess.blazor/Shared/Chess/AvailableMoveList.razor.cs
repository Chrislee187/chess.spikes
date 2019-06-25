﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using chess.webapi.client.csharp;
using Microsoft.AspNetCore.Components;

namespace chess.blazor.Shared.Chess
{
    /// <summary>
    /// Simple component to generate an anchor tags that targets a new tab
    /// </summary>
    public class AvailableMoveListComponent : ComponentBase
    {
        private IEnumerable<Move> _moves;

        [Parameter]
        public IEnumerable<Move> Moves // TODO: This should not be the APIClient Move but a specific model for use by the blazor component
        {
            get => _moves;
            set
            {
                _moves = value;
                StateHasChanged();
            }
        }

        [Parameter] public bool ShowMoveList { get; set; } = true;

        [Parameter]
        private EventCallback<string> OnMoveSelectedAsync { get; set; }

        [Parameter]
        public string Title { get; set; }

        public async Task<bool> MoveSelected(string move)
        {
            if(OnMoveSelectedAsync.HasDelegate) await OnMoveSelectedAsync.InvokeAsync(move);

            return true;
        }
    }
}