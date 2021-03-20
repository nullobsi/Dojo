using System;
using System.ServiceModel;

namespace DojoCommon
{
	[ServiceContract]
	public interface IDojoService
	{
		// allow breaks
		[OperationContract]
		DateTime TakeBreak(string uuid, int minutes);

		[OperationContract]
		bool StopBreak(string uuid);

		// sensei
		[OperationContract]
		NinjaData[] GetNinjas();

		// data retrieval
		[OperationContract]
		NinjaData GetData(string uuid);

		// break polling
		[OperationContract]
		bool BreakStatus(string uuid);

		[OperationContract]
		DateTime BreakStarted(string uuid);
	}
}