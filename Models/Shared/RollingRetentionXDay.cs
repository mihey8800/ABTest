using System;
using Models.Entities;

namespace Models.Shared
{
    public class RollingRetentionXDay
    {
        public DateTime Day { get; set; }
        public double Percent { get; set; }
    }
}
