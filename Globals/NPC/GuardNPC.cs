using Redemption.Base;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Globals.NPC
{
    public class GuardNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int GuardPoints;
        public bool IgnoreArmour;
        public bool GuardBroken;

        public void GuardHit(Terraria.NPC npc, ref double damage, SoundStyle sound, float dmgReduction = 0.25f)
        {
            if (IgnoreArmour || npc.HasBuff(BuffID.BrokenArmor) || npc.RedemptionNPCBuff().stunned || GuardPoints < 0 || GuardBroken)
                return;

            damage = (int)(damage * dmgReduction);
            npc.HitEffect();
            SoundEngine.PlaySound(sound, npc.position);
            if (npc.HitSound.HasValue)
                SoundEngine.PlaySound(npc.HitSound.Value, npc.position);
            CombatText.NewText(npc.getRect(), Colors.RarityPurple, (int)damage, true, true);
            GuardPoints -= (int)damage;
            damage = 0;
            IgnoreArmour = false;
        }
        public void GuardBreakCheck(Terraria.NPC npc, int dustType, SoundStyle sound, int dustAmount = 10, float dustScale = 1, int damage = 0)
        {
            if (GuardPoints > 0 || GuardBroken)
                return;

            SoundEngine.PlaySound(sound, npc.position);

            CombatText.NewText(npc.getRect(), Colors.RarityPurple, "Guard Broken!", false, true);
            for (int i = 0; i < dustAmount; i++)
                Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, dustType, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, Scale: dustScale);
            GuardBroken = true;
            if (damage > 0)
                BaseAI.DamageNPC(npc, damage, 0, Main.LocalPlayer, true, true);
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (GuardPoints <= 0)
                return;

            if (ItemTags.Psychic.Has(item.type))
                IgnoreArmour = true;
            if (item.hammer > 0 || item.Redemption().TechnicallyHammer)
                damage *= 4;
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (GuardPoints <= 0)
                return;

            if (ProjectileTags.Psychic.Has(projectile.type))
                IgnoreArmour = true;
            if (projectile.Redemption().IsHammer || projectile.type == ProjectileID.PaladinsHammerFriendly)
                damage *= 4;
            if (ProjectileTags.Explosive.Has(projectile.type))
                damage *= 4;
        }
        public override void SetDefaults(Terraria.NPC npc)
        {
            base.SetDefaults(npc);
            if (npc.type == NPCID.GreekSkeleton || npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigHelmet ||
                npc.type == NPCID.AngryBonesBigMuscle || npc.type == NPCID.GoblinWarrior)
                GuardPoints = 15;
            if (npc.type == NPCID.ArmoredSkeleton || npc.type == NPCID.ArmoredViking || npc.type == NPCID.PossessedArmor)
                GuardPoints = 30;
            if (npc.type == NPCID.BlueArmoredBones || npc.type == NPCID.BlueArmoredBonesMace || npc.type == NPCID.BlueArmoredBonesNoPants ||
                npc.type == NPCID.BlueArmoredBonesSword || npc.type == NPCID.RustyArmoredBonesAxe ||
                npc.type == NPCID.RustyArmoredBonesFlail || npc.type == NPCID.RustyArmoredBonesSword ||
                npc.type == NPCID.HellArmoredBones || npc.type == NPCID.HellArmoredBonesMace ||
                npc.type == NPCID.HellArmoredBonesSpikeShield || npc.type == NPCID.HellArmoredBonesSword)
                GuardPoints = 60;
            if (npc.type == NPCID.Paladin)
                GuardPoints = 500;
        }
        public override bool StrikeNPC(Terraria.NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.type == NPCID.GreekSkeleton)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.RedemptionNPCBuff().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4);
                    if (GuardPoints >= 0)
                        return false;
                }
                GuardBreakCheck(npc, DustID.Gold, CustomSounds.GuardBreak);
            }
            if (npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigHelmet || npc.type == NPCID.AngryBonesBigMuscle ||
                npc.type == NPCID.ArmoredSkeleton || npc.type == NPCID.ArmoredViking || npc.type == NPCID.BlueArmoredBones ||
                npc.type == NPCID.BlueArmoredBonesMace || npc.type == NPCID.BlueArmoredBonesNoPants ||
                npc.type == NPCID.BlueArmoredBonesSword || npc.type == NPCID.RustyArmoredBonesAxe ||
                npc.type == NPCID.RustyArmoredBonesFlail || npc.type == NPCID.RustyArmoredBonesSword)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.RedemptionNPCBuff().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return false;
                }
                GuardBreakCheck(npc, DustID.Bone, CustomSounds.GuardBreak, damage: 50);
            }
            if (npc.type == NPCID.HellArmoredBones || npc.type == NPCID.HellArmoredBonesMace ||
                npc.type == NPCID.HellArmoredBonesSpikeShield || npc.type == NPCID.HellArmoredBonesSword)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.RedemptionNPCBuff().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return false;
                }
                GuardBreakCheck(npc, DustID.Torch, CustomSounds.GuardBreak, damage: 50);
            }
            if (npc.type == NPCID.PossessedArmor)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.RedemptionNPCBuff().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4, 0.5f);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return false;
                }
                GuardBreakCheck(npc, DustID.Demonite, CustomSounds.GuardBreak, damage: 10);
            }
            if (npc.type == NPCID.GoblinWarrior)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.RedemptionNPCBuff().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4, 0.5f);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return false;
                }
                GuardBreakCheck(npc, DustID.Iron, CustomSounds.GuardBreak, damage: 10);
            }
            if (npc.type == NPCID.Paladin)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.RedemptionNPCBuff().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4, 0.2f);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return false;
                }
                GuardBreakCheck(npc, DustID.GoldCoin, CustomSounds.GuardBreak, damage: npc.lifeMax / 3);
            }
            return true;
        }
    }
}