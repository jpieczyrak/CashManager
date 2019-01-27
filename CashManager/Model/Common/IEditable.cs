using System;

namespace CashManager.WPF.Model.Common
{
    public interface IEditable
    {
        DateTime LastEditDate { get; }

        DateTime InstanceCreationDate { get; }
    }
}