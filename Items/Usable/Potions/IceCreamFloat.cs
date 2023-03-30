using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class IceCreamFloat : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Massive improvements to all stats" +
                "\n'Sluuuuuurp'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(247, 232, 222),
                new Color(193, 147, 120),
                new Color(119, 84, 73)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(24, 40, ModContent.BuffType<WellFed4>(), 10000, true);
            Item.value = 120;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Cyan;
        }
    }
}