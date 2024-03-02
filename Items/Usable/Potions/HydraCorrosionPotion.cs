using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class HydraCorrosionPotion : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.PoisonS);
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(234, 255, 254),
                new Color(77, 255, 247),
                new Color(66, 107, 99)
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
            Item.width = 16;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 4, 0);
            Item.rare = ItemRarityID.Blue;
            Item.buffType = ModContent.BuffType<HydraCorrosionBuff>();
            Item.buffTime = 21600;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<ToxicGlooper>()
                .AddIngredient<CarbonMyofibre>(4)
                .AddTile(TileID.Bottles)
                .DisableDecraft()
                .Register();
        }
    }
}