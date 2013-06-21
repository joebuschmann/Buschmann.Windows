﻿namespace Buschmann.Windows.Controls
{
    public interface IFilterStrategy
    {
        bool IsMatch(object item, string filterText);
        string GetDisplayText(object item);
    }
}