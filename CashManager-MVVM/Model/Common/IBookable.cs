using System;

namespace CashManager_MVVM.Model.Common
{
    public interface IBookable : IEditable
    {
        DateTime BookDate { get; set; }
    }
}