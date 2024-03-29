using Redemption.BaseExtension;
using Redemption.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class Panacea : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Panacea Pill");
            // Tooltip.SetDefault("Cures radiation poisoning instantly and grants complete immunity to it for 10 minutes");
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Purple;
        }
        public override bool? UseItem(Player player)
        {
            player.RedemptionRad().radiationLevel = 0;
            player.AddBuff(ModContent.BuffType<PanaceaBuff>(), 36000);
            return true;
        }
    }
}
