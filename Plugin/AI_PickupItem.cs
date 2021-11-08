using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using ThunderRoad;
using UnityEngine;

using ThunderRoad.AI.Action;

namespace UndyingWave
{
    class AI_PickupItem : ThunderRoad.AI.Action.PickupItem
    {

        public override void StopGrabbing()
        {
            if (!this.isGrabbing)
                return;
            if (this.coroutine != null)
                this.creature.brain.StopCoroutine(this.coroutine);
            this.itemHandle = null;
            this.creature.ragdoll.ik.SetHandAnchor(Side.Right, null);
            this.creature.ragdoll.ik.SetHandAnchor(Side.Left, null);
            this.creature.SetAnimatorHeightRatio(1f);
            this.isGrabbing = false;
            this.done = false;
            this.fail = false;
        }

        public override void OnRun()
        {
            if (this.isGrabbing)
                return;
            this.coroutine = this.creature.brain.StartCoroutine(this.useIK ? this.GrabIKCoroutine() : this.GrabCoroutine());
        }

        private IEnumerator GrabCoroutine()
        {
            isGrabbing = true;
            if (side == Side.Right)
            {
                AnimationData.Clip clip = pickupAnimationData.Pick(AnimationData.Clip.Direction.Forward, AnimationData.Clip.Direction.Right);
                creature.PlayAnimation(clip.animationClip, false);
            }
            else
            {
                AnimationData.Clip clip = pickupAnimationData.Pick(AnimationData.Clip.Direction.Forward, AnimationData.Clip.Direction.Left);
                creature.PlayAnimation(clip.animationClip, false);
            }
            yield return new WaitForSeconds(pickupDuration);
            if ((side == Side.Right
                && !(bool)(UnityEngine.Object)creature.handRight.grabbedHandle
                && !creature.handRight.isSliced)
                || (side == Side.Left
                && !(bool)(UnityEngine.Object)creature.handLeft.grabbedHandle
                && !creature.handLeft.isSliced))
            {
                if (side == Side.Right)
                    creature.handRight.Grab(itemHandle.item.GetMainHandle(side));
                else
                    creature.handLeft.Grab(itemHandle.item.GetMainHandle(side));
                creature.ragdoll.ik.SetHandWeight(side, 1f, 0.0f);
                grabbedHeight = creature.GetAnimatorHeightRatio();
            }
            while (creature.IsAnimatorBusy())
                yield return new WaitForEndOfFrame();
            isGrabbing = false;
            done = true;
        }

        private IEnumerator GrabIKCoroutine()
        {
            PickupItem pickupItem = this;
            isGrabbing = true;
            ikTarget = creature.transform.Find("PickupIKTarget");
            if ((UnityEngine.Object)ikTarget == (UnityEngine.Object)null)
                ikTarget = new GameObject("PickupIKTarget").transform;
            ikTarget.SetParent(creature.transform);
            grabTime = 0.0f;
            standUpTime = 0.0f;
            grabbedHeight = 0.0f;
            creature.ragdoll.ik.SetFullbody(true);
            creature.ragdoll.ik.SetHandAnchor(side, ikTarget);
            creature.ragdoll.ik.SetHandWeight(side, 0.0f, 0.0f);
            while ((double)grabTime < (double)pickupIKDuration)
            {
                if ((bool)(UnityEngine.Object)itemHandle.item.holder || itemHandle.item.isTelekinesisGrabbed || itemHandle.item.IsHanded())
                {
                    fail = true;
                    yield break;
                }
                else
                {
                    distance = UnityEngine.Vector3.Distance(itemHandle.transform.position.ToXZ(), creature.transform.position.ToXZ());
                    if ((double)distance > (double)creature.morphology.armsLength * 2.0)
                    {
                        fail = true;
                        yield break;
                    }
                    else
                    {
                        ikTarget.position = itemHandle.transform.position;
                        creature.ragdoll.ik.SetHandWeight(side, grabTime / pickupIKDuration, 0.0f);
                        creature.SetAnimatorHeightRatio(creature.transform.InverseTransformPoint(creature.ragdoll.headPart.bone.animation.position).y / creature.morphology.headHeight);
                        grabTime += Time.deltaTime;
                        yield return (object)new WaitForEndOfFrame();
                    }
                }
            }
            if ((side == Side.Right 
                && !(bool)(UnityEngine.Object)creature.handRight.grabbedHandle
                && !creature.handRight.isSliced)
                || (side == Side.Left
                && !(bool)(UnityEngine.Object)creature.handLeft.grabbedHandle
                && !creature.handLeft.isSliced))
            {
                if (side == Side.Right)
                    creature.handRight.Grab(itemHandle.item.GetMainHandle(side));
                else
                    creature.handLeft.Grab(itemHandle.item.GetMainHandle(side));
                creature.ragdoll.ik.SetHandWeight(side, 1f, 0.0f);
                grabbedHeight = creature.GetAnimatorHeightRatio();
            }
            while ((double)standUpTime < (double)pickupIKDuration)
            {
                if (!creature.ragdoll.ik.fullbody)
                    creature.ragdoll.ik.SetFullbody(true);
                creature.ragdoll.ik.SetHandWeight(side, (float)(1.0 - (double)standUpTime / (double)pickupIKDuration), 0.0f);
                creature.SetAnimatorHeightRatio(Mathf.Lerp(grabbedHeight, 1f, standUpTime / pickupIKDuration));
                standUpTime += Time.deltaTime;
                yield return (object)new WaitForEndOfFrame();
            }
            creature.ragdoll.ik.SetFullbody(creature.ragdoll.isGrabbed || creature.ragdoll.isTkGrabbed);
            UnityEngine.Object.Destroy((UnityEngine.Object)ikTarget.gameObject);
            isGrabbing = false;
            done = true;
        }
    }
}
