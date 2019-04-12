using LibraryAPI.Entities;
using LibraryAPI.Helpers;
using LibraryAPI.Models;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Controllers
{
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection([FromBody] ICollection<AuthorForCreationDto> listAuthor)
        {
            if (listAuthor == null)
            {
                return BadRequest();
            }

            var authorEntities = AutoMapper.Mapper.Map<ICollection<Author>>(listAuthor).ToList();

            authorEntities.ForEach(a =>
            {
                _libraryRepository.AddAuthor(a);
            });

            if (!_libraryRepository.Save())
            {
                throw new Exception("An error occurred while saving collection of author.");
            }

            var authorsToReturn = AutoMapper.Mapper.Map<ICollection<AuthorDto>>(authorEntities);

            var ids = string.Join(",", authorsToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = ids }, authorsToReturn);
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _libraryRepository.GetAuthors(ids);

            if (authorEntities.Count() != ids.Count())
            {
                return NotFound();
            }

            var authorsToReturn = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorsToReturn);
        }
    }
}
