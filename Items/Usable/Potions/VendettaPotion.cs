using Redemption.Buffs;
using Redemption.Items.Placeable.Plants;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class VendettaPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Attackers also take damage, and get inflicted by poison");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void SetDefaults()
		{
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 32;
            Item.maxStack = 30;
            Item.value = Item.sellPrice(0, 0, 3, 0);
            Item.rare = ItemRarityID.Blue;
            Item.buffType = ModContent.BuffType<VendettaPotionBuff>();
            Item.buffTime = 10800;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Nightshade>(), 2)
                .AddIngredient(ItemID.Stinger)
                .AddIngredient(ItemID.WormTooth)
                .AddIngredient(ItemID.Cactus)
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.Bottles)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Nightshade>(), 2)
                .AddIngredient(ItemID.ThornsPotion)
                .AddTile(TileID.Bottles)
                .Register();
        }
	}
}
