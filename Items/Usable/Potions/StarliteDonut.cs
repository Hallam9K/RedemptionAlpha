using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class StarliteDonut : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Starlite Donut");
            /* Tooltip.SetDefault("Medium improvements to all stats" +
                "\n'Do you know who ate all the donuts?'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(111, 226, 158),
                new Color(66, 210, 220),
                new Color(114, 167, 111)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(26, 30, BuffID.WellFed2, 26000);
            Item.value = Item.sellPrice(silver: 80);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Orange;
        }
    }
}