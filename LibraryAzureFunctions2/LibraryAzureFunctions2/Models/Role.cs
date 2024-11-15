using System;
using System.Collections.Generic;

namespace LibraryAzureFunctions2.Models
{
    public partial class Role
    {
        public Role()
        {
            People = new HashSet<Person>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<Person> People { get; set; }
    }
}
