using System;

namespace Models.Shared
{
    public class UserRetention
    {
        public DateTime Day { get; set; }
        public int LiveUsers { get; set; }
    }
}
