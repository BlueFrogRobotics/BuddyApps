using System.Collections.Generic;
using System;

namespace BuddyApp.PlayMath{
    public class EquationTable : Equation {

        private int mTable;

        public EquationTable(int table, int multiplier, int maxValue, Random rnd)
        {
            List<Object> lOperation = new List<Object>();

            lOperation.Add(table);
            lOperation.Add(Operand.MULTI);
            lOperation.Add(multiplier);

            this.Text = OperationToText(lOperation);

            long lAnswer = ComputeAnswer(lOperation);
            this.Answer = lAnswer.ToString();

            this.Choices = GenerateChoices(lAnswer, maxValue, rnd, false);
        }
    }
}

