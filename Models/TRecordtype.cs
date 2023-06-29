using System;
using System.Collections.Generic;

#nullable disable

namespace Adient_DashBoard.Models
{
    public partial class TRecordtype
    {
        public TRecordtype()
        {
            TDataplants = new HashSet<TDataplant>();
            TDatarecords = new HashSet<TDatarecord>();
        }

        public int FTypeid { get; set; }
        public string FTypename { get; set; }
        public string FTypedescription { get; set; }
        public bool? FBand { get; set; }

        public virtual ICollection<TDataplant> TDataplants { get; set; }
        public virtual ICollection<TDatarecord> TDatarecords { get; set; }
    }
}
