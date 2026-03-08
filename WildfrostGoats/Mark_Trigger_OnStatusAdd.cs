using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADV;

namespace WildfrostGoats
{
    public class Mark_Trigger_OnStatusAdd : Mark_Trigger
    {
        public List<string> RequiredKeys = new List<string>();
        public List<string> AvoidedKeys = new List<string>();
        public override void InputSignal(Signal S)
        {
            if (GetKey("SourceCanBeCaster")==0 && Source == Caster)
            {
                return;
            }
            if (S is Signal_AddStatus addStatus)
            {
                IEnumerable<Mark_Status> statuses = addStatus.StatusPrefabs.Select(s => s.GetComponent<Mark_Status>()).Where(st => st != null);
                foreach (Mark_Status status in statuses)
                {
                    if (!Trigger(status))
                    {
                        continue;
                    }

                    Card signalTarget = S.Target;
                    if (GetKey("ReplaceTargetWithCaster") == 1)
                    {
                        signalTarget = Caster;
                    }

                    TryTrigger(signalTarget, 1f);
                }
                
            }
            

        }
        
        public virtual bool Trigger(Mark_Status S)
        {
            bool result = true;
            foreach (string requiredKey in RequiredKeys)
            {
                if (!S.HasKey(requiredKey) || S.GetKey(requiredKey) == 0f)
                {
                    result = false;
                }
            }

            foreach (string avoidedKey in AvoidedKeys)
            {
                if (S.GetKey(avoidedKey) > 0f)
                {
                    result = false;
                }
            }

            return result;
        }

        public virtual Mark_Trigger_OnStatusAdd AddRequiredKeys(params string[] keys)
        {
            foreach(string key in keys)
            {
                RequiredKeys.Add(key);
            }
            return this;
        }

        public virtual Mark_Trigger_OnStatusAdd AddAvoidedKeys(params string[] keys)
        {
            foreach (string key in keys)
            {
                AvoidedKeys.Add(key);
            }
            return this;
        }
    }
}
