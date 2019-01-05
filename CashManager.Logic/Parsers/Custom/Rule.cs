using System;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers.Custom
{
    public class Rule
    {
        public TransactionField Property { get; set; }

        public int Column { get; set; }

        public bool IsOptional { get; set; } = true;

        internal int Index => Column - 1;
    }
}