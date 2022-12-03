using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.PostML
{
    public class BlackholeHamaxe : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
			Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.width = 52;
            Item.height = 50;
            Item.useTime = 7;
            Item.useAnimation = 27;
            Item.axe = 30;
            Item.hammer = 100;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.tileBoost = 4;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BlackholeFragment>(14)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
