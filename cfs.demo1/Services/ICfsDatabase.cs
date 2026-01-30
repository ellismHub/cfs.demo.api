using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfs.demo.Models;

namespace cfs.demo.Services
{
    public interface ICfsDatabase
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid id);
    }
}