using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Volleyball_Teams.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> UpdateItemsAsync(List<T> item);
        Task<bool> DeleteItemAsync(T item);
        Task DeleteAllItemsAsync();
        Task<T?> GetItemAsync(int id);
        Task<T?> GetItemByNameAsync(string name);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
        Task<IEnumerable<T>> GetItemsHereAsync(bool forceRefresh = false);
    }
}
