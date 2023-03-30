using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class Olives : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'Picked fresh from a Garden'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(110, 100, 146),
                new Color(65, 63, 72),
                new Color(50, 44, 70)
            };
            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Ambrosia;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(24, 30, BuffID.WellFed, 18000);
            Item.value = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
    }
}