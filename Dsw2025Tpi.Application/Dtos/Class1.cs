namespace Dsw2025Tpi.Application.Dtos;
public record ProductModel
{
	public record ProductRequest(string Sku, string InternalCode, string Name, string Description, decimal CurrentUnitPrice, int StockQuantity);

	//si esto tiene que estar como el Modelo de Dominio esta diferente falta isActive
	public record ProductResponse(Guid Id, string Sku, string Name, decimal CurrentUnitPrice, string InternalCode, string Description, int StockQuantity);

	public record ProductResponseUpdate(Guid Id, string Sku, string Name, decimal CurrentUnitPrice, string InternalCode, string Description, int StockQuantity, bool IsActive);
	
	public record ProductResponseID(Guid Id);
}