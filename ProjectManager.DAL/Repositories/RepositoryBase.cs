using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Contracts;
using ProjectManager.DAL.Data;

namespace ProjectManager.DAL.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly DataContext Context;
        protected readonly DbSet<T> DbSet;

        protected RepositoryBase(DataContext context)
        {
            this.Context = context;
            DbSet = context.Set<T>();
        }

        public void Insert(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(int id)
        {
            DbSet.Remove(DbSet.Find(id));
        }

        public IEnumerable<T> GetAll()
        {
            return DbSet;
        }

        public T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public void Update(T entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public int Commit()
        {
            return Context.SaveChanges();
        }
    }
}
