using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class PocketSans : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Throws a strange skull with unfathomable power\n" +
                "'Bad times all around'"); */
        }
        public override void SetDefaults()
		{
			Item.shootSpeed = 5f;
            Item.crit = 0;
            Item.damage = 1;
            Item.knockBack = 0;
            Item.DamageType = DamageClass.Default;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.width = 26;
            Item.height = 48;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Cyan;
            Item.consumable = true;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(1, 1, 1, 1);
            Item.shoot = ModContent.ProjectileType<badtimekid>();
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PocketSand>(999)
                .AddIngredient(ItemID.FragmentStardust, 20)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
