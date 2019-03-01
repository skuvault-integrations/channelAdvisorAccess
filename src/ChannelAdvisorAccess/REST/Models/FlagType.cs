using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.REST.Models
{
	public enum FlagType
	{
		NotSpecified = -9999,
		ItemCopied = -2,
		ExclamationPoint = -1,
		NoFlag = 0,
		RedFlag = 1,
		QuestionMark = 2,
		NotAvailable = 3,
		Price = 4,
		YellowFlag = 5,
		GreenFlag = 6,
		BlueFlag = 7
	}
}
