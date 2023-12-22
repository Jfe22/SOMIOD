using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOMIOD.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Creation_Date { get; set; }
        public int Parent_Id { get; set; }
        public string Event { get; set; }
        public string Endpoint { get; set; }
    }
}