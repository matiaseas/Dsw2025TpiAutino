using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data.Repositories;

internal class InMemory : IRepository
{
    private List<Customer>? _customers;
    public InMemory()
    {
        LoadMemory();
    }
    private void LoadMemory()
    {
        var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Sources\\products.json"));
        _customers = JsonSerializer.Deserialize<List<Customer>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
    }

    public List<T>? GetList<T>() where T : EntityBase
    {
        return typeof(T).Name switch
        {
            nameof(Customer) => _customers as List<T>,
            _ => throw new NotSupportedException(),
        };
    }

    public Task<T> Add<T>(T entity) where T : EntityBase
    {
        throw new NotImplementedException();
    }

    public Task<T> Delete<T>(T entity) where T : EntityBase
    {
        throw new NotImplementedException();
    }

    public Task<T?> First<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<T>?> GetAll<T>(params string[] include) where T : EntityBase
    {
        return await Task.FromResult(GetList<T>());
    }

    public Task<T?> GetById<T>(Guid id, params string[] include) where T : EntityBase
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>?> GetFiltered<T>(Expression<Func<T, bool>> predicate, params string[] include) where T : EntityBase
    {
        throw new NotImplementedException();
    }

    public Task<T> Update<T>(T entity) where T : EntityBase
    {
        throw new NotImplementedException();
    }
}
