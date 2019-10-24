using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace BuddyApp.Calcul{
    public abstract class Equation
    {
        public string Text { get; protected set;}

        public string Answer { get; protected set;}

        public string[] Choices { get; protected set;}

        protected Equation()
        {
        }

        /// <summary>
        /// Translate the equation into a string
        /// MULTI AND DIV MUST BE THE FIRST OPERAND OR DON'T BE IN THE EQUATION
        /// </summary>
        /// <returns>The text corresponding to the equation</returns>
        /// <param name="operations">The equation</param>
        protected String OperationToText(List<Object> operations) {
            StringBuilder lStringBuilder = new StringBuilder();

            // first part of the equation
            int lLeftValue = (int) operations[0];
            Operand lOperand = (Operand) operations[1];
            int lRightValue = (int) operations[2];
            bool mustBrackets = (operations.Count > 3) && (lOperand == Operand.MULTI || lOperand == Operand.DIV);

            if (mustBrackets) {
                lStringBuilder.Append("(");
            }
            lStringBuilder.Append(lLeftValue.ToString() + " " + GameParameters.OperandToString(lOperand) + " " + lRightValue.ToString());
            if (mustBrackets) {
                lStringBuilder.Append(")");
            }

            // others parts
            for (int i = 3; i < operations.Count; i++) {
                Object lItem = operations[i];

                if (lItem is int) {
                    int lInt = (int) lItem;
                    lStringBuilder.Append(lInt.ToString());
                } else {
                    lOperand = (Operand) lItem;
                    lStringBuilder.Append(" " + GameParameters.OperandToString(lOperand) + " ");
                }
            }

            return lStringBuilder.ToString(); 
        }

        /// <summary>
        /// MULTI AND DIV MUST BE THE FIRST OPERAND OR DON'T BE IN THE EQUATION
        /// </summary>
        /// <returns>The answer.</returns>
        /// <param name="operations">Operations.</param>
        protected long ComputeAnswer(List<Object> operations) {
            long lAnswer = Convert.ToInt64(((int) operations[0]));  // TODO Clean cast

            for (int i = 1; i + 1 < operations.Count; i += 2) {
                Operand lOperand = (Operand)operations[i];
                int lValue = (int) operations[i + 1];

                switch (lOperand) {
                    case Operand.ADD:
                        lAnswer += lValue;
                        break;
                    case Operand.SUB:
                        lAnswer -= lValue;
                        break;
                    case Operand.MULTI:
                        lAnswer *= lValue;
                        break;
                    case Operand.DIV:
                        lAnswer /= lValue;
                        break;
                }
            }

            return lAnswer;
        }       

        protected string[] GenerateChoices(long answer, int maxValue, Random rnd, bool AllowNegative) {
            // generate the choices
            List<long> lChoices = new List<long>();

            lChoices.Add(answer);

            while (lChoices.Count < 4) {
                long lBase = rnd.Next(0, maxValue);
                long lChoice = rnd.Next (0, 2) == 0 ? answer + lBase : answer - lBase;
                if ( !lChoices.Contains(lChoice) && (AllowNegative || lChoice >= 0) ) {
                    lChoices.Add(lChoice);
                }
            }

            // take the choices randomly
            string[] lChoicesStr = new string[4];

            for (int i = 0; i < 4; i++) {
                // choose a random index
                int randomIndex = rnd.Next(0, lChoices.Count);
                // add the value in the array of choics
                lChoicesStr[i] = lChoices[randomIndex].ToString();
                // remove the value from the tmp list to avoid to choose this choice again
                lChoices.RemoveAt(randomIndex);
            }

            return lChoicesStr;
        }

        public override string ToString() {
            String lChoices = string.Format("[{0}, {1}, {2}, {3}]", Choices[0], Choices[1], Choices[2], Choices[3]);
            return string.Format("[Equation: Text={0}, Answer={1}, Choices={2}]", Text, Answer, lChoices);
        }

        public override bool Equals(Object o) {
            if (! (o is Equation)) {
                return false;
            }

            Equation oEquation = (Equation)o;
            return (this.Text == oEquation.Text);
        }
    }
}

