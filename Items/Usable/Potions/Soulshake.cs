using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class Soulshake : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Medium improvements to all stats" +
                "\n'Tastes like marshmallow'"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(255, 255, 255),
                new Color(203, 255, 255),
                new Color(120, 216, 216)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 38, BuffID.WellFed2, 16000, true);
            Item.value = 80;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
    }
}