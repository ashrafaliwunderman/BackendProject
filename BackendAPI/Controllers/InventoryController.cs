using AutoMapper;
using BackendAPI.Model;
using Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInvetoryService _invertoryService;
        private readonly IMapper _mapper;

        public InventoryController(IInvetoryService inventoryService, IMapper mapper)
        {
            _invertoryService = inventoryService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<int> Post(Model.Product product)
        {
            return await _invertoryService.AddProduct(_mapper.Map<Model.Product, Core.Model.Product>(product));
        }

        [HttpPut]
        public async Task Put(ProductEdit product)
        {
            await _invertoryService.UpdateProduct(_mapper.Map<Model.Product, Core.Model.Product>(product));
        }


        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _invertoryService.DeleteProduct(id);
        }
    }
}
