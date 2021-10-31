using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.NPCs.Critters;
using Redemption.Projectiles.Hostile;
using Redemption.Projectiles.Ranged;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals.NPC
{
    public class BuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool infested;
        public bool devilScented;
        public int infestedTime;
        public bool rallied;
        public bool moonflare;
        public bool dirtyWound;
        public int dirtyWoundTime;
        public bool spiderSwarmed;
        public bool pureChill;
        public bool dragonblaze;
        public bool necroticGouge;
        public bool iceFrozen;
        public bool frozenFallen;
        public bool disarmed;
        public bool silverwoodArrow;
        public bool blackHeart;

        public override void ResetEffects(Terraria.NPC npc)
        {
            devilScented = false;
            rallied = false;
            moonflare = false;
            spiderSwarmed = false;
            pureChill = false;
            dragonblaze = false;
            necroticGouge = false;
            iceFrozen = false;
            disarmed = false;
            silverwoodArrow = false;
            blackHeart = false;
            if (!npc.HasBuff(ModContent.BuffType<InfestedDebuff>()))
            {
                infested = false;
                infestedTime = 0;
            }
            if (!npc.HasBuff(ModContent.BuffType<DirtyWoundDebuff>()))
            {
                dirtyWound = false;
                dirtyWoundTime = 0;
            }
        }

        #region Debuff Immunities
        public static void AddDebuffImmunity(int npcType, int[] array)
        {
            if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(npcType, out var entry) || entry?.SpecificallyImmuneTo is null)
                return;

            int[] array2 = NPCID.Sets.DebuffImmunitySets[npcType].SpecificallyImmuneTo;
            NPCID.Sets.DebuffImmunitySets[npcType] = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = array2.Concat(array).ToArray()
            };
        }

        public override void SetStaticDefaults()
        {
            for (int i = 0; i < NPCID.Sets.AllNPCs.Length; i++)
            {
                if (NPCTags.Demon.Has(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<DragonblazeDebuff>(),
                    ModContent.BuffType<MoonflareDebuff>() });
                }
                if (NPCTags.Cold.Has(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<PureChillDebuff>(),
                    ModContent.BuffType<IceFrozen>() });
                }
                if (NPCTags.Plantlike.Has(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<InfestedDebuff>() });
                }
                if (NPCTags.Dragonlike.Has(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<DragonblazeDebuff>() });
                }
                if (NPCTags.Inorganic.Has(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>() });
                }
                if (NPCLists.IsSlime.Contains(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>() });
                }

            }
        }
        #endregion

        public override void UpdateLifeRegen(Terraria.NPC npc, ref int damage)
        {
            if (infested)
            {
                infestedTime++;
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= infestedTime / 120;
            }
            if (dirtyWound)
            {
                dirtyWoundTime++;
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= dirtyWoundTime / 500;

                if (npc.wet && !npc.lavaWet)
                    npc.DelBuff(ModContent.BuffType<DirtyWoundDebuff>());
            }
            if (moonflare)
            {
                int dot = 2;
                if (!Main.dayTime)
                {
                    if (Main.moonPhase == 0)
                        dot = 18;
                    else if (Main.moonPhase == 1 || Main.moonPhase == 7)
                        dot = 14;
                    else if (Main.moonPhase == 2 || Main.moonPhase == 6)
                        dot = 10;
                    else if (Main.moonPhase == 3 || Main.moonPhase == 5)
                        dot = 6;
                }
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= dot;
            }
            if (spiderSwarmed)
            {
                npc.lifeRegen -= 4;
            }
            if (pureChill)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 8;

                if (damage < 2)
                    damage = 2;
            }
            if (dragonblaze)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                if (NPCTags.Plantlike.Has(npc.type) || NPCTags.Cold.Has(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    npc.lifeRegen -= 24;
                    if (damage < 2)
                        damage = 2;
                }
                else
                    npc.lifeRegen -= 12;
            }
            if (silverwoodArrow)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int arrowCount = 0;
                for (int i = 0; i < 1000; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<SilverwoodArrow>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                        arrowCount++;
                }
                npc.lifeRegen -= arrowCount * 7;
                if (damage < arrowCount * 8)
                    damage = arrowCount * 8;
            }
            if (blackHeart)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 400;
                if (damage < 2)
                    damage = 2;
            }
        }
        public override bool StrikeNPC(Terraria.NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (infested)
            {
                if (npc.defense > 0)
                    npc.defense -= infestedTime / 120;
            }
            if (rallied)
                damage *= 0.85;
            return true;
        }
        public override void ModifyHitPlayer(Terraria.NPC npc, Terraria.Player target, ref int damage, ref bool crit)
        {
            if (rallied)
                damage = (int)(damage * 1.15f);
            if (dragonblaze)
                damage = (int)(damage * 0.85f);
            if (disarmed)
                damage = (int)(damage * 0.2f);
        }
        public override void ModifyHitNPC(Terraria.NPC npc, Terraria.NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (rallied)
                damage = (int)(damage * 1.15f);
            if (dragonblaze)
                damage = (int)(damage * 0.85f);
            if (disarmed)
                damage = (int)(damage * 0.2f);

        }
        public override void DrawEffects(Terraria.NPC npc, ref Color drawColor)
        {
            if (infested)
                drawColor = new Color(197, 219, 171);
            if (rallied)
                drawColor = new Color(200, 150, 150);
            if (pureChill)
            {
                drawColor = new Color(180, 220, 220);
                if (Main.rand.NextBool(14))
                {
                    int sparkle = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<SnowflakeDust>(), newColor: Color.White);
                    Main.dust[sparkle].velocity *= 0.5f;
                    Main.dust[sparkle].noGravity = true;
                }
            }
            if (moonflare)
            {
                drawColor = new Color(255, 255, 218);
                int intensity = 30;
                if (Main.moonPhase == 0)
                    intensity = 5;
                else if (Main.moonPhase == 1 || Main.moonPhase == 7)
                    intensity = 10;
                else if (Main.moonPhase == 2 || Main.moonPhase == 6)
                    intensity = 15;
                else if (Main.moonPhase == 3 || Main.moonPhase == 5)
                    intensity = 20;
                if (Main.rand.NextBool(intensity))
                {
                    int sparkle = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<MoonflareDust>(), Scale: 2);
                    Main.dust[sparkle].velocity *= 0;
                    Main.dust[sparkle].noGravity = true;
                }
            }
            if (spiderSwarmed)
            {
                if (Main.rand.Next(10) == 0 && npc.alpha < 200)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<SpiderSwarmerDust>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                }
            }
            if (dragonblaze)
            {
                drawColor = new Color(220, 150, 150);
                if (Main.rand.NextBool(5))
                {
                    int sparkle = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.FlameBurst, Scale: 2);
                    Main.dust[sparkle].velocity *= 0.3f;
                    Main.dust[sparkle].noGravity = true;
                }
            }
            if (iceFrozen)
            {
                drawColor = new Color(0.4f, 1f, 1f, 0.8f);
                if (Main.rand.NextBool(30))
                {
                    int sparkle = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Ice, Scale: 1);
                    Main.dust[sparkle].velocity *= 0;
                    Main.dust[sparkle].noGravity = true;
                }
            }
            if (blackHeart)
            {
                if (Main.rand.Next(3) == 0 && npc.alpha < 200)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<VoidFlame>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override bool PreAI(Terraria.NPC npc)
        {
            if (iceFrozen)
            {
                if (npc.noGravity && !npc.noTileCollide)
                    npc.velocity.Y += 0.5f;
                npc.position.X = npc.oldPosition.X;
                if (npc.noTileCollide)
                {
                    npc.position.Y = npc.oldPosition.Y;
                    npc.velocity.Y = 0;
                }
                npc.velocity.X = 0;

                npc.frameCounter = 0;

                if ((npc.velocity.Y == 0 || npc.collideY) && !frozenFallen)
                {
                    if (npc.oldVelocity.Y > 7)
                        BaseAI.DamageNPC(npc, (int)(npc.oldVelocity.Y * 20), 0, Main.LocalPlayer, true, true);
                    frozenFallen = true;
                }
                return false;
            }
            return true;
        }

        public override bool CanHitPlayer(Terraria.NPC npc, Terraria.Player target, ref int cooldownSlot)
        {
            if (iceFrozen)
                return false;
            return true;
        }

        public override void PostAI(Terraria.NPC npc)
        {
            if (moonflare)
            {
                foreach (Terraria.NPC target in Main.npc.Take(Main.maxNPCs))
                {
                    if (!target.active || target.whoAmI == npc.whoAmI || target.GetGlobalNPC<BuffNPC>().moonflare)
                        continue;

                    if (!target.Hitbox.Intersects(npc.Hitbox))
                        continue;

                    target.AddBuff(ModContent.BuffType<MoonflareDebuff>(), 360);
                }
            }
            if (pureChill && npc.knockBackResist > 0 && !npc.boss)
            {
                if (npc.noGravity)
                    npc.velocity.Y *= 0.94f;
                npc.velocity.X *= 0.94f;
            }
        }
        public override void HitEffect(Terraria.NPC npc, int hitDirection, double damage)
        {
            if (npc.life <= 0 && necroticGouge && npc.lifeMax > 5)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath19, npc.position);
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex4 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Blood, Scale: 3f);
                    Main.dust[dustIndex4].velocity *= 5f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 6; i++)
                        Projectile.NewProjectile(npc.GetProjectileSpawnSource(), npc.Center, RedeHelper.SpreadUp(14), ModContent.ProjectileType<Blood_Proj>(), npc.damage, 0, Main.myPlayer);
                }
            }
            if (iceFrozen)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, npc.position);
                Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Ice, Scale: 1);
            }
        }
        public override bool PreKill(Terraria.NPC npc)
        {
            if (infested && infestedTime >= 60 && npc.lifeMax > 5)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath19, npc.position);
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex4 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.GreenBlood, Scale: 3f);
                    Main.dust[dustIndex4].velocity *= 5f;
                }
                int larvaCount = infestedTime / 180 + 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < MathHelper.Clamp(larvaCount, 1, 8); i++)
                        Projectile.NewProjectile(npc.GetProjectileSpawnSource(), npc.Center, RedeHelper.SpreadUp(8), ModContent.ProjectileType<GrandLarvaFall>(), 0, 0, Main.myPlayer);
                }
            }
            if (iceFrozen)
            {
                SoundEngine.PlaySound(SoundID.Item27, npc.position);
                for (int i = 0; i < 30; i++)
                {
                    int dustIndex4 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Ice, Scale: 1);
                    Main.dust[dustIndex4].velocity *= 3f;
                }
            }
            return true;
        }
    }
}