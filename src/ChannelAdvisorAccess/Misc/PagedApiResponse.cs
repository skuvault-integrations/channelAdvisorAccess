using System.Collections.Generic;
using System.Linq;

namespace ChannelAdvisorAccess.Misc
{
	/// <summary>
	/// Wrapper for API Response. Used for keeping track of paging.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PagedApiResponse< T >
	{
		/// <summary>
		/// Response from the API request
		/// </summary>
		public IEnumerable< T > Response{ get; private set; }

		/// <summary>
		/// Final page number used in query
		/// </summary>
		public int FinalPageNumber{ get; private set; }

		/// <summary>
		/// When true, indicates that there are no more pages to query
		/// </summary>
		public bool AllPagesQueried { get; private set; }

		public PagedApiResponse( IEnumerable< T > results, int pageNumber, bool allPages )
		{
			Response = results;
			FinalPageNumber = pageNumber;
			AllPagesQueried = allPages;
		}

		public static PagedApiResponse< T > Mock()
		{
			return new PagedApiResponse< T >( Enumerable.Empty< T >(), 1, true ); 
		}
	}
}
