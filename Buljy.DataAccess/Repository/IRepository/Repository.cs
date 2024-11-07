﻿using Buljy.DataAccess.Data;
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

        public async Task<T?> Get(Expression<Func<T, bool>> filter, bool asNoTracking = false)
        {
            IQueryable<T> query = dbset;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<IEnumerable<T>> GetAll()
        {

            IQueryable<T> query = dbset;
            return await query.ToListAsync();
        }

        
       

       
    }
}
