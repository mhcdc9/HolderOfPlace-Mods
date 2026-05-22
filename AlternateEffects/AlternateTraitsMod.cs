using ADV;
using ModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ModUtils.KeyLib;
using static ModUtils.MarkFactory;
using static Mono.Math.BigInteger;

namespace AlternateTraits
{
    public class AlternateTraitsMod : HopMod
    {

        public override string Guid => "mhcdc9.alternatetraits";

        public override string Title => "Alternate Traits";

        public override string[] Depends => Array.Empty<string>();

        public override string Description => "When a run starts, some followers will have alternate traits.";

        public static AlternateTraitsMod instance;

        public static void DebugLog(string msg) => instance.DebugLog("AltTrt", msg);

        public bool assetsLoaded = false;
        public Dictionary<string, ModdedCard> altCards = new Dictionary<string, ModdedCard>();
        public AlternateTraitsMod(string path) : base(path)
        {
            instance = this;
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
            assetsLoaded = true;

            CreateT6();
            CreateT5();
            CreateT4();
            CreateT3();
            CreateT1();
            CreateT2();
            
        }

        protected override void PostCreateAssets()
        {
            KeyList activeList = GameObject.FindObjectsByType<KeyList>(FindObjectsSortMode.None).FirstOrDefault(l => l.Key == "Active");
            KeyList passiveList = GameObject.FindObjectsByType<KeyList>(FindObjectsSortMode.None).FirstOrDefault(l => l.Key == "Passive");

            foreach (string key in altCards.Keys)
            {
                if (activeList.Keys.Contains(key))
                {
                    activeList.Keys.Remove(key);
                    activeList.Keys.Add(altCards[key]._cardInfo.GetID());
                }
                else if (passiveList.Keys.Contains(key))
                {
                    passiveList.Keys.Remove(key);
                    passiveList.Keys.Add(altCards[key]._cardInfo.GetID());
                }
            }
        }

        public ModdedCard Alternate(string cardKey, CardType type)
        {
            altCards[cardKey] = ModdedCard.CreateNewCard(this, type)
                .CopyVisuals(cardKey);
            return altCards[cardKey];
        }

        public void CreateT1()
        {
            var militiaInvoke = NewChannelID();

            Alternate("Militia", CardType.Follower)
                .SetName("Wayfarer", "Militia")
                .SetStats(1, 1, 1)
                .SetDesc("When replaced, premanently *BI**Yellow*transfer all stats*CE**BIE* to a random follower")
                .AddSkills(
                    NewMark<Mark_Skill>("Transfer", TRAIT, "InvokeChannel[" + militiaInvoke)
                        .AddTargeting(NewTargeting<Targeting_RandomFriendly>("OutCombat[1", "IgnoreSource[1"))
                        .AddSignal(Effect_BuffStats(1, 1, true, false))
                ).AddStatuses(
                    NewTriggerSignal("ReplaceTrigger", PERMANENT, TARGET_SELF, TRAIT)
                        .AddSignal(NewInvoke("", "Channel[" + militiaInvoke))
                ).Modify(card =>
                {
                    //skill > signal
                    Signal_AddStatus signal = card.IniSkills[1].transform.GetChild(1).GetComponent<Signal_AddStatus>();
                    signal.SetKey("AddDamage", 0);
                    signal.SetKey("AddLife", 0);
                    signal.SetKey("SourceStatScaling", 1);
                    signal.InheritKeys = new List<string>() { "AddDamage", "AddLife" };
                    signal.EditSignalInfo("[Source] passed the torch to [Target]");
                });
        }

        public void CreateT2()
        {
            Alternate("Duelist", CardType.Follower)
                .SetName("Dualist", "Duelist")
                .SetStats(2, 2, 3)
                .SetDesc("At the start of combat, gain *BI**Red*+{1} Attack Damage*CE**BIE* or *BI**Green*+{2} Health*CE**BIE* until end of combat")
                .AddSkills(
                    NewMark<Mark_Skill>("AttackOrHealth", PRECOMBAT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal( NewInvokeRandom("Dualist_Attack", "Dualist_Health") ),

                    NewMark<Mark_Skill>("Attack", TRAIT)
                    .AddMarkInfo("Dualist_Attack","...")
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(Effect_BuffStats(1, 0, false)),

                    NewMark<Mark_Skill>("Health", TRAIT)
                    .AddMarkInfo("Dualist_Health", "...")
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(Effect_BuffStats(0, 2, false))
                ).AddRelevantTags();

            Alternate("Hoarder", CardType.Follower)
                .SetName("Armorer", "Hoarder")
                .SetStats(1, 2, 3)
                .SetDesc("After surviving combat, give a random follower *BI**Red*+{1} Attack Damage*CE**BIE* or *BI**Green*+{1} health*CE**BIE* permanently")
                .AddSkills(
                    NewMark<Mark_Skill>("AttackOrHealth", POSTCOMBAT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(NewInvokeRandom("Armorer_Attack", "Armorer_Health")),

                    NewMark<Mark_Skill>("Attack", TRAIT)
                    .AddMarkInfo("Armorer_Attack", "...")
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(
                        MultiTarget_Random(ApplyToFlags.Friendly, IgnoreFlags.Untargeted | IgnoreFlags.Dead, false, 1, true,
                                Effect_BuffStats(1, 0, true, false))),

                    NewMark<Mark_Skill>("Health", TRAIT)
                    .AddMarkInfo("Armorer_Health", "...")
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(
                        MultiTarget_Random(ApplyToFlags.Friendly, IgnoreFlags.Untargeted | IgnoreFlags.Dead, false, 1, true,
                                Effect_BuffStats(0, 1, true, false)))
                ).AddRelevantTags();

            Alternate("Initiate", CardType.Follower)
                .SetName("Hopeful", "Initiate")
                .SetStats(1, 4, 3)
                .SetDesc("After surviving combat, gain *BI**Pink*+{1} Faith*CE**BIE*")
                .SetKeys("Generator[1", "EarlyFaith[1")
                .AddSkills(
                    NewMark<Mark_Skill>("Faith", POSTCOMBAT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(Effect_AddFaith())
                ).AddRelevantTags();

            Alternate("Sentinel", CardType.Follower)
                .SetName("Guardian", "Sentinel")
                .SetStats(0, 3, 3)
                .SetDesc("After *BI*NOT*BIE* surviving combat, gain *BI**Green*+{1} Health*CE**BIE* permanently")
                .AddSkills(
                    NewMark<Mark_Skill>("HealthIfDead", POSTCOMBAT_DEATH, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(Effect_BuffStats(0, 1, true))
                );

            Alternate("Templar", CardType.Follower)
                .SetName("Whippersnapper", "Templar")
                .SetStats(6, 2, 3)
                .SetDesc("At the end of combat, lose *BI**Red*-{1} Attack Damage*CE**BIE* permanently")
                .AddSkills(
                    NewMark<Mark_Skill>("LoseAttack", POSTCOMBAT, POSTCOMBAT_DEATH, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(Effect_BuffStats(-1,0,true))
                );

            Alternate("Supplicant", CardType.Follower)
                .SetName("Doomsayer", "Supplicant")
                .SetStats(1, 3, 4)
                .SetDesc("Upon death, gain *BI**Pink*+{1} Faith*CE**BIE* and deal *BI**Orange*Random Damage*CE**BIE* equal to *BI**Pink*Faith*CE**BIE*")
                .SetKeys("Generator[1", "EarlyFaith[1")
                .Modify2(m =>
                {
                    Signal[] rDamageArray = Effect_RandomDamage(1, 0, 0.423f);
                    rDamageArray[0].AddScriptableKey("Count", ScriptableAmount.Type.Source, NewSAmountGeneric(c => CombatControl.Main.GetFate()));

                    m.AddSkills(
                    NewMark<Mark_Skill>("OnDeath", ONDEATH, TRAIT)
                        .AddTargeting(NewTargeting<Targeting_ToSelf>())
                        .AddSignal(NewSignal<Signal_FateChange>(TARGET_SELF, VALUE(1), "NumberEffect[1")
                            .AddNobleKey("Value", 1, new Vector3(1f, 0f, 0f))
                            .NewSignalInfo("[Source] gains *Pink*+[Value1] Faith*CE*", new Vector3(1, 0, 0))
                        ).AddSignal(rDamageArray)
                    ).AddRelevantTags();
                });

            /*
            //Will throw errors on the next recruited follower
            Alternate("Cleric", CardType.Follower)
                .SetName("Priest", "Cleric")
                .SetStats(2, 1, 3)
                .SetDesc("The next recruited follower gains a *BI*random low-cost aspect*BIE*")
                .Modify2(m =>
                {
                    var aspect = NewSignal<Signal_DirectRecruit>(TARGET_OTHER, "SetGlobalTarget[1", "RecruitEffect[1", "Delay[5");
                    aspect.Keys = new List<string>() { "Sacrificial", "Seeking", "Leading", "Incubating", "Volatile", "Swarming", "Diligent", "Aspiring", "Scavenging", "Undying" };

                m.AddStatuses(
                    NewTriggerSignal("OnNewRecruit", PERMANENT, TARGET_OTHER, "IgnoreUntargeted[1")
                        .AddSignal(aspect,
                        NewAnimEffect("AE_YellowBuff", TARGET_OTHER),
                        NewSignal<Signal_AddActingDelay>(TARGET_SELF, "AddDelay[1.5"))
                        );
                });
            */

            

        }

        public void CreateT3()
        {
            Alternate("Barrier", CardType.PassiveFollower)
                .SetName("Barricade", "Barrier")
                .SetStats(0, 1, 6)
                .SetDesc("Summon a copy of the follower behind with *BI**Red*No Attack Damage*CE**BIE* and *BI**Green*Half Health*CE**BIE*")
                .Modify2(m =>
                {
                    Signal[] signals = Effect_SummonCopy(false, 0, 1);
                    signals[0].AddScriptableKey("SummonLife", ScriptableAmount.Type.Target, NewSAmountGeneric(c => Math.Max((int)((c.Life + 0.51f) / 2), 1)));

                    m.AddSkills(
                        NewMark<Mark_Skill>("Summon", PRECOMBAT, TRAIT)
                        .AddTargeting(NewTargeting<Targeting_FriendlyNext>())
                        .AddSignal(signals)
                        );
                }).AddRelevantTags();

            Alternate("Botanist", CardType.Follower)
                .SetName("Dealer", "Botanist")
                .SetStats(2, 1, 6)
                .SetDesc("When recruited, give followers *BI**Red*+{1} Attack Damage*CE**BIE* permanently")
                .AddSkills(
                    NewMark<Mark_Skill>("Recruit", ON_RECRUIT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>("OutCombat[1"))
                    .AddSignal(
                        MultiTarget_All(ApplyToFlags.OutFriendly, IgnoreFlags.Source, false,
                            Effect_BuffStats(1, 0, true, false)
                        )
                    ),

                    NewMark<Mark_Skill>("Retrigger", ON_RECRUIT_RETRIGGER, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>("OutCombat[1"))
                    .AddSignal(
                        MultiTarget_All(ApplyToFlags.OutFriendly, IgnoreFlags.None, false,
                            Effect_BuffStats(1, 0, true, false)
                        )
                    )
                ).AddRelevantTags();

            Alternate("Dancer", CardType.Follower)
                .SetName("Performer", "Dancer")
                .SetStats(0, 1, 6)
                .SetDesc("At the start of combat, *BI*trigger the left follower's start of combat effect*BIE* and *BI*trigger the right follower's death effect*BIE*")
                .AddSkills(
                    NewMark<Mark_Skill>("InvokeLeft", PRECOMBAT, TRAIT)
                    .AddTargeting( NewTargeting<Targeting_FriendlyInFront>() )
                    .AddSignal( NewInvoke("PreCombat", TARGET_OTHER),
                        NewAnimEffect("AE_Special_Empty", TARGET_OTHER, "Priority[-1"),
                        NewAnimEffect("AE_Special", TARGET_SELF, "Priority[-1"),
                        NewSignal<Signal_AddActingDelay>(TARGET_SELF, DELAY_SCALE(0.5f))
                    ),

                    NewMark<Mark_Skill>("InvokeRight", PRECOMBAT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_FriendlyNext>())
                    .AddSignal(NewInvoke("OnDeath", TARGET_OTHER),
                        NewAnimEffect("AE_Special_Empty", TARGET_OTHER, "Priority[-1"),
                        NewAnimEffect("AE_Special", TARGET_SELF, "Priority[-1"),
                        NewSignal<Signal_AddActingDelay>(TARGET_SELF, DELAY_SCALE(0.5f))
                    )
                ).AddRelevantTags();

            Alternate("Flagbearer", CardType.Follower)
                .SetName("Flagbearer B", "Flagbearer")
                .SetStats(1, 4, 6)
                .SetDesc("Upon death, give this follower's *BI**Red*Attack Damage*CE**BIE* to the frontmost living follower and this follower's *BI**Green*Health*CE**BIE* to the backmost living follower")
                .Modify2(m =>
                {
                    Signal[] signals = Effect_BuffStats(1, 0, false, false);
                    Signal_AddStatus buffAttack = signals[0] as Signal_AddStatus;
                    buffAttack.SetKey("AddDamage", 0);
                    buffAttack.SetKey("SourceStatScaling", 1);
                    buffAttack.InheritKeys = new List<string>() { "AddDamage" };
                    buffAttack.EditSignalInfo("[Target] picks up [Source]'s flag");

                    m.AddSkills(
                            NewMark<Mark_Skill>("GiveAttack", ONDEATH, TRAIT)
                           .AddTargeting(NewTargeting<Targeting_FriendlyAggro>("IgnoreSource[1"))
                           .AddSignal(signals)
                        );

                    signals = Effect_BuffStats(0, 1, false, false);
                    Signal_AddStatus buffHealth = signals[0] as Signal_AddStatus;
                    buffHealth.SetKey("AddLife", 0);
                    buffHealth.SetKey("SourceStatScaling", 1);
                    buffHealth.InheritKeys = new List<string>() { "AddLife" };
                    buffHealth.EditSignalInfo("[Target] steels themselves");

                    m.AddSkills(
                            NewMark<Mark_Skill>("GiveHealth", ONDEATH, TRAIT)
                           .AddTargeting(NewTargeting<Targeting_FriendlyAggro_Backward>("IgnoreSource[1"))
                           .AddSignal(signals)
                        );
                }).AddRelevantTags();

            Alternate("Medic", CardType.Follower)
                .SetName("Fortifier", "Medic")
                .SetStats(1, 3, 6)
                .SetDesc("After each attack, give the frontmost follower *BI**Green*+{2} Health*CE**BIE*")
                .AddSkills(
                    NewMark<Mark_Skill>("HealLeftmost", AFTER_ATTACK, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_FriendlyAggro>())
                    .AddSignal(Effect_BuffStats(0, 2, false, false))
                ).AddRelevantTags();

            ModdedCard.CreateNewCard(this, CardType.Follower)
                .CopyVisuals("Hound")
                .SetName("Wolf", "HoundSummon")
                .SetStats(1,1,0)
                .SetKeys("NoTrait[1");

            //Doesn't have the trigger key for taking damage, so it won't proc when losing health from refiner or sacrificial or cinder emeblem
            Alternate("Hound", CardType.Follower)
                .SetName("Alpha Wolf", "Hound")
                .SetStats(1, 3, 7)
                .SetDesc("After taking damage, *BI**Purple*Summon*CE**BIE* a *BI**Red*{1}*CE**BIE* / *BI**Green*{1}*CE**BIE* Wolf")
                .AddSkills(
                    NewMark<Mark_Skill>("CallWolf", AFTER_ATTACKED, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(Effect_SummonWithStats(Guid+".HoundSummon", 1, 1, 1))
                ).AddRelevantTags();

            /*
            Alternate("Zealot", CardType.Follower)
                .SetName("Aspirant", "Zealot")
                .SetStats(1, 1, 6)
                .SetDesc("At the start of combat, copy the highest *BI**Red*Attack Damage*CE**BIE* among followers until end of combat")
                .Modify2(m =>
                {
                    Signal[] signals = Effect_BuffStats(1, 0, false);
                    Signal_AddStatus signal = signals[0] as Signal_AddStatus;
                    signal.InheritKeys = new List<string>() { "AddDamage" };
                    signal.SetKey("OverrideDamage", 1);
                    signal.AddScriptableKey("AddDamage", ScriptableAmount.Type.Target, NewSAmountGeneric(c => c.GetStatus("TempVarPower").GetKey("Stack")));
                    m.AddSkills(NewMark<Mark_Skill>("Copy Attack", PRECOMBAT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_Damage>("TargetFriendly[1", "TargetMax[1"))
                    .AddSignal(Effect_ClearVar("Power"))
                    .AddSignal(Effect_AddToVar("Power", c => c.GetBaseDamage_NonAura(), true))
                    .AddSignal(signals)
                    );
                });
            */
        }

        public void CreateT4()
        {
            Alternate("Helhound", CardType.Follower)
                .SetName("Shaman", "Helhound")
                .SetStats(3, 1, 10)
                .SetDesc("While dead, deal *BI**Orange*Random Damage*CE**BIE* equal to twice its *BI**Red*Attack Damage*CE**BIE*")
                .SetKeys("DeathProcessActive[1")
                .Modify2(m =>
                {
                    Signal[] signals = Effect_RandomDamage(1, 0);
                    signals[0].AddScriptableKey("Count", ScriptableAmount.Type.Source,
                        NewSAmountGeneric(c => 2*c.GetBaseDamage()));

                    m.AddSkills(
                    NewMark<Mark_Skill>("AttackInDeath", AUTO_DEATH, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(signals)
                    );
                }).AddRelevantTags();

            /*
            //Probably should make ranger save the attack value and then apply it
            Alternate("Ranger", CardType.Follower)
                .SetName("Thief", "Ranger")
                .SetStats(2, 3, 10)
                .SetDesc("After attacking, deal *BI**Orange*Random Damage*CE**BIE* equal to target's *BI**Red*Attack Damage*CE**BIE*")
                .Modify2(m =>
                {
                    Signal[] signals = Effect_RandomDamage(1, 0);
                    signals[0].AddScriptableKey("Count", ScriptableAmount.Type.Source,
                        NewSAmountGeneric(c => c.GetStatus("TempVarPower").GetKey("Stack")));

                    m.AddSkills(
                    NewMark<Mark_Skill>("FoulPlay", AFTER_ATTACK, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_EnemyAggro>())
                    .AddSignal(Effect_ClearVar("Power"))
                    .AddSignal(Effect_AddToVar("Power", c => c.GetBaseDamage(), true))
                    .AddSignal(signals)
                    );
                }).AddRelevantTags();
            */

            Alternate("Shielder", CardType.Follower)
                .SetName("Forger", "Shielder")
                .SetStats(2, 2, 10)
                .SetDesc("When recruited, give a random follower *BI**Green*+{4} Life*CE**BIE* permanently")
                .AddSkills(
                    NewMark<Mark_Skill>("Recruit", ON_RECRUIT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>("OutCombat[1"))
                    .AddSignal(
                        MultiTarget_Random(ApplyToFlags.OutFriendly, IgnoreFlags.Source, false, 1, true,
                            Effect_BuffStats(0, 4, true, false)
                        )
                    ),

                    NewMark<Mark_Skill>("Retrigger", ON_RECRUIT_RETRIGGER, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>("OutCombat[1"))
                    .AddSignal(
                        MultiTarget_Random(ApplyToFlags.OutFriendly, IgnoreFlags.None, false, 1, true,
                            Effect_BuffStats(0, 4, true, false)
                        )
                    )
                ).AddRelevantTags();
        }

        public void CreateT5()
        {
            Alternate("Reaper", CardType.Follower)
                .SetName("Harbinger", "Reaper")
                .SetStats(3, 3, 14)
                .SetDesc("At the start of combat, *BI**Pink*consume {3} Faith*CE**BIE* to *BI**Yellow*remove one third of all enemies' stats*CE**BIE*")
                .Modify2(m =>
                {
                    var condition = NewMark<Condition_Fate>("AtLeast3","MinValue[3");
                    condition.GKB().AddNobleKey("MinValue", 1);
                    var fate = NewSignal<Signal_FateChange>(TARGET_SELF, VALUE(-3), "NumberEffect[1");
                    fate.GKB().AddNobleKey("Value", -1);

                    Signal[] targetedDebuff = Effect_BuffStats(0, 0, false, false);
                    (targetedDebuff[0] as Signal_AddStatus).InheritKeys = new List<string>() { "AddDamage", "AddLife" };
                    targetedDebuff[0].SetKey("TargetStatScaling", -0.334f);
                    targetedDebuff[0].SetKey("AddDamage", 0);
                    targetedDebuff[0].SetKey("AddLife", 0);

                    Signal[] untargetedDebuff = Effect_BuffStats(0, 0, false, false);
                    (untargetedDebuff[0] as Signal_AddStatus).InheritKeys = new List<string>() { "AddDamage" };
                    untargetedDebuff[0].SetKey("TargetStatScaling", -0.334f);
                    untargetedDebuff[0].SetKey("AddDamage", 0);
                    untargetedDebuff[0].SetKey("AddLife", 0);

                    m.AddSkills(
                    NewMark<Mark_Skill>("Disaster", PRECOMBAT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddCondition(condition)
                    .AddSignal(fate)
                    .AddSignal(MultiTarget_All(ApplyToFlags.Enemies, IgnoreFlags.Untargeted, false, targetedDebuff))
                    .AddSignal(MultiTarget_All(ApplyToFlags.Enemies, IgnoreFlags.Targeted, false, untargetedDebuff))
                    );
                }).AddRelevantTags();

            Alternate("Reclaimer", CardType.Follower)
                .SetName("Soul Shepherd", "Reclaimer")
                .SetStats(2, 4, 14)
                .SetDesc("When another follower dies, *BI**Purple*Summon*CE**BIE* a *BI**Green*{1}-Health*CE**BIE* tentacle with this follower's *BI**Red*Attack Damage*CE**BIE*")
                .Modify2(m =>
                {
                    Signal[] signals = Effect_SummonWithStats("Tentacle", -1, 1, 1);
                    signals[0].AddScriptableKey("SummonDamage", ScriptableAmount.Type.Target, NewSAmountGeneric(c => c.GetBaseDamage_NonAura()));
                    signals[0].SetKey("Target", 1);
                    m.AddStatuses(
                        NewTriggerSignal("OnFriendlyDeath", PERMANENT, "Alive[1", TRAIT)
                        .AddSignal(signals)
                    );
                }).AddRelevantTags();

            Alternate("Sculptor", CardType.Follower)
                .SetName("Sculptor B", "Sculptor")
                .SetStats(2, 4, 14)
                .SetDesc("At the start of combat, *BI**Pink*consume {4} Faith*CE**BIE* to *BI**Purple*Summon*CE**BIE* a copy of the follower to the left")
                .Modify2(m =>
                {
                    var condition = NewMark<Condition_Fate>("AtLeast4Faith","MinValue[4");
                    condition.GKB().AddNobleKey("MinValue", 1);
                    var fate = NewSignal<Signal_FateChange>(TARGET_SELF, VALUE(-4), "NumberEffect[1");
                    fate.GKB().AddNobleKey("Value", -1);
                    m.AddSkills(
                    NewMark<Mark_Skill>("Portrait", PRECOMBAT, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_FriendlyInFront>())
                    .AddCondition(condition)
                    .AddSignal(fate)
                    .AddSignal(Effect_SummonCopy())
                    );
                }).AddRelevantTags();
        }

        public void CreateT6()
        {
            ModdedCard.CreateNewCard(this, CardType.Follower)
                .CopyVisuals("Crow")
                .SetName("Rookie", "RookSummon")
                .SetStats(1, 1, 0)
                .SetKeys("NoTrait[1");

            Alternate("Crow", CardType.Follower)
                .SetName("Rook", "Crow")
                .SetStats(2, 6, 18)
                .SetDesc("After attacking, *BI**Purple*Summon*CE**BIE* a *BI**Green*{1}-Health*CE**BIE* rookie with this follower's *BI**Red*Attack Damage*CE**BIE*")
                .Modify2(m =>
                {
                    Signal[] signals = Effect_SummonWithStats(Guid + ".RookSummon", -1, 1, 1);
                    signals[0].AddScriptableKey("SummonDamage", ScriptableAmount.Type.Source, NewSAmountGeneric(c => c.GetBaseDamage_NonAura()));
                    m.AddSkills(
                    NewMark<Mark_Skill>("CallRookie", AFTER_ATTACK, TRAIT)
                    .AddTargeting(NewTargeting<Targeting_ToSelf>())
                    .AddSignal(signals)
                    );
                }).AddRelevantTags();

            //Testing if I can reuse the same damageReduction status: Works
            Alternate("Paladin", CardType.Follower)
                .SetName("Paragon", "Paladin")
                .SetStats(2, 6, 18)
                .SetDesc("If all followers are unique, all other followers take *BI**Red*{2} damage*CE**BIE* less from each attack")
                .Modify2(m =>
                {
                    var damageReduction = NewMark<Mark_Status_DIM_SourceCondition>("DamageReduction", "DamageReduction[2");
                    damageReduction.GKB().AddNobleKey("DamageReduction", 1);
                    Condition_AllUnique condition = NewMark<Condition_AllUnique>("AllUnique");
                    damageReduction.Conditions = new List<GameObject>() { condition.gameObject };
                    condition.transform.SetParent(damageReduction.transform);
                    var addReduction = NewAddStatus(damageReduction, TARGET_OTHER);
                    m.AddSkills(
                        NewMark<Mark_Skill>("GiveResistance", PRECOMBAT_EARLY, TRAIT)
                        .AddTargeting(NewTargeting<Targeting_ToSelf>())
                        .AddSignal(
                            MultiTarget_All(ApplyToFlags.Friendly, IgnoreFlags.Source, false,
                                addReduction)
                            )
                    ).AddStatuses(
                        NewTriggerSignal("OnNewSummon", PERMANENT, TARGET_OTHER, TRAIT)
                        .AddSignal( addReduction)
                    );
                }).AddRelevantTags();
        }
    }
}
