﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace BuddyApp.Calcul{

	public class EquationLong : Equation {

		private enum OperandsFilter
		{
			/// <summary>
			/// If there are MULTI and/or DIV => don't keep ADD and SUB
			/// </summary>
			PREFER_MULTI_OR_DIV,
			/// <summary>
			// Don't keep MULTI and DIV
			/// </summary>
			DISABLE_MULTI_AND_DIV,
			/// <summary>
			// Keep all operands
			/// </summary>
			ALL
		}

		public EquationLong(List<Operand> operands, int numberOfOperands, int minValue, int maxValue, Random rnd) {
			List<Object> lOperations;

			if (numberOfOperands == 1) {
				lOperations = GenerateEquationOneOperand(operands, OperandsFilter.ALL, numberOfOperands, minValue, maxValue, rnd);
			}
			else {
				lOperations = GenerateEquationManyOperands(operands, numberOfOperands, minValue, maxValue, rnd);
			}

			this.Text = OperationToText(lOperations);

			long lAnswer = ComputeAnswer(lOperations);
			this.Answer = lAnswer.ToString();

			this.Choices = GenerateChoices(lAnswer, maxValue, rnd, true);
		}

		private List<Object> GenerateEquationOneOperand(List<Operand> operands, OperandsFilter operandsFilter,
			int numberOfOperands, int minValue, int maxValue, Random rnd) {
			List<Object> lOperations = new List<Object>();

			// generate left value
			int lLeftValue = rnd.Next(minValue, maxValue);
			lOperations.Add(lLeftValue);

			// generate operand
			Operand lOperand = GenerateOperand(operands, operandsFilter, rnd);
			lOperations.Add(lOperand);

			// generate right value
			int lRightValue;
			switch (lOperand) {
			case Operand.ADD:
				lRightValue = GenerateAdd(minValue, maxValue, rnd);
				break;
			case Operand.SUB:
				lRightValue = GenerateSub(minValue, maxValue, rnd);
				break;
			case Operand.MULTI:
				lRightValue = GenerateMulti(rnd);
				break;
			case Operand.DIV:
				lRightValue = GenerateDiv(lLeftValue, rnd);
				break;
			default:
				throw new ArgumentException();
			}
			lOperations.Add(lRightValue);

			return lOperations;
		}

		private List<Object> GenerateEquationManyOperands(List<Operand> operands, int numberOfOperands, int minValue, int maxValue, Random rnd) {
			List<Object> lOperations = new List<Object>();

			// generate the first part of the equation
			lOperations.AddRange(GenerateEquationOneOperand(operands, OperandsFilter.PREFER_MULTI_OR_DIV, numberOfOperands, minValue, maxValue, rnd));

			// generate the others parts
			for (int i = 1; i < numberOfOperands; i++) {
				// generate operand
				Operand lOperand = GenerateOperand(operands, OperandsFilter.DISABLE_MULTI_AND_DIV, rnd);
				lOperations.Add(lOperand);

				// generate right value
				int lRightValue;
				switch (lOperand) {
				case Operand.ADD:
					lRightValue = GenerateAdd(minValue, maxValue, rnd);
					break;
				case Operand.SUB:
					lRightValue = GenerateSub(minValue, maxValue, rnd);
					break;
				default:
					throw new ArgumentException();	
				}
				lOperations.Add(lRightValue);
			}

			return lOperations;
		}

		/// <summary>
		/// Generate a random Operand which is in the list of allowed operands.
		/// </summary>
		/// <param name="operands">Operands is the list of allowed operands.</param>
		/// <param name="operandsFilter">Selection filter</param>
		/// <param name="rnd">Random is the random number generator.</param>
		private Operand GenerateOperand(List<Operand> operands, OperandsFilter operandsFilter, Random rnd) {
			List<Operand> lOperands = new List<Operand>();

			switch (operandsFilter) {
			case OperandsFilter.PREFER_MULTI_OR_DIV:
				for (int i = 0; i < operands.Count; i++) {
					Operand o = operands[i];
					if (o == Operand.DIV || o == Operand.MULTI) {
						lOperands.Add(o);  // don't add sub or add
					}
				}
				// if no multi and no div with PREFER_MULTI_OR_DIV => add others operands
				if (lOperands.Count == 0) {
					for (int i = 0; i < operands.Count; i++) {
						lOperands.Add(operands[i]);
					}
				}
				break;
			case OperandsFilter.DISABLE_MULTI_AND_DIV:
				for (int i = 0; i < operands.Count; i++) {
					Operand o = operands [i];
					if (o == Operand.DIV || o == Operand.MULTI) {
						continue;  // don't add multi or div
					}
					lOperands.Add (o);
				}
				break;
			case OperandsFilter.ALL:
				for (int i = 0; i < operands.Count; i++) {
					lOperands.Add (operands [i]);
				}
				break;
			}

			int lRandomIndex = rnd.Next(0, lOperands.Count);
			return lOperands[lRandomIndex];
		}

		private int GenerateAdd(int minValue, int maxValue, Random rnd) {
			return rnd.Next(minValue, maxValue);
		}

		private int GenerateSub(int minValue, int maxValue, Random rnd) {
			return rnd.Next(minValue, maxValue);
		}

		private int GenerateMulti(Random rnd) {
			return rnd.Next(1, 10);
		}

		/// <summary>
		/// Generate a random value in the list of leftValue factors.
		/// </summary>
		/// <returns>The right value of the div.</returns>
		/// <param name="leftValue">The left value of the div.</param>
		/// <param name="rnd">The random generator<./param>
		private int GenerateDiv(int leftValue, Random rnd) {
			List<int> lFactors = Factor(leftValue);
			int lRandomIndex = rnd.Next(0, lFactors.Count);
			return lFactors[lRandomIndex];
		}

    	/// <summary>
		/// Return all factors of a number.
		/// <para>If the number has at least one factor != 1 and != itself,
		/// then we return only the others factors</para>
		/// </summary>
		/// <param name="number">Number that would be divided.</param>
		private static List<int> Factor(int number) {
			List<int> lFactors = new List<int>();
			int lMax = (int)Math.Sqrt(number);
			for(int lFactor = 1; lFactor <= lMax; ++lFactor) {
				if(number % lFactor == 0) {
					lFactors.Add(lFactor);
					if(lFactor != number/lFactor) {
						lFactors.Add(number/lFactor);
					}
				}
			}

			// if number has at least one factor != 1 and != itself
			// then we return only the others factors
			if (lFactors.Count > 2) {
				lFactors.Remove(1);
				lFactors.Remove(number);
			}

			return lFactors;
		}
	}
}