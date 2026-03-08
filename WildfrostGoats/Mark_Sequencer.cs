using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADV;
using UnityEngine;
using static ModUtils.MarkFactory;
using static ModUtils.KeyLib;

namespace WildfrostGoats
{
    public class Mark_Sequencer : Mark_Trigger
    {
        public string triggerKey = "triggerKey";
        public static Mark_Sequencer Create(string name, string triggerKey, float delay, params string[] keys)
        {
            Mark_Sequencer sequencer = NewMark<Mark_Sequencer>(name,keys);
            sequencer.triggerKey = triggerKey;
            sequencer.SetKey("Cooldown", delay);
            return sequencer;
        }

        public override void InputSignal(Signal S)
        {
            if (Source != null && Source.CombatActive() && S.GetKey(triggerKey) == 1)
            {
                float delay = GetKey("Cooldown");
                float currentDelay = FindCurrentDelay();
                OnTrigger(Source, currentDelay);
                AddDelay(delay);
            }
        }

        public float FindCurrentDelay()
        {
            if (CombatControl.Main.GetActingCard() != null)
            {
                return CombatControl.Main.GetActingCard().GetKey("ActingDelay");
            }
            return CombatControl.Main.GlobalCard.GetKey("ActingDelay");
        }

        public void AddDelay(float value)
        {
            if (CombatControl.Main.GetActingCard() != null)
            {
                CombatControl.Main.GetActingCard().ChangeKey("ActingDelay", value);
            }
            else
            {
                CombatControl.Main.GlobalCard.ChangeKey("ActingDelay", value);
            }
        }
    }
}
