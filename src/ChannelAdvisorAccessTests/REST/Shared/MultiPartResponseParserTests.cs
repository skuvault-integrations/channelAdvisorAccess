using ChannelAdvisorAccess.REST.Models;
using ChannelAdvisorAccess.REST.Models.Infrastructure;
using ChannelAdvisorAccess.REST.Shared;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ChannelAdvisorAccessTests.REST.Shared
{
	[ TestFixture ]
	public class MultiPartResponseParserTests
	{
		const string responseProductsSuccess = "{\"responses\":[{\"id\":\"1\",\"status\":200,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"@odata.context\":\"https://api.channeladvisor.com/v1/$metadata#Products(ID,Sku)\",\"value\":[{\"ID\":10062976,\"Sku\":\"testkit1\"},{\"ID\":11111111,\"Sku\":\"testsku1\"}]}},{\"id\":\"2\",\"status\":200,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"@odata.context\":\"https://api.channeladvisor.com/v1/$metadata#Products(ID,Sku)\",\"value\":[{\"ID\":10059896,\"Sku\":\"testkit2\"},{\"ID\":22222222,\"Sku\":\"testsku2\"}]}}]}";

		[ Test ]
		public void GivenBatchItemsReturn200Status_WhenCallMultiPartResponseParser_ThenReturns200BatchStatusCode()
		{
			int batchStatusCode;
			const int successCode = ( int ) HttpStatusCode.OK;
			IEnumerable< ODataResponse< Product > > parsedResponse;

			var isParseSuccessful = MultiPartResponseParser.TryParse< ODataResponse< Product > >( responseProductsSuccess, out batchStatusCode, out parsedResponse );

			isParseSuccessful.Should().BeTrue();
			batchStatusCode.Should().Be( successCode );
		}

		[ Test ]
		public void MultiPartResponseParser_ShouldReturn200Success_WhenProductHasNullTotalAvailableQuantity()
		{
			int batchStatusCode;
			const int successCode = ( int ) HttpStatusCode.OK;
			IEnumerable< ODataResponse< Product > > parsedResponse;
			var responseProductWithNullTotalAvailableQuantity = "{\"responses\":[{\"id\":\"3708\",\"status\":200,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"@odata.context\":\"https://api.channeladvisor.com/v1/$metadata#Products(Sku,TotalAvailableQuantity,DCQuantities,DCQuantities())\",\"value\":[{\"Sku\":\"someSku\",\"TotalAvailableQuantity\":null,\"DCQuantities\":[]}]}}]}";

			var isParseSuccessful = MultiPartResponseParser.TryParse< ODataResponse< Product > >( responseProductWithNullTotalAvailableQuantity, out batchStatusCode, out parsedResponse );

			isParseSuccessful.Should().BeTrue();
			batchStatusCode.Should().Be( successCode );
			parsedResponse.Single().Value.Single().TotalAvailableQuantity.Should().Be( null );
		}

		[ Test ]
		public void GivenBatchItemsProducts_WhenCallMultiPartResponseParser_ThenParsesProducts()
		{
			int batchStatusCode;
			IEnumerable< ODataResponse< Product > > parsedResponse;

			var isParseSuccessful = MultiPartResponseParser.TryParse< ODataResponse< Product > >( responseProductsSuccess, out batchStatusCode, out parsedResponse );
			var result = parsedResponse.ToList();

			isParseSuccessful.Should().BeTrue();
			result.Count().Should().Be( 2 );
			result[ 0 ].Value[ 0 ].ID.Should().Be( 10062976 );
			result[ 0 ].Value[ 0 ].Sku.Should().Be( "testkit1" );
			result[ 0 ].Value[ 1 ].ID.Should().Be( 11111111 );
			result[ 0 ].Value[ 1 ].Sku.Should().Be( "testsku1" );
			result[ 1 ].Value[ 0 ].ID.Should().Be( 10059896 );
			result[ 1 ].Value[ 0 ].Sku.Should().Be( "testkit2" );
			result[ 1 ].Value[ 1 ].ID.Should().Be( 22222222 );
			result[ 1 ].Value[ 1 ].Sku.Should().Be( "testsku2" );
		}

		[ Test ]
		public void GivenBatchItemsReturn401Status_WhenCallMultiPartResponseParser_ThenReturns401BatchStatusCode()
		{
			int batchStatusCode;
			const int unauthorized401 = ( int ) HttpStatusCode.Unauthorized;
			var responseItemsStatus401 = "{\"responses\":[{\"id\":\"1\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"2\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"3\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"4\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"5\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"6\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}}]}";

			IEnumerable< ODataResponse< Product > > parsedResponse;
			var isParseSuccessful = MultiPartResponseParser.TryParse< ODataResponse< Product > >( responseItemsStatus401, out batchStatusCode, out parsedResponse );
			var result = parsedResponse.ToList();

			isParseSuccessful.Should().BeTrue();
			batchStatusCode.Should().Be( unauthorized401 );
			result[ 0 ].Error.Message.Should().Be( "Authorization has been denied for this request." );
		}

		[ Test ]
		public void GivenSomeItemsHave401StatusButLastOneHas200_WhenCallMultiPartResponseParser_ThenReturns401BatchStatusCode()
		{
			int batchStatusCode;
			const int unauthorized401 = ( int ) HttpStatusCode.Unauthorized;
			var responseItems401TheLast200 = "{\"responses\":[{\"id\":\"1\",\"status\":500,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"2\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"3\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"4\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"5\",\"status\":401,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"error\":{\"code\":\"\",\"message\":\"Authorization has been denied for this request.\"}}},{\"id\":\"6\",\"status\":200,\"headers\":{\"content-type\":\"application/json; odata.metadata=minimal; odata.streaming=true\",\"odata-version\":\"4.0\"}, \"body\" :{\"@odata.context\":\"https://api.channeladvisor.com/v1/$metadata#Products(ID,Sku)\",\"value\":[{\"ID\":10059896,\"Sku\":\"testkit2\"},{\"ID\":22222222,\"Sku\":\"testsku2\"}]}}]}";

			IEnumerable< ODataResponse< Product > > parsedResponse;
			var isParseSuccessful = MultiPartResponseParser.TryParse< ODataResponse< Product > >( responseItems401TheLast200, out batchStatusCode, out parsedResponse );
			var result = parsedResponse.ToList();

			isParseSuccessful.Should().BeTrue();
			batchStatusCode.Should().Be( unauthorized401 );
			result[ 0 ].Error.Message.Should().Be( "Authorization has been denied for this request." );
			result[ 5 ].Value.First().Sku.Should().Be( "testkit2" );
		}

		[ Test ]
		public void GivenInvalidJsonResponse_WhenCallMultiPartResponseParser_ThenReturnsFalse()
		{
			int batchStatusCode;
			var invalidJsonResponse = "MESSAGE\":\"TOO MANY REQUESTS\"";

			IEnumerable< ODataResponse< Product > > parsedResponse;
			var isParseSuccessful = MultiPartResponseParser.TryParse< ODataResponse< Product > >( invalidJsonResponse, out batchStatusCode, out parsedResponse );
			
			isParseSuccessful.Should().BeFalse();
		}
	}
}
