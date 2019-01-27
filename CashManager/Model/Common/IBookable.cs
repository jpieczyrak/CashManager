using System;

namespace CashManager.WPF.Model.Common
{
    public interface IBookable : IEditable
    {
        DateTime BookDate { get; set; }
    }
}