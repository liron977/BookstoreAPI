namespace BookstoreAPI.Models
{
  public class Book
  {
    public string Isbn { get; set; }
    public string Title { get; set; }
    public string[] Authors { get; set; }
    public string Category { get; set; }
    public int Year { get; set; }
    public double Price { get; set; }
    public string Cover { get; set; }
  }
}
