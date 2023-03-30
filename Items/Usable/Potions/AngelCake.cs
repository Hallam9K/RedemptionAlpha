using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class AngelCake : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Massive improvements to all stats" +
                "\n'Fluffy'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(237, 182, 105),
                new Color(255, 253, 246),
                new Color(106, 108, 249)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(28, 30, ModContent.BuffType<WellFed4>(), 10000);
            Item.value = 120;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Cyan;
        }
    }
}