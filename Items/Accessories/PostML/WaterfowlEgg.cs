using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class WaterfowlEgg : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.CelestialS);
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<WaterfowlEgg_Player>().equipped = true;
        }
    }
    public class WaterfowlEgg_Player : ModPlayer
    {
        public bool equipped;
        public int duration = 0;
        public int originalHeal;
        public bool consume;
        public override void ResetEffects()
        {
            equipped = false;
        }
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if (equipped)
            {
                if (Player.potionDelay >= Player.potionDelayTime)
                {
                    originalHeal = healValue;
                    duration = 300;
                }

                if (duration > 0)
                {
                    for (int i = 1; i <= 14; i++)
                        Player.RedemptionPlayerBuff().ElementalResistance[i] -= 0.15f;
                }

                if (duration-- % 60 == 30 && duration > 0)
                    Player.Heal((int)(originalHeal * 0.08f));

                duration = (int)MathHelper.Clamp(duration, 0, 300);
            }
            else
            {
                duration = 0;
                originalHeal = 0;
            }
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            duration = 0;
            originalHeal = 0;
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            duration = 0;
            originalHeal = 0;
        }
    }
}