using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThunderRoad;
using UnityEngine;

namespace UndyingWave
{
    class BrainModuleElectrocute : ThunderRoad.BrainModuleElectrocute
    {
        public override void OnBrainStart()
        {
            base.OnBrainStart();
            this.creature.OnDamageEvent += Creature_OnDamageEvent;
    }

        public override void OnBrainStop()
        {
            base.OnBrainStop();
            this.creature.OnDamageEvent -= Creature_OnDamageEvent;
        }

        private void Creature_OnDamageEvent(CollisionInstance collisionInstance)
        {
            if (this.coroutine == null) return;
            if (!collisionInstance.casterHand) return;
            
            SpellCastData spell = collisionInstance.casterHand.spellInstance;
            if (spell == null) return;

            if (spell.id == "Undying")
            {
                this.creature.brain.StopCoroutine(this.coroutine);
            }
        }
    }
}
