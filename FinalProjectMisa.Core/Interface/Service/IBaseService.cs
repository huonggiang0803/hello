namespace FinalProjectMisa.Core.Interface.Service;

public interface IBaseService<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> InsertServiceAsync(T entity);
    Task<int> UpdateServiceAsync(Guid id, T entity);
    Task<int> DeleteServiceAsync(Guid id);
}