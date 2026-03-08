using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADV;
using UnityEngine;

namespace WildfrostGoats
{
    public class Signal_AddRelic : Signal
    {
        public override void EndEffect()
        {
            System.Console.WriteLine("[Goat] Adding Relic...");
            if (LevelInfo.Main.CurrentRelic == null)
            {
                System.Console.WriteLine("[Goat] Creating New Relic...");
                Relic relic = LevelInfo.Main.gameObject.AddComponent<Relic>();
                relic.Score = 1;
                relic.Complexity = 1;
                relic.Description = "";
                relic.Keys = new List<string>();
            }
            foreach(string key in GKB().Keys)
            {
                string k = KeyBase.Translate(key, out float value);
                if (value == 1)
                {
                    LevelInfo.Main.CurrentRelic.Keys.Add(k);
                }
            }
            base.EndEffect();
        }
    }
}
