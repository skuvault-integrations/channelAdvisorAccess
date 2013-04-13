using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.Misc
{
	public static class LinqExtensions
	{
		public static IEnumerable< TResult > ProcessWithPages< TData, TResult >( this IEnumerable< TData > data, int pageSize,
			Func< IEnumerable< TData >, IEnumerable< TResult > > processor )
		{
			var dataList = data as IList< TData > ?? data.ToList();
			var pagesCount = dataList.Count / pageSize + ( dataList.Count % pageSize > 0 ? 1 : 0 );

			var pages = new ConcurrentDictionary< int, IEnumerable< TResult > >();

			var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 12 };
			Parallel.For( 0, pagesCount, options, pageNumber =>
				{
					var dataPage = dataList.Skip( pageNumber * pageSize ).Take( pageSize );
					pages[ pageNumber ] = processor( dataPage );
				});

			// NOTE: force ToList to avoid possible multiple redownloads to reiterate over the list
			return pages.OrderBy( kv => kv.Key ).SelectMany( kv => kv.Value ).ToList();
		}

		public static void DoWithPages< TData >( this IEnumerable< TData > data, int pageSize, Action< IEnumerable< TData > > processor )
		{
			var dataList = data as IList< TData > ?? data.ToList();
			var pagesCount = dataList.Count / pageSize + ( dataList.Count % pageSize > 0 ? 1 : 0 );

			var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 12 };
			Parallel.For( 0, pagesCount, options, pageNumber =>
				{
					var dataPage = dataList.Skip( pageNumber * pageSize ).Take( pageSize );
					processor( dataPage );
				});
		}
	}
}