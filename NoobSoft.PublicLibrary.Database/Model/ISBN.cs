namespace NoobSoft.PublicLibrary.Database.Model;

public class Isbn
{
    private readonly string _value;
    
    public Isbn(string isbn)
    {
        if(!IsValid(isbn))
        {
            throw new ArgumentException("Invalid ISBN-13 format", nameof(isbn));
        }
        _value = isbn;
    }
    public override string ToString() =>_value;

    public static bool IsValid(string isbn)
    {
        if (isbn == null) return false;

        isbn = isbn.Replace("-", "").Replace(" ", "");
        if (isbn.Length != 13 || !isbn.All(char.IsDigit)) return false;

        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            int digit = isbn[i] - '0';
            sum += (i % 2 == 0) ? digit : digit * 3;
        }

        int checksum = (10 - (sum % 10)) % 10;
        return checksum == (isbn[12] - '0');
    }
}