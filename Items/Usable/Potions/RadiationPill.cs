using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Buffs.Debuffs;
using Redemption.Globals.Player;
using Redemption.BaseExtension;

namespace Redemption.Items.Usable.Potions
{
    public class RadiationPill : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Cures radiation poisoning"
                + "\nREAD INSTRUCTIONS:"
                + "\n'Radiation normally cannot be cured, but with this new medicine, we are slowly progressing."
                + "\n- Make sure you know for a fact you have radiation poisoning, this will do more harm than good!"
                + "\n- This medicine only works when the user is in a specific stage of poisoning,"
                + "\nthe stage which is recommended to use contains the following symptoms:"
                + "\nFatigue and Nausea"
                + "\n- After successful use, you will feel weak and fragile, this will go away in a few minutes.'"); */

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
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<PillSickness>());
        }
        public override bool? UseItem(Player player)
        {
            Radiation modPlayer = player.RedemptionRad();
            if (modPlayer.irradiatedEffect == 1 || modPlayer.irradiatedEffect == 2)
            {
                modPlayer.irradiatedEffect = 0;
                modPlayer.irradiatedLevel = 0;
                modPlayer.irradiatedTimer = 0;
                player.AddBuff(ModContent.BuffType<PillSickness>(), Main.rand.Next(3600, 7200));
                player.AddBuff(BuffID.Weak, Main.rand.Next(3600, 7200));
                player.ClearBuff(ModContent.BuffType<HeadacheDebuff>());
                player.ClearBuff(ModContent.BuffType<NauseaDebuff>());
                player.ClearBuff(ModContent.BuffType<FatigueDebuff>());
                player.ClearBuff(ModContent.BuffType<FeverDebuff>());
                player.ClearBuff(ModContent.BuffType<HairLossDebuff>());
                player.ClearBuff(ModContent.BuffType<SkinBurnDebuff>());
            }
            else
                player.AddBuff(ModContent.BuffType<PillSickness>(), Main.rand.Next(3600, 7200));
            return true;
        }
    }
}
