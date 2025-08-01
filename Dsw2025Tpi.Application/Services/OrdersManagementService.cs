using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.DTOs;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService
    {
        private readonly IRepository _repository;

        public OrdersManagementService(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Crea una nueva orden, valida stock y calcula montos.
        /// </summary>
        public async Task<OrderDto> CreateOrderAsync(OrderCreateDto dto)
        {
            // Validar cliente existente
            var customer = await _repository.GetById<Customer>(dto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Cliente no encontrado.");

            // Crear entidad Orden
            var order = new Order(dto.CustomerId, DateTime.UtcNow);
            decimal subtotal = 0;

            // Procesar cada item
            foreach (var item in dto.Items)
            {
                var product = await _repository.GetById<Product>(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Producto {item.ProductId} no existe.");
                if (item.Quantity <= 0 || item.Quantity > product.StockQuantity)
                    throw new ArgumentException($"Cantidad inválida para producto {product.Name}.");

                // Reducir stock
                product.StockQuantity -= item.Quantity;
                await _repository.Update(product);

                // Agregar detalle a la orden
                order.AddItem(product.Id, product.Name, product.CurrentUnitPrice, item.Quantity);
                subtotal += product.CurrentUnitPrice * item.Quantity;
            }

            // Asignar montos
            order.SetSubtotal(subtotal);
            order.SetTotal(subtotal); // si no hay impuestos adicionales

            // Guardar orden
            await _repository.Add(order);

            // Mapear a DTO
            var response = new OrderDto(
                order.Id,
                order.CustomerId,
                order.CreatedAt,
                order.Subtotal,
                order.Total,
                order.Status.ToString(),
                order.Items.Select(i => new OrderItemDetailDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.LineTotal))
            );

            return response;
        }

        /// <summary>
        /// Recupera órdenes paginadas.
        /// </summary>
        public async Task<PagedResult<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            var allOrders = await _repository.GetAll<Order>();
            var ordersList = allOrders?.ToList() ?? new List<Order>();
            var total = ordersList.Count;
            var pageItems = ordersList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = pageItems.Select(order => new OrderDto(
                order.Id,
                order.CustomerId,
                order.CreatedAt,
                order.Subtotal,
                order.Total,
                order.Status.ToString(),
                order.Items.Select(i => new OrderItemDetailDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.LineTotal))))
                .ToList();

            return new PagedResult<OrderDto>(dtos, total, pageNumber, pageSize);
        }

        /// <summary>
        /// Obtiene una orden por su Id.
        /// </summary>
        public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _repository.GetById<Order>(id, "Items");
            if (order == null)
                return null;

            return new OrderDto(
                order.Id,
                order.CustomerId,
                order.CreatedAt,
                order.Subtotal,
                order.Total,
                order.Status.ToString(),
                order.Items.Select(i => new OrderItemDetailDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.LineTotal)));
        }

        /// <summary>
        /// Actualiza el estado de una orden.
        /// </summary>
        public async Task<OrderDto?> UpdateOrderStatusAsync(Guid id, string status)
        {
            var order = await _repository.GetById<Order>(id);
            if (order == null)
                return null;

            // Validar y asignar nuevo estado
            if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
                throw new ArgumentException("Estado de orden inválido.");

            order.UpdateStatus(newStatus);
            await _repository.Update(order);

            return new OrderDto(
                order.Id,
                order.CustomerId,
                order.CreatedAt,
                order.Subtotal,
                order.Total,
                order.Status.ToString(),
                order.Items.Select(i => new OrderItemDetailDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.LineTotal)));
        }
    }
}
