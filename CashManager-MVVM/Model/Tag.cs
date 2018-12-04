using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    /// <summary>
    /// Stores "tag" information of transaction.
    /// Each transaction can contains unlimited tags.
    /// Tag can be assigned to many transactions.
    /// Sum of values of all tags dont have to be equal to sum of all transaction value (one tag can be assigned to one or more transaction)
    /// </summary>
    public class Tag : BaseSelectable
    {
        #region Override

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}