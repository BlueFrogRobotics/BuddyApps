using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BuddyApp.PlayMath{
    public class EquationGenerator : Generator {

		private static readonly Operand[][] ALLOWED_OPERANDS = { new Operand[] {Operand.ADD, Operand.SUB, Operand.MULTI, Operand.DIV},
			new Operand[] {Operand.ADD, Operand.SUB, Operand.MULTI, Operand.DIV},
			new Operand[] {Operand.ADD, Operand.SUB, Operand.MULTI, Operand.DIV},
			new Operand[] {Operand.ADD, Operand.SUB, Operand.MULTI, Operand.DIV},
			new Operand[] {Operand.ADD, Operand.SUB, Operand.MULTI, Operand.DIV} };

        //TODO replace with exception ?
		private const Operand DEFAULT_OPERAND = Operand.ADD;
				
		private static readonly int[] NUMBER_OF_OPERANDS = { 1, 1, 2, 2 ,2 };

		private static readonly int[] MIN_VALUE = { 1, 10, 10, 100, 100 };

		private static readonly int[] MAX_VALUE = { 10, 100, 100, 1000, 1000 };

		public override List<Equation> Equations { get; }

		public EquationGenerator(GameParameters parameters) 
            : base(parameters)
        {
			this.Parameters = parameters;
			this.Equations = new List<Equation>();
		}

		/// <summary>
		/// Generate the value of the property Equations
		/// </summary>
        public override void generate() {
			// use always the same seed to generate the same values
			System.Random lRandom = new System.Random(5);

			this.Equations.Clear();

			List<Operand> lOperands = SelectOperands();
			int lNumberOfOperands = NUMBER_OF_OPERANDS[this.Parameters.Difficulty - 1];
			int lMin = MIN_VALUE[this.Parameters.Difficulty - 1];
			int lMax = MAX_VALUE[this.Parameters.Difficulty - 1];

			while (this.Equations.Count < this.Parameters.Sequence) {
				EquationLong lEquation = new EquationLong(lOperands, lNumberOfOperands, lMin, lMax, lRandom);
				if (!this.Equations.Contains(lEquation)) {
					this.Equations.Add(lEquation);
				}
			}
		}

		/// <summary>
		/// Keep only operands which are enabled with the current difficulty.
		/// <para>If no difficulty are selected, select the default value</para>
		/// <para>If difficulty > 2 and , add ADD and SUB</para></para>
		/// </summary>
		/// <returns>A list with all the Operands selected and allowed.</returns>
		private List<Operand> SelectOperands() {
			List<Operand> lOperands = new List<Operand>();

			Operand[] lAllowedOperandsForOurDifficulty = ALLOWED_OPERANDS[this.Parameters.Difficulty - 1];
			for (int i = 0; i < lAllowedOperandsForOurDifficulty.Length; i++) {
				Operand o = lAllowedOperandsForOurDifficulty[i];
				if (this.Parameters.CheckOperand(o)) {
					lOperands.Add(o);
				}
			}

			// use the default value if the list is empty
			if (lOperands.Count == 0) {
				lOperands.Add(DEFAULT_OPERAND);
			}

			// if difficulty > 2 => add ADD and SUB
			if (this.Parameters.Difficulty > 2) {
				if (!lOperands.Contains (Operand.ADD)) {
					lOperands.Add(Operand.ADD);
				}
				if (!lOperands.Contains(Operand.SUB)) {
					lOperands.Add(Operand.SUB);
				}
			}

			return lOperands;
		}
	}
}