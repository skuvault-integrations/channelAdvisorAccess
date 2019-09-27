using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models.Infrastructure
{
	public sealed class ProductExportResponse
	{
		[ JsonProperty( PropertyName = "$id") ]
		public int Id { get; set; }
		[ JsonProperty( PropertyName = "Token" ) ]
		public string Token { get; set; }
		[ JsonProperty( PropertyName = "Status" ) ]
		public ImportStatus Status { get; set; }
		[ JsonProperty( PropertyName = "StartedOnUtc" ) ]
		public DateTime StartedOnUtc { get; set; }
		[ JsonProperty( PropertyName = "ResponseFileUrl" ) ]
		public string ResponseFileUrl { get; set; }
	}

	public enum ImportStatus
	{
		Aborted,
		AbortedAcknowledged,
		AcknowledgedNotVisible,
		Complete,
		CompleteWithErrors,
		CompleteWithSystemicErrors,
		DeletedReadyForRemoval,
		FailedValidation,
		InProgress,
		InProgressPartitioning,
		InProgressProcessing,
		InProgressQueuedforProcessing,
		InProgressValidation,
		Pending,
		PendingPartitioning,
		Requeue,
		SelectedForPartitioning,
		SystemicFailure
	}
}
