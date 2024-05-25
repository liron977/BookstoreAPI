using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BookstoreAPI.Models;

namespace BookstoreAPI.Services
{
  public class BookstoreService
  {
    private readonly string _filePath;

    public BookstoreService(string filePath)
    {
      _filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
    }

    public List<Book> GetAllBooks()
    {
      var doc = XDocument.Load(_filePath);
      return doc.Descendants("book")
                .Select(book => new Book
                {
                  Isbn = book.Element("isbn")?.Value,
                  Title = book.Element("title")?.Value,
                  Authors = book.Elements("author").Select(a => a.Value).ToArray(),
                  Category = book.Attribute("category")?.Value,
                  Year = int.Parse(book.Element("year")?.Value ?? "0"),
                  Price = double.Parse(book.Element("price")?.Value ?? "0"),
                  Cover = book.Attribute("cover")?.Value
                }).ToList();
    }

    public void AddBook(Book newBook)
    {
      var doc = XDocument.Load(_filePath);
      if (doc.Descendants("book").Any(book => book.Element("isbn")?.Value == newBook.Isbn))
      {
        throw new ArgumentException("A book with the same ISBN already exists.");
      }

      ValidateBook(newBook);

      var newElement = new XElement("book",
          new XAttribute("category", newBook.Category),
          new XElement("isbn", newBook.Isbn),
          new XElement("title", newBook.Title),
          newBook.Authors.Select(author => new XElement("author", author)),
          new XElement("year", newBook.Year),
          new XElement("price", newBook.Price)
      );

      if (!string.IsNullOrEmpty(newBook.Cover))
      {
        newElement.Add(new XAttribute("cover", newBook.Cover));
      }

      doc.Element("bookstore")?.Add(newElement);
      doc.Save(_filePath);
    }

    public void DeleteBook(string isbn)
    {
      var doc = XDocument.Load(_filePath);
      var bookToRemove = doc.Descendants("book")
                            .FirstOrDefault(book => book.Element("isbn")?.Value == isbn);

      bookToRemove?.Remove();
      doc.Save(_filePath);
    }

    public void UpdateBook(string isbn, Book updatedBook)
    {
      var doc = XDocument.Load(_filePath);
      var bookToUpdate = doc.Descendants("book")
                            .FirstOrDefault(book => book.Element("isbn")?.Value == isbn);

      if (bookToUpdate == null)
      {
        throw new ArgumentException("Book with the given ISBN does not exist.");
      }

      bookToUpdate.SetAttributeValue("category", updatedBook.Category);
      bookToUpdate.Element("title")?.SetValue(updatedBook.Title);
      bookToUpdate.Elements("author").Remove();
      foreach (var author in updatedBook.Authors)
      {
        bookToUpdate.Add(new XElement("author", author));
      }
      bookToUpdate.Element("year")?.SetValue(updatedBook.Year);
      bookToUpdate.Element("price")?.SetValue(updatedBook.Price);
      if (!string.IsNullOrEmpty(updatedBook.Cover))
      {
        bookToUpdate.SetAttributeValue("cover", updatedBook.Cover);
      }
      else
      {
        bookToUpdate.Attribute("cover")?.Remove();
      }

      doc.Save(_filePath);
    }

    public string GenerateReport()
    {
      var books = GetAllBooks();
      var htmlReport = "<html><body><table border='1'>";
      htmlReport += "<tr><th>Title</th><th>Authors</th><th>Category</th><th>Year</th><th>Price</th></tr>";

      foreach (var book in books)
      {
        htmlReport += $"<tr><td>{book.Title}</td><td>{string.Join(", ", book.Authors)}</td><td>{book.Category}</td><td>{book.Year}</td><td>{book.Price}</td></tr>";
      }

      htmlReport += "</table></body></html>";
      return htmlReport;
    }

    private void ValidateBook(Book book)
    {
      if (string.IsNullOrEmpty(book.Isbn) || string.IsNullOrEmpty(book.Title) ||
          book.Authors == null || !book.Authors.Any() ||
          string.IsNullOrEmpty(book.Category) ||
          book.Year <= 0 || book.Price <= 0)
      {
        throw new ArgumentException("All book fields are mandatory.");
      }
    }
  }
}
