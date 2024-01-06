using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOMIOD.Requests
{
    public class RequestCreateDataSubscription
    {
        public string Res_type { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Event { get; set; }
        public string Endpoint { get; set; }
    }
}