using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BuddyLab
{
    public sealed class HeadInputElement : AGraphicElement
    {

        protected override void SetParameter()
        {
            if (mInstruction == null)
                mInstruction = new MoveHeadInputBehaviourInstruction();
            MoveHeadInputBehaviourInstruction lHeadInstruction = (MoveHeadInputBehaviourInstruction)mInstruction;
            List<GazePosition> lGazePositions = new List<GazePosition>();
            lGazePositions.Add(GazePosition.LEFT);
            lGazePositions.Add(GazePosition.RIGHT);
            lHeadInstruction.ConditionalPositions = lGazePositions.ToArray();
        }
    }
}