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
        public string mod_version = "0.0";
        public string mod_name = "UnnamedMod";
        public string logger_level = "Basic";

        public bool dieOnHeadChop = true;

        public override IEnumerator OnLoadCoroutine()
        {
            Logger.init(mod_name, mod_version, logger_level);
            Logger.Basic("Loading " + mod_name);

            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
            return base.OnLoadCoroutine();
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (!creature.isPlayer)
            {
                if (!creature.gameObject.TryGetComponent<UndyingCreatureModule>(out UndyingCreatureModule ur))
                {
                    if (creature.data.id.StartsWith("Undying"))
                    {
                        var uc = creature.gameObject.AddComponent<UndyingCreatureModule>();
                        uc.dieOnHeadChop = dieOnHeadChop;
                    }
                }
            }
        }
    }
}
