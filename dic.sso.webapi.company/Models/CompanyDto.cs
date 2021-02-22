using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dic.sso.webapi.company.Models
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FullAddress { get; set; }
    }
}
