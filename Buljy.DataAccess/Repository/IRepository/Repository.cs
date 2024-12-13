using Buljy.DataAccess.Data;
using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Buljy.DataAccess.Reposoitory.IRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext context;
        internal DbSet<T> dbset;
        public Repository(AppDbContext context )
        {
            this.context = context;
            this.dbset = context.Set<T>();
        }




        public async Task Add(T entity)
        {
            await dbset.AddAsync(entity);
        }

        public  async Task Delete(T entity)
        {
             context.Remove(entity);
        }

        public async Task DeleteRange(T entities)
        {
            context.RemoveRange(entities);
        }


        public async Task<T?> Get(
            Expression<Func<T, bool>> filter,
            bool asNoTracking = false,
            string? includeProperties = null)
        {
            IQueryable<T> query = dbset;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
                Console.WriteLine("Query set to NoTracking mode.");
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Console.WriteLine($"Including property: {includeProperty}");
                    query = query.Include(includeProperty);
                }
            }

           

            var result = await query.FirstOrDefaultAsync(filter);
            return result;
        }


        public async Task<IEnumerable<T>> GetAll(
            Expression<Func<T, bool>> filter = null,
            string? includeProperties = null,
            bool asNoTracking = true)
        {
            IQueryable<T> query = dbset;

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include related entities if includeProperties is specified
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Apply AsNoTracking based on the asNoTracking flag
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetValue(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> Query = dbset;

            return await Query.FirstOrDefaultAsync(filter);
        }
       
       
    }
}
