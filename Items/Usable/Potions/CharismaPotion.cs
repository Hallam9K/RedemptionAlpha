using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Items.Placeable.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class CharismaPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Shops have lower prices"
                + "\nEnemies drop more gold"); */
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(249, 249, 175),
                new Color(208, 191, 80),
                new Color(158, 105, 41)
            };
            Item.ResearchUnlockCount = 20;
        }

		public override void SetDefaults()
		{
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.buffType = ModContent.BuffType<CharismaPotionBuff>();
            Item.buffTime = 36000;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldenCarp)
                .AddIngredient(ModContent.ItemType<Nightshade>())
                .AddIngredient(ItemID.Blinkroot)
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.Bottles)
                .DisableDecraft()
                .Register();
        }
    }
}
