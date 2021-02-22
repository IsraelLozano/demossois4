using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dic.sso.webapi.company.Repository
{
    public interface IRepositoryManager
    {
        ICompanyRepository Company { get; }
    }
}
