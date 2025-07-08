using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.DataManagement
{
    public class IsbnConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return new Isbn(text);  // let constructor handle validation
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value is Isbn isbn ? isbn.ToString() : base.ConvertToString(value, row, memberMapData);
        }
    }
}

