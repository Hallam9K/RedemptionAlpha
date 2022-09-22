using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.NPCs.Critters;
using Redemption.Projectiles.Hostile;
using Redemption.Projectiles.Misc;
using Redemption.Projectiles.Ranged;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Particles;
using ParticleLibrary;
using Redemption.Globals.Player;

namespace Redemption.Globals.NPC
{
    public class BuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

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
        public bool bileDebuff;
        public bool electrified;
        public bool stunned;
        public bool infected;
        public int infectedTime;
        public bool stomachAcid;
        public bool incisored;
        public bool stoneskin;

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
            bileDebuff = false;
            electrified = false;
            stunned = false;
            stomachAcid = false;
            incisored = false;
            stoneskin = false;

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
            if (!npc.HasBuff(ModContent.BuffType<ViralityDebuff>()))
            {
                infected = false;
                infectedTime = 0;
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
            for (int i = 0; i < NPCLoader.NPCCount; i++)
            {
                if (NPCLists.Demon.Contains(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<DragonblazeDebuff>(),
                    ModContent.BuffType<MoonflareDebuff>() });
                }
                if (NPCLists.Cold.Contains(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<PureChillDebuff>(),
                    ModContent.BuffType<IceFrozen>() });
                }
                if (NPCLists.Plantlike.Contains(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>(),
                    ModContent.BuffType<InfestedDebuff>() });
                }
                if (NPCLists.Dragonlike.Contains(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<DragonblazeDebuff>() });
                }
                if (NPCLists.Inorganic.Contains(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>() });
                }
                if (NPCLists.Infected.Contains(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>() });
                }
                if (NPCLists.IsSlime.Contains(i))
                {
                    AddDebuffImmunity(i, new int[] {
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>() });
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
            if (infected)
            {
                infectedTime++;
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 500;

                if (damage < 100)
                    damage = 100;
            }
            if (dirtyWound)
            {
                dirtyWoundTime++;
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= dirtyWoundTime / 500;

                //if (npc.wet && !npc.lavaWet && npc.HasBuff(ModContent.BuffType<DirtyWoundDebuff>()))
                //    npc.DelBuff(ModContent.BuffType<DirtyWoundDebuff>());
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

                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
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
                if (damage < 20)
                    damage = 20;
            }
            if (bileDebuff || stomachAcid)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 5;
            }
            if (electrified)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= (int)(npc.velocity.Length() * 20);

                if (damage < 6)
                    damage = 6;
            }
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (stomachAcid)
                player.GetArmorPenetration(DamageClass.Generic) += 8;
            if (bileDebuff)
                player.GetArmorPenetration(DamageClass.Generic) += 15;
            if (infected)
                player.GetArmorPenetration(DamageClass.Generic) += 20;
            if (incisored)
                player.GetArmorPenetration(DamageClass.Generic) += (player.GetModPlayer<RitualistPlayer>().SpiritLevel + 1) * 5;
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Terraria.Player player = Main.player[projectile.owner];
            if (stomachAcid)
                player.GetArmorPenetration(DamageClass.Generic) += 8;
            if (bileDebuff)
                player.GetArmorPenetration(DamageClass.Generic) += 15;
            if (infected)
                player.GetArmorPenetration(DamageClass.Generic) += 20;
            if (incisored)
                player.GetArmorPenetration(DamageClass.Generic) += (player.GetModPlayer<RitualistPlayer>().SpiritLevel + 1) * 5;
        }
        public override bool StrikeNPC(Terraria.NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (infected)
                damage *= 1.2;
            if (infested)
            {
                if (npc.defense > 0)
                    npc.defense -= infestedTime / 120;
            }
            if (rallied)
                damage *= 0.85;
            if (stoneskin)
                damage *= 0.75;
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
            if (infected)
                drawColor = new Color(32, 158, 88);
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
                if (Main.rand.NextBool(10)&& npc.alpha < 200)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<SpiderSwarmerDust>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                }
            }
            if (dragonblaze)
            {
                drawColor = new Color(220, 150, 150);
                if (Main.rand.NextBool(5) && !Main.gamePaused)
                {
                    ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(npc), RedeHelper.SpreadUp(1), new EmberParticle(), Color.OrangeRed, 1);
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
                if (Main.rand.NextBool(3)&& npc.alpha < 200)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<VoidFlame>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                }
            }
            if (bileDebuff)
            {
                if (Main.rand.NextBool(4))
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.GreenFairy, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
            if (electrified)
            {
                if (Main.rand.NextBool(5) && !Main.gamePaused)
                {
                    DustHelper.DrawParticleElectricity(new Vector2(npc.position.X, npc.position.Y + Main.rand.Next(0, npc.height)), new Vector2(npc.TopRight.X, npc.TopRight.Y + Main.rand.Next(0, npc.height)), new LightningParticle(), 1f, 10, 0.2f);
                }
            }
            if (stomachAcid)
            {
                drawColor = new Color(52, 178, 108);
                if (Main.rand.NextBool(4))
                    Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.ToxicBubble, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, Alpha: 100);
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
            if (stunned)
            {
                if (npc.noGravity && !npc.noTileCollide)
                    npc.velocity.Y += 0.3f;
                npc.position.X = npc.oldPosition.X;
                if (npc.noTileCollide)
                {
                    npc.position.Y = npc.oldPosition.Y;
                    npc.velocity.Y = 0;
                }
                npc.velocity.X = 0;

                npc.frameCounter = 0;
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
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    Terraria.NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy() || target.whoAmI == npc.whoAmI || target.RedemptionNPCBuff().moonflare)
                        continue;

                    if (!target.Hitbox.Intersects(npc.Hitbox))
                        continue;

                    target.AddBuff(ModContent.BuffType<MoonflareDebuff>(), 360);
                }
            }
            if (infected)
            {
                if (infectedTime >= 360 && npc.lifeMax < 7500)
                {
                    BaseAI.DamageNPC(npc, 7500, 0, Main.LocalPlayer, true, true);
                }

                if (infectedTime >= 100)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        Terraria.NPC target = Main.npc[i];
                        if (!target.active || !target.CanBeChasedBy() || target.whoAmI == npc.whoAmI || target.RedemptionNPCBuff().infected)
                            continue;

                        if (!target.Hitbox.Intersects(npc.Hitbox))
                            continue;

                        if (Main.rand.NextBool(75))
                            target.AddBuff(ModContent.BuffType<ViralityDebuff>(), 420);
                    }
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
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, RedeHelper.SpreadUp(14), ModContent.ProjectileType<Blood_Proj>(), npc.damage, 0, Main.myPlayer);
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
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, RedeHelper.SpreadUp(8), ModContent.ProjectileType<GrandLarvaFall>(), 0, 0, Main.myPlayer);
                }
            }
            if (infected && infectedTime >= 360 && npc.lifeMax > 5)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath19, npc.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GasCanister_Gas>(), 0, 0, Main.myPlayer);
            }
            if (iceFrozen)
            {
                SoundEngine.PlaySound(SoundID.Item27, npc.position);
                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Ice);
                    Main.dust[dust].velocity *= 3f;
                }
            }
            return true;
        }
    }
}