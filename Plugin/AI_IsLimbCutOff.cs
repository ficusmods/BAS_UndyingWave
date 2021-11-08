using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThunderRoad;
using ThunderRoad.AI;
using UnityEngine;

namespace UndyingWave
{
    class AI_IsLimbCutOff : ConditionNode
    {
        protected Creature creature;
        protected Side side = Side.Right;
        public override void Init(Creature p_creature, Blackboard p_blackboard)
        {
            Debug.Log("Is limb cut off init");
            base.Init(p_creature, p_blackboard);
            this.creature = p_creature;
        }

        public override bool Evaluate()
        {
            bool ret = false;
            if(side == Side.Right)
            {
                ret = creature.handRight.isSliced;
            }
            else
            {
                ret = creature.handLeft.isSliced;
            }
            Debug.Log("Is limb cut off? " + ret);
            return ret;
        }
    }
}
