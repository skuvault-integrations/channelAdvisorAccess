using System;
using System.Runtime.Caching;

namespace ChannelAdvisorAccess.Misc
{
	public class CacheManager
	{
		private readonly ObjectCache _cache;
		private readonly TimeSpan _slidingExpiration = new TimeSpan( 2, 0, 0, 0 );

		public CacheManager( ObjectCache cache )
		{
			this._cache = cache;
		}

		public T Get< T >( string id )
		{
			return ( T )this._cache[ this.GetId( id ) ];
		}

		public void AddOrUpdate< T >( T obj, string id )
		{
			this.AddOrUpdate< T >( obj, id, this._slidingExpiration );
		}

		public void AddOrUpdate< T >( T obj, string id, TimeSpan slidingExpiration )
		{
			id = this.GetId( id );
			if( this._cache.Contains( id ) )
				this._cache.Remove( id );
			var policy = new CacheItemPolicy { SlidingExpiration = slidingExpiration };
			this._cache.Set( id, obj, policy );
		}

		public void Remove( string id )
		{
			this._cache.Remove( id );
		}

		private string GetId( string id )
		{
			return string.Format( "CAAccess_Error429_{0}", id );
		}
	}

	public class Data429Error
	{
		public string AccountId{ get; set; }
		public DateTime DateAppearance{ get; set; }
		public DateTime DateResolve{ get; set; }

		private Data429Error()
		{
		}

		public Data429Error( string accountId, DateTime dateAppearance, DateTime dateResolve )
		{
			this.AccountId = accountId;
			this.DateAppearance = dateAppearance;
			this.DateResolve = dateResolve;
		}
	}
}