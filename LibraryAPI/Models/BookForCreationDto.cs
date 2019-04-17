using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class BookForCreationDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters.")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "The descriptions shouldn't have more than 500 characters.")]
        public string Description { get; set; }
    }
}
