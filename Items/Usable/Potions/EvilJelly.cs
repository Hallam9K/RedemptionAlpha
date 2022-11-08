using Redemption.Buffs;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class EvilJelly : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Greatly increased chance of Shadow Fuel to drop upon slaying enemies with Shadow weapons");
            SacrificeTotal = 20;
        }

        public override void SetDefaults()
		{
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 28;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 2, 50);
            Item.rare = ItemRarityID.Green;
            Item.buffType = ModContent.BuffType<EvilJellyBuff>();
            Item.buffTime = 5400;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GrimShard>()
                .AddIngredient(ItemID.Gel, 20)
                .AddTile(TileID.CookingPots)
                .Register();
        }
	}
}
