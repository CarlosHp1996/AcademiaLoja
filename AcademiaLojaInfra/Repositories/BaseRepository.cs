﻿using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Helpers;
using AcademiaLoja.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLoja.Infra.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<TEntity> _entities;

        public BaseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _entities = dbContext.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<AsyncOutResult<IEnumerable<TEntity>, int>> GetAll(int? take, int? offSet, string sortingProp, bool? asc)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (!string.IsNullOrEmpty(sortingProp) && asc != null)
                if (DataHelpers.CheckExistingProperty<TEntity>(sortingProp))
                    query = query.OrderByDynamic(sortingProp, (bool)asc);

            if (take != null && offSet != null)
                return new AsyncOutResult<IEnumerable<TEntity>, int>(await query.Skip((int)offSet).Take((int)take).ToListAsync(), await query.CountAsync());

            return new AsyncOutResult<IEnumerable<TEntity>, int>(await query.ToListAsync(), await query.CountAsync());
        }

        public virtual async Task<IEnumerable<TEntity>> Get(int? take, int? offSet, string sortingProp, bool? asc)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (!string.IsNullOrEmpty(sortingProp) && asc != null)
                if (DataHelpers.CheckExistingProperty<TEntity>(sortingProp))
                    query = query.OrderByDynamic(sortingProp, (bool)asc);

            if (take != null && offSet != null)
                return await query.Skip((int)offSet).Take((int)take).ToListAsync();

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entity)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Remove(entity);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<TEntity> listEntity)
        {
            try
            {
                _dbContext.Set<TEntity>().RemoveRange(listEntity);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                _dbContext.Update(entity);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<TEntity> listEntity)
        {
            try
            {
                _dbContext.UpdateRange(listEntity);

                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<AsyncOutResult<IEnumerable<TEntity>, int>> PaginateAsync(
     int? take,
     int? offSet,
     string sortingProp,
     bool? asc)
        {
            IQueryable<TEntity> query = _entities.AsQueryable();

            if (!string.IsNullOrEmpty(sortingProp) && asc.HasValue)
            {
                if (DataHelpers.CheckExistingProperty<TEntity>(sortingProp))
                {
                    query = query.OrderByDynamic(sortingProp, asc.Value);
                }
            }

            // Counting total items
            var totalItems = await query.CountAsync();

            // Applying pagination
            if (take.HasValue && offSet.HasValue)
            {
                query = query.Skip(offSet.Value).Take(take.Value);
            }

            // Returning the paginated list
            var result = await query.ToListAsync();

            return new AsyncOutResult<IEnumerable<TEntity>, int>(result, totalItems);
        }

    }
}
