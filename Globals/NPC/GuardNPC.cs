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
        public void GuardHit(ref Terraria.NPC.HitInfo info, Terraria.NPC npc, SoundStyle sound, float dmgReduction = .25f, bool noNPCHitSound = false)
        {
            if (IgnoreArmour || GuardPoints < 0 || GuardBroken)
            {
                IgnoreArmour = false;
                return;
            }

            int guardDamage = (int)(info.Damage * dmgReduction);
            SoundEngine.PlaySound(sound, npc.position);
            CombatText.NewText(npc.getRect(), Colors.RarityPurple, guardDamage, true, true);
            GuardPoints -= guardDamage;

            if (GuardPierce)
            {
                info.Damage /= 4;
                info.Knockback /= 2;
                GuardPierce = false;
                return;
            }
            npc.HitEffect();
            info.HideCombatText = true;
            if (!noNPCHitSound && npc.HitSound.HasValue)
                SoundEngine.PlaySound(npc.HitSound.Value, npc.position);
            info.Damage = 0;
            info.Knockback = 0;
            npc.life++;
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
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (GuardPoints <= 0)
                return;

            if (npc.RedemptionNPCBuff().brokenArmor || npc.RedemptionNPCBuff().stunned)
                GuardPierce = true;
            if (item.HasElement(ElementID.Psychic))
                IgnoreArmour = true;
            if (item.hammer > 0 || item.Redemption().TechnicallyHammer)
                modifiers.FinalDamage *= 4;
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (GuardPoints <= 0)
                return;

            if (projectile.HasElement(ElementID.Psychic))
                IgnoreArmour = true;
            if (projectile.Redemption().IsHammer || projectile.type == ProjectileID.PaladinsHammerFriendly)
                modifiers.FinalDamage *= 4;
            if (npc.RedemptionNPCBuff().brokenArmor || npc.RedemptionNPCBuff().stunned || projectile.Redemption().EnergyBased)
                GuardPierce = true;
            if (projectile.HasElement(ElementID.Explosive))
                modifiers.FinalDamage *= 2;
        }
        public override void SetDefaults(Terraria.NPC npc)
        {
            base.SetDefaults(npc);
            if (npc.type is NPCID.GreekSkeleton or NPCID.AngryBonesBig or NPCID.AngryBonesBigHelmet or NPCID.AngryBonesBigMuscle or NPCID.GoblinWarrior)
                GuardPoints = 25;
            if (npc.type is NPCID.ArmoredSkeleton or NPCID.ArmoredViking or NPCID.PossessedArmor)
                GuardPoints = 80;
            if (npc.type is NPCID.BlueArmoredBones or NPCID.BlueArmoredBonesMace or NPCID.BlueArmoredBonesNoPants or NPCID.BlueArmoredBonesSword or NPCID.RustyArmoredBonesAxe or NPCID.RustyArmoredBonesFlail or NPCID.RustyArmoredBonesSword or NPCID.HellArmoredBones or NPCID.HellArmoredBonesMace or NPCID.HellArmoredBonesSpikeShield or NPCID.HellArmoredBonesSword)
                GuardPoints = 160;
            if (npc.type is NPCID.Paladin)
                GuardPoints = 500;
        }
        public override void ModifyIncomingHit(Terraria.NPC npc, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (npc.type is NPCID.GreekSkeleton)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4);
                    if (GuardPoints >= 0)
                        return;
                }
                GuardBreakCheck(npc, DustID.Gold, CustomSounds.GuardBreak, damage: npc.lifeMax / 4);
            }
            if (npc.type is NPCID.AngryBonesBig or NPCID.AngryBonesBigHelmet or NPCID.AngryBonesBigMuscle or NPCID.ArmoredSkeleton or NPCID.ArmoredViking or NPCID.BlueArmoredBones or NPCID.BlueArmoredBonesMace or NPCID.BlueArmoredBonesNoPants or NPCID.BlueArmoredBonesSword or NPCID.RustyArmoredBonesAxe or NPCID.RustyArmoredBonesFlail or NPCID.RustyArmoredBonesSword)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4);
                    if (GuardPoints >= 0)
                        return;
                }
                GuardBreakCheck(npc, DustID.Bone, CustomSounds.GuardBreak, damage: npc.lifeMax / 4);
            }
            if (npc.type is NPCID.HellArmoredBones or NPCID.HellArmoredBonesMace or NPCID.HellArmoredBonesSpikeShield or NPCID.HellArmoredBonesSword)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4);
                    if (GuardPoints >= 0)
                        return;
                }
                GuardBreakCheck(npc, DustID.Torch, CustomSounds.GuardBreak, damage: npc.lifeMax / 4);
            }
            if (npc.type is NPCID.PossessedArmor)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4);
                    if (GuardPoints >= 0)
                        return;
                }
                GuardBreakCheck(npc, DustID.Demonite, CustomSounds.GuardBreak, damage: npc.lifeMax / 2);
            }
            if (npc.type is NPCID.GoblinWarrior)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4, .4f);
                    if (GuardPoints >= 0)
                        return;
                }
                GuardBreakCheck(npc, DustID.Iron, CustomSounds.GuardBreak, damage: npc.lifeMax / 4);
            }
            if (npc.type is NPCID.Paladin)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4, .2f);
                    if (GuardPoints >= 0)
                        return;
                }
                GuardBreakCheck(npc, DustID.GoldCoin, CustomSounds.GuardBreak, damage: npc.lifeMax / 3);
            }
        }
    }
}