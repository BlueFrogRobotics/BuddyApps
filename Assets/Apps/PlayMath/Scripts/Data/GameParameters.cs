using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Buddy;

namespace BuddyApp.PlayMath{

	[Flags]
	public enum Operand
	{
		NONE = 0,
		ADD = 0x01, 
		SUB = 0x02, 
		DIV = 0x04, 
		MULTI = 0x08
	}

	[Serializable]
	public class GameParameters {
		
		public const int DIFFICULTY_MAX = 5;

		private const string FILE_TO_SERIALIZE = "game_parameters.xml";

		private Operand mOperands = Operand.ADD;

		private int mTable = 0;

		private int mDifficulty = 1;

		private int mSequence = 4;

		private int mTimer = 20;

		public int Table
		{
			get 
			{
				return mTable;
			}

			set 
			{
				mTable = value;
			}
		}

		public int Difficulty
		{
			get 
			{
				return mDifficulty;
			}

			set 
			{
				if (value <= 0 || value > DIFFICULTY_MAX) {
					throw new System.ArgumentException("Difficulty must be > 0 and <= " + DIFFICULTY_MAX);
				}

				mDifficulty = value;
			}
		}

		public int Sequence
		{
			get 
			{
				return mSequence;
			}

			set 
			{
				mSequence = value;
			}
		}

		public int Timer
		{
			get 
			{
				return mTimer;
			}

			set 
			{
				mTimer = value;
			}
		}

		/// <summary>
		/// This property is useful only for serialize the object.
		/// <para>Don't call it, prefer <see cref="CheckOperand(Operand operand)"/> and <see cref="SetOperand(Operand operand, bool toSet)"/>.</para>
		/// </summary>
		public Operand Operands 
		{
			get 
			{
				return mOperands;
			}

			set 
			{
				mOperands = value;
			}
		}

		public bool CheckOperand(Operand operand) {
			if (operand == Operand.NONE) {
				return mOperands == 0;
			}

			return (mOperands & operand) != 0;
		}		

		public void SetOperand(Operand operand, bool toSet) {
			if (operand == Operand.NONE) {
				throw new System.ArgumentException("Can't set NONE");
			}
				
			if (toSet) { 
				mOperands |= operand;
			} 
			else {
				mOperands &= ~operand;
			}
		}

		public static void SaveDefault(GameParameters gameParameters) {
			Utils.SerializeXML<GameParameters>(gameParameters, PathToSerialize());
		}

		public static GameParameters LoadDefault() {
			GameParameters gameParameters = Utils.UnserializeXML<GameParameters>(PathToSerialize());

			if (gameParameters == null) {
				gameParameters = new GameParameters();
			}			

			return gameParameters;
		}

		private static string PathToSerialize() {
			return BYOS.Instance.Resources.GetPathToRaw(FILE_TO_SERIALIZE);
		}

		public override string ToString() {
			StringBuilder s = new StringBuilder();

			s.Append("difficulty:" + mDifficulty);
			s.Append(", operands:" + mOperands);
			s.Append(", table:" + mTable);
			s.Append(", timer:" + mTimer);

			return s.ToString();
		}
	}
}
