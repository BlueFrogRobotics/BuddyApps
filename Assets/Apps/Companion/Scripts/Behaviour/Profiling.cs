using Buddy;

using System;

namespace BuddyApp.Companion
{

	public enum COLOUR
	{
		NONE,
		BLUE,
		RED,
		YELLOW,
		GREEN,
		BROWN,
		BLACK,
		PINK,
		PURPLE,
		ORANGE,
		GREY,
		WHITE,
		CYAN,
		MAGENTA
	}

	public enum SPORT
	{
		NONE,
		BASKETBALL,
		FOOTBALL,
		HANDBALL,
		RUGBY,
		TENNIS,
		TABLETENNIS,
		CRICKET,
		SWIMMING,
		RUNNING,
		ARCHERY,
		FENCING,
		CURLING,
		CLIMBING
	}

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
		public DateTime BirthDate { get; set; }

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
		public COLOUR Colour { get; set; }
		public SPORT Sport { get; set; }
		public string MusicBand { get; set; }

		public UserTastes()
		{
		}
	}
	
}