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
        LinkedList<RagdollHand> handlers = new LinkedList<RagdollHand>();
        public override void Load(Creature creature)
        {
            base.Load(creature);
        }

        protected override void OnGrab(RagdollHand ragdollHand, HandleRagdoll handleRagdoll)
        {
            Logger.Detailed("Handling grab");
            if (!handleRagdoll.ragdollPart.isSliced)
            {
                handlers.AddLast(ragdollHand);
            }

            if(handlers.Count > 0)
            {
                Logger.Detailed("Grab refresh");
                Refresh();
            }
            else
            {
                handleRagdoll.ragdollPart.ragdoll.isGrabbed = false;
            }
        }

        public override void OnBrainStart()
        {
            base.OnBrainStart();
        }

        protected override void OnUngrab(RagdollHand ragdollHand, HandleRagdoll handleRagdoll, bool lastHandler)
        {
            Logger.Detailed("Handling ungrab");
            if (handlers.Count > 0)
            {
                if (!handleRagdoll.ragdollPart.isSliced)
                {
                    Refresh();
                    if (handleRagdoll.ragdollPart.rb.velocity.magnitude >= this.grabThrowMinVelocity)
                    {
                        this.creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                    }
                }

                handlers.Remove(ragdollHand);
            }
        }
    }
}
