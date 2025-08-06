using Dsw2025Ej15.Application.Exceptions;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/products/")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductsManagementService _service;
    public ProductsController(IProductsManagementService service)
    {
        _service = service;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _service.GetProducts();
            return Ok(products);
        }
        catch (EntityNotFoundException enfe)
        {
            return StatusCode(204, enfe.Message);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }     
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        try
        {
            var product = await _service.GetProductById(id);
            return Ok(product);
        }
        catch (EntityNotFoundException enfe)
        {
            return NotFound(enfe.Message);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }

    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody]ProductModel.ProductRequest request)
    {
        try
        {
            var product = await _service.AddProduct(request);
            return Created("/product",product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (DuplicatedEntityException de)
        {
            return BadRequest(de.Message);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.ProductRequest request)
    {
        try
        {
            var updatedProduct = await _service.Update(id, request);
            if (updatedProduct == null) throw new EntityNotFoundException($"No se encontró un producto con el ID {id}");
            return Ok(updatedProduct);
        }
        catch (EntityNotFoundException ef)
        {
            return NotFound(ef.Message);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (DuplicatedEntityException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> ToggleProductStatus(Guid id)
    {
        try
        {
            var updatedProduct = await _service.ToggleStatus(id);
            if (updatedProduct == null) throw new EntityNotFoundException($"No se encontró un producto con el ID {id}");
            return Ok(updatedProduct);
        }
        catch (EntityNotFoundException ef)
        {
            return NotFound(ef.Message);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
