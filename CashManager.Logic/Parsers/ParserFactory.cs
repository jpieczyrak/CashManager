using System;

namespace CashManager.Logic.Parsers
{
    public class ParserFactory
    {
        public static IParser Create(ParserInputType parserInputType)
        {
            switch (parserInputType)
            {
                case ParserInputType.Excel:
                    return new ExcelParser();
                case ParserInputType.GetinBank:
                    return new GetinBankParser();
            }
            throw new NotImplementedException();
        }
    }
}