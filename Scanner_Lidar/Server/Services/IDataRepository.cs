using System;
using System.Collections.Generic;
using System.Text;

namespace Server_Lidar.Services
{
    public interface IDataRepository<T>
    {
        T Get(int id);
        IEnumerable<T> Get();
        T Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
