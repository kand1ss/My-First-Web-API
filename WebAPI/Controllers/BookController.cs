using Application.DTO;
using Application.Permissions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("/api/books")]
[Authorize]
public class BookController(IBookService bookService) : ControllerBase
{
    [HttpPost]
    [Authorize(Permissions.Create)]
    public async Task<IActionResult> AddBookAsync([FromBody] BookDTO bookData)
    {
        await bookService.AddBook(bookData);
        return Ok("Book added");
    }

    [HttpPut("{id:int}")]
    [Authorize(Permissions.Edit)]
    public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] BookUpdateDTO bookData)
    {
        await bookService.UpdateBook(id, bookData);
        return Ok("Book updated");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Permissions.Delete)]
    public async Task<IActionResult> DeleteBookAsync(int id)
    {
        await bookService.DeleteBook(id);
        return Ok("Book deleted");
    }

    [HttpGet("{id:int}")]
    [Authorize(Permissions.Read)]
    public async Task<IActionResult> GetBookAsync(int id)
    {
        var result = await bookService.GetBookById(id);
        return Ok(result);
    }

    [HttpGet("{title}")]
    [Authorize(Permissions.Read)]
    public async Task<IActionResult> GetBookByTitleAsync(string title)
    {
        var result = await bookService.GetBookByTitle(title);
        return Ok(result);
    }
}