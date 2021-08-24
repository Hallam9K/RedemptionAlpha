using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class MoonflareFragment : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("'Shines in the moon's reflective light'");
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
		}
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = 15;
			Item.rare = ItemRarityID.Blue;
		}
		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, Color.AntiqueWhite.ToVector3() * 0.55f * Main.essScale);
		}
	}
}