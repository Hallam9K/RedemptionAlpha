using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals.Players;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class ElementalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        [CloneByReference] public sbyte[] OverrideElement = new sbyte[16];

        public override void ModifyWeaponCrit(Item item, Terraria.Player player, ref float crit)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                if (modPlayer.powerCell)
                {
                    if (item.HasElementItem(ElementID.Fire))
                        crit += 4;
                    if (item.HasElementItem(ElementID.Holy))
                        crit += 4;
                }
                if (modPlayer.gracesGuidance)
                {
                    if (item.HasElementItem(ElementID.Fire))
                        crit += 6;
                    if (item.HasElementItem(ElementID.Holy))
                        crit += 6;
                }
                if (modPlayer.sacredCross && item.HasElementItem(ElementID.Holy))
                    crit += 6;
                if (modPlayer.forestCore && player.dryadWard && item.HasElementItem(ElementID.Nature))
                    crit += 10;
                if (modPlayer.thornCirclet && item.HasElementItem(ElementID.Nature))
                    crit += 6;
            }
        }
        public override void ModifyHitNPC(Item item, Terraria.Player player, Terraria.NPC target, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                if (item.HasElement(ElementID.Explosive))
                    modifiers.ScalingArmorPenetration += .2f;
            }
        }
        public override void OnHitNPC(Item item, Terraria.Player player, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                if (player.RedemptionPlayerBuff().hydraCorrosion && item.HasElementItem(ElementID.Poison))
                    target.AddBuff(BuffType<HydraAcidDebuff>(), 240);
            }
        }
    }
}