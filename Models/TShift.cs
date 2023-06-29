using System;
using System.Collections.Generic;

#nullable disable

namespace Adient_DashBoard.Models
{
    public partial class TShift
    {
        public TShift()
        {
            TDatarecords = new HashSet<TDatarecord>();
        }

        public int FShiftid { get; set; }
        public int FConfigid { get; set; }
        public string FHall { get; set; }
        public string FCoordinator { get; set; }
        public string FSection { get; set; }
        public DateTime FDate { get; set; }
        public DateTime FTimestamp { get; set; }
        public string FShift { get; set; }

        public virtual TConfig FConfig { get; set; }
        public virtual ICollection<TDatarecord> TDatarecords { get; set; }
    }
}
