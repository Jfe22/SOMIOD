﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SOMIOD.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Creation_dt { get; set; }
        
    }
}