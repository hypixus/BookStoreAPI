using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<BookAvailability> BookAvailabilities { get; set; }
}

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string BookMiniatureFileName { get; set; }
    public double Price { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public double Balance { get; set; }
    public int AccountType { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public ulong OrderTime { get; set; }
    public int CustomerId { get; set; }
    public int BookId { get; set; }
    public int BookAmount { get; set; }
}

public class BookAvailability
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int AmountAvailable { get; set; }
}