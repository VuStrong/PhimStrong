using PhimStrong.Domain.PagingModel;
using System.Linq.Expressions;

namespace PhimStrong.Domain.Interfaces
{
	public interface IRepository<T> where T : class
	{
		/// <summary>
		///	Get IEnumarable of T entities
		/// </summary>
		/// <param name="filter">A lambda expression based on the T type to filter entities</param>
		/// <param name="orderBy">A lambda expression return an ordered version of that IQueryable object</param>
		/// <param name="includes">An array of lambda expression to include properties</param>
		/// <returns></returns>
		Task<IEnumerable<T>> GetAsync(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			Expression<Func<T, object?>>[]? includes = null
		);

		/// <summary>
		/// Get PagedList of T entities
		/// </summary>
		/// <param name="pagingParameter">Class for paging</param>
		/// <param name="filter">A lambda expression based on the T type to filter entities</param>
		/// <param name="orderBy">A lambda expression return an ordered version of that IQueryable object</param>
		/// <param name="includes">An array of lambda expression to include properties</param>
		/// <returns></returns>
		Task<PagedList<T>> GetAsync(
			PagingParameter pagingParameter,
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			Expression<Func<T, object?>>[]? includes = null
		);

		/// <summary>
		/// Return the first entity match the filter
		/// </summary>
		/// <param name="filter">A lambda expression based on the T type to filter entity</param>
		/// <param name="includes">An array of lambda expression to include properties</param>
		/// <returns></returns>
		Task<T?> FirstOrDefaultAsync(
			Expression<Func<T, bool>> filter,
			Expression<Func<T, object?>>[]? includes = null
		);

		T Create(T entity);
		
		T Update(T entity);
		
		void Delete(T entity);

		Task<bool> AnyAsync();
	}
}
