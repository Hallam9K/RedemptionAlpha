using Redemption.Buffs;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class AbyssalSmoothie : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Massive improvements to all stats\n" +
                "Causes blindness for a short duration" +
                "\n'Tastes abyssmal'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 20;
            Item.height = 44;
            Item.value = 8000;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.buffType = ModContent.BuffType<WellFed4>();
            Item.buffTime = 36000;
        }
        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.Darkness, 600);
            return base.UseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AbyssBloskus>())
                .AddIngredient(ItemID.BlackCurrant, 2)
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}