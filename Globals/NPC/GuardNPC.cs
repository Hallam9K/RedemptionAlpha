using Redemption.Base;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using log4net.Filter;
using Redemption.NPCs.PreHM;
using Redemption.NPCs.Friendly.SpiritSummons;

namespace Redemption.Globals.NPC
{
    public class GuardNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int GuardPoints;
        public bool IgnoreArmour;
        public bool GuardBroken;
        public bool GuardPierce;
        public double GuardDamage;
        public void GuardHit(ref Terraria.NPC.HitInfo info, Terraria.NPC npc, SoundStyle sound, float dmgReduction = .25f, bool noNPCHitSound = false, int dustType = 0, SoundStyle breakSound = default, int dustAmount = 10, float dustScale = 1, int damage = 0)
        {
            if (breakSound == default)
                breakSound = CustomSounds.GuardBreak;
            if (IgnoreArmour || GuardPoints < 0 || GuardBroken)
            {
                IgnoreArmour = false;
                return;
            }

            int guardDamage = (int)(GuardDamage * dmgReduction);
            SoundEngine.PlaySound(sound, npc.position);
            CombatText.NewText(npc.getRect(), Colors.RarityPurple, guardDamage, true, true);
            GuardPoints -= guardDamage;

            if (GuardPierce)
            {
                info.Damage /= 4;
                info.Knockback /= 2;
                GuardPierce = false;

                GuardBreakCheck(npc, dustType, breakSound, dustAmount, dustScale, damage);
                return;
            }
            npc.HitEffect();
            info.HideCombatText = true;
            if (!noNPCHitSound && npc.HitSound.HasValue)
                SoundEngine.PlaySound(npc.HitSound.Value, npc.position);
            info.Damage = 0;
            info.Knockback = 0;
            npc.life++;

            GuardBreakCheck(npc, dustType, breakSound, dustAmount, dustScale, damage);
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

            if (npc.type == ModContent.NPCType<SkeletonWarden>())
            {
                if (Main.netMode != NetmodeID.Server)
                    Gore.NewGore(npc.GetSource_FromThis(), npc.position, npc.velocity, ModContent.Find<ModGore>("Redemption/SkeletonWardenGore2").Type, 1);
            }
            else if (npc.type == ModContent.NPCType<SkeletonWarden_SS>())
            {
                for (int i = 0; i < 4; i++)
                {
                    int dust = Dust.NewDust(npc.position + npc.velocity, npc.width, npc.height, DustID.DungeonSpirit,
                        npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, Scale: 2);
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (GuardPoints <= 0)
                return;
            GuardDamage = damage;
            if (npc.RedemptionNPCBuff().brokenArmor || npc.RedemptionNPCBuff().stunned)
                GuardPierce = true;
            if (item.HasElement(ElementID.Psychic))
                IgnoreArmour = true;
            if (item.hammer > 0 || item.Redemption().TechnicallyHammer)
                GuardDamage *= 4;
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (GuardPoints <= 0)
                return;
            GuardDamage = damage;
            if (projectile.HasElement(ElementID.Psychic))
                IgnoreArmour = true;
            if (projectile.Redemption().IsHammer || projectile.type == ProjectileID.PaladinsHammerFriendly)
                GuardDamage *= 4;
            if (npc.RedemptionNPCBuff().brokenArmor || npc.RedemptionNPCBuff().stunned || projectile.Redemption().EnergyBased)
                GuardPierce = true;
            if (projectile.HasElement(ElementID.Explosive))
                GuardDamage *= 2;
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
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4, .25f, false, DustID.Gold, damage: npc.lifeMax / 4);
                }
            }
            if (npc.type is NPCID.AngryBonesBig or NPCID.AngryBonesBigHelmet or NPCID.AngryBonesBigMuscle or NPCID.ArmoredSkeleton or NPCID.ArmoredViking or NPCID.BlueArmoredBones or NPCID.BlueArmoredBonesMace or NPCID.BlueArmoredBonesNoPants or NPCID.BlueArmoredBonesSword or NPCID.RustyArmoredBonesAxe or NPCID.RustyArmoredBonesFlail or NPCID.RustyArmoredBonesSword)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4, .25f, false, DustID.Bone, damage: npc.lifeMax / 4);
                }
            }
            if (npc.type is NPCID.HellArmoredBones or NPCID.HellArmoredBonesMace or NPCID.HellArmoredBonesSpikeShield or NPCID.HellArmoredBonesSword)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4, .25f, false, DustID.Torch, damage: npc.lifeMax / 4);
                }
            }
            if (npc.type is NPCID.PossessedArmor)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4, .25f, false, DustID.Demonite, damage: npc.lifeMax / 2);
                }
            }
            if (npc.type is NPCID.GoblinWarrior)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4, .4f, false, DustID.Iron, damage: npc.lifeMax / 4);
                }
            }
            if (npc.type is NPCID.Paladin)
            {
                if (GuardPoints >= 0)
                {
                    modifiers.DisableCrit();
                    modifiers.ModifyHitInfo += (ref Terraria.NPC.HitInfo n) => GuardHit(ref n, npc, SoundID.NPCHit4, .2f, false, DustID.GoldCoin, damage: npc.lifeMax / 3);
                }
            }
        }
    }
}
