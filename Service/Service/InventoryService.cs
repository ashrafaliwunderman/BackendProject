using Core.IRepository;
using Core.Model;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class InventoryService : IInvetoryService
    {
        private readonly IInvertoryRepository _inventoryRepo;

        public InventoryService(IInvertoryRepository inventoryRepo)
        {
            this._inventoryRepo = inventoryRepo;
        }

        public async Task<int> AddProduct(Product product)
        {
            return await _inventoryRepo.AddProduct(product);
        }

        public async Task DeleteProduct(int id)
        {
            await _inventoryRepo.DeleteProduct(id);
        }

        public async Task UpdateProduct(Product product)
        {
            await _inventoryRepo.UpdateProduct(product);
        }
    }
}
