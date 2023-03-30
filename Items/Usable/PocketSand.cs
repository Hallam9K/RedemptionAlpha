using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class PocketSand : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Throws a dust cloud that slightly reduces defense and confuses the target\n" +
                "'Sand in the eyes!'"); */
            Item.ResearchUnlockCount = 99;
        }
        public override void SetDefaults()
		{
			Item.shootSpeed = 3f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.width = 26;
            Item.height = 48;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
            Item.consumable = true;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 0, 0, 10);
            Item.shoot = ModContent.ProjectileType<SandDust_Proj>();
		}
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ItemID.Leather)
                .AddIngredient(ItemID.SandBlock, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
