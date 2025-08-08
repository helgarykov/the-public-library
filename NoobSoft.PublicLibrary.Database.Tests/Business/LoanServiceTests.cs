using Moq;
using NoobSoft.PublicLibrary.Database.Business;
using NoobSoft.PublicLibrary.Database.Business.Loaning;
using NoobSoft.PublicLibrary.Database.Model;
using NoobSoft.PublicLibrary.Database.Repository;


namespace NoobSoft.PublicLibrary.Database.Tests.Business;

public class LoanServiceTests
{
    [Fact]
    public void TryLoanBook_ShouldSucceed_WhenBookAvailable()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var loanerId = Guid.NewGuid();
        var fixedNow = new DateTime(2025, 8, 8, 0, 0, 0, DateTimeKind.Utc);

        var mockRepo = new Mock<ILibraryRepository>();
        var mockTime = new Mock<ITimeProvider>();

        mockRepo.Setup(r => r.GetBookById(bookId))
            .Returns(new Book { Id = bookId });

        mockRepo.Setup(r => r.GetLoanerById(loanerId))
            .Returns(new Loaner { Id = loanerId });

        mockRepo.Setup(r => r.GetAllLoans())
            .Returns(new List<Loan>());  // no loans = book is available

        mockTime.Setup(tp => tp.Now).Returns(fixedNow);

        var service = new LoanService(mockRepo.Object, mockTime.Object);

        // Act
        var result = service.TryLoanBook(bookId, loanerId);

        // Assert
        Assert.True(result);
        mockRepo.Verify(r => r.AddLoan(It.Is<Loan>(l =>
            l.BookId == bookId &&
            l.LoanerId == loanerId &&
            l.LoanedAt == fixedNow &&
            l.DueAt == fixedNow.AddDays(30) &&
            l.ReturnedAt == null
        )), Times.Once);
    }
}