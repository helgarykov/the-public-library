namespace NoobSoft.PublicLibrary.Database.Business.Reservations;

public interface IReservationService
{
    Reservation PlaceReservation(Guid bookId, Guid loanerId, DateTime now);
    void CancelReservation(Guid reservationId);
    
    // Called by LoanService when a book is returned
    Reservation? OfferNextIfAny(Guid bookId, DateTime now);
    
    // Called by LoanService before a book is loaned out
    // If someone else has an active offer, return false
    bool CanLoanTo(Guid loanerId, Guid bookId, DateTime now);
    
    // Housekeeping (can be called periodically or on actions)
    int ExpireTimedOffers(DateTime now);
    
    IReadOnlyList<Reservation> GetQueue(Guid bookId);   // for tests
}