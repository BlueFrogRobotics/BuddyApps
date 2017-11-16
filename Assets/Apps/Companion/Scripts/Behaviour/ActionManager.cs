using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.Companion
{
	/// <summary>
	/// Manager class that have reference to the differents stimuli and subscribes to their callbacks
	/// </summary>
	[RequireComponent(typeof(RoombaNavigation))]
	public class ActionManager : MonoBehaviour
	{

		public bool Wandering { get; private set; }
		public bool ThermalFollow { get; private set; }


		/// <summary>
		/// Speaker volume
		/// </summary>
		public int Volume { get; set; }

		public RoombaNavigation Roomba { get; private set; }

		void Start()
		{
			Volume = BYOS.Instance.Primitive.Speaker.GetVolume();
			Roomba = BYOS.Instance.Navigation.Roomba;
			Roomba.enabled = false;
		}

		public void StartWander()
		{
			if (ThermalFollow) {
				StopThermalFollow();
			}
			Roomba.enabled = true;
			Wandering = true;
		}

		public void StopWander()
		{
			Roomba.enabled = false;
			Wandering = false;
		}

		public void StartThermalFollow()
		{
			if (Roomba.enabled) {
				StopWander();
			}
			BYOS.Instance.Navigation.Follow<HumanFollow>().Facing();

		}

		public void StopThermalFollow()
		{
			BYOS.Instance.Navigation.Stop();
			ThermalFollow = false;
		}

		public void StopAllActions()
		{
			StopWander();
			StopThermalFollow();
		}

	}
}
