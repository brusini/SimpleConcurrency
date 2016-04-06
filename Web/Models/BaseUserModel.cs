using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleConcurrency.Web.Models
{
    public class BaseUserModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public long EpochUpdateDate { get; set; }
        public string OwnerId { get; set; }
    }
}