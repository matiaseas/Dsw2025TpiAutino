using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;
public class ProductsController : ControllerBase
{
	private readonly ProductsManagementService _service;

	public ProductsController(ProductsManagementService service)
	{
		_service = service;
	}
}

