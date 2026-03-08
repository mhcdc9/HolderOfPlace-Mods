using ADV;
using ModdingCore;
using ModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityExplorer;
using UnityExplorer.UI;

namespace UnityExplorerMod
{
    public class UnityExplorerMod : HopMod
    {
        public override string Guid => "mhcdc9.unityexplorer";

        public override string Title => "Unity Explorer Mod";

        public override string[] Depends => Array.Empty<string>();

        public override string Description => "Allows you to inspect Unity scenes and object during runtime. Useful for devving. Credits to Sinai and co. for creating this legendary tool.";


        public bool unityStarted = false;
        public static ExplorerStandalone explorer;
        public UnityExplorerMod(string path) : base(path)
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!unityStarted)
            {
                explorer = UnityExplorer.ExplorerStandalone.CreateInstance();
                unityStarted = true;

                Command.AddCommand(new Command.CommandCard()
                {
                    id = "inspect",
                    action = Inspect
                });
            }
        }

        public static void Inspect(List<string> messages, Card card)
        {
            InspectorManager.Inspect(card.gameObject);
            UIManager.ShowMenu = true;
        }

    }
}
