using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class ClockworkOrange : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Minor improvements to all stats" +
                "\n'We can destroy what we have eaten, but we cannot uneat it.'");
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(236, 161, 31),
                new Color(213, 108, 36),
                new Color(161, 54, 12)
            };
            ItemID.Sets.IsFood[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(36, 30, ModContent.BuffType<WellFed4>(), 9000);
            Item.value = 80;
            Item.rare = ItemRarityID.Cyan;
        }
    }
}