using ADV;
using ModUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AlternateTraits
{
    internal static class Ext
    {
        public static ModdedCard CopyVisuals(this ModdedCard m, string cardKey)
        {
            Card card = Library.Main.GetCard(cardKey)?.GetComponent<Card>();
            if (card == null)
            {
                AlternateTraitsMod.DebugLog("Could not find " + cardKey);
                return m;
            }

            Sprite basePicture = card.transform.Find("AnimBase/NewAliveBase/Base").GetComponent<SpriteRenderer>().sprite;
            Sprite background = card.transform.Find("AnimBase/NewAliveBase/Background").GetComponent<SpriteRenderer>().sprite;
            Sprite portrait = card.Info.Portrait;

            SpriteRenderer sr = m._card.transform.Find("AnimBase/NewAliveBase/Base").GetComponent<SpriteRenderer>();
            sr.color = new Color(1f, 0.85f, 0.85f);

            return m.SetBasePicture(basePicture)
                .SetBackground(background)
                .SetPortrait(portrait);
        }
    }
}
