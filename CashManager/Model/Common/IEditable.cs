using System;

namespace CashManager.Model.Common
{
    public interface IEditable
    {
        DateTime LastEditDate { get; }

        DateTime InstanceCreationDate { get; }
    }
}