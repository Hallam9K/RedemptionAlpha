using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.NPCs.Critters;
using Redemption.Projectiles.Misc;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Buffs;
using Redemption.Biomes;
using Terraria.GameInput;
using Redemption.Buffs.Cooldowns;
using Redemption.Projectiles.Magic;
using Redemption.Projectiles.Melee;
using Redemption.Projectiles.Minions;
using Redemption.Projectiles.Ranged;
using Redemption.BaseExtension;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Accessories.PostML;
using System;

namespace Redemption.Globals.Player
{
    public class BuffPlayer : ModPlayer
    {
        public bool infested;
        public bool devilScented;
        public int infestedTime;
        public bool charisma;
        public bool vendetta;
        public bool thornCirclet;
        public int thornCircletCounter;
        public bool skeletonFriendly;
        public bool dirtyWound;
        public int dirtyWoundTime;
        public bool heartInsignia;
        public bool wellFed4;
        public bool spiderSwarmed;
        public bool greenRashes;
        public bool glowingPustules;
        public bool fleshCrystals;
        public bool hemorrhageDebuff;
        public bool necrosisDebuff;
        public bool shockDebuff;
        public bool antibodiesBuff;
        public int infectionTimer;
        public bool eldritchRoot;
        public bool ensnared;
        public bool lantardPet;
        public bool erhanCross;
        public bool hairLoss;
        public bool bileDebuff;
        public bool hazmatSuit;
        public bool HEVSuit;
        public bool snipped;
        public bool island;
        public bool trappedSoul;
        public int trappedSoulTimer;
        public float trappedSoulBoost;
        public bool brokenBlade;
        public bool shellCap;
        public bool shieldGenerator;
        public int shieldGeneratorLife = 400;
        public int shieldGeneratorCD;
        public float shieldGeneratorAlpha;
        public bool holyFire;

        public bool pureIronBonus;
        public bool dragonLeadBonus;
        public bool xeniumBonus;
        public int hardlightBonus;
        public bool shinkiteHead;
        public bool vortiHead;
        public bool hikariteHead;
        public bool blastBattery;
        public bool xenomiteBonus;

        public bool MetalSet;
        public bool WastelandWaterImmune;
        public bool ChickenForm;

        public float TrueMeleeDamage = 1f;

        public float[] ElementalResistance = new float[14];
        public float[] ElementalDamage = new float[14];

        public override void ResetEffects()
        {
            devilScented = false;
            charisma = false;
            vendetta = false;
            thornCirclet = false;
            skeletonFriendly = false;
            heartInsignia = false;
            MetalSet = false;
            spiderSwarmed = false;
            greenRashes = false;
            glowingPustules = false;
            fleshCrystals = false;
            hemorrhageDebuff = false;
            necrosisDebuff = false;
            shockDebuff = false;
            antibodiesBuff = false;
            pureIronBonus = false;
            dragonLeadBonus = false;
            eldritchRoot = false;
            ensnared = false;
            lantardPet = false;
            erhanCross = false;
            hairLoss = false;
            bileDebuff = false;
            hazmatSuit = false;
            HEVSuit = false;
            WastelandWaterImmune = false;
            hardlightBonus = 0;
            xeniumBonus = false;
            snipped = false;
            island = false;
            trappedSoul = false;
            shinkiteHead = false;
            vortiHead = false;
            hikariteHead = false;
            brokenBlade = false;
            TrueMeleeDamage = 1f;
            shellCap = false;
            ChickenForm = false;
            blastBattery = false;
            xenomiteBonus = false;
            shieldGenerator = false;
            holyFire = false;

            for (int k = 0; k < ElementalResistance.Length; k++)
            {
                ElementalResistance[k] = 0;
            }
            for (int k = 0; k < ElementalDamage.Length; k++)
            {
                ElementalDamage[k] = 0;
            }
            if (!Player.HasBuff(ModContent.BuffType<InfestedDebuff>()))
            {
                infested = false;
                infestedTime = 0;
            }
            if (!Player.HasBuff(ModContent.BuffType<DirtyWoundDebuff>()))
            {
                dirtyWound = false;
                dirtyWoundTime = 0;
            }
        }

        public override void UpdateDead()
        {
            devilScented = false;
            spiderSwarmed = false;
            greenRashes = false;
            glowingPustules = false;
            fleshCrystals = false;
            hemorrhageDebuff = false;
            necrosisDebuff = false;
            shockDebuff = false;
            antibodiesBuff = false;
            ensnared = false;
            hairLoss = false;
            bileDebuff = false;
            trappedSoulTimer = 0;
            trappedSoulBoost = 0;
            shieldGenerator = false;
            shieldGeneratorAlpha = 0;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Redemption.RedeSpecialAbility.JustPressed && Player.active && !Player.dead)
            {
                if (xeniumBonus && !Player.HasBuff(ModContent.BuffType<XeniumCooldown>()))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.GrenadeLauncher, Player.position);
                    Player.AddBuff(ModContent.BuffType<XeniumCooldown>(), 20 * 60);
                    Vector2 spawn = new(Player.Center.X, Player.Center.Y - 10);

                    Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, RedeHelper.PolarVector(15, (Main.MouseWorld - Player.Center).ToRotation()), ModContent.ProjectileType<GasCanister>(), 0, 0, Main.myPlayer);
                }
                if (xenomiteBonus && !Player.HasBuff(ModContent.BuffType<XenomiteCooldown>()))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Gas1, Player.position);
                    Player.AddBuff(ModContent.BuffType<XenomiteCooldown>(), 20 * 60);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<XenomiteGas_Proj>(), 100, 0, Main.myPlayer);
                }

                if (hardlightBonus != 0 && !Player.HasBuff(ModContent.BuffType<HardlightCooldown>()))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Alarm2, Player.position);

                    Player.AddBuff(ModContent.BuffType<HardlightCooldown>(), 60 * 60);
                    Vector2 spawn = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);
                    switch (hardlightBonus)
                    {
                        case 1: // Ritualist

                            break;
                        case 2: // Magic
                            Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<Hardlight_ManaDrone>(), 0, 0, Main.myPlayer);
                            break;
                        case 3: // Melee
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 spawn2 = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);

                                Projectile.NewProjectile(Player.GetSource_FromThis(), spawn2, Vector2.Zero, ModContent.ProjectileType<MiniSpaceship>(), 50, 1, Main.myPlayer, i);
                            }
                            break;
                        case 4: // Summoner
                            Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<Hardlight_Magnet>(), 0, 0, Main.myPlayer);

                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 spawn2 = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);

                                Projectile.NewProjectile(Player.GetSource_FromThis(), spawn2, Vector2.Zero, ModContent.ProjectileType<Hardlight_MissileDrone>(), 0, 0, Main.myPlayer);
                            }
                            break;
                        case 5: // Ranger
                            if (Player.whoAmI == Main.myPlayer)
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Hardlight_SoSCrosshair>(), 400, 8, Main.myPlayer);
                            break;

                    }
                }
            }
        }

        public override void UpdateEquips()
        {
            if (snipped)
            {
                if (Player.mount.CanFly())
                {
                    Player.mount.Dismount(Player);
                }

                Player.wingTimeMax /= 2;
                if (Player.wingTime > Player.wingTimeMax)
                    Player.wingTime = Player.wingTimeMax;
            }
        }
        public override void PostUpdateEquips()
        {
            if (trappedSoul)
            {
                if (trappedSoulTimer++ >= 600)
                {
                    trappedSoulBoost = 0;
                    Projectile.NewProjectile(Player.GetSource_Accessory(new Item(ModContent.ItemType<TrappedSoulBauble>())), Player.Center, Vector2.Zero, ModContent.ProjectileType<SoulShockwave_Proj>(), 0, 0, Main.myPlayer);
                    trappedSoulTimer = 0;
                }
            }
        }
        public override void PostUpdateBuffs()
        {
            #region Infection
            if (greenRashes)
            {
                infectionTimer++;
                if (antibodiesBuff)
                {
                    Player.ClearBuff(ModContent.BuffType<GreenRashesDebuff>());
                    infectionTimer = 0;
                }
                if (infectionTimer >= 3600)
                {
                    Player.ClearBuff(ModContent.BuffType<GreenRashesDebuff>());
                    Player.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 10000);
                    infectionTimer = 0;
                }
            }
            else if (glowingPustules)
            {
                infectionTimer++;
                if (infectionTimer >= 3600)
                {
                    Player.ClearBuff(ModContent.BuffType<GlowingPustulesDebuff>());
                    Player.AddBuff(ModContent.BuffType<FleshCrystalsDebuff>(), 10000);
                    infectionTimer = 0;
                }
            }
            else if (fleshCrystals)
            {
                infectionTimer++;
                if (infectionTimer >= 3600)
                {
                    Player.AddBuff(ModContent.BuffType<ShockDebuff>(), 10000);
                    infectionTimer = 0;
                }
            }
            else
            {
                if (infectionTimer >= 3000)
                    Player.AddBuff(ModContent.BuffType<AntibodiesDebuff>(), 18000);
                infectionTimer = 0;
            }

            if (shockDebuff)
            {
                Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.3f).UseIntensity(1f)
                    .UseColor(Color.DarkOliveGreen).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Perlin", AssetRequestMode.ImmediateLoad).Value);
                Player.ManageSpecialBiomeVisuals("MoR:FogOverlay", shockDebuff);
            }
            #endregion
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (thornCirclet && item.CountsAsClass(DamageClass.Magic))
            {
                if (++thornCircletCounter >= 5)
                {
                    for (int i = 0; i < Main.rand.Next(2, 6); i++)
                    {
                        Projectile.NewProjectile(source, position, RedeHelper.PolarVector(Main.rand.NextFloat(7, 13), (Main.MouseWorld - Player.Center).ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<StingerFriendly>(), damage, knockback, Main.myPlayer);
                    }
                    thornCircletCounter = 0;
                }
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
        public override void OnHitByNPC(Terraria.NPC npc, int damage, bool crit)
        {
            if (vendetta)
                npc.AddBuff(BuffID.Poisoned, 300);
        }
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                #region Elemental Resistances
                if (ProjectileLists.Arcane.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[0]));
                if (ProjectileLists.Fire.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[1]));
                if (ProjectileLists.Water.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[2]));
                if (ProjectileLists.Ice.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[3]));
                if (ProjectileLists.Earth.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[4]));
                if (ProjectileLists.Wind.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[5]));
                if (ProjectileLists.Thunder.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[6]));
                if (ProjectileLists.Holy.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[7]));
                if (ProjectileLists.Shadow.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[8]));
                if (ProjectileLists.Nature.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[9]));
                if (ProjectileLists.Poison.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[10]));
                if (ProjectileLists.Blood.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[11]));
                if (ProjectileLists.Psychic.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[12]));
                if (ProjectileLists.Celestial.Contains(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[13]));
                #endregion
            }
            if (shellCap && proj.velocity.Y > 1 && proj.Bottom.Y < Player.Center.Y)
            {
                SoundEngine.PlaySound(SoundID.NPCHit38, Player.position);
                Player.noKnockback = true;
                damage = (int)(damage * 0.75f);
            }
        }
        public override void ModifyHitByNPC(Terraria.NPC npc, ref int damage, ref bool crit)
        {
            if (shellCap && npc.velocity.Y > 1 && npc.Bottom.Y < Player.Center.Y)
            {
                SoundEngine.PlaySound(SoundID.NPCHit38, Player.position);
                Player.noKnockback = true;
                damage = (int)(damage * 0.75f);
            }
        }
        public override void ModifyHitNPC(Item item, Terraria.NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                #region Elemental Damage
                if (ItemLists.Arcane.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[0]));
                if (ItemLists.Fire.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[1]));
                if (ItemLists.Water.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[2]));
                if (ItemLists.Ice.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[3]));
                if (ItemLists.Earth.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[4]));
                if (ItemLists.Wind.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[5]));
                if (ItemLists.Thunder.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[6]));
                if (ItemLists.Holy.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[7]));
                if (ItemLists.Shadow.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[8]));
                if (ItemLists.Nature.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[9]));
                if (ItemLists.Poison.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[10]));
                if (ItemLists.Blood.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[11]));
                if (ItemLists.Psychic.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[12]));
                if (ItemLists.Celestial.Contains(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[13]));
                #endregion
            }
            if (Player.HasBuff(ModContent.BuffType<BileFlaskBuff>()))
                target.AddBuff(ModContent.BuffType<BileDebuff>(), 900);
            damage = (int)(damage * TrueMeleeDamage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, Terraria.NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                #region Elemental Damage
                if (ProjectileLists.Arcane.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[0]));
                if (ProjectileLists.Fire.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[1]));
                if (ProjectileLists.Water.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[2]));
                if (ProjectileLists.Ice.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[3]));
                if (ProjectileLists.Earth.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[4]));
                if (ProjectileLists.Wind.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[5]));
                if (ProjectileLists.Thunder.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[6]));
                if (ProjectileLists.Holy.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[7]));
                if (ProjectileLists.Shadow.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[8]));
                if (ProjectileLists.Nature.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[9]));
                if (ProjectileLists.Poison.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[10]));
                if (ProjectileLists.Blood.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[11]));
                if (ProjectileLists.Psychic.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[12]));
                if (ProjectileLists.Celestial.Contains(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[13]));
                #endregion
            }
            if (proj.Redemption().TechnicallyMelee)
            {
                if (Player.HasBuff(ModContent.BuffType<BileFlaskBuff>()))
                    target.AddBuff(ModContent.BuffType<BileDebuff>(), 900);

                damage = (int)(damage * TrueMeleeDamage);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            if (RedeProjectile.projOwners.TryGetValue(proj.whoAmI, out (Entity entity, IEntitySource source) value) && value.entity is Terraria.NPC)
                return;

            if (charisma)
                target.AddBuff(BuffID.Midas, 300);
            if (pureIronBonus && Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
            if (dragonLeadBonus && Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
            if (brokenBlade && proj.Redemption().TechnicallyMelee && Player.ownedProjectileCounts[ModContent.ProjectileType<PhantomCleaver_F2>()] == 0 && RedeHelper.Chance(0.1f))
            {
                Projectile.NewProjectile(proj.GetSource_FromAI(), new Vector2(target.Center.X, target.position.Y - 200), Vector2.Zero, ModContent.ProjectileType<PhantomCleaver_F2>(), proj.damage * 3, proj.knockBack, Main.myPlayer, target.whoAmI);
            }
        }
        public override void OnHitNPC(Item item, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            if (charisma)
                target.AddBuff(BuffID.Midas, 300);
            if (pureIronBonus && Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
            if (dragonLeadBonus && Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
            if (brokenBlade && Player.ownedProjectileCounts[ModContent.ProjectileType<PhantomCleaver_F2>()] == 0 && RedeHelper.Chance(0.1f))
            {
                Projectile.NewProjectile(Player.GetSource_ItemUse(item), new Vector2(target.Center.X, target.position.Y - 200), Vector2.Zero, ModContent.ProjectileType<PhantomCleaver_F2>(), item.damage * 3, item.knockBack, Main.myPlayer, target.whoAmI);
            }
        }
        public override void UpdateBadLifeRegen()
        {
            if (infested)
            {
                infestedTime++;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= infestedTime / 120;
                if (Player.statDefense > 0)
                    Player.statDefense -= infestedTime / 120;
                if (infestedTime > 120)
                    Player.moveSpeed *= 0.8f;
            }
            if (dirtyWound)
            {
                dirtyWoundTime++;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= dirtyWoundTime / 500;

                if (Player.wet && !Player.lavaWet)
                    Player.ClearBuff(ModContent.BuffType<DirtyWoundDebuff>());
            }
            if (spiderSwarmed)
                Player.lifeRegen -= 4;
            if (ensnared)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= (int)(Player.velocity.Length() * 20);
            }
            if (bileDebuff)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 5;
                Player.statDefense -= 30;
            }
            if ((Player.InModBiome<WastelandPurityBiome>() || Player.InModBiome<LabBiome>()) && Player.wet && !Player.lavaWet && !Player.honeyWet && !Player.RedemptionPlayerBuff().WastelandWaterImmune)
            {
                if (Player.lifeRegen > 10)
                    Player.lifeRegen = 10;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 60;
            }
            if (holyFire)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 500;
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (infested)
            {
                r = 0.5f;
                g = 1;
                b = 0.3f;
            }
            if (glowingPustules || fleshCrystals || shockDebuff)
            {
                r = 0.3f;
                g = 0.8f;
                b = 0.3f;
            }
            if (spiderSwarmed)
            {
                if (Main.rand.NextBool(10) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position, Player.width, Player.height, ModContent.DustType<SpiderSwarmerDust>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                    drawInfo.DustCache.Add(dust);
                }
            }
            if (Player.HasBuff(ModContent.BuffType<StaticStunDebuff>()))
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position, Player.width, Player.height, DustID.Electric, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.8f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
            }
            if (bileDebuff)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, DustID.GreenFairy, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
            }
            if (Player.GetModPlayer<ObliterationDashPlayer>().DashTimer > 0)
            {
                r = 1f;
                g = 0.5f;
                b = 0.5f;
            }
            if (holyFire)
            {
                if (Main.rand.NextBool(2)&& drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, DustID.YellowTorch, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
            }
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (shieldGenerator && shieldGeneratorCD <= 0)
            {
                for (int k = 0; k < 30; k++)
                {
                    Vector2 vector;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector.X = (float)(Math.Sin(angle) * 60);
                    vector.Y = (float)(Math.Cos(angle) * 60);
                    Dust dust2 = Main.dust[Dust.NewDust(Player.Center + vector, 2, 2, DustID.Frost)];
                    dust2.noGravity = true;
                    dust2.velocity = -Player.DirectionTo(dust2.position) * 6f;
                }
                if (damage >= shieldGeneratorLife)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath56, Player.position);
                    shieldGeneratorAlpha = 0;
                    shieldGenerator = false;
                    shieldGeneratorCD = 3600;
                    damage *= 3;
                    damage -= shieldGeneratorLife;
                    shieldGeneratorLife = 400;
                    for (int k = 0; k < 30; k++)
                    {
                        Vector2 vector;
                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                        vector.X = (float)(Math.Sin(angle) * 60);
                        vector.Y = (float)(Math.Cos(angle) * 60);
                        Dust dust2 = Main.dust[Dust.NewDust(Player.Center + vector, 2, 2, DustID.Frost, Scale: 2)];
                        dust2.noGravity = true;
                        dust2.velocity = Player.DirectionTo(dust2.position) * 3f;
                    }
                    return true;
                }
                playSound = false;
                SoundEngine.PlaySound(SoundID.NPCHit34, Player.position);
                shieldGeneratorLife -= damage;
                Player.noKnockback = true;
                damage = 0;
                return true;
            }
            return true;
        }
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            if (!shieldGenerator || shieldGeneratorCD > 0)
            {
                if (MetalSet)
                    SoundEngine.PlaySound(SoundID.NPCHit4, Player.position);
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (infested && infestedTime >= 60)
            {
                if (damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " burst into larva!");

                SoundEngine.PlaySound(SoundID.NPCDeath19, Player.position);
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex4 = Dust.NewDust(new Vector2(Player.position.X, Player.position.Y), Player.width, Player.height, DustID.GreenBlood, Scale: 3f);
                    Main.dust[dustIndex4].velocity *= 5f;
                }
                int larvaCount = infestedTime / 180 + 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < MathHelper.Clamp(larvaCount, 1, 8); i++)
                        Projectile.NewProjectile(Player.GetSource_Buff(Player.FindBuffIndex(ModContent.BuffType<InfestedDebuff>())), Player.Center, RedeHelper.SpreadUp(8), ModContent.ProjectileType<GrandLarvaFall>(), 0, 0, Main.myPlayer);
                }
            }
            if (dirtyWound && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " had an infection");

            if (spiderSwarmed && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " got nibbled to death");

            if ((fleshCrystals || shockDebuff) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was turned into a crystal");

            if (Player.FindBuffIndex(ModContent.BuffType<RadiationDebuff>()) != -1 && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was irradiated");

            if (Player.FindBuffIndex(ModContent.BuffType<HolyFireDebuff>()) != -1 && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was too glorious");

            return true;
        }
    }
}
