using System.Collections;
using System.Collections.Generic;

namespace Logic.TransactionManagement
{
    /// <summary>
    /// List of all transaction saved in app.
    /// </summary>
    public class Transactions : IList<Transaction>
    {
        private List<Transaction> TransactionsList { get; set; }

        public Transactions()
        {
            TransactionsList = new List<Transaction>();
        }

        public IEnumerator<Transaction> GetEnumerator()
        {
            return TransactionsList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) TransactionsList).GetEnumerator();
        }

        public void Add(Transaction item)
        {
            TransactionsList.Add(item);
        }

        public void Clear()
        {
            TransactionsList.Clear();
        }

        public bool Contains(Transaction item)
        {
            return TransactionsList.Contains(item);
        }

        public void CopyTo(Transaction[] array, int arrayIndex)
        {
            TransactionsList.CopyTo(array, arrayIndex);
        }

        public bool Remove(Transaction item)
        {
            return TransactionsList.Remove(item);
        }

        public int Count
        {
            get { return TransactionsList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(Transaction item)
        {
            return TransactionsList.IndexOf(item);
        }

        public void Insert(int index, Transaction item)
        {
            TransactionsList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            TransactionsList.RemoveAt(index);
        }

        public Transaction this[int index]
        {
            get { return TransactionsList[index]; }
            set { TransactionsList[index] = value; }
        }
    }
}