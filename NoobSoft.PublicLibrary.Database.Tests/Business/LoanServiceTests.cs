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

    [Fact]
    public void TryLoanBook_ShouldFail_WhenBookAlreadyLoaned()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var loanerId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        
        var mockRepo = new Mock<ILibraryRepository>();
        var mockTime = new Mock<ITimeProvider>();
        
        mockRepo.Setup(r => r.GetBookById(bookId))
            .Returns(new Book { Id = bookId });
        mockRepo.Setup(r => r.GetLoanerById(loanerId))
            .Returns(new Loaner { Id = loanerId });
        mockRepo.Setup(r => r.GetAllLoans()).Returns(new List<Loan>
        {
            
                new Loan { BookId = bookId, ReturnedAt = null }
                
        });
        
        mockTime.Setup(tp => tp.Now).Returns(now);
        
        var service = new LoanService(mockRepo.Object, mockTime.Object);
        
        // Act
        var result = service.TryLoanBook(bookId, loanerId);
        
        // Assert
        Assert.False(result);
        mockRepo.Verify(r => r.AddLoan(It.IsAny<Loan>()), Times.Never);
    }

    [Fact]
    public void ReturnBook_ShouldMarkLoanAsReturned_WhenLoanExists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var fixedNow = new DateTime(2025, 8, 9, 0, 0, 0, DateTimeKind.Utc);

        var loan = new Loan()
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            LoanedAt = fixedNow.AddDays(-10),
            DueAt = fixedNow.AddDays(20),
            ReturnedAt = null
        };

        var mockRepo = new Mock<ILibraryRepository>();
        var mockTime = new Mock<ITimeProvider>();
        
        mockRepo.Setup(r => r.GetAllLoans())
            .Returns(new List<Loan> { loan });
        mockTime.Setup(tp => tp.Now).Returns(fixedNow);
        
        var service = new LoanService(mockRepo.Object, mockTime.Object);
        
        // Act

        var result = service.ReturnBook(bookId);
        
        // Assert
        Assert.True(result);
        Assert.Equal(fixedNow, loan.ReturnedAt );   // loan object was updated
    }

    [Fact]
    public void GetBooksLoanedByPerson_TestCase1_ReturnsTwoActiveBooks()
    {
        //Arrange
        var loanerId = Guid.NewGuid();
        var bookId1 = Guid.NewGuid();
        var bookId2 = Guid.NewGuid();

        var loans = new List<Loan>
        {
            new Loan { BookId = bookId1, LoanerId = loanerId, ReturnedAt = null },
            new Loan { BookId = bookId2, LoanerId = loanerId, ReturnedAt = null }

        };

        var mockRepo = new Mock<ILibraryRepository>();
        mockRepo.Setup(r => r.GetAllLoans())
            .Returns(loans);
        mockRepo.Setup(r => r.GetBookById(bookId1)).Returns( new Book { Id = bookId1, Title = "Book 1"  });
        mockRepo.Setup(r => r.GetBookById(bookId2)).Returns( new Book { Id = bookId2, Title = "Book 2" });
        
        var mockTime = new Mock<ITimeProvider>();
        var service = new LoanService(mockRepo.Object, mockTime.Object);
        
        // Act
        var result = service.GetBooksLoanedByPerson(loanerId).ToList();
        
        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, b => b?.Id == bookId1);
        Assert.Contains(result, b => b?.Id == bookId2);
    }

    [Fact]
    public void IsBookLoaned_ShouldReturnLoan_WhenBookIsLoaned()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var loan = new Loan { BookId = bookId, ReturnedAt = null };
        
        var mockRepo = new Mock<ILibraryRepository>();
        mockRepo.Setup(r => r.GetAllLoans()).Returns(new List<Loan> { loan });
        
        var mockTime = new Mock<ITimeProvider>();
        var service = new LoanService(mockRepo.Object, mockTime.Object);
        
        // Act
        var result = service.IsBookLoaned_ReturnsLoanOrNull(bookId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookId, result!.BookId);
    }
    
    [Fact]
    public void IsBookLoaned_ShouldReturnNull_WhenBookIsNotLoaned()
    {
        // Arrange
        var bookId = Guid.NewGuid();

        var mockRepo = new Mock<ILibraryRepository>();
        mockRepo.Setup(r => r.GetAllLoans()).Returns(new List<Loan>()); // no loans

        var mockTime = new Mock<ITimeProvider>();
        var service = new LoanService(mockRepo.Object, mockTime.Object);

        // Act
        var result = service.IsBookLoaned_ReturnsLoanOrNull(bookId);

        // Assert
        Assert.Null(result);
    }
}