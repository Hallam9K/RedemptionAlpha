using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class SkirmishPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(198, 240, 125),
                new Color(90, 191, 93),
                new Color(33, 143, 91)
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
            Item.width = 22;
            Item.height = 40;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.Blue;
            Item.buffType = ModContent.BuffType<SkirmishBuff>();
            Item.buffTime = 10800;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ModContent.ItemType<ToxicGlooper>())
                .AddIngredient(ItemID.Deathweed, 2)
                .AddTile(TileID.Bottles)
                .DisableDecraft()
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ToxicGlooper>())
                .AddIngredient(ItemID.BattlePotion)
                .AddTile(TileID.Bottles)
                .DisableDecraft()
                .Register();
        }
    }
}