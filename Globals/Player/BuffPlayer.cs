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
using Redemption.DamageClasses;
using Redemption.Buffs;
using Redemption.Biomes;
using Terraria.GameInput;
using Redemption.Buffs.Cooldowns;
using Redemption.Projectiles.Magic;
using Redemption.Projectiles.Melee;
using Redemption.Projectiles.Minions;
using Redemption.Projectiles.Ranged;

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
        public bool antiXenomiteBuff;
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

        public bool pureIronBonus;
        public bool dragonLeadBonus;
        public int hardlightBonus;

        public bool MetalSet;
        public bool WastelandWaterImmune;

        public int MeleeDamageFlat;
        public int DruidDamageFlat;

        public float[] ElementalResistance = new float[14];
        public float[] ElementalDamage = new float[14];

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage, ref float flat)
        {
            if (item.CountsAsClass(DamageClass.Melee))
                flat += MeleeDamageFlat;
            if (item.CountsAsClass<DruidClass>())
                flat += DruidDamageFlat;
        }

        public override void ResetEffects()
        {
            devilScented = false;
            charisma = false;
            vendetta = false;
            thornCirclet = false;
            skeletonFriendly = false;
            heartInsignia = false;
            MeleeDamageFlat = 0;
            DruidDamageFlat = 0;
            MetalSet = false;
            spiderSwarmed = false;
            greenRashes = false;
            glowingPustules = false;
            fleshCrystals = false;
            hemorrhageDebuff = false;
            necrosisDebuff = false;
            shockDebuff = false;
            antibodiesBuff = false;
            antiXenomiteBuff = false;
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
            snipped = false;

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
            antiXenomiteBuff = false;
            ensnared = false;
            hairLoss = false;
            bileDebuff = false;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Redemption.RedeSpecialAbility.JustPressed)
            {
                if (hardlightBonus != 0 && !Player.HasBuff(ModContent.BuffType<HardlightCooldown>()))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Alarm2"), Player.position);

                    Player.AddBuff(ModContent.BuffType<HardlightCooldown>(), 60 * 60);
                    Vector2 spawn = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);
                    switch (hardlightBonus)
                    {
                        case 1: // Ritualist

                            break;
                        case 2: // Magic
                            Projectile.NewProjectile(Player.GetProjectileSource_SetBonus(hardlightBonus), spawn, Vector2.Zero, ModContent.ProjectileType<Hardlight_ManaDrone>(), 0, 0, Main.myPlayer);
                            break;
                        case 3: // Melee
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 spawn2 = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);

                                Projectile.NewProjectile(Player.GetProjectileSource_SetBonus(hardlightBonus), spawn2, Vector2.Zero, ModContent.ProjectileType<MiniSpaceship>(), 50, 1, Main.myPlayer, i);
                            }
                            break;
                        case 4: // Summoner
                            Projectile.NewProjectile(Player.GetProjectileSource_SetBonus(hardlightBonus), spawn, Vector2.Zero, ModContent.ProjectileType<Hardlight_Magnet>(), 0, 0, Main.myPlayer);

                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 spawn2 = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);

                                Projectile.NewProjectile(Player.GetProjectileSource_SetBonus(hardlightBonus), spawn2, Vector2.Zero, ModContent.ProjectileType<Hardlight_MissileDrone>(), 0, 0, Main.myPlayer);
                            }
                            break;
                        case 5: // Druid

                            break;
                        case 6: // Ranger
                            if (Player.whoAmI == Main.myPlayer)
                                Projectile.NewProjectile(Player.GetProjectileSource_SetBonus(hardlightBonus), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Hardlight_SoSCrosshair>(), 400, 8, Main.myPlayer);
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
                infectionTimer = 0;

            if (shockDebuff)
            {
                Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.3f).UseIntensity(1f)
                    .UseColor(Color.DarkOliveGreen).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Perlin", AssetRequestMode.ImmediateLoad).Value);
                Player.ManageSpecialBiomeVisuals("MoR:FogOverlay", shockDebuff);
            }
            #endregion
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (hairLoss)
                PlayerDrawLayers.HairBack.Hide();
        }

        public override bool Shoot(Item item, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
                if (ProjectileTags.Arcane.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[0]));
                if (ProjectileTags.Fire.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[1]));
                if (ProjectileTags.Water.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[2]));
                if (ProjectileTags.Ice.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[3]));
                if (ProjectileTags.Earth.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[4]));
                if (ProjectileTags.Wind.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[5]));
                if (ProjectileTags.Thunder.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[6]));
                if (ProjectileTags.Holy.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[7]));
                if (ProjectileTags.Shadow.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[8]));
                if (ProjectileTags.Nature.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[9]));
                if (ProjectileTags.Poison.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[10]));
                if (ProjectileTags.Blood.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[11]));
                if (ProjectileTags.Psychic.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[12]));
                if (ProjectileTags.Celestial.Has(proj.type))
                    damage = (int)(damage * (1 - ElementalResistance[13]));
                #endregion
            }
        }
        public override void ModifyHitNPC(Item item, Terraria.NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                #region Elemental Damage
                if (ItemTags.Arcane.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[0]));
                if (ItemTags.Fire.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[1]));
                if (ItemTags.Water.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[2]));
                if (ItemTags.Ice.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[3]));
                if (ItemTags.Earth.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[4]));
                if (ItemTags.Wind.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[5]));
                if (ItemTags.Thunder.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[6]));
                if (ItemTags.Holy.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[7]));
                if (ItemTags.Shadow.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[8]));
                if (ItemTags.Nature.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[9]));
                if (ItemTags.Poison.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[10]));
                if (ItemTags.Blood.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[11]));
                if (ItemTags.Psychic.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[12]));
                if (ItemTags.Celestial.Has(item.type))
                    damage = (int)(damage * (1 + ElementalDamage[13]));
                #endregion
            }
        }
        public override void ModifyHitNPCWithProj(Projectile proj, Terraria.NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!RedeConfigClient.Instance.ElementDisable)
            {
                #region Elemental Damage
                if (ProjectileTags.Arcane.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[0]));
                if (ProjectileTags.Fire.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[1]));
                if (ProjectileTags.Water.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[2]));
                if (ProjectileTags.Ice.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[3]));
                if (ProjectileTags.Earth.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[4]));
                if (ProjectileTags.Wind.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[5]));
                if (ProjectileTags.Thunder.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[6]));
                if (ProjectileTags.Holy.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[7]));
                if (ProjectileTags.Shadow.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[8]));
                if (ProjectileTags.Nature.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[9]));
                if (ProjectileTags.Poison.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[10]));
                if (ProjectileTags.Blood.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[11]));
                if (ProjectileTags.Psychic.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[12]));
                if (ProjectileTags.Celestial.Has(proj.type))
                    damage = (int)(damage * (1 + ElementalDamage[13]));
                #endregion
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            if (charisma)
                target.AddBuff(BuffID.Midas, 300);
            if (pureIronBonus && Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
            if (dragonLeadBonus && Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
            if (eldritchRoot && target.life <= 0)
                Player.AddBuff(ModContent.BuffType<EldritchRootBuff>(), 180);
        }
        public override void OnHitNPC(Item item, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            if (charisma)
                target.AddBuff(BuffID.Midas, 300);
            if (pureIronBonus && Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
            if (dragonLeadBonus && Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
            if (eldritchRoot && target.life <= 0)
                Player.AddBuff(ModContent.BuffType<EldritchRootBuff>(), 180);
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
            if ((Player.InModBiome(ModContent.GetInstance<WastelandPurityBiome>()) || Player.InModBiome(ModContent.GetInstance<LabBiome>())) && Player.wet && !Player.lavaWet && !Player.honeyWet && !Player.GetModPlayer<BuffPlayer>().WastelandWaterImmune)
            {
                if (Player.lifeRegen > 10)
                    Player.lifeRegen = 10;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 60;
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
        }

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (MetalSet)
                SoundEngine.PlaySound(SoundID.NPCHit4, Player.position);
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
                        Projectile.NewProjectile(Player.GetProjectileSource_Buff(Player.FindBuffIndex(ModContent.BuffType<InfestedDebuff>())), Player.Center, RedeHelper.SpreadUp(8), ModContent.ProjectileType<GrandLarvaFall>(), 0, 0, Main.myPlayer);
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

            return true;
        }
    }
}