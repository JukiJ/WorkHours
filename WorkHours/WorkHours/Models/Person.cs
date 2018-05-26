using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkHours.Models
{
    [Table("Person")]
    public class Person
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public string Surname { get; set; }
        public string Position { get; set; }
        [NotNull]
        public bool Admin { get; set; }

        public override string ToString()
        {
            return Name + " " + Surname;
        }
    }
}
