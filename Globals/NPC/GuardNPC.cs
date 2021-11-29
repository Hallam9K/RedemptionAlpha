using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals.NPC
{
    public class GuardNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public int GuardPoints;
        public bool IgnoreArmour;
        public bool GuardBroken;

        public void GuardHit(Terraria.NPC npc, ref double damage, LegacySoundStyle sound, float dmgReduction = 0.25f)
        {
            if (IgnoreArmour || npc.HasBuff(BuffID.BrokenArmor) || npc.GetGlobalNPC<BuffNPC>().stunned || GuardPoints < 0 || GuardBroken)
                return;

            damage = (int)(damage * dmgReduction);
            npc.HitEffect();
            SoundEngine.PlaySound(sound, npc.position);
            SoundEngine.PlaySound(npc.HitSound, npc.position);
            CombatText.NewText(npc.getRect(), Colors.RarityPurple, (int)damage, true, true);
            GuardPoints -= (int)damage;
            damage = 0;
            IgnoreArmour = false;
        }
        public void GuardBreakCheck(Terraria.NPC npc, int dustType, LegacySoundStyle sound, int dustAmount = 10, float dustScale = 1, int damage = 0, bool customSound = true, string soundString = "GuardBreak")
        {
            if (GuardPoints > 0 || GuardBroken)
                return;

            if (customSound && !Main.dedServ)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/" + soundString), npc.position);
            else
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
            if (item.hammer > 0)
                damage *= 4;
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (GuardPoints <= 0)
                return;

            if (ProjectileTags.Psychic.Has(projectile.type))
                IgnoreArmour = true;
            if (projectile.type == ProjectileID.PaladinsHammerFriendly)
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
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.GetGlobalNPC<BuffNPC>().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4);
                    return false;
                }
                GuardBreakCheck(npc, DustID.Gold, SoundID.Item37);
            }
            if (npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigHelmet || npc.type == NPCID.AngryBonesBigMuscle ||
                npc.type == NPCID.ArmoredSkeleton || npc.type == NPCID.ArmoredViking || npc.type == NPCID.BlueArmoredBones ||
                npc.type == NPCID.BlueArmoredBonesMace || npc.type == NPCID.BlueArmoredBonesNoPants ||
                npc.type == NPCID.BlueArmoredBonesSword || npc.type == NPCID.RustyArmoredBonesAxe ||
                npc.type == NPCID.RustyArmoredBonesFlail || npc.type == NPCID.RustyArmoredBonesSword)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.GetGlobalNPC<BuffNPC>().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4);
                    return false;
                }
                GuardBreakCheck(npc, DustID.Bone, SoundID.Item37, damage: 50);
            }
            if (npc.type == NPCID.HellArmoredBones || npc.type == NPCID.HellArmoredBonesMace ||
                npc.type == NPCID.HellArmoredBonesSpikeShield || npc.type == NPCID.HellArmoredBonesSword)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.GetGlobalNPC<BuffNPC>().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4);
                    return false;
                }
                GuardBreakCheck(npc, DustID.Torch, SoundID.Item37, damage: 50);
            }
            if (npc.type == NPCID.PossessedArmor)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.GetGlobalNPC<BuffNPC>().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4, 0.5f);
                    return false;
                }
                GuardBreakCheck(npc, DustID.Demonite, SoundID.Item37, damage: 10);
            }
            if (npc.type == NPCID.GoblinWarrior)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.GetGlobalNPC<BuffNPC>().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4, 0.5f);
                    return false;
                }
                GuardBreakCheck(npc, DustID.Iron, SoundID.Item37, damage: 10);
            }
            if (npc.type == NPCID.Paladin)
            {
                if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && !npc.GetGlobalNPC<BuffNPC>().stunned && GuardPoints >= 0)
                {
                    GuardHit(npc, ref damage, SoundID.NPCHit4, 0.2f);
                    return false;
                }
                GuardBreakCheck(npc, DustID.GoldCoin, SoundID.Item37, damage: npc.lifeMax / 3);
            }
            return true;
        }
    }
}