using Buddy;

using System;

namespace BuddyApp.Companion
{
	//public enum MusicStyle
	//{
	//	CLASSICAL,
	//	JAZZ,
	//	BLUES,
	//	ROCK,
	//	METAL,
	//	RAP,
	//	HIP_HOP,
	//	R_AND_B,
	//	ELECTRO,
	//	COUNTRY,
	//	POP,
	//	UNKNOWN
	//}

	[Serializable]
	public class UserProfile
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }

		//Birthdate can be written "YYYY-MM-DD"
		public string BirthDate { get; set; }

		public Gender Gender { get; set; }
		public UserTastes Tastes { get; set; }
		public string Occupation { get; set; }
		public string CityAddress { get; set; }

		public UserProfile()
		{
			Tastes = new UserTastes();
		}
	}

	[Serializable]
	public class UserTastes
	{
		public string Color { get; set; }
		public string MusicBand { get; set; }

		public UserTastes()
		{

		}
	}
	
}