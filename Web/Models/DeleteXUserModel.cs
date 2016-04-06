using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleConcurrency.Web.Models
{
    public class DeleteXUserModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public long EpochUpdateDate { get; set; }
    }
}