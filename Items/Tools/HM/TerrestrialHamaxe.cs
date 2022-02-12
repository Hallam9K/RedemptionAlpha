using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Tools.HM
{
    public class TerrestrialHamaxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terrestrial Hamaxe");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		public override void SetDefaults()
		{
			Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
			Item.height = 52;
			Item.crit = 4;
			Item.useTime = 7;
			Item.useAnimation = 27;
			Item.axe = 37;
			Item.hammer = 100;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 7f;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            if (!Main.dedServ)
            {
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<TerrestrialFragment>(), 14)
            .AddIngredient(ItemID.LunarBar, 12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
	}
}
