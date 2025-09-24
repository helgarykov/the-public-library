using NoobSoft.PublicLibrary.Database.Repository;

namespace NoobSoft.PublicLibrary.Database.Business.Reservations;

public sealed class ReservationService : IReservationService
{
    private readonly ILibraryRepository _repo;
    private readonly TimeSpan _pickupGrace = TimeSpan.FromDays(10); // 10 days to actually come and loan it; after that my reservation expires, and the next person (or the public) can get it.
    
    // Naive in-memory store for now; later persist via a repo
    private readonly List<Reservation> _reservations = new();
    public ReservationService(ILibraryRepository repo) => _repo = repo;
    
    
    public Reservation PlaceReservation(Guid bookId, Guid loanerId, DateTime now)
    {
        throw new NotImplementedException();
    }

    public void CancelReservation(Guid reservationId)
    {
        throw new NotImplementedException();
    }

    public Reservation? OfferNextIfAny(Guid bookId, DateTime now)
    {
        throw new NotImplementedException();
    }

    public bool CanLoanTo(Guid loanerId, Guid bookId, DateTime now)
    {
        throw new NotImplementedException();
    }

    public int ExpireTimedOffers(DateTime now)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<Reservation> GetQueue(Guid bookId)
    {
        throw new NotImplementedException();
    }
}