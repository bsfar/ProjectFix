using InstrumentService.Models;
using InstrumentService_DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentService_DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public ApplicationTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(ApplicationType category)
        {
            var objFromDb = await base.FirstOrDefaultAsync(u => u.Id == category.Id);
            if (objFromDb != null)
            {
                objFromDb.Name= category.Name;
            }
        }


    }
}
