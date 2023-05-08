using Microsoft.EntityFrameworkCore;

namespace PhimStrong.Domain.PagingModel
{
	public class PagedList<T> : List<T>
	{
		public int CurrentPage { get; set; }
		public int TotalPage { get; set; }
		public int PageSize { get; set; }
		public int TotalItems { get; set; }

		public PagedList() { }

		public PagedList(List<T> items, int page, int size, int totalItems) 
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			this.AddRange(items);

			TotalPage = (int)Math.Ceiling((double)totalItems / size);
			CurrentPage = page;
			PageSize = size;
			TotalItems = totalItems;
		}

		public static async Task<PagedList<T>> ToPagedList(IQueryable<T> items, int page, int size)
		{
			int totalItems = await items.CountAsync();

			if (size <= 0) size = 1;

			int totalPage = (int)Math.Ceiling((double)totalItems / size);
			
			if (totalPage <= 0) totalPage = 1;
			
			page = Math.Clamp(page, 1, totalPage);

			var pagingItems = await items.Skip((page - 1) * size).Take(size).ToListAsync();

			return new PagedList<T>(pagingItems, page, size, totalItems);
		}
	}
}
