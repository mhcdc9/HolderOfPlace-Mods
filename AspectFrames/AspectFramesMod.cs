using ADV;
using ModdingCore;
using ModUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace AspectFrames
{
    public class AspectFramesMod : HopMod
    {

        public override string Guid => "mhcdc9.aspectframes";

        public override string Title => "Aspect Frames";

        public override string[] Depends => Array.Empty<string>();

        public override string Description => "The frame around a card will change to the most recent aspect used.";

        public bool commandsAdded = false;

        [Config("ignoreCustomFrames", true)]
        public bool ignoreCustomFrames = true;

        public static AspectFramesMod instance;
        public static void Debug(string msg) => instance.DebugLog("AspectFrames", msg);

        public AspectFramesMod(string path) : base(path)
        {
            instance = this;
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ModEvents.OnSpellInvoked += SpellInvoked;

            if (!commandsAdded)
            {
                Command.AddCommand(new Command.CommandCard()
                {
                    id = "frame",
                    action = (messages, c) => TryAddFrame(c).AspectAdded(null, GetImage(messages[0] + ".png"))
                });
                Command.AddCommand(new Command.CommandCard()
                {
                    id = "framespin",
                    action = (messages, c) => SpinMode(c)
                });
            }
           
        }

        protected override void OnDisable()
        {
            ModEvents.OnSpellInvoked -= SpellInvoked;
            base.OnDisable();
        }

        public void SpellInvoked(Card source, Card target, Mark_Skill skill)
        {
            if (source.GetKey("Aspect") == 1 || source.GetKey("ManaAspect") == 1)
            {
                Sprite sprite = source.transform.Find("AnimBase/NewAliveBase/Base")?.GetComponent<SpriteRenderer>()?.sprite;
                TryAddFrame(target).AspectAdded(source, GetImage(source.GetID() + ".png"));
            }
            
        }


        public AspectFrame TryAddFrame(Card c)
        {
            Transform t = c.transform.Find("AnimBase/NewAliveBase/Frame");
            if (t == null)
            {
                Debug("Couldn't find transform");
                return null;
            }
            Transform frame = t.Find("AspectFrame");
            if (frame == null)
            {
                GameObject obj = new GameObject("AspectFrame", typeof(SpriteRenderer), typeof(AspectFrame));
                frame = obj.transform;
                frame.SetParent(t, false);
                obj.GetComponent<AspectFrame>()?.Set(c);
            }
            return frame.GetComponent<AspectFrame>();
        }

        public void SpinMode(Card c)
        {
            AspectFrame frame = GetFrame(c);
            if (frame == null)
            {
                return;
            }
            frame.StartCircle();
        }

        public AspectFrame GetFrame(Card c)
        {
            Transform t = c.transform.Find("AnimBase/NewAliveBase/Frame");
            if (t == null)
            {
                Debug("Couldn't find transform");
                return null;
            }
            return t.Find("AspectFrame")?.GetComponent<AspectFrame>();
        }

        public static float scale = 0.5f;

        /*
        public void AddFrame(Transform t, Sprite sprite, bool shrink = false)
        {
            if (sprite == null)
            {
                return;
            }
            Transform transform = t.Find("AspectFrame");
            if (transform == null)
            {
                GameObject obj = new GameObject("AspectFrame");
                obj.AddComponent<SpriteRenderer>().sprite = sprite;
                obj.transform.SetParent(t, false);
                transform = obj.transform;
            }
            else
            {
                transform.GetComponent<SpriteRenderer>().sprite = sprite;
            }
            if (shrink)
            {
                transform.localScale = new Vector3(scale, scale, 1f);
                transform.localPosition = new Vector3(1.4f, 2f, -0.1f);
            }
            else
            {
                transform.localScale = Vector3.one;
                transform.localPosition = new Vector3(0, 0, -0.1f);
            }
        }
        */
    }
}
