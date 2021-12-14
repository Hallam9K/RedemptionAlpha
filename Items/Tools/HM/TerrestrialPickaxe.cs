using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.HM
{
	public class TerrestrialPickaxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terrestrial Pickaxe");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		public override void SetDefaults()
		{
			Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.width = 38;
			Item.height = 38;
			Item.crit = 4;
			Item.useTime = 6;
			Item.useAnimation = 11;
			Item.pick = 225;
			Item.useStyle = ItemUseStyleID.SwingThrow;
			Item.knockBack = 5.5f;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.tileBoost += 4;
            if (!Main.dedServ)
            {
                Item.GetGlobalItem<ItemUseGlow>().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            }
        }
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<TerrestrialFragment>(), 12)
            .AddIngredient(ItemID.LunarBar, 10)
			.AddTile(TileID.LunarCraftingStation)
			.Register();
		}
	}
}
