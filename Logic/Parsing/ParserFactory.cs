using System;

namespace Logic.Parsing
{
    public class ParserFactory
    {
        public static IParser Create(eParserInputType eParserInputType)
        {
            switch (eParserInputType)
            {
                case eParserInputType.Excel:
                    return new ExcelParser();
            }
            throw new NotImplementedException();
        }
    }
}