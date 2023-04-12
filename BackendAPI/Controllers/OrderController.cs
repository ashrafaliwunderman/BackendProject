using AutoMapper;
using BackendAPI.Model;
using Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Reflection.Metadata.Ecma335;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task Post(Model.Order order)
        {
            var orderProduct = order.OrderProduct.GroupBy(x => x.ProductID).Select( x => new Model.OrderProduct { 
                ProductID = x.Select(x => x.ProductID).FirstOrDefault(),
                Quantity = x.Sum(x => x.Quantity)
             }).ToList();

            order.OrderProduct = orderProduct;
            await _orderService.AddOrder(_mapper.Map<Model.Order, Core.Model.Order>(order));
        }

        [HttpPost("closePeddingOrder/")]
        public async Task Post()
        {
            await _orderService.ClosePenddingOrder();
        }

        [HttpGet]
        public async Task<IEnumerable<Model.Order>> Get()
        {
            List<Core.Model.Order> res = (List<Core.Model.Order>) await _orderService.GetAllOrders();
            return _mapper.Map<List<Core.Model.Order>, List<Model.Order>>(res);
        }
        
        [HttpPost("CloseOrder/{id}")]
        public async Task Post(int id)
        {
            await _orderService.CloseOrder(id);
        }
    }
}
