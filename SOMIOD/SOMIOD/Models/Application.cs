using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SOMIOD.Models
{
    //[Serializable]
    //[XmlRoot("Application")]
    public class Application
    {
       // [XmlElement("Id")]
        public int Id { get; set; }

       // [XmlElement("Name")]
        public string Name { get; set; }

      //  [XmlElement("Creation_Dt")]
       // public DateTime Creation_Dt { get; set; }
        public string Creation_Dt { get; set; }
        
    }
}