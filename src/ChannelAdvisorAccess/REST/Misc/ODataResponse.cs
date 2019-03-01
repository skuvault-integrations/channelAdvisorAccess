﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	public class ODataResponse< T >
	{
		[JsonProperty(PropertyName = "@odata.context")]
		public string Context { get; set; }
		public T[] Value { get; set; }
		[JsonProperty(PropertyName = "@odata.nextLink")]
		public string NextLink { get; set; }
	}
}
