using Redemption.Tags;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption
{
	public class Redemption : Mod
	{
		public override void AddRecipes() => RedeRecipes.Load(this);
		public static Redemption Instance { get; private set; }
		public Redemption()
		{
			Instance = this;
		}
        public override void Load()
        {
			ProjTags.SetProjTags();
			TileTags.SetTileTags();
			NPCTags.SetNPCTags();
		}
    }
}