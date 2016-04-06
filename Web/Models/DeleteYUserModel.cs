using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleConcurrency.Web.Models
{
    public class DeleteYUserModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string OwnerId { get; set; }
    }
}