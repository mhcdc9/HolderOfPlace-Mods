using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADV;
using ModUtils;
using UnityEngine;

namespace MothsOnCards
{
    public class MothOnCards : HopMod
    {
        public override string Guid => "mhcdc9.moths";
        public override string Title => "Flying Moths!";
        public override string[] Depends => Array.Empty<string>();
        public override string Description => "Adds 3-4 moths to Fate's Beloved and all moth card.";

        public List<string> redMoths = new List<string> { "Moth of Clawing", "Moth of Sacrificing", "Moth of Burning", "Moth of Materializing" };
        public List<string> blueMoths = new List<string> { "Moth of Shining", "Moth of Growing", "Moth of Splitting" };
        public List<string> pinkMoths = new List<string> { "Moth of Incubating", "Moth of Hunting" };
        public List<string> purpleMoths = new List<string> { "Moth of Draining", "Moth of Sculpting", "Moth of Transmuting", "Moth of Annihilating" };
        public List<string> greenMoths = new List<string> { "Moth of Culling", "Moth of Weaving", "Moth of Healing", "Moth of Shocking", "Moth of Shielding" };
        public List<string> yellowMoths = new List<string> { "Moth of Blessing", "Moth of Binding", "Moth of Channeling", "Moth of Cultivating" };

        public MothOnCards(string path) : base(path)
        {

        }

        protected override void OnEnable()
        {
            ModEvents.OnCardGenerated += AddMoth;
            Moth.CreatePrefab(modPath);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            ModEvents.OnCardGenerated -= AddMoth;
            base.OnDisable();
        }

        public void AddMoth(Card card)
        {
            if (card.Info.RealName == "Fate's Beloved")
            {
                Moth.Create(card, Color.black, 4);
            }
            else if (redMoths.Contains(card.Info.RealName))
            {
                Moth.Create(card, new Color(0.8f, 0.3f, 0.25f), 3);
            }
            else if (blueMoths.Contains(card.Info.RealName))
            {
                Moth.Create(card, new Color(0, 0.8f, 0.9f), 3);
            }
            else if (pinkMoths.Contains(card.Info.RealName))
            {
                Moth.Create(card, new Color(0.9f, 0.6f, 0.7f), 3);
            }
            else if (purpleMoths.Contains(card.Info.RealName))
            {
                Moth.Create(card, new Color(0.35f, 0.25f, 0.8f), 3);
            }
            else if (yellowMoths.Contains(card.Info.RealName))
            {
                Moth.Create(card, new Color(0.8f, 0.8f, 0f), 3);
            }
            else if (greenMoths.Contains(card.Info.RealName))
            {
                Moth.Create(card, new Color(0.2f, 0.8f, 0.2f), 3);
            }
        }


    }
}
