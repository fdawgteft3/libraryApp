using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryFunctions.Models
{
    public partial class Borrow_Record
    {
        public int RecordNumber { get; set; }
        public string UserId { get; set; }
        public string Isbn { get; set; }
        public DateTime DateBorrowed { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public decimal LateFee { get; set; }
        public decimal? OutstandingFee { get; set; }
        public decimal? AmountPaid { get; set; }
        public bool? IsActive { get; set; }
        [JsonIgnore]
        public virtual StudentUser User { get; set; }
        [JsonIgnore]
        public virtual BookCopy Copy { get; set; }
    }
}
