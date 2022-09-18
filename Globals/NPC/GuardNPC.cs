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
        public bool GuardPierce;

        public void GuardHit(Terraria.NPC npc, ref bool vanillaDamage, ref double damage, ref float knockback, SoundStyle sound, float dmgReduction = 0.25f, bool noNPCHitSound = false)
        {
            vanillaDamage = true;
            if (IgnoreArmour || GuardPoints < 0 || GuardBroken)
            {
                IgnoreArmour = false;
                return;
            }

            int guardDamage = (int)(damage * dmgReduction);
            SoundEngine.PlaySound(sound, npc.position);
            CombatText.NewText(npc.getRect(), Colors.RarityPurple, guardDamage, true, true);
            GuardPoints -= guardDamage;

            if (npc.HasBuff(BuffID.BrokenArmor) || npc.RedemptionNPCBuff().stunned || GuardPierce)
            {
                vanillaDamage = true;
                damage /= 2;
                knockback /= 2;
                GuardPierce = false;
                return;
            }
            npc.HitEffect();
            if (!noNPCHitSound && npc.HitSound.HasValue)
                SoundEngine.PlaySound(npc.HitSound.Value, npc.position);
            vanillaDamage = false;
            damage = 0;
        }
        public void GuardBreakCheck(Terraria.NPC npc, int dustType, SoundStyle sound, int dustAmount = 10, float dustScale = 1, int damage = 0)
        {
            if (IgnoreArmour)
                IgnoreArmour = false;

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
            if (projectile.Redemption().EnergyBased)
                GuardPierce = true;
            if (ProjectileTags.Explosive.Has(projectile.type))
                damage *= 4;
        }
        public override void SetDefaults(Terraria.NPC npc)
        {
            base.SetDefaults(npc);
            if (npc.type == NPCID.GreekSkeleton || npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigHelmet ||
                npc.type == NPCID.AngryBonesBigMuscle || npc.type == NPCID.GoblinWarrior)
                GuardPoints = 25;
            if (npc.type == NPCID.ArmoredSkeleton || npc.type == NPCID.ArmoredViking || npc.type == NPCID.PossessedArmor)
                GuardPoints = 80;
            if (npc.type == NPCID.BlueArmoredBones || npc.type == NPCID.BlueArmoredBonesMace || npc.type == NPCID.BlueArmoredBonesNoPants ||
                npc.type == NPCID.BlueArmoredBonesSword || npc.type == NPCID.RustyArmoredBonesAxe ||
                npc.type == NPCID.RustyArmoredBonesFlail || npc.type == NPCID.RustyArmoredBonesSword ||
                npc.type == NPCID.HellArmoredBones || npc.type == NPCID.HellArmoredBonesMace ||
                npc.type == NPCID.HellArmoredBonesSpikeShield || npc.type == NPCID.HellArmoredBonesSword)
                GuardPoints = 160;
            if (npc.type == NPCID.Paladin)
                GuardPoints = 500;
        }
        public override bool StrikeNPC(Terraria.NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool vDmg = true;
            if (npc.type == NPCID.GreekSkeleton)
            {
                if (GuardPoints >= 0)
                {
                    GuardHit(npc, ref vDmg, ref damage, ref knockback, SoundID.NPCHit4);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return vDmg;
                }
                GuardBreakCheck(npc, DustID.Gold, CustomSounds.GuardBreak, damage: npc.lifeMax / 4);
            }
            if (npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigHelmet || npc.type == NPCID.AngryBonesBigMuscle ||
                npc.type == NPCID.ArmoredSkeleton || npc.type == NPCID.ArmoredViking || npc.type == NPCID.BlueArmoredBones ||
                npc.type == NPCID.BlueArmoredBonesMace || npc.type == NPCID.BlueArmoredBonesNoPants ||
                npc.type == NPCID.BlueArmoredBonesSword || npc.type == NPCID.RustyArmoredBonesAxe ||
                npc.type == NPCID.RustyArmoredBonesFlail || npc.type == NPCID.RustyArmoredBonesSword)
            {
                if (GuardPoints >= 0)
                {
                    GuardHit(npc, ref vDmg, ref damage, ref knockback, SoundID.NPCHit4);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return vDmg;
                }
                GuardBreakCheck(npc, DustID.Bone, CustomSounds.GuardBreak, damage: npc.lifeMax / 4);
            }
            if (npc.type == NPCID.HellArmoredBones || npc.type == NPCID.HellArmoredBonesMace ||
                npc.type == NPCID.HellArmoredBonesSpikeShield || npc.type == NPCID.HellArmoredBonesSword)
            {
                if (GuardPoints >= 0)
                {
                    GuardHit(npc, ref vDmg, ref damage, ref knockback, SoundID.NPCHit4);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return vDmg;
                }
                GuardBreakCheck(npc, DustID.Torch, CustomSounds.GuardBreak, damage: npc.lifeMax / 4);
            }
            if (npc.type == NPCID.PossessedArmor)
            {
                if (GuardPoints >= 0)
                {
                    GuardHit(npc, ref vDmg, ref damage, ref knockback, SoundID.NPCHit4);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return vDmg;
                }
                GuardBreakCheck(npc, DustID.Demonite, CustomSounds.GuardBreak, damage: npc.lifeMax / 2);
            }
            if (npc.type == NPCID.GoblinWarrior)
            {
                if (GuardPoints >= 0)
                {
                    GuardHit(npc, ref vDmg, ref damage, ref knockback, SoundID.NPCHit4, 0.5f);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return vDmg;
                }
                GuardBreakCheck(npc, DustID.Iron, CustomSounds.GuardBreak, damage: npc.lifeMax / 4);
            }
            if (npc.type == NPCID.Paladin)
            {
                if (GuardPoints >= 0)
                {
                    GuardHit(npc, ref vDmg, ref damage, ref knockback, SoundID.NPCHit4, 0.2f);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, (float)damage, knockback, hitDirection, 0, 0, 0);
                    if (GuardPoints >= 0)
                        return vDmg;
                }
                GuardBreakCheck(npc, DustID.GoldCoin, CustomSounds.GuardBreak, damage: npc.lifeMax / 3);
            }
            return true;
        }
    }
}