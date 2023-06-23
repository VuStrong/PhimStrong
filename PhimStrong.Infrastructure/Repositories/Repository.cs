using Microsoft.EntityFrameworkCore;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using PhimStrong.Infrastructure.Context;
using System.Linq.Expressions;

namespace PhimStrong.Infrastructure.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
	{
		protected readonly PhimStrongDbContext _context;

		public Repository(PhimStrongDbContext context)
		{
			_context = context;
		}

		public virtual async Task<IEnumerable<T>> GetAsync(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			Expression<Func<T, object?>>[]? includes = null)
		{
			IQueryable<T> query = _context.Set<T>();
			
			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (orderBy != null)
			{
				query = orderBy(query);
			}

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			return await query.ToListAsync();
		}

		public virtual async Task<PagedList<T>> GetAsync(
			PagingParameter pagingParameter,
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			Expression<Func<T, object?>>[]? includes = null)
		{
			IQueryable<T> query = _context.Set<T>();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (orderBy != null)
			{
				query = orderBy(query);
			}

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			return await PagedList<T>.ToPagedListAsync(
				query, pagingParameter.Page, pagingParameter.Size, pagingParameter.AllowCalculateCount);
		}

		public virtual T Create(T entity)
		{
			return _context.Add(entity).Entity;
		}

		public virtual void Delete(T entity)
		{
			_context.Remove(entity);
		}

		public virtual async Task<T?> FirstOrDefaultAsync(
			Expression<Func<T, bool>> filter,
			Expression<Func<T, object?>>[]? includes = null)
		{
			IQueryable<T> query = _context.Set<T>();

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
				
				if (includes.Length > 1)
				{
					query = query.AsSplitQuery();
				}
			}

			return await query.FirstOrDefaultAsync(filter);
		}

		public virtual T Update(T entity)
		{
			return _context.Update(entity).Entity;
		}

		public async Task<bool> AnyAsync()
		{
			return await _context.Set<T>().AnyAsync();
		}

		public async Task<int> CountAsync()
		{
			return await _context.Set<T>().CountAsync();
		}
	}
}
