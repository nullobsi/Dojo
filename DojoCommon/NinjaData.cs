using System;
using System.Runtime.Serialization;

namespace DojoCommon
{
	[DataContract]
	public class NinjaData
	{
		public NinjaData(ScanInData i)
		{
			FirstName = i.firstName;
			LastName = i.lastName;
			CreatedAt = i.dateCreated;
			BeltName = i.beltName;
			SessionLength = i.scanInSessionLength;
			UserGuid = i.userGuid;
		}

		[DataMember] public string FirstName { get; private set; }

		[DataMember] public string LastName { get; private set; }

		[DataMember] public DateTime CreatedAt { get; private set; }

		[DataMember] public string BeltName { get; private set; }

		[DataMember] public int SessionLength { get; private set; }

		[DataMember] public string UserGuid { get; private set; }

		public override string ToString()
		{
			return FirstName + LastName;
		}
	}
}