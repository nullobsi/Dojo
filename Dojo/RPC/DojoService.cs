using System;
using DojoCommon;

namespace Dojo.RPC
{
	public class DojoService : IDojoService
	{
		public DateTime TakeBreak(string uuid, int minutes)
		{
			if (!BreakStatus(uuid))
			{
				BrowserProxy.GetScanIn(uuid).StartBreak(minutes);
				return DateTime.Now;
			}

			return DateTime.MinValue;
		}

		public bool StopBreak(string uuid)
		{
			if (!BreakStatus(uuid)) return false;
			BrowserProxy.GetScanIn(uuid).StopBreak();
			return true;
		}

		public NinjaData[] GetNinjas()
		{
			return BrowserProxy.GetAllNinjas();
		}

		public NinjaData GetData(string uuid)
		{
			return BrowserProxy.FindNinja(uuid);
		}

		public bool BreakStatus(string uuid)
		{
			return BrowserProxy.GetScanIn(uuid).BreakStatus != TimeSpan.Zero;
		}

		public DateTime BreakStarted(string uuid)
		{
			return DateTime.Now - BrowserProxy.GetScanIn(uuid).BreakStatus;
		}
	}
}