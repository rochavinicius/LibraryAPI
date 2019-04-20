using LibraryAPI.Entities;
using LibraryAPI.Models;
using LibraryAPI.Services;
using LibraryAPI.Helpers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LibraryAPI.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private ILibraryRepository _libraryRepository;
        private ILogger<BooksController> _logger;

        public BooksController(ILibraryRepository libraryRepository, ILogger<BooksController> logger)
        {
            _libraryRepository = libraryRepository;
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var booksFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var books = AutoMapper.Mapper.Map<IEnumerable<BookDto>>(booksFromRepo);

            return Ok(books);
        }

        [HttpGet("{bookId}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var bookFromRepo = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepo == null)
            {
                return NotFound();
            }

            var book = AutoMapper.Mapper.Map<BookDto>(bookFromRepo);

            return Ok(book);
        }

        [HttpPost]
        public IActionResult AddBookForAuthor(Guid authorId, [FromBody]BookForCreationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (book.Description == book.Title)
            {
                ModelState.AddModelError(nameof(BookForCreationDto),
                    "The title and description must be different.");
            }

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookToAdd = AutoMapper.Mapper.Map<Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"An error occurred while creating a book for author id:{authorId}.");
            }

            var bookToReturn = AutoMapper.Mapper.Map<BookDto>(bookToAdd);

            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToReturn.Id }, bookToReturn);
        }

        [HttpDelete("{bookId}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepo == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Failed on delete book of id {bookId}");
            }

            return NoContent();
        }

        [HttpPut("{bookId}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid bookId, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (book.Description == book.Title)
            {
                ModelState.AddModelError(nameof(BookForUpdateDto),
                    "The title and description must be different.");
            }

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepo == null)
            {
                var bookToAdd = AutoMapper.Mapper.Map<Book>(book);
                bookToAdd.Id = bookId;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Failed on create book of id {bookId} for author id {authorId}");
                }
                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToAdd.Id }, bookToAdd);
            }

            AutoMapper.Mapper.Map(book, bookFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Failed on update book of id {bookId}");
            }

            return NoContent();
        }

        [HttpPatch("{bookId}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid bookId, 
            [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepo == null)
            {
                var bookDto = new BookForUpdateDto();
                patchDoc.ApplyTo(bookDto, ModelState);

                if (bookDto.Description == bookDto.Title)
                {
                    ModelState.AddModelError(nameof(BookForUpdateDto),
                        "The title and description must be different.");
                }

                TryValidateModel(bookDto);

                if (!ModelState.IsValid)
                {
                    return new Helpers.UnprocessableEntityObjectResult(ModelState);
                }

                var bookToAdd = AutoMapper.Mapper.Map<Book>(bookDto);
                bookToAdd.Id = bookId;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Failed on create book of id {bookId} for author id {authorId}");
                }
                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToAdd.Id }, bookToAdd);
            }

            var bookToPatch = AutoMapper.Mapper.Map<BookForUpdateDto>(bookFromRepo);

            patchDoc.ApplyTo(bookToPatch, ModelState);

            if (bookToPatch.Description == bookToPatch.Title)
            {
                ModelState.AddModelError(nameof(BookForUpdateDto),
                    "The title and description must be different.");
            }

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            AutoMapper.Mapper.Map(bookToPatch, bookFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Failed on patch book of id {bookId} for author id {authorId}");
            }

            return NoContent();
        }
    }
}
