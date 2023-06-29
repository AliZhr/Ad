using System;
using System.Collections.Generic;

#nullable disable

namespace Adient_DashBoard.Models
{
    public partial class TUser
    {
        public string FUserId { get; set; }
        public string FLastName { get; set; }
        public string FFirstName { get; set; }
        public string FPassword { get; set; }
        /*public char FLastAccess { get; set; }*/
        public string FEmail { get; set; }
        public string FChangePw { get; set; }
        public string FActive { get; set; }
    }
    

}
