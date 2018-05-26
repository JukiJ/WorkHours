using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Models
{
    public class Verification
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Username { get; set; }
        [NotNull]
        public string Password { get; set; }
        [NotNull]
        public int PersonId { get; set; }
    }
}
