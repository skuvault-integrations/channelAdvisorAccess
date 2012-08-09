ChannelAdvisorAccess is .NET wrapper around [Channel Advisor API](http://developer.channeladvisor.com/display/cadn/ChannelAdvisor+Developer+Network) I created to simplify and speed up working with it.

Features
========
* Channel Advisor provides different webservices for different parts of their system: listing, orders, inventory, etc. All these services are consolidated into specific account through `ChannelAdvisorAcount` class. 
* Server requests will by retried several times in case of an error or time out (in case CA servers are unstable or overloaded right now) using action policy.
* Automatic pagination when working with bulks of items or orders.
* Automatic caching of downloaded results. For example if several reports would required download of the whole inventory, it is actually downloaded only once. Cached data is provided for consecutive calls to get inventory.

Usage
=====
Account constructor takes all information required to authenticate with Channel Advisor servers, as well as interfaces for `IChannelAdvisorServicesFactory` and `ICurrencyConverter`:
* `IChannelAdvisorServicesFactory` - class responsible for creating services to access CA servers. `ChannelAdvisorServicesFactory` is default implementation.
* `ICurrencyConverter` - helper class to allow currency conversion between different countries. Useful for working with accounts for countries outside of the main country. `YahooCurrencyConverter` is the default implementation.

In addition each account can be marked as active or inactive. Just in case you have more accounts in CA, but don't use some of them.

`ChannelAdvisorManager` is a helper class to assist working with several accounts for different countries. It can find account or specific service (inventory, orders, etc.) by account name or id. `ChannelAdvisorManager` extensions provide shortcuts for common operations by account id (`GetOrders`, etc.)

Example initialization

	var servicesFactory = new ChannelAdvisorServicesFactory( "devKey", "devPass", MemoryCache.Default, TimeSpan.FromHours( 3 ) );
	var currencyConverter = new YahooCurrencyConverter( "USD" );
	var accountUS = new ChannelAdvisorAccount( "accountName", "id", "USD", true, servicesFactory, currencyConverter );
	var accountUK = new ChannelAdvisorAccount( "accountNameUk", "idUk", "GBP", true, servicesFactory, currencyConverter );
	var manager = new ChannelAdvisorManager( new []{ accountUS, accountUK });