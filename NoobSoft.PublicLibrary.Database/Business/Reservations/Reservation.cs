namespace NoobSoft.PublicLibrary.Database.Business.Reservations;

public sealed class Reservation
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid BookId { get; init; }
    public Guid LoanerId { get; init; }
    public DateTime Date { get; init; }

    public ReservationStatus Status { get; private set; } = ReservationStatus.PendingQueue;
    public DateTime? OfferStartedAt { get; private set; }
    public DateTime? PickupDeadline { get; private set; }

    public void MarkOffered(DateTime now, TimeSpan grace)
    {
        Status = ReservationStatus.Offered;
        OfferStartedAt = now;
        PickupDeadline = now.Add(grace);
    }
    
    public void MarkFulfilled() => Status = ReservationStatus.Fulfilled;
    public void MarkCancelled() => Status = ReservationStatus.Cancelled;
    public void MarkExpired() => Status = ReservationStatus.Expired;
}