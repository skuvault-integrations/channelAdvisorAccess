using ChannelAdvisorAccess.REST.Models;
using ChannelAdvisorAccess.REST.Models.Infrastructure;
using ChannelAdvisorAccess.REST.Shared;
using FluentAssertions;
using NUnit.Framework;
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
			const int success = ( int ) HttpStatusCode.OK;

			MultiPartResponseParser.Parse< ODataResponse< Product > >( responseProductsSuccess, out batchStatusCode );

			batchStatusCode.Should().Be( success );
		}
		
		[ Test ]
		public void GivenBatchItemsProducts_WhenCallMultiPartResponseParser_ThenParsesProducts()
		{
			int batchStatusCode;

			var result = MultiPartResponseParser.Parse< ODataResponse< Product > >( responseProductsSuccess, out batchStatusCode ).ToList();

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

			var result = MultiPartResponseParser.Parse< ODataResponse< Product > >( responseItemsStatus401, out batchStatusCode ).ToList();

			batchStatusCode.Should().Be( unauthorized401 );
			result[ 0 ].Error.Message.Should().Be( "Authorization has been denied for this request." );
		}
	}
}
