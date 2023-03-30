using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class WineBottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bottle of Wine");
            /* Tooltip.SetDefault("Massive improvements to all stats" +
                "\n'A reward for a long days work'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(212, 75, 104),
                new Color(155, 26, 53),
                new Color(104, 17, 36)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(24, 58, ModContent.BuffType<WellFed4>(), 10000, true);
            Item.value = 80;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
    }
}