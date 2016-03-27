using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Contracts
{
    public interface IRepository<T> where T : class 
    {
        void Insert(T entity);

        void Delete(int id);

        IEnumerable<T> GetAll();

        T GetById(int id);

        void Update(T entity);

        int Commit();
    }
}
