using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

namespace BuddyApp.PlayMath{

    [Flags]
	public enum Operand : int
	{
        // We should not serialize none operand value
		NONE = 0,
		ADD = 0x01, 
		SUB = 0x02, 
		DIV = 0x04, 
		MULTI = 0x08,
	}

    public class GameParameters {
		
		public const int DIFFICULTY_MAX = 5;

		/// <summary>
		/// These getter/setter are useful only for serialize the object.
		/// <para>Prefer <see cref="CheckOperand(Operand operand)"/> and <see cref="SetOperand(Operand operand, bool toSet)"/>.</para>
		/// </summary>
		public Operand Operands { get; private set; }

		public int Table { get ; set; }

		private int mDifficulty;
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

		public int Sequence { get ; set; }

		public int Timer { get; set; }

		public GameParameters () {
			this.Operands = Operand.ADD;
			this.Table = 0;
			this.Difficulty = 1;
			this.Sequence = 4;
			this.Timer = 20;
		}

		public bool CheckOperand(Operand operand) {
			if (operand == Operand.NONE) {
				return this.Operands == 0;
			}

			return (this.Operands & operand) != 0;
		}		

		public void SetOperand(Operand operand, bool toSet) {
			if (operand == Operand.NONE) {
				throw new System.ArgumentException("Can't set NONE");
			}
				
			if (toSet) { 
				this.Operands |= operand;
			} 
			else {
				this.Operands &= ~operand;
			}
		}

		public override string ToString() {
			StringBuilder s = new StringBuilder();

			s.Append("difficulty:" + this.Difficulty);
			s.Append(", operands:" + this.Operands);
			s.Append(", table:" + this.Table);
			s.Append(", timer:" + this.Timer);

			return s.ToString();
		}

		public static string OperandToString(Operand operand) {
			switch (operand) {
			case Operand.ADD:
				return "+";
			case Operand.SUB:
				return "-";
			case Operand.MULTI:
				return "×";
			case Operand.DIV:
				return "÷";
			default:
				throw new ArgumentException();	
			}
		}
	}
}
