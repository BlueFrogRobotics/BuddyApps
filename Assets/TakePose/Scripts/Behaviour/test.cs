﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.TakePose
{
	public class test : MonoBehaviour
	{

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			BYOS.Instance.Interaction.VocalManager.StartInstantReco();
		}
	}
}
