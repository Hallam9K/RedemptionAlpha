using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class Avocado : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'Eat it before it gets nicked'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(111, 128, 78),
                new Color(64, 83, 46),
                new Color(81, 77, 36)
            };
            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Ambrosia;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(24, 32, BuffID.WellFed, 18000);
            Item.value = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
    }
}