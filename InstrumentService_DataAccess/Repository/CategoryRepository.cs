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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Category category)
        {
            var objFromDb = await FirstOrDefaultAsync(u => u.Id == category.Id);
            if (objFromDb != null)
            {
                objFromDb.Name= category.Name;
                objFromDb.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}
