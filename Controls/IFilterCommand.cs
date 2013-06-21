﻿namespace Buschmann.Windows.Controls
{
    public interface IFilterCommand : IFilterStrategy
    {
        string CommandText { get; }
    }
}