using System;
using System.Collections.Generic;

#nullable disable

namespace Adient_DashBoard.Models
{
    public partial class TConfig
    {
        public TConfig()
        {
            TShifts = new HashSet<TShift>();
        }

        public int FConfigid { get; set; }
        public string FField { get; set; }
        public int FValue { get; set; }

        public virtual ICollection<TShift> TShifts { get; set; }
    }
}
