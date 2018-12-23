using System;

namespace CashManager_MVVM.Model.Common
{
    public interface IEditable
    {
        DateTime LastEditDate { get; }

        DateTime InstanceCreationDate { get; }
    }
}