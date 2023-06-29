using System;
using System.Collections.Generic;

#nullable disable

namespace Adient_DashBoard.Models
{
    public partial class TDatarecord
    {
        public int FRecordid { get; set; }
        public int FTypeid { get; set; }
        public int FShiftid { get; set; }
        public int FRecordtypeid { get; set; }
        public double FRecordvalue { get; set; }

        public virtual TShift FShift { get; set; }
        public virtual TRecordtype FType { get; set; }
    }
}
