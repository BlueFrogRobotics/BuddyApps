using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.BuddyLab
{ 
    public interface IEditableParameter {

        string GetEditableParameter();
        void SetEditableParameter(string iParameter);

    }
}