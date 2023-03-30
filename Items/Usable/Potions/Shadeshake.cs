using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class Shadeshake : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Massive improvements to all stats" +
                "\n'Tastes like black liquorice'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(10, 14, 23),
                new Color(24, 28, 38),
                new Color(210, 200, 191)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 38, ModContent.BuffType<WellFed4>(), 15000, true);
            Item.value = 80;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
    }
}