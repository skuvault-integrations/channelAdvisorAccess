using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChannelAdvisorAccess.Extensions
{
	internal static class IEnumerableExtensions
	{
		public static void ProcessInParallel<TSource>(this IEnumerable<TSource> source, int batchSize, Action<TSource> body)
		{
			Parallel.ForEach(source, new ParallelOptions { MaxDegreeOfParallelism = batchSize }, body);
		}
	}
}
