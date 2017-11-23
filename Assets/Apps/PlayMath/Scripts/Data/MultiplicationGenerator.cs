using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BuddyApp.PlayMath{
    public class MultiplicationGenerator : Generator {

        public override List<Equation> Equations { get; }

        public MultiplicationGenerator(GameParameters parameters)
            : base(parameters)
        {
            this.Parameters = parameters;
            this.Equations = new List<Equation>();
        }

        /// <summary>
        /// Generate the value of the property Equations
        /// </summary>
        public override void generate() {
            System.Random lRandom = new System.Random(5);

            this.Equations.Clear();

            int lMax = 5; //TODO

            for(int i=1 ; i<=this.Parameters.Sequence ; i++) {
                EquationTable lEquation = new EquationTable(this.Parameters.Table, i, lMax, lRandom);
                this.Equations.Add(lEquation);
            }
        }
    }
}
