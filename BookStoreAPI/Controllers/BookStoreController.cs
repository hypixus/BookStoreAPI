using BookStoreAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BookStoreAPI.Authentication;


namespace BookStoreAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BookStoreController : ControllerBase
{
    #region general
    private readonly DataContext _context;

    public BookStoreController(DataContext context)
    {
        _context = context;
    }

    
    #endregion
    
    #region books

    [HttpPost]
    public async Task<ActionResult<List<Book>>> BookCreate([FromForm] string email,
        [FromForm] string password, [FromForm] Book request)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth != CredentialsLevel.Admin)
            return BadRequest("Unauthorized.");
        var toAdd = new Book()
        {
            Author = request.Author,
            BookMiniatureFileName = request.BookMiniatureFileName,
            Description = request.Description,
            Name = request.Name,
            Price = request.Price
        };
        await _context.Books.AddAsync(toAdd);
        await _context.SaveChangesAsync();
        return Ok(await _context.Books.ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Book>> BookDetailsGet([FromForm] int bookId, [FromForm] string email,
        [FromForm] string password)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth == CredentialsLevel.Unauthorized)
            BadRequest("Unauthorized.");

        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            return BadRequest("Book not found.");
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<List<Book>>> BooksGet([FromForm] string email, [FromForm] string password)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth == CredentialsLevel.Unauthorized)
            return BadRequest("Unauthorized.");

        var books = await _context.Books.ToListAsync();
        return Ok(books);
    }

    [HttpPut]
    public async Task<ActionResult<List<Book>>> BookUpdate([FromForm] Book request, [FromForm] string email,
        [FromForm] string password)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth == Authentication.CredentialsLevel.Unauthorized)
            BadRequest("Unauthorized.");
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null) return BadRequest("Book not found.");

        book.Author = request.Author;
        if (auth == Authentication.CredentialsLevel.Admin)
            book.BookMiniatureFileName = request.BookMiniatureFileName;
        book.Description = request.Description;
        book.Name = request.Name;
        book.Price = request.Price;

        await _context.SaveChangesAsync();

        return Ok(await _context.Books.ToListAsync());
    }

    [HttpDelete]
    public async Task<ActionResult<List<Book>>> BookDelete([FromForm] Book request, [FromForm] string email,
        [FromForm] string password)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth == Authentication.CredentialsLevel.Unauthorized)
            BadRequest("Unauthorized.");
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null) return BadRequest("Book not found.");

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return Ok(await _context.Books.ToListAsync());
    }

    #endregion

    #region book availability

    [HttpPost]
    public async Task<ActionResult<List<BookAvailability>>> BooksAvailabilityGet([FromForm] string email,
        [FromForm] string password)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth == CredentialsLevel.Unauthorized) return BadRequest("Unauthorized.");
        var avail = await _context.BookAvailabilities.ToListAsync();
        return Ok(avail);
    }

    [HttpPut]
    public async Task<ActionResult<List<BookAvailability>>> BookAvailabilityUpdate(
        [FromForm] BookAvailability request,
        [FromForm] string email,
        [FromForm] string password)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth != CredentialsLevel.Admin) return BadRequest("Unauthorized.");

        var dbAvail = await _context.BookAvailabilities.FindAsync(request.Id);
        if (dbAvail == null) return BadRequest("Availability entry not found.");

        dbAvail.AmountAvailable = request.AmountAvailable;
        dbAvail.BookId = request.BookId;
        await _context.SaveChangesAsync();
        return Ok(await _context.BookAvailabilities.ToListAsync());
    }

    [HttpDelete]
    public async Task<ActionResult<List<BookAvailability>>> BookAvailabilityDelete([FromForm] string email,
        [FromForm] string password, [FromForm] int id)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth != CredentialsLevel.Admin) return BadRequest("Unauthorized.");

        var dbAvail = await _context.BookAvailabilities.FindAsync(id);
        if (dbAvail == null) return BadRequest("Availability entry not found.");
        
        _context.BookAvailabilities.Remove(dbAvail);

        await _context.SaveChangesAsync();

        return Ok(await _context.BookAvailabilities.ToListAsync());
    }
    #endregion

    #region orders

    [HttpPost]
    public async Task<ActionResult<List<Order>>> OrdersGet([FromForm] string email,
        [FromForm] string password)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth != CredentialsLevel.Admin) return BadRequest("Unauthorized.");
        var avail = await _context.BookAvailabilities.ToListAsync();
        return Ok(avail);
    }

    [HttpPut]
    public async Task<ActionResult<List<Order>>> OrderUpdate([FromForm] string email,
        [FromForm] string password, [FromForm] Order request)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth != CredentialsLevel.Admin) return BadRequest("Unauthorized.");

        var dbOrder = await _context.Orders.FindAsync(request.Id);
        if (dbOrder == null) return BadRequest("Availability entry not found.");

        dbOrder.BookAmount = request.BookAmount;
        dbOrder.CustomerId = request.CustomerId;
        dbOrder.OrderTime = request.OrderTime;
        dbOrder.BookId = request.BookId;
        await _context.SaveChangesAsync();
        return Ok(await _context.Orders.ToListAsync());
    }

    [HttpDelete]
    public async Task<ActionResult<List<Order>>> OrderDelete([FromForm] string email,
        [FromForm] string password, [FromForm] int id)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth != CredentialsLevel.Admin) return BadRequest("Unauthorized.");

        var dbOrder = await _context.Orders.FindAsync(id);
        if (dbOrder == null) return BadRequest("Availability entry not found.");
        
        _context.Orders.Remove(dbOrder);

        await _context.SaveChangesAsync();

        return Ok(await _context.Orders.ToListAsync());
    }
    #endregion

    #region non CRUD

    [HttpPost]
    public async Task<IActionResult> MakeOrder([FromForm] string email,
        [FromForm] string password, [FromForm] Order request)
    {
        var auth = await AuthenticateUser(email, password, _context);
        if (auth == CredentialsLevel.Unauthorized) return BadRequest("Unauthorized.");
        //user authorized. Check the balance.
        var user = _context.Customers.First(c => c.Email == email);
        //calculate order cost.
        var book = (await _context.Books.FindAsync(request.BookId));
        if (book == null) return BadRequest("Book not found.");
        var totalCost = request.BookAmount * book.Price;
        //can customer afford it.
        if (totalCost > user.Balance) return BadRequest("User does not have enough balance.");
        //do we have enough books.
        var avail = _context.BookAvailabilities.First(a => a.BookId == request.BookId);
        if (avail.AmountAvailable > request.BookAmount) return BadRequest("Not enough books available to purchase.");
        //if everything is okay, process the purchase.
        //reassign values to ensure EntityFramework processing works fine.
        avail = await _context.BookAvailabilities.FindAsync(avail.Id);
        if (avail == null) return BadRequest("Internal error occurred."); 
        avail.AmountAvailable -= request.BookAmount;
        
        user = await _context.Customers.FindAsync(user.Id);
        if (user == null) return BadRequest("Internal error occurred.");
        user.Balance -= totalCost;
        var newOrder = new Order();
        newOrder.BookAmount = request.BookAmount;
        newOrder.BookId = request.BookId;
        newOrder.CustomerId = user.Id;
        DateTimeOffset now = DateTimeOffset.Now;
        newOrder.OrderTime = (ulong)now.ToUnixTimeMilliseconds();
        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();

        return Ok();
    }

    #endregion
    
}

