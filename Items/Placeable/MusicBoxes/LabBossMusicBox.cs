using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class LabBossMusicBox : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Abandoned Lab Minibosses)");
            Tooltip.SetDefault("inSignia - Safety Violation");

			SacrificeTotal = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic"), ModContent.ItemType<LabBossMusicBox>(), ModContent.TileType<LabBossMusicBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LabBossMusicBoxTile>(), 0);
			Item.createTile = ModContent.TileType<LabBossMusicBoxTile>();
			Item.width = 32;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<LabPlating>(), 20)
				.AddIngredient(ModContent.ItemType<XenomiteItem>(), 8)
				.AddTile(TileID.MythrilAnvil)
				.Register();
        }
    }
}
