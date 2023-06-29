using System;
using System.Collections.Generic;

#nullable disable

namespace Adient_DashBoard.Models
{
    public partial class TDataplant
    {
        public int FId { get; set; }
        public int FValue { get; set; }
        public int FTypeid { get; set; }
        public DateTime FDate { get; set; }

        public virtual TRecordtype FType { get; set; }
    }
}
