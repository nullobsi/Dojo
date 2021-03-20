using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DojoCommon;
using Newtonsoft.Json;

namespace Dojo
{
	[ComVisible(true)]
	public class BrowserProxy
	{
		private static readonly List<ScanInData>               ScanIns    = new List<ScanInData>();
		private static readonly Dictionary<string, ScanInData> ScanInDict = new Dictionary<string, ScanInData>();

		public static NinjaData FindNinja(string uuid)
		{
			return new NinjaData(ScanInDict[uuid]);
		}

		public static ScanInData GetScanIn(string uuid)
		{
			return ScanInDict[uuid];
		}

		public static NinjaData[] GetAllNinjas()
		{
			return (from i in ScanInDict select new NinjaData(i.Value)).ToArray();
		}

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
				if (ScanInDict.ContainsKey(j.userGuid))
				{
					var existing = ScanInDict[j.userGuid];
					if (j.dateCreated > existing.dateCreated) ScanInDict[j.userGuid] = j;
				}
				else
				{
					ScanInDict.Add(j.userGuid, j);
				}

				OnData?.Invoke(j);
			}
		}
	}
}