using Terraria.ModLoader;

namespace Redemption.Tags
{
    public class TagLoader : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();

            Globals.ItemTags.SetItemTags();
            Globals.ProjectileTags.SetProjTags();
            Globals.TileTags.SetTileTags();
            Globals.NPCTags.SetNPCTags();
        }
    }
}