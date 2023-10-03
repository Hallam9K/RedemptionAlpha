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
using Redemption.Projectiles.Magic;

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
        public bool brokenArmor;
        public bool sandDust;
        public bool badtime;
        public bool holyFire;
        public bool ukonArrow;
        public bool bInfection;
        public bool roosterBoost;
        public bool contagionShard;
        public bool soaked;

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
            brokenArmor = false;
            sandDust = false;
            badtime = false;
            holyFire = false;
            ukonArrow = false;
            bInfection = false;
            roosterBoost = false;
            contagionShard = false;
            soaked = false;

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
        public enum NPCDebuffImmuneType : byte
        {
            Demon,
            Cold,
            Inorganic,
            Infected,
            NoBlood,
            Hot
        }
        public static void NPCTypeImmunity(int Type, NPCDebuffImmuneType npcImmuneType)
        {
            switch (npcImmuneType)
            {
                case NPCDebuffImmuneType.Demon:
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<InfestedDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<DragonblazeDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<MoonflareDebuff>()] = true;
                    break;
                case NPCDebuffImmuneType.Cold:
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Chilled] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<PureChillDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<IceFrozen>()] = true;
                    break;
                case NPCDebuffImmuneType.Hot:
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<DragonblazeDebuff>()] = true;
                    break;
                case NPCDebuffImmuneType.Inorganic:
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.BloodButcherer] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
                    break;
                case NPCDebuffImmuneType.Infected:
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<BileDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<GreenRashesDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<GlowingPustulesDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<FleshCrystalsDebuff>()] = true;
                    break;
                case NPCDebuffImmuneType.NoBlood:
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
                    NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.BloodButcherer] = true;
                    break;
            }
        }
        public override void SetStaticDefaults()
        {
            for (int i = 0; i < NPCLoader.NPCCount; i++)
            {
                if (NPCLists.Demon.Contains(i))
                {
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<InfestedDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<DragonblazeDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<MoonflareDebuff>()] = true;
                }
                if (NPCLists.Cold.Contains(i))
                {
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<PureChillDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<IceFrozen>()] = true;
                }
                if (NPCLists.Plantlike.Contains(i))
                {
                    NPCID.Sets.SpecificDebuffImmunity[i][BuffID.Bleeding] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][BuffID.BloodButcherer] = true;
                }
                if (NPCLists.Dragonlike.Contains(i))
                {
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<DragonblazeDebuff>()] = true;
                }
                if (NPCLists.Inorganic.Contains(i))
                {
                    NPCID.Sets.SpecificDebuffImmunity[i][BuffID.Bleeding] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][BuffID.BloodButcherer] = true;
                }
                if (NPCLists.Infected.Contains(i))
                {
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<BileDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<GreenRashesDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<GlowingPustulesDebuff>()] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][ModContent.BuffType<FleshCrystalsDebuff>()] = true;
                }
                if (NPCLists.IsSlime.Contains(i))
                {
                    NPCID.Sets.SpecificDebuffImmunity[i][BuffID.Bleeding] = true;
                    NPCID.Sets.SpecificDebuffImmunity[i][BuffID.BloodButcherer] = true;
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

                npc.lifeRegen -= 10;
            }
            if (electrified)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= (int)(npc.velocity.Length() * 10);

                if (damage < 6)
                    damage = 6;
            }
            if (badtime)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 4000;
                if (damage < 1)
                    damage = 1;
                if (npc.knockBackResist > 0)
                {
                    npc.velocity.X *= 0.4f;
                    npc.velocity.Y *= 0.4f;
                }
            }
            if (holyFire)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 200;
                if (damage < 10)
                    damage = 10;
            }
            if (ukonArrow)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int arrowCount = 0;
                for (int i = 0; i < 1000; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<UkonvasaraArrow>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                        arrowCount++;
                }
                npc.lifeRegen -= arrowCount * 100;
                if (damage < arrowCount * 50)
                    damage = arrowCount * 50;
            }
            if (bInfection)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 200;
                if (damage < 20)
                    damage = 20;
            }
            if (contagionShard)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int shardCount = 0;
                for (int i = 0; i < 1000; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<ContagionShard_Proj>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                        shardCount++;
                }
                npc.lifeRegen -= shardCount * 8;
                if (damage < shardCount * 8)
                    damage = shardCount * 8;
            }
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (incisored)
                modifiers.ArmorPenetration += (player.GetModPlayer<RitualistPlayer>().SpiritLevel + 1) * 5;
            if (soaked && item.HasElement(ElementID.Ice))
                modifiers.FinalDamage *= 1.15f;
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (incisored)
                modifiers.ArmorPenetration += (Main.player[projectile.owner].GetModPlayer<RitualistPlayer>().SpiritLevel + 1) * 5;
            if (soaked && projectile.HasElement(ElementID.Ice))
                modifiers.FinalDamage *= 1.15f;
        }
        public override void ModifyIncomingHit(Terraria.NPC npc, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (roosterBoost && Main.expertMode)
                modifiers.Knockback *= .8f;
            if (stomachAcid)
                modifiers.Defense.Flat -= 8;
            if (bileDebuff)
                modifiers.Defense.Flat -= 15;
            if (infected)
                modifiers.Defense.Flat -= 20;
            if (badtime)
                modifiers.Defense.Flat -= 99;
            if (infected)
                modifiers.FinalDamage *= 1.2f;
            if (infested)
            {
                if (modifiers.Defense.Flat > 0)
                    modifiers.Defense.Flat -= infestedTime / 120;
            }
            if (rallied)
                modifiers.FinalDamage *= 0.85f;
            if (roosterBoost && Main.expertMode)
                modifiers.FinalDamage *= 0.9f;
            if (stoneskin)
                modifiers.FinalDamage *= 0.75f;
            if (brokenArmor)
                modifiers.Defense *= .5f;
            if (sandDust)
                modifiers.Defense *= .75f;
        }
        public override void ModifyHitPlayer(Terraria.NPC npc, Terraria.Player target, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (rallied || roosterBoost)
                modifiers.IncomingDamageMultiplier *= 1.15f;
            if (dragonblaze)
                modifiers.IncomingDamageMultiplier *= .85f;
            if (disarmed)
                modifiers.IncomingDamageMultiplier /= 3;
        }
        public override void ModifyHitNPC(Terraria.NPC npc, Terraria.NPC target, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (rallied || roosterBoost)
                modifiers.FinalDamage *= 1.15f;
            if (dragonblaze)
                modifiers.FinalDamage *= .85f;
            if (disarmed)
                modifiers.FinalDamage /= 3;
        }
        public override void DrawEffects(Terraria.NPC npc, ref Color drawColor)
        {
            if (infected)
                drawColor = Color.Lerp(drawColor, new Color(32, 158, 88), 0.2f);
            if (infested)
                drawColor = Color.Lerp(drawColor, new Color(197, 219, 171), 0.2f);
            if (rallied || roosterBoost)
                drawColor = Color.Lerp(drawColor, new Color(200, 150, 150), 0.2f);
            if (pureChill)
            {
                drawColor = Color.Lerp(drawColor, new Color(180, 220, 220), 0.3f);
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
                if (Main.rand.NextBool(10) && npc.alpha < 200)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<SpiderSwarmerDust>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                }
            }
            if (dragonblaze)
            {
                drawColor = Color.Lerp(drawColor, new Color(220, 150, 150), 0.5f);
                if (Main.rand.NextBool(5) && !Main.gamePaused)
                {
                    ParticleManager.NewParticle(npc.RandAreaInEntity(), RedeHelper.SpreadUp(1), new EmberParticle(), Color.OrangeRed, 1);
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
                if (Main.rand.NextBool(3) && npc.alpha < 200)
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
                if (Main.rand.NextBool(10) && !Main.gamePaused)
                {
                    DustHelper.DrawParticleElectricity<LightningParticle>(new Vector2(npc.position.X, npc.position.Y + Main.rand.Next(0, npc.height)), new Vector2(npc.TopRight.X, npc.TopRight.Y + Main.rand.Next(0, npc.height)), .5f, 10, 0.2f);
                    DustHelper.DrawParticleElectricity<LightningParticle>(new Vector2(npc.TopRight.X, npc.TopRight.Y + Main.rand.Next(0, npc.height)), new Vector2(npc.position.X, npc.position.Y + Main.rand.Next(0, npc.height)), .5f, 10, 0.2f);
                }
            }
            if (stomachAcid)
            {
                drawColor = new Color(52, 178, 108);
                if (Main.rand.NextBool(4))
                    Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.ToxicBubble, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, Alpha: 100);
            }
            if (holyFire)
            {
                if (Main.rand.NextBool(4) && !Main.gamePaused)
                    ParticleManager.NewParticle(npc.RandAreaInEntity(), new Vector2(0, -1), new GlowParticle2(), Color.LightGoldenrodYellow, 1, .45f, Main.rand.Next(50, 60));
            }
            if (soaked)
            {
                drawColor = Color.Lerp(drawColor, new Color(100, 100, 255), 0.5f);
                if (Main.rand.NextBool(3))
                    Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Water, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, Scale: 2);
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
            if (soaked && npc.knockBackResist > 0 && !npc.boss)
            {
                if (npc.noGravity)
                    npc.velocity.Y *= 0.96f;
                if (!npc.noTileCollide)
                    npc.velocity.Y += 0.04f;

                npc.velocity.X *= 0.96f;
            }
        }
        public override void HitEffect(Terraria.NPC npc, Terraria.NPC.HitInfo hit)
        {
            if (npc.life <= 0 && necroticGouge && npc.lifeMax > 5)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath19);
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
            if (iceFrozen && hit.Damage > 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);
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
