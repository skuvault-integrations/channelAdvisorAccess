namespace ChannelAdvisorAccess.CurrencyConversion
{
	public interface ICurrencyConverter
	{
		/// <summary>Converts currency from specified country to base country using current conversion ratio.</summary>
		/// <param name="countryCode">The country code.</param>
		/// <param name="countryCurrency">The country currency.</param>
		/// <returns>Country currency converted to base currency.</returns>
		decimal ToBaseCurrency( string countryCode, decimal countryCurrency );

		/// <summary>Converts currency from the base currency to the specified country currency</summary>
		/// <param name="countryCode">The country code.</param>
		/// <param name="baseCurrency">The base currency.</param>
		/// <returns>Base currency converted to the specified country currency.</returns>
		decimal ToCountryCurrency( string countryCode, decimal baseCurrency );
	}
}