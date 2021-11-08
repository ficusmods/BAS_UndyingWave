using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThunderRoad;
using UnityEngine;

namespace UndyingWave
{
    public class LoadModule : LevelModule
    {
        public bool dieOnHeadChop = true;
        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            return base.OnLoadCoroutine();
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (!creature.gameObject.TryGetComponent<UndyingCreature>(out UndyingCreature ur))
            {
                var uc = creature.gameObject.AddComponent<UndyingCreature>();
                uc.dieOnHeadChop = dieOnHeadChop;
            }
        }
    }
}
