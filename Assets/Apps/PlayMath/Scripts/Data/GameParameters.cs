using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Runtime.Serialization;

namespace BuddyApp.PlayMath{

    [Flags,DataContract(Name="Operand")]
	public enum Operand : int
	{
        // We should not serialize none operand value
		NONE = 0,
        [EnumMember]
		ADD = 0x01, 
        [EnumMember]
		SUB = 0x02, 
        [EnumMember]
		DIV = 0x04, 
        [EnumMember]
		MULTI = 0x08,
        //TODO Find a clean way to do this...
        // For serialization only, define flags combination
        [EnumMember] AS = ADD | SUB,
        [EnumMember] AM = ADD | MULTI,
        [EnumMember] AD = ADD | DIV,
        [EnumMember] SM = SUB | MULTI,
        [EnumMember] SD = SUB | DIV,
        [EnumMember] MD = MULTI | DIV,
        [EnumMember] ASM = ADD | SUB | MULTI,
        [EnumMember] ASD = ADD | SUB | DIV,
        [EnumMember] AMD = ADD | MULTI | DIV,
        [EnumMember] SMD = SUB | MULTI | DIV,
        [EnumMember] ASMD = ADD | SUB | MULTI | DIV
	}

	[DataContract]
    public class GameParameters : SerializableData {
		
		public const int DIFFICULTY_MAX = 5;

		/// <summary>
		/// These getter/setter are useful only for serialize the object.
		/// <para>Prefer <see cref="CheckOperand(Operand operand)"/> and <see cref="SetOperand(Operand operand, bool toSet)"/>.</para>
		/// </summary>
        [DataMember(Name="operand")]
		public Operand Operands { get; private set; }

        [DataMember(Name="table")]
		public int Table { get ; set; }

        [DataMember(Name="difficulty")]
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

        [DataMember(Name="sequence")]
		public int Sequence { get ; set; }

        [DataMember(Name="timer")]
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
