using System;

namespace SimpleConcurrency.Model
{
    public class XUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdateDate { get; set; }
        public long EpochUpdateDate { get; set; }
    }
}