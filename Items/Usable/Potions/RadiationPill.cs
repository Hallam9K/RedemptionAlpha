using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class RadiationPill : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 4;
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.width = 42;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 50);
            Item.rare = ItemRarityID.Yellow;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<PillSickness>());
        }
        public override bool? UseItem(Player player)
        {
            Radiation modPlayer = player.RedemptionRad();
            if (modPlayer.radiationLevel is >= 1 and < 3)
                player.AddBuff(BuffID.Weak, Main.rand.Next(3600, 7200));

            modPlayer.pillCureTimer = 3600;
            player.AddBuff(ModContent.BuffType<PillSickness>(), Main.rand.Next(3600, 7200));
            return true;
        }
    }
}