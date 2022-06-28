# The Public Library

A C# learning challenge.

## The task

The task is to create bookloaning software for a library, one task at a time.

## The given

We have two projects:

- `NoobSoft.PublicLibrary.Database`: represents our data layer 
- `NoobSoft.PublicLibrary.Database.Tests`: contains unit tests for our data layer.

Inside the data layer project we have a folder, `Data`, which contains three data files:

- `authors.csv`: a collection of authors. Contains all the authors known to the library.
- `books.csv`: a collection of books, written by the authors. Every book is written by exactly one of the known authors.
- `loaners.csv`: a directory of the known loaners at the library.

## How to get started

- Fork this project to your own repository, so you can work on it - and continue your work in your own repository by cloning your fork onto your local computer. Work _test-driven_, so the only way you test is by way of your unit tests.

## Updates

This repository may change. The assignments may get updated, or the test data may change. Your fork will not receive those changes automatically. To update your fork, see [this article on Github](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/syncing-a-fork).

## The assignments

### Data retrieval

- Create code that can populate a collection of `Author` objects from the data source `authors.csv`. _Tip: CSV is a very popular data format. Do not reinvent the wheel. Use NuGet packages._
- Do the same for `Book`.
- Create a fitting type for the loaner, and populate this as well. _Tip: does this type have anything in common with others? If so, is this an opportunity to refactor?._
- Create a repository / data hub so you can access your data thorugh a single entry point. It should allow us to retrieve the whole list of any type, or a single item by its `Id`. This should perform well.

### Validation and search

- ISBN-13 numbers follow a particular pattern. Three of the books in the books source have invalid ISBN numbers. The `ISBN` class should be able to validate these patterns, and you should consider carefully how you want to handle the resulting failure.
- The repository should allow us to find a book by its ISBN number. Partial matches should also work.
- The repository should allow us to find any person (author or loaner) by a partial name match or a birthday.

### Book loans

_At this point, we start implementing business cases, and so it will be logical to add a business layer on top of the data layer. As a part of your continuing implementation, you should plan your architecture carefully._

- A bookloaner should be able to loan a book.
- The loan should be time limited to 30 days. _Tip: make the current time configurable in your code base, to allow for easy testing._
- Once a book has been lent, it is not available to anyone else, and an attempt to loan it to someone else should fail.
- The book becomes available again when it is actively returned.
- We can get the current loan/reservation status for a person (bookloaner).
- We can get the current loan/reservation status for a book.

### Fees

- If the time limit is exceeded before it is returned, a fee of 10 coins will be issued. For every 5 days thereafter, an additional fee of 5 coins will be added. This is on a per-book basis.
- Someone with a debt exceeding 100 coins cannot make any new loans until they have paid enough of their debt to bring it below 100 coins.
- If a time limit for returning a book is exceeded more than 180 days, the book is assumed to be lost from the library collection (but _not_ from the authorship), and a fee of 300 coins is issued to the loaner. This replaces the late fees issued for that book.

### Reservations

- A reservation can be made for a book. This means the loaner is the only one who can loan the book when it is returned. When it is not picked up, there is a grace period of 10 days before it is either offered to the next reservation (under the same conditions) or the public (when there are no more reservations).

### Popular books

- It is interesting for the library to identify the books that are most popular, and they feel the data should be able to provide that overview, but they don't know the details. Suggest a solution for ranking books by popularity.
- The really popular books just don't get around to all the loaners waiting for it. The standard time limit should be reduced to 10 days for the very popular books (a category they want you to identify from the ranking system).
- It should be possible to have several copies of the same book, so we can loan a copy to several people.

### Data persistence

- It is a bit annoying that there is a complete data loss each time the app shuts down. The database should be persisted in files at logical points in time. When the app starts, it should take the locally persisted data if there is any, or the initial data otherwise. Do not overwrite the initial data files. Do not persist inside the repository folders. Instead, persist in a logical place in the local machine user data directory.

### Frontend

- Build an interactive command line frontend that supports all of the functions of the library: see status for book and person, making loans, reservations, viewing the popular books and so forth.