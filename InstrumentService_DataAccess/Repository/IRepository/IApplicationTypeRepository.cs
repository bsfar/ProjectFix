using InstrumentService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentService_DataAccess.Repository.IRepository
{
    public interface IApplicationTypeRepository : IRepository<ApplicationType>
    {
        Task UpdateAsync(ApplicationType applicationType);
    }
}
