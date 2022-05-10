using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class Pomegranate : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Massive improvements to all stats" +
                "\n'No more pomegranates!'");
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(187, 52, 52),
                new Color(220, 119, 112),
                new Color(246, 190, 186)
            };
            ItemID.Sets.IsFood[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(32, 30, ModContent.BuffType<WellFed4>(), 20000);
            Item.value = 80;
            Item.rare = ItemRarityID.Cyan;
        }
    }
}