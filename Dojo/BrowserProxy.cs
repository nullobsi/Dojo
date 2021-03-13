using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Dojo
{
	[ComVisible(true)]
	public class BrowserProxy
	{
		private static readonly List<ScanInData> ScanIns = new List<ScanInData>();

		public static event Action<ScanInData> OnData;


		public void JSGotError(dynamic d)
		{
			Console.WriteLine("JS Error!");
			Console.WriteLine(d?.message);
		}
		public void JSGotData(object d)
		{
			// deserialize JSON
			Console.WriteLine(d);
			var data = JsonConvert.DeserializeObject<ScanInData[]>(d as string);
			foreach (var j in data)
			{
				// do not add redundant data
				if (j.MinutesLeft < 0 || ScanIns.Any(i => i.key == j.key))
				{
					j.Dispose();
					continue;
				}

				ScanIns.Add(j);
				OnData?.Invoke(j);
			}
		}
	}
}