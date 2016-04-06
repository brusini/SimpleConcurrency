using System;

namespace SimpleConcurrency.Model
{
    public class YUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsBlocked { get; set; }
        public string OwnerId { get; set; }
    }
}