using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThunderRoad;
using UnityEngine;

namespace UndyingWave
{
    class BrainModuleGrabbed : ThunderRoad.BrainModuleGrabbed
    {

        public override void Load(Creature creature)
        {
            base.Load(creature);
        }

        public override void OnBrainStart()
        {
            base.OnBrainStart();
        }

        protected override void OnUngrab(RagdollHand ragdollHand, HandleRagdoll handleRagdoll, bool lastHandler)
        {
            this.Refresh();
            if (!lastHandler
                || (double)handleRagdoll.ragdollPart.rb.velocity.magnitude <= (double)this.grabThrowMinVelocity
                || handleRagdoll.ragdollPart.isSliced)
                return;
            this.creature.ragdoll.SetState(Ragdoll.State.Destabilized);
        }
    }
}
