using System;

namespace CashManager.Model.Common
{
    public interface IBookable : IEditable
    {
        DateTime BookDate { get; set; }
    }
}