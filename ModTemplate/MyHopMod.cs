using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADV;
using ModUtils;

namespace ModTemplate
{
    //The line below says that this class is a subclass of the HopMod. You need exactly one class that does this
    public class MyHopMod : HopMod
    {
        //GUID: The mod's unique ID. Try to stick to the convention "name.mod"
        //Title: The title of the mod. This will show up in-game, so keep it classy.
        //Depends: A list of mod guids that this mod depends on to function properly. This helps determine the mod load order.
        //Description: A small description of the mod.
        public override string Guid => "name.mod";

        public override string Title => "My HoP Mod";

        public override string[] Depends => Array.Empty<string>();

        public override string Description => "The developer has yet to replace me with something meaningful. Maybe I haven't been assertive enough :/";

        //Each mod will only have one instance of their HopMod floating around. Making this Main (alt. instance) will help you keep track of it.
        public static MyHopMod instance;

        //Helpful to not double load things
        public bool assetsLoaded = false;

        public static void Log(string msg) => instance.DebugLog("TBD", msg);
        public MyHopMod(string path) : base(path)
        {
            instance = this;
            //Do not try to interact with other mods at this stage!
            //Do not create assets here!
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //OnEnable is called when the mod is turned on. This can be done multiple times!
            //The other mods exist but they may be a bit groggy from their time asleep. Refrain from contacting them.
            //You may hook onto events here; save most of the asset-building for CreateAssets().
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            //OnDisable is called when the mod is turned off. This can be done multiple times!
            //You typically want to undo anything you did in OnEnable here.
        }

        protected override void CreateAssets()
        {
            if (assetsLoaded)
            {
                return;
            }
            //You have access to the Card library now! This is the perfect time to create your cards.
            //The other mods will be doing the same. It's best not to disturb them while they do their work.
        }

        protected override void PostCreateAssets()
        {
            //Break Time! You can communicate with other mods here using the SendData method.
        }
    }
}
