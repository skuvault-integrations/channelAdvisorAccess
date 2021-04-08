using System;
using System.Collections.Generic;
using ChannelAdvisorAccess.CurrencyConversion;
using ChannelAdvisorAccess.Misc;
using ChannelAdvisorAccess.Services;
using ChannelAdvisorAccess.Services.Items;
using ChannelAdvisorAccess.Services.Listing;
using ChannelAdvisorAccess.Services.Orders;
using ChannelAdvisorAccess.Services.Shipping;

namespace ChannelAdvisorAccess
{
	public class ChannelAdvisorAccount : IEquatable< ChannelAdvisorAccount >
	{
		private readonly ICurrencyConverter _currencyConverter;

		public ChannelAdvisorAccount( string name, string id, string currencyCode, bool isActive, bool isMain, IChannelAdvisorServicesFactory servicesFactory, ICurrencyConverter currencyConverter ) :
			this( name, id, 0, currencyCode, isActive, isMain, servicesFactory, currencyConverter )
		{
		}
		
		public ChannelAdvisorAccount( string name, string id, int privateId, string currencyCode, bool isActive, bool isMain, IChannelAdvisorServicesFactory servicesFactory, ICurrencyConverter currencyConverter )
		{
			this._currencyConverter = currencyConverter;
			this.Name = name;
			this.Id = id;
			this.PrivateId = privateId;
			this.CurrencyCode = currencyCode;
			this.IsActive = isActive;
			this.IsMain = isMain;

			this.ItemsService = servicesFactory.CreateItemsService( new ChannelAdvisorConfig( name ) { AccountId = id }, new REST.Shared.ChannelAdvisorTimeouts() );
			this.OrdersService = servicesFactory.CreateOrdersService( new ChannelAdvisorConfig( name ) { AccountId = id }, new REST.Shared.ChannelAdvisorTimeouts() );
			this.ShippingService = servicesFactory.CreateShippingService( name, id );
			this.ListingService = servicesFactory.CreateListingService( name, id );

			this.Meta = new Dictionary< string, object >();
		}

		public string Name { get; private set; }
		public string Id { get; private set; }
		public int PrivateId { get; private set; }
		public string CurrencyCode { get; private set; }
		public bool IsActive { get; private set; }
		public bool IsMain{ get; private set; }

		public IItemsService ItemsService { get; private set; }
		public IOrdersService OrdersService { get; private set; }
		public IShippingService ShippingService { get; private set; }
		public IListingService ListingService { get; private set; }

		/// <summary>
		/// Dictionary to store additional data related to account.
		/// </summary>
		/// <remarks>Can be used to store additional ids or related services. It's best to create extension methods to access specific meta data.</remarks>
		public Dictionary< string , object > Meta{ get; private set; }

		/// <summary>Converts currency from account currency to the base currency.</summary>
		/// <param name="accountCurrency">The account currency.</param>
		/// <returns>Account currency converted to base currency.</returns>
		/// <remarks>Account currency is specified by <see cref="CurrencyCode"/>.</remarks>
		public decimal ToBaseCurrency( decimal accountCurrency )
		{
			return this._currencyConverter.ToBaseCurrency( this.CurrencyCode, accountCurrency );
		}

		/// <summary>Converts currency from base currency to the currency of the account country specified by <see cref="CurrencyCode"/>.</summary>
		/// <param name="baseCurrency">The base currency.</param>
		/// <returns>Base currency converted to the currency specified by the account country.</returns>
		public decimal ToAccountCurrency( decimal baseCurrency )
		{
			return this._currencyConverter.ToCountryCurrency( this.CurrencyCode, baseCurrency );
		}

		public bool Equals( ChannelAdvisorAccount other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return other.IsActive.Equals( this.IsActive ) && Equals( other.CurrencyCode, this.CurrencyCode ) && Equals( other.Id, this.Id ) && Equals( other.Name, this.Name );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != typeof( ChannelAdvisorAccount ) )
				return false;
			return this.Equals( ( ChannelAdvisorAccount )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = this.IsActive.GetHashCode();
				result = ( result * 397 ) ^ ( this.CurrencyCode != null ? this.CurrencyCode.GetHashCode() : 0 );
				result = ( result * 397 ) ^ ( this.Id != null ? this.Id.GetHashCode() : 0 );
				result = ( result * 397 ) ^ ( this.Name != null ? this.Name.GetHashCode() : 0 );
				return result;
			}
		}
	}
}