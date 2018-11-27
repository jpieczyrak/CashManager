using System;

namespace CashManager.Logic.Parsers
{
    public class ParserFactory
    {
        public static IParser Create(eParserInputType eParserInputType)
        {
            switch (eParserInputType)
            {
                case eParserInputType.Excel:
                    return new ExcelParser();
                case eParserInputType.GetinBank:
                    return new GetinBankParser();
            }
            throw new NotImplementedException();
        }
    }
}