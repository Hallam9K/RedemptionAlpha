using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Buffs;
using Redemption.Buffs.Cooldowns;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Accessories.PreHM;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Critters;
using Redemption.Particles;
using Redemption.Projectiles.Magic;
using Redemption.Projectiles.Melee;
using Redemption.Projectiles.Minions;
using Redemption.Projectiles.Misc;
using Redemption.Projectiles.Ranged;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
        public bool mechSheath;
        public bool shellCap;
        public bool shieldGenerator;
        public int shieldGeneratorLife = 200;
        public int shieldGeneratorCD;
        public float shieldGeneratorAlpha;
        public bool holyFire;
        public int stunTimer;
        public int stunFrame;
        public bool bowString;
        public bool leatherSheath;
        public bool sandDust;
        public bool badtime;
        public bool powerCell;
        public bool sacredCross;
        public bool shellNecklace;
        public bool gracesGuidance;
        public bool forestCore;
        public bool infectionHeart;
        public int infectionHeartTimer;
        public bool crystalKnowledge;
        public bool seaEmblem;
        public bool pureChill;
        public bool hydraCorrosion;
        public bool skirmish;
        public bool wardbreaker;
        public bool erleasFlower;
        public bool spiderFriendly;
        public bool cruxSpiritExtractor;

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

        public float[] ElementalResistance = new float[15];
        public float[] ElementalDamage = new float[15];

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
            mechSheath = false;
            TrueMeleeDamage = 1f;
            shellCap = false;
            ChickenForm = false;
            blastBattery = false;
            xenomiteBonus = false;
            shieldGenerator = false;
            holyFire = false;
            bowString = false;
            leatherSheath = false;
            sandDust = false;
            badtime = false;
            powerCell = false;
            sacredCross = false;
            shellNecklace = false;
            gracesGuidance = false;
            forestCore = false;
            infectionHeart = false;
            crystalKnowledge = false;
            pureChill = false;
            seaEmblem = false;
            skirmish = false;
            hydraCorrosion = false;
            wardbreaker = false;
            erleasFlower = false;
            spiderFriendly = false;
            cruxSpiritExtractor = false;

            Player.RedemptionRad().protectionLevel = 0;

            for (int k = 0; k < ElementalResistance.Length; k++)
                ElementalResistance[k] = 0;
            for (int k = 0; k < ElementalDamage.Length; k++)
                ElementalDamage[k] = 0;
            if (!Player.HasBuff(BuffType<InfestedDebuff>()))
            {
                infested = false;
                infestedTime = 0;
            }
            if (!Player.HasBuff(BuffType<DirtyWoundDebuff>()))
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
            sandDust = false;
            badtime = false;
            infectionHeart = false;
            infectionHeartTimer = 0;
            skirmish = false;
            hydraCorrosion = false;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Redemption.RedeSpecialAbility.JustPressed && Player.active && !Player.dead)
            {
                if (xeniumBonus && !Player.HasBuff(BuffType<XeniumCooldown>()))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.GrenadeLauncher, Player.position);
                    Player.AddBuff(BuffType<XeniumCooldown>(), 20 * 60);
                    Vector2 spawn = new(Player.Center.X, Player.Center.Y - 10);

                    Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, RedeHelper.PolarVector(15, (Main.MouseWorld - Player.Center).ToRotation()), ProjectileType<GasCanister>(), 0, 0, Main.myPlayer);
                }

                if (xenomiteBonus && !Player.HasBuff(BuffType<XenomiteCooldown>()))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Gas1, Player.position);
                    Player.AddBuff(BuffType<XenomiteCooldown>(), 20 * 60);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<XenomiteGas_Proj>(), 100, 0, Main.myPlayer);
                }
                if (hardlightBonus != 0 && !Player.HasBuff(BuffType<HardlightCooldown>()))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Alarm2, Player.position);

                    Player.AddBuff(BuffType<HardlightCooldown>(), 60 * 60);
                    Vector2 spawn = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);
                    switch (hardlightBonus)
                    {
                        case 1: // Ritualist
                            break;
                        case 2: // Magic
                            Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero, ProjectileType<Hardlight_ManaDrone>(), 0, 0, Main.myPlayer);
                            break;
                        case 3: // Melee
                            for (int i = 0; i < 2; i++)
                            {
                                spawn = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);

                                Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero, ProjectileType<MiniSpaceship>(), 50, 1, Main.myPlayer, i);
                            }
                            break;
                        case 4: // Summoner
                            Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero, ProjectileType<Hardlight_Magnet>(), 0, 0, Main.myPlayer);

                            for (int i = 0; i < 2; i++)
                            {
                                spawn = new(Player.Center.X + Main.rand.Next(-200, 201), Player.Center.Y - 800);

                                Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero, ProjectileType<Hardlight_MissileDrone>(), 0, 0, Main.myPlayer);
                            }
                            break;
                        case 5: // Ranger
                            if (Player.whoAmI == Main.myPlayer)
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Main.MouseWorld, Vector2.Zero, ProjectileType<Hardlight_SoSCrosshair>(), 400, 8, Main.myPlayer);
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
                    Player.mount.Dismount(Player);

                Player.wingTimeMax /= 2;
                if (Player.wingTime > Player.wingTimeMax)
                    Player.wingTime = Player.wingTimeMax;
            }
            if (Terraria.NPC.AnyNPCs(NPCType<Nebuleus2>()) || Terraria.NPC.AnyNPCs(NPCType<Nebuleus2_Clone>()))
                Player.wingTime = Player.wingTimeMax;
        }
        public override void PostUpdateEquips()
        {
            if (trappedSoul)
            {
                if (trappedSoulTimer++ >= 600)
                {
                    trappedSoulBoost = 0;
                    Projectile.NewProjectile(Player.GetSource_Accessory(new Item(ItemType<TrappedSoulBauble>())), Player.Center, Vector2.Zero, ProjectileType<SoulShockwave_Proj>(), 0, 0, Main.myPlayer);
                    trappedSoulTimer = 0;
                }
            }
            if (infectionHeart)
            {
                if (infectionHeartTimer++ >= 300)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(new Item(ItemType<HeartOfInfection>())), Player.Center, Vector2.Zero, ProjectileType<InfectionShockwave_Proj>(), 0, 0, Main.myPlayer);
                    infectionHeartTimer = 0;
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
                    Player.ClearBuff(BuffType<GreenRashesDebuff>());
                    infectionTimer = 0;
                }
                if (infectionTimer >= 3600)
                {
                    Player.ClearBuff(BuffType<GreenRashesDebuff>());
                    Player.AddBuff(BuffType<GlowingPustulesDebuff>(), 10000);
                    infectionTimer = 0;
                }
            }
            else if (glowingPustules)
            {
                infectionTimer++;
                if (infectionTimer >= 3600)
                {
                    Player.ClearBuff(BuffType<GlowingPustulesDebuff>());
                    Player.AddBuff(BuffType<FleshCrystalsDebuff>(), 10000);
                    infectionTimer = 0;
                }
            }
            else if (fleshCrystals)
            {
                infectionTimer++;
                if (infectionTimer >= 3600)
                {
                    Player.AddBuff(BuffType<ShockDebuff>(), 10000);
                    infectionTimer = 0;
                }
            }
            else
            {
                if (infectionTimer >= 3000)
                    Player.AddBuff(BuffType<AntibodiesDebuff>(), 18000);
                infectionTimer = 0;
            }

            if (shockDebuff)
            {
                Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.3f).UseIntensity(1f)
                    .UseColor(Color.DarkOliveGreen).UseImage(Request<Texture2D>("Redemption/Effects/Perlin", AssetRequestMode.ImmediateLoad).Value);
                Player.ManageSpecialBiomeVisuals("MoR:FogOverlay", shockDebuff);
            }
            #endregion

            if (Player.HasBuff<StunnedDebuff>())
            {
                if (stunTimer++ > 4)
                {
                    stunTimer = 0;
                    if (stunFrame++ >= 3)
                        stunFrame = 0;
                }
            }
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (thornCirclet && item.CountsAsClass(DamageClass.Magic))
            {
                if (++thornCircletCounter >= 5)
                {
                    for (int i = 0; i < Main.rand.Next(2, 6); i++)
                    {
                        Projectile.NewProjectile(source, position, RedeHelper.PolarVector(Main.rand.NextFloat(7, 13), (Main.MouseWorld - Player.Center).ToRotation() + Main.rand.NextFloat(-0.2f, 0.2f)), ProjectileType<StingerFriendly>(), damage, knockback, Main.myPlayer);
                    }
                    thornCircletCounter = 0;
                }
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
        public override void OnHitByNPC(Terraria.NPC npc, Terraria.Player.HurtInfo hurtInfo)
        {
            if (vendetta)
                npc.AddBuff(BuffID.Poisoned, 300);
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                #region Elemental Resistances
                float multiplier = 1;
                if (proj.HasElement(ElementID.Arcane))
                    multiplier *= 1 - ElementalResistance[ElementID.Arcane];
                if (proj.HasElement(ElementID.Fire))
                    multiplier *= 1 - ElementalResistance[ElementID.Fire];
                if (proj.HasElement(ElementID.Water))
                    multiplier *= 1 - ElementalResistance[ElementID.Water];
                if (proj.HasElement(ElementID.Ice))
                    multiplier *= 1 - ElementalResistance[ElementID.Ice];
                if (proj.HasElement(ElementID.Earth))
                    multiplier *= 1 - (ElementalResistance[ElementID.Earth]);
                if (proj.HasElement(ElementID.Wind))
                    multiplier *= 1 - ElementalResistance[ElementID.Wind];
                if (proj.HasElement(ElementID.Thunder))
                    multiplier *= 1 - ElementalResistance[ElementID.Thunder];
                if (proj.HasElement(ElementID.Holy))
                    multiplier *= 1 - ElementalResistance[ElementID.Holy];
                if (proj.HasElement(ElementID.Shadow))
                    multiplier *= 1 - ElementalResistance[ElementID.Shadow];
                if (proj.HasElement(ElementID.Nature))
                    multiplier *= 1 - ElementalResistance[ElementID.Nature];
                if (proj.HasElement(ElementID.Poison))
                    multiplier *= 1 - ElementalResistance[ElementID.Poison];
                if (proj.HasElement(ElementID.Blood))
                    multiplier *= 1 - ElementalResistance[ElementID.Blood];
                if (proj.HasElement(ElementID.Psychic))
                    multiplier *= 1 - ElementalResistance[ElementID.Psychic];
                if (proj.HasElement(ElementID.Celestial))
                    multiplier *= 1 - ElementalResistance[ElementID.Celestial];

                ElementalNPC.ElementalEffects(Player, proj, ref multiplier, ref modifiers);

                if (multiplier == 1)
                    return;

                multiplier = (int)Math.Round(multiplier * 100);
                multiplier /= 100;

                if (multiplier >= 1.1f)
                    CombatText.NewText(Player.getRect(), Color.IndianRed, multiplier + "x", true, true);
                else if (multiplier <= 0.9f)
                    CombatText.NewText(Player.getRect(), Color.CornflowerBlue, multiplier + "x", true, true);

                modifiers.FinalDamage *= multiplier;
                #endregion
            }
            if (shellCap && proj.velocity.Y > 1 && proj.Bottom.Y < Player.Center.Y)
            {
                SoundEngine.PlaySound(SoundID.NPCHit38, Player.position);
                Player.noKnockback = true;
                modifiers.FinalDamage *= 0.75f;
            }
        }
        public override void ModifyHitByNPC(Terraria.NPC npc, ref Terraria.Player.HurtModifiers modifiers)
        {
            if (!RedeConfigServer.Instance.ElementDisable)
            {
                #region Elemental Resistances
                float multiplier = 1;
                if (npc.HasElement(ElementID.Arcane))
                    multiplier *= 1 - ElementalResistance[ElementID.Arcane];
                if (npc.HasElement(ElementID.Fire))
                    multiplier *= 1 - ElementalResistance[ElementID.Fire];
                if (npc.HasElement(ElementID.Water))
                    multiplier *= 1 - ElementalResistance[ElementID.Water];
                if (npc.HasElement(ElementID.Ice))
                    multiplier *= 1 - ElementalResistance[ElementID.Ice];
                if (npc.HasElement(ElementID.Earth))
                    multiplier *= 1 - ElementalResistance[ElementID.Earth];
                if (npc.HasElement(ElementID.Wind))
                    multiplier *= 1 - ElementalResistance[ElementID.Wind];
                if (npc.HasElement(ElementID.Thunder))
                    multiplier *= 1 - ElementalResistance[ElementID.Thunder];
                if (npc.HasElement(ElementID.Holy))
                    multiplier *= 1 - ElementalResistance[ElementID.Holy];
                if (npc.HasElement(ElementID.Shadow) || (npc.netID is NPCID.BlackSlime or NPCID.BabySlime or NPCID.Slimeling))
                    multiplier *= 1 - ElementalResistance[ElementID.Shadow];
                if (npc.HasElement(ElementID.Nature) || (npc.netID is NPCID.JungleSlime))
                    multiplier *= 1 - ElementalResistance[ElementID.Nature];
                if (npc.HasElement(ElementID.Poison))
                    multiplier *= 1 - ElementalResistance[ElementID.Poison];
                if (npc.HasElement(ElementID.Blood))
                    multiplier *= 1 - ElementalResistance[ElementID.Blood];
                if (npc.HasElement(ElementID.Psychic))
                    multiplier *= 1 - ElementalResistance[ElementID.Psychic];
                if (npc.HasElement(ElementID.Celestial))
                    multiplier *= 1 - ElementalResistance[ElementID.Celestial];

                ElementalNPC.ElementalEffects(Player, npc, ref multiplier, ref modifiers);

                if (multiplier == 1)
                    return;

                multiplier = (int)Math.Round(multiplier * 100);
                multiplier /= 100;

                if (multiplier >= 1.1f)
                    CombatText.NewText(Player.getRect(), Color.IndianRed, multiplier + "x", true, true);
                else if (multiplier <= 0.9f)
                    CombatText.NewText(Player.getRect(), Color.CornflowerBlue, multiplier + "x", true, true);

                modifiers.FinalDamage *= multiplier;
                #endregion
            }
            if (shellCap && npc.velocity.Y > 1 && npc.Bottom.Y < Player.Center.Y)
            {
                SoundEngine.PlaySound(SoundID.NPCHit38, Player.position);
                Player.noKnockback = true;
                modifiers.FinalDamage *= 0.75f;
            }
        }
        public override void ModifyHitNPCWithItem(Item item, Terraria.NPC target, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (leatherSheath && target.life >= target.lifeMax && target.type != NPCID.TargetDummy)
                modifiers.SetCrit();

            modifiers.FinalDamage *= TrueMeleeDamage;
            if (item.axe > 0 || item.Redemption().TechnicallyAxe)
                modifiers.CritDamage += 1f;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, Terraria.NPC target, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (proj.Redemption().TechnicallyMelee)
            {
                if (leatherSheath && target.life >= target.lifeMax && target.type != NPCID.TargetDummy)
                    modifiers.SetCrit();

                modifiers.FinalDamage *= TrueMeleeDamage;
            }
            if (proj.Redemption().IsAxe)
                modifiers.CritDamage += 1f;
        }
        public override void OnHitNPCWithProj(Projectile proj, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (RedeProjectile.projOwners.TryGetValue(proj.whoAmI, out (Entity entity, IEntitySource source) value) && value.entity is Terraria.NPC)
                return;

            if (charisma)
                target.AddBuff(BuffID.Midas, 300);
            if (pureIronBonus && Main.rand.NextBool(3))
                target.AddBuff(BuffType<PureChillDebuff>(), 300);
            if (dragonLeadBonus && Main.rand.NextBool(3))
                target.AddBuff(BuffType<DragonblazeDebuff>(), 300);
            if (mechSheath && proj.Redemption().TechnicallyMelee && Player.ownedProjectileCounts[ModContent.ProjectileType<PhantomCleaver_F2>()] == 0 && RedeHelper.Chance(0.1f))
            {
                if (proj.DamageType == DamageClass.Melee || proj.DamageType == DamageClass.SummonMeleeSpeed)
                    Projectile.NewProjectile(proj.GetSource_FromAI(), new Vector2(target.Center.X, target.position.Y - 200), Vector2.Zero, ProjectileType<PhantomCleaver_F2>(), proj.damage * 3, proj.knockBack, Main.myPlayer, target.whoAmI);
            }
            if ((sacredCross || gracesGuidance) && hit.Crit && Main.rand.NextBool(2) && proj.type != ProjectileType<Lightmass>() && proj.HasElement(ElementID.Holy))
            {
                SoundEngine.PlaySound(SoundID.Item101, Player.Center);
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                    Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), ProjectileType<Lightmass>(), 15, proj.knockBack / 4, Main.myPlayer, 0, 1);
            }
            if (crystalKnowledge && Main.rand.NextBool(4) && proj.type != ProjectileType<ElementalCrystal>() && proj.GetFirstElement(true) != 0 && Player.ownedProjectileCounts[ProjectileType<ElementalCrystal>()] < 6)
            {
                Player.AddBuff(BuffType<CrystalKnowledgeBuff>(), 10);
                Projectile.NewProjectile(proj.GetSource_FromAI(), Player.Center, Vector2.Zero, ProjectileType<ElementalCrystal>(), 40, 1, Main.myPlayer, 0, proj.GetFirstElement(true));
            }
            if (seaEmblem && Main.rand.NextBool(3) && proj.HasElement(ElementID.Water))
                target.AddBuff(BuffType<SoakedDebuff>(), 600);
        }
        public override void OnHitNPCWithItem(Item item, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (charisma)
                target.AddBuff(BuffID.Midas, 300);
            if (pureIronBonus && Main.rand.NextBool(3))
                target.AddBuff(BuffType<PureChillDebuff>(), 300);
            if (dragonLeadBonus && Main.rand.NextBool(3))
                target.AddBuff(BuffType<DragonblazeDebuff>(), 300);
            if (mechSheath && Player.ownedProjectileCounts[ProjectileType<PhantomCleaver_F2>()] == 0 && RedeHelper.Chance(0.1f))
            {
                if (item.DamageType == DamageClass.Melee || item.DamageType == DamageClass.SummonMeleeSpeed)
                    Projectile.NewProjectile(Player.GetSource_ItemUse(item), new Vector2(target.Center.X, target.position.Y - 200), Vector2.Zero, ProjectileType<PhantomCleaver_F2>(), item.damage * 3, item.knockBack, Main.myPlayer, target.whoAmI);
            }
            if ((sacredCross || gracesGuidance) && hit.Crit && Main.rand.NextBool(2) && item.HasElement(ElementID.Holy))
            {
                SoundEngine.PlaySound(SoundID.Item101, Player.Center);
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                    Projectile.NewProjectile(Player.GetSource_ItemUse(item), target.Center, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), ProjectileType<Lightmass>(), 15, item.knockBack / 4, Main.myPlayer);
            }
            if (crystalKnowledge && Main.rand.NextBool(4) && item.GetFirstElement(true) != 0 && Player.ownedProjectileCounts[ProjectileType<ElementalCrystal>()] < 6)
            {
                Player.AddBuff(BuffType<CrystalKnowledgeBuff>(), 10);
                Projectile.NewProjectile(Player.GetSource_ItemUse(item), Player.Center, Vector2.Zero, ProjectileType<ElementalCrystal>(), 40, 1, Main.myPlayer, 0, item.GetFirstElement(true));
            }
            if (seaEmblem && Main.rand.NextBool(3) && item.HasElement(ElementID.Water))
                target.AddBuff(BuffType<SoakedDebuff>(), 600);
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
                    Player.ClearBuff(BuffType<DirtyWoundDebuff>());
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
                Player.lifeRegen -= 10;
                Player.statDefense -= 15;
            }
            if ((Player.InModBiome<WastelandPurityBiome>() || Player.InModBiome<LabBiome>()) && Player.wet && !Player.lavaWet && !Player.honeyWet && !Player.RedemptionPlayerBuff().WastelandWaterImmune)
            {
                if (Player.lifeRegen > 10)
                    Player.lifeRegen = 10;

                Player.lifeRegenTime = 0;
                int DoT = 60;
                Player.lifeRegen -= DoT;
            }
            if (holyFire)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 500;
            }
            if (sandDust)
                Player.statDefense -= 8;
            if (badtime)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 1000;
                Player.statDefense -= 99;
            }
            if (pureChill)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 8;
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
                    int dust = Dust.NewDust(drawInfo.Position, Player.width, Player.height, DustType<SpiderSwarmerDust>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                    drawInfo.DustCache.Add(dust);
                }
            }
            if (Player.HasBuff(BuffType<StaticStunDebuff>()))
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
            if (Player.GetModPlayer<ObliterationDashPlayer>().DashTimer > 0 && drawInfo.shadow == 0f)
            {
                r = 1f;
                g = 0.5f;
                b = 0.5f;
            }
            if (holyFire)
            {
                if (Main.rand.NextBool(4) && !Main.gamePaused && drawInfo.shadow == 0f)
                    ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(Player), new Vector2(0, -1), new GlowParticle2(), Color.LightGoldenrodYellow, 1, .45f, Main.rand.Next(50, 60));
            }
            if (pureChill)
            {
                r = MathHelper.Lerp(r, .7f, 0.3f);
                g = MathHelper.Lerp(g, .85f, 0.3f);
                b = MathHelper.Lerp(b, .85f, 0.3f);
                if (Main.rand.NextBool(14))
                {
                    int sparkle = Dust.NewDust(drawInfo.Position, Player.width, Player.height, DustType<SnowflakeDust>(), newColor: Color.White);
                    Main.dust[sparkle].velocity *= 0.5f;
                    Main.dust[sparkle].noGravity = true;
                }
            }
        }
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (hairLoss)
                PlayerDrawLayers.HairBack.Hide();
        }
        public override void ModifyHurt(ref Terraria.Player.HurtModifiers modifiers)
        {
            if (BasePlayer.HasAccessory(Player, ItemType<EggShield>(), true, false))
                modifiers.Knockback *= 0.75f;

            if (shieldGenerator && shieldGeneratorCD <= 0)
            {
                modifiers.ScalingArmorPenetration += .5f;
                modifiers.ModifyHurtInfo += ModifyDamage;
            }
        }
        public void ModifyDamage(ref Terraria.Player.HurtInfo info)
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
            if (info.Damage >= shieldGeneratorLife)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath56, Player.position);
                shieldGeneratorAlpha = 0;
                shieldGenerator = false;
                shieldGeneratorCD = 3600;
                info.Damage -= shieldGeneratorLife;
                info.Damage *= 2;
                shieldGeneratorLife = 200;
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
                return;
            }
            info.SoundDisabled = true;
            SoundEngine.PlaySound(SoundID.NPCHit34, Player.position);
            shieldGeneratorLife -= info.Damage;
            info.Knockback = 0;
            info.Damage = 1;
            Player.statLife++;
        }
        public override void PostHurt(Terraria.Player.HurtInfo info)
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
                    damageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Infested", Player.name));

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
                        Projectile.NewProjectile(Player.GetSource_Buff(Player.FindBuffIndex(BuffType<InfestedDebuff>())), Player.Center, RedeHelper.SpreadUp(8), ProjectileType<GrandLarvaFall>(), 0, 0, Main.myPlayer);
                }
            }
            if (dirtyWound && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.DirtyWound", Player.name));

            if (spiderSwarmed && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Swarmed", Player.name));

            if ((fleshCrystals || shockDebuff) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Xenomite", Player.name));

            if (Player.FindBuffIndex(BuffType<RadiationDebuff>()) != -1 && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Radiation", Player.name));

            if (Player.FindBuffIndex(BuffType<HolyFireDebuff>()) != -1 && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Incandesence", Player.name));

            if (Player.FindBuffIndex(BuffType<EnsnaredDebuff>()) != -1 && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
                damageSource = PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Ensnared", Player.name));

            return true;
        }
    }
}
