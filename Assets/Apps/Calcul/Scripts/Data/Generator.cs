using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BuddyApp.Calcul{
    public abstract class Generator {

        public GameParameters Parameters { get; protected set;}

		virtual public List<Equation> Equations { get; protected set;}

        public Generator(GameParameters parameters) {
            this.Parameters = parameters;
        }

        abstract public void generate();
    }
}

