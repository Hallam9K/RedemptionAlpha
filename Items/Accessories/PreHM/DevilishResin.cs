using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class DevilishResin : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Critters are attracted to you" +
                "\nShy critters won't fear you"
                + "\n'YOU STINK!'"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 0, 35, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<DevilScentedDebuff>(), 30);
        }
    }
}
