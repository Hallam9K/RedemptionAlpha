using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class FriedChicken : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'I'm lovin' it'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(239, 185, 113),
                new Color(215, 109, 2),
                new Color(160, 72, 27)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, BuffID.WellFed, 20000);
            Item.value = 80;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
    }
}