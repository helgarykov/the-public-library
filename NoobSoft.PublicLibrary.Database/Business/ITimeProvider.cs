namespace NoobSoft.PublicLibrary.Database.Business;

public interface ITimeProvider
{
    DateTime Now { get; }
}