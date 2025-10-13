using LAFABRICA.Data.DB;
using LAFABRICA.Models.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LAFABRICA.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _service.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _service.GetById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            try
            {
                await _service.Update(id, order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            var createdOrder = await _service.Create(order);
            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var success = await _service.Delete(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}