using ADV;
using ModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ModUtils.MarkFactory;
using static ModUtils.KeyLib;

namespace WildfrostGoats
{
    public class WildfrostGoats : HopMod
    {
        public override string Guid => "mhcdc9.goats";

        public override string Title => "Wildfrost Goats";

        public override string[] Depends => Array.Empty<string>();

        public override string Description => "Adds the demon goats to the game.";

        public static WildfrostGoats Main;

        public bool assetsLoaded = false;
        public WildfrostGoats(string path) : base(path)
        {
            Main = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void CreateAssets()
        {
            if (assetsLoaded)
            {
                return;
            }

            var signalDemonize = NewSignal<Signal_AddStatus>(TARGET_OTHER);
            signalDemonize.NewSignalInfo("[Source] *BI**Purple*Demonizes*CE* [Target]");
            var demonize = NewMark<Mark_Status_DamageInputMod>("Demonize", "DamageMod[2", "Mod[1", "TriggerCount[1", "Demonize[1");
            demonize.transform.SetParent(signalDemonize.transform);
            signalDemonize.StatusPrefabs = new List<GameObject> { demonize.gameObject };

            var applyDemonize = NewMark<Mark_Skill>("Apply Demonize", PERMANENT, AFTER_ATTACK, TRAIT)
                .AddMarkInfo("Demonize", "Doubles the damage taken on the next hit")
                .AddTargeting(NewTargeting<Targeting_RandomEnemy>())
                .AddSignal(signalDemonize,
                    NewColorEffect(new Color(0.4f, 0.25f, 0.8f), TARGET_OTHER),
                    NewSignal<Signal_AddActingDelay>(TARGET_SELF, DELAY_SCALE(0.49F)));

            //Good
            ModdedCard.CreateNewCard(this, CardType.Follower)
                .SetName("Loki", "Loki")
                .SetDesc("After attacking, give a random enemy *BI**Purple*Demonize*CE*")
                .SetStats(1, 3, 4)
                .SetPools("Active") //The real pools are Active, Passive, Aspect, Trinket, Moth, ManaAspect
                .SetBasePicture("LokiIcon.png")
                .SetBackground("Background.png")
                .SetIdeogram("IdeogramWF.png")
                .SetPortrait("PortraitLoki.png")
                .AddSkills(applyDemonize);

            var retaliateDemonize = applyDemonize.InstantiateEditName("Apply Demonize When Hit");
            retaliateDemonize.SetKey("InvokeChannel", 1.6f);

            //Good
            ModdedCard.CreateNewCard(this, CardType.Follower)
                .SetName("Gok", "Gok")
                .SetDesc("After taking damage, give a random enemy *BI**Purple*Demonize*CE*")
                .SetStats(2, 6, 8)
                .SetPools("Active")
                .SetBasePicture("GokIcon.png")
                .SetBackground("Background.png")
                .SetIdeogram("IdeogramWF.png")
                .AddSkills(retaliateDemonize);

            var triggerOnDemonize = NewAddStatus(
                NewMark<Mark_Trigger_OnStatusAdd>("Proc on Demonize", TARGET_OTHER, "ReplaceTargetWithCaster[1")
                    .AddRequiredKeys("Demonize")
                    .AddSignal( NewSignal<Signal>(TARGET_OTHER, "Attack[1")
                    ),
                TARGET_OTHER
                );
            var medium = NewMedium<Medium_Explosion>("Medium", out var explosion, TARGET_OTHER);
            explosion.AddKeys("TargetEnemies[1");
            explosion.AddSignal_Explosion(false, triggerOnDemonize
            );
            var sequencer = Mark_Sequencer.Create("Attack Sequencer", "Attack", 1f, TARGET_OTHER, PERMANENT)
                .AddSignal(NewInvoke("Auto", TARGET_SELF, "OneAction[0")
                );

            //Need to separate attacks
            ModdedCard.CreateNewCard(this, CardType.Follower)
                .SetName("Jab Joat", "JabJoat")
                .SetDesc("When an enemy is demonized, perform an attack")
                .SetStats(2, 2, 10)
                .SetPools("Active")
                .SetBasePicture("JabJoatIcon.png")
                .SetBackground("Background.png")
                .SetIdeogram("IdeogramWF.png")
                .AddSkills(NewMark<Mark_Skill>("Attack on Demonize", "PreCombat_Early[1", TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(medium)
                )
                .AddStatuses(sequencer);

            var specialDemonize = applyDemonize.InstantiateEditName("Apply Demonize When Buffed");
            specialDemonize.SetKey("InvokeChannel", 6.66f);

            //Need to experiment with delay
            ModdedCard.CreateNewCard(this, CardType.Follower)
                .SetName("Muttonhead", "GukaGuka")
                .SetDesc("When this follower gains stats, give a random enemy *BI**Purple*Demonize*CE*")
                .SetStats(3, 7, 14)
                .SetPools("Active")
                .SetBasePicture("GukaGukaIcon.png")
                .SetBackground("Background.png")
                .SetIdeogram("IdeogramWF.png")
                .AddSkills(specialDemonize)
                .AddStatuses(NewMark<Mark_Trigger_GainStats>("DemonizeAfterGainingStats", PERMANENT, TARGET_OTHER, TRAIT)
                .AddSignal(NewSignal<Signal_Invoke>("Channel[6.66","Delay[0.15"))
                );

            var applyDemonize2 = applyDemonize.InstantiateEditName("Apply Demonize");
            var demonicPrefix = NewSignal<Signal_NameChange>(TARGET_OTHER);
            demonicPrefix.Prefix = "Demonic";
            demonicPrefix.Suffix = "";
            var demonicDesc = NewSignal<Signal_DescriptionChange>(TARGET_OTHER, "AspectMode[1");
            demonicDesc.Suffix = "After attacking, give a random enemy *BI**Purple*Demonize*CE*";

            //Good
            ModdedCard.CreateNewCard(this, CardType.Aspect)
                .SetName("Goat Charm", "Demonic")
                .SetDesc("After attacking, give a random enemy *BI**Purple*Demonize*CE*")
                .SetStats(1, 1, 4)
                .SetPools("Aspect")
                .SetBasePicture("GoatCharmIcon.png")
                .SetIdeogram("IdeogramWF.png")
                .AddSkills(NewMark<Mark_Skill>("Add Apply Demonize", ON_RECRUIT, SPELL, TRAIT, "Mod[1")
                    .AddTargeting(NewTargeting<Targeting_GlobalTarget>())
                    .AddSignal(NewSignal<Signal_RemoveMod>(TARGET_OTHER))
                    .AddSignal(NewAddSkill(applyDemonize2, TARGET_OTHER))
                    .AddSignal(demonicPrefix)
                    .AddSignal(demonicDesc)
                    .AddAspectTrigger()
                );

            
            var createMedium = NewMedium<Medium_Explosion>("Random 2 Demonize", out var random2, TARGET_SELF);
            random2.AddKeys("TargetEnemies[1", "MaxCount[2");
            random2.AddSignal_Explosion(false, signalDemonize.InstantiateEditName("Demonize"),
                    NewColorEffect(new Color(0.4f, 0.25f, 0.8f), TARGET_OTHER),
                    NewSignal<Signal_AddActingDelay>(TARGET_SELF, DELAY_SCALE(2f)));



            //Need to add proper delay
            ModdedCard.CreateNewCard(this, CardType.PassiveFollower)
                .SetName("Pygmy", "Pygmy")
                .SetDesc("When another follower dies, give up to 2 random enemies *BI**Purple*Demonize*CE*")
                .SetStats(1, 1, 6)
                .SetPools("Passive") //Virtual pool. The real pools are Active, Passive, Aspect, Trinket, Moth, ManaAspect
                .SetBasePicture("PygmyIcon.png")
                .SetBackground("Background.png")
                .SetIdeogram("IdeogramWF.png")
                .AddStatuses(NewTriggerSignal("OnFriendlyDeath", PERMANENT, "Alive[1", TRAIT)
                    .AddSignal(createMedium)
                );

            ModdedCard.CreateNewCard(this, CardType.Trinket)
                .SetName("Lumin Vase", "LuminVase")
                .SetDesc("All future recuited followers will have upgraded traits")
                .SetPools("Trinket","Trinket_Early","Trinket_Middle","Trinket_Late")
                .SetCost(10)
                .SetBasePicture("LuminVaseIcon.png")
                .SetIdeogram("IdeogramWF.png")
                .AddSkills(NewMark<Mark_Skill>("Ennoble", ON_RECRUIT, SPELL)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(NewSignal<Signal_AddRelic>("Artificing[1"),
                        NewSignal<Signal_Banish>())
                    .AddTrinketTrigger()
                );

            assetsLoaded = true;
        }

        protected override void PostCreateAssets()
        {
            if (CombatControl.Main != null && CombatControl.Main.HasRelic("Artificing"))
            {
                RecruitPanel.Main.BannedCards.Add(Guid + ".LuminVase");
            }
        }

        protected override void ReceiveData(string guid, List<object> data)
        {
            //You can determine how to respond to other mod's data.
        }
    }
}
