using Dsw2025Tpi.Application.DTOs;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersManagementService _service;

        public OrdersController(OrdersManagementService service)
        {
            _service = service;
        }

        /// <summary>
        /// Crea una nueva orden.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Obtiene las órdenes paginadas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<OrderDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] OrderQueryParameters query)
        {
            var result = await _service.GetOrdersAsync(query.PageNumber, query.PageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una orden por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _service.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        /// <summary>
        /// Actualiza el estado de una orden.
        /// </summary>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateOrderStatusAsync(id, dto.Status);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
    }
}
