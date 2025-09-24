namespace NoobSoft.PublicLibrary.Database.Business.Reservations;

public enum ReservationStatus
{
    PendingQueue,   // Default state
    Offered,        // Copy returned, 10-day pickup window started
    Fulfilled,
    Cancelled,      // Loaner cancelled it
    Expired         // Offer deadline passed
}