using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmnimetSQLXAML
{
     public class Licenses
    {
        public int Id { get; set; }
        public string License { get; set; }
        public string Version { get; set; }
        public string SType { get; set; }
        public bool Dongle { get; set; }
        public string TheDate { get; set; }
        public string Customer { get; set; }
        public string Seller { get; set; }
        public string Notes { get; set; }


        public override string ToString()
        {
           return string.Format("{0}",Version);
        }
    }
}
