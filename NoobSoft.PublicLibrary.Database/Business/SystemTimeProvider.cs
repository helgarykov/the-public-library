namespace NoobSoft.PublicLibrary.Database.Business;

public class SystemTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.UtcNow;
}
