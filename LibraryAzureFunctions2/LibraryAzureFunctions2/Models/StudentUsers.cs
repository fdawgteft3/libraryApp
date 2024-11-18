using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAzureFunctions2.Models
{
    public partial class StudentUsers
    {
        [Key]
        public string UserId {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SelectedRoleIds {  get; set; }
        public virtual ICollection<Borrow_Record> Borrow_Records { get; set; }

    }
}
