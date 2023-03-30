using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class P0T4T0 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("P0T4T0");
            /* Tooltip.SetDefault("Medium improvements to all stats\n" +
                "'Now with 100% less AI!'\n" +
                "Increases Energy regeneration"); */
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(210, 145, 83),
                new Color(182, 118, 82),
                new Color(115, 120, 141)
            };
            ItemID.Sets.IsFood[Type] = true;
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.DefaultToFood(36, 30, BuffID.WellFed2, 72000);
            Item.value = Item.sellPrice(silver: 60);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Orange;
        }
        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<EnergyRegenBuff>(), 300);
        }
    }
}