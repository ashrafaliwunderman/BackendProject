using Core.IRepository;
using Core.Model;
using Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class InventoryRepository : GenericRepository<Product>, IInvertoryRepository
    {
        
        public InventoryRepository(BaseDbContext context) { 
            base.context = context;
            base.dbSet = context.Set<Product>();
        }

        public async Task<int> AddProduct(Product product) 
        {
            await base.InsertData(product);
            return await base.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            await base.DeleteData(id);
            await base.SaveChangesAsync(); 
        }

        public async Task UpdateProduct(Product product)
        {
            await base.UpdateData(product);
            await base.SaveChangesAsync();
        }
    }
}
