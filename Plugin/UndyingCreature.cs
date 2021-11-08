﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThunderRoad;
using UnityEngine;

namespace UndyingWave
{
    class UndyingCreature : MonoBehaviour
    {
        public bool dieOnHeadChop = true;
        public Creature creature;

        private void Awake()
        {
            creature = gameObject.GetComponentInChildren<Creature>();
            this.creature.OnDamageEvent += Creature_OnDamageEvent;
            creature_to_max_health();
            foreach (RagdollPart part in this.creature.ragdoll.parts)
            {
                part.data.sliceForceKill = false;
            }
        }

        public void creature_to_max_health()
        {
            this.creature.maxHealth = float.PositiveInfinity;
            this.creature.currentHealth = this.creature.maxHealth;
        }

        private bool check_valid_stab(CollisionInstance ci)
        {
            return (ci.damageStruct.damageType == DamageType.Pierce
                && (ci.damageStruct.hitRagdollPart.type == RagdollPart.Type.Neck
                    || ci.damageStruct.hitRagdollPart.type == RagdollPart.Type.Head)
                && ci.damageStruct.penetrationDeepReached);
        }

        private bool cant_think_straight()
        {
            if (!this.dieOnHeadChop) return false;
            if (this.creature.ragdoll.state == Ragdoll.State.NoPhysic
                || this.creature.ragdoll.state == Ragdoll.State.Disabled
                || this.creature.ragdoll.state == Ragdoll.State.Kinematic)
                return false;
            if(!this.creature.ragdoll.headPart.isSliced) return false;

            return true;
        }

        private bool where_it_hurts(CollisionInstance ci)
        {
            if (!ci.sourceColliderGroup) return false;
            if (!ci.damageStruct.hitRagdollPart) return false;
            if (ci.damageStruct.hitRagdollPart.ragdoll.state == Ragdoll.State.NoPhysic
                || ci.damageStruct.hitRagdollPart.ragdoll.state == Ragdoll.State.Disabled
                || ci.damageStruct.hitRagdollPart.ragdoll.state == Ragdoll.State.Kinematic)
                return false;

            Item item = ci.sourceColliderGroup.collisionHandler.item;
            if (!item) return false;

            foreach (Imbue imbue in item.imbues)
            {
                if (imbue.spellCastBase != null)
                {
                    if (imbue.spellCastBase.id == "Undying")
                    {
                        if (check_valid_stab(ci))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void kill_myself()
        {
            CollisionInstance killing_collision = new CollisionInstance(new DamageStruct(DamageType.Energy, 0xDEAD2BAD));
            this.creature.maxHealth = 0xDEAD2BAD - 1.0f;
            this.creature.currentHealth = creature.maxHealth;
            this.creature.Damage(killing_collision);
        }

        private void LateUpdate()
        {
            if (cant_think_straight())
            {
                kill_myself();
            }
        }

        private void Creature_OnDamageEvent(CollisionInstance collisionInstance)
        {
            RagdollPart rp = collisionInstance.damageStruct.hitRagdollPart;

            Debug.Log("Undying brain " + rp.ragdoll.creature.brain.instance.id);
            Debug.Log("Undying braintree " + rp.ragdoll.creature.brain.instance.tree.id);

            if (collisionInstance.damageStruct.baseDamage == 0xDEAD2BAD) return;

            creature_to_max_health();

            if (where_it_hurts(collisionInstance))
            {
                kill_myself();
            }
        }
    }
}
