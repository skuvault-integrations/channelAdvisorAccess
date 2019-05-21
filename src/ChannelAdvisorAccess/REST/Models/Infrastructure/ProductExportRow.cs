using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace ChannelAdvisorAccess.REST.Models.Infrastructure
{
	public class ProductExportRow
	{
		public int ID { get; set; }
		public string Sku { get; set; }
	}
}
