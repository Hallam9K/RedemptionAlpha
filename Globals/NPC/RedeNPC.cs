using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Lab;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.PreHM;
using Redemption.NPCs.Wasteland;
using Redemption.Tiles.Tiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Donator.Megaswave;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Buffs;
using Redemption.Items.Armor.Vanity.Dev;
using Redemption.Projectiles.Misc;
using Redemption.Items.Weapons.PreHM.Summon;
using System;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Items.Donator.Lordfunnyman;
using Redemption.Globals.World;
using Redemption.Buffs.Cooldowns;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.WorldGeneration.Misc;
using SubworldLibrary;
using Redemption.Items.Accessories.PreHM;
using Redemption.Base;

namespace Redemption.Globals.NPC
{
    public class RedeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool decapitated;
        public bool invisible;
        public bool fallDownPlatform;
        public bool spiritSummon;
        public Entity attacker = Main.LocalPlayer;
        public Terraria.NPC npcTarget;

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Demolitionist)
                shop.Add<EggBomb>(Condition.PlayerCarriesItem(ModContent.ItemType<GreneggLauncher>()));
            if (shop.NpcType == NPCID.SkeletonMerchant)
                shop.Add<CalciteWand>();
            if (shop.NpcType == NPCID.Clothier)
            {
                shop.Add<ThornPlush>(RedeConditions.DownedThorn);
                shop.Add<EaglecrestGolemPlush>(RedeConditions.DownedEaglecrestGolem);
            }
            if (shop.NpcType == NPCID.Wizard)
                shop.Add<Taikasauva>();
            if (shop.NpcType == NPCID.Princess)
            {
                shop.Add<HamPatPainting>();
                shop.Add<TiedBoiPainting>();
                shop.Add<HallamHoodie>(Condition.InExpertMode, RedeConditions.BroughtCat);
                shop.Add<HallamLeggings>(Condition.InExpertMode, RedeConditions.BroughtCat);
            }
        }
        public override void ResetEffects(Terraria.NPC npc)
        {
            invisible = false;
        }

        public override void ModifyIncomingHit(Terraria.NPC npc, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (spiritSummon)
            {
                modifiers.FinalDamage *= .75f;
                if (Main.expertMode)
                    modifiers.FinalDamage /= Main.masterMode ? 3 : 2;
            }
        }
        public override bool PreKill(Terraria.NPC npc)
        {
            if (spiritSummon)
            {
                bool apply = true;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    Terraria.NPC other = Main.npc[n];
                    if (!other.active || other.life <= 0 || !other.Redemption().spiritSummon)
                        continue;

                    apply = false;
                }
                if (apply)
                    Main.player[(int)npc.ai[3]].AddBuff(ModContent.BuffType<CruxCardCooldown>(), 3600);
            }
            return base.PreKill(npc);
        }
        public override bool CanHitPlayer(Terraria.NPC npc, Terraria.Player target, ref int cooldownSlot)
        {
            if (target.RedemptionPlayerBuff().skeletonFriendly)
            {
                if (NPCLists.SkeletonHumanoid.Contains(npc.type))
                    return false;
            }
            return true;
        }

        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref Terraria.NPC.HitModifiers modifiers)
        {
            // Decapitation
            bool humanoid = NPCLists.SkeletonHumanoid.Contains(npc.type) || NPCLists.Humanoid.Contains(npc.type);
            if (npc.life < npc.lifeMax && npc.life < item.damage * 100 && item.CountsAsClass(DamageClass.Melee) && item.pick == 0 && item.hammer == 0 && !item.noUseGraphic && item.damage > 0 && item.useStyle == ItemUseStyleID.Swing && humanoid)
            {
                if (Main.rand.NextBool(200) && !ItemLists.BluntSwing.Contains(item.type))
                {
                    CombatText.NewText(npc.getRect(), Color.Orange, "Decapitated!");
                    decapitated = true;
                    modifiers.SetInstantKill();
                    modifiers.SetCrit();
                }
                else if (Main.rand.NextBool(80) && (item.axe > 0 || item.Redemption().TechnicallyAxe) && item.type != ModContent.ItemType<BeardedHatchet>())
                {
                    CombatText.NewText(npc.getRect(), Color.Orange, "Decapitated!");
                    decapitated = true;
                    modifiers.SetInstantKill();
                    modifiers.SetCrit();
                }
            }
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (spiritSummon && projectile.hostile && !projectile.Redemption().friendlyHostile)
                modifiers.FinalDamage *= 4;
        }
        public override void OnHitNPC(Terraria.NPC npc, Terraria.NPC target, Terraria.NPC.HitInfo hit)
        {
            target.Redemption().attacker = npc;
        }
        public override void OnHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ItemLists.NoElement.Contains(item.type))
            {
                #region Elemental Attributes
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && npc.life < npc.lifeMax && item.HasElement(ElementID.Ice))
                        npc.AddBuff(ModContent.BuffType<PureChillDebuff>(), 600);
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(8) && npc.life < npc.lifeMax && npc.knockBackResist > 0 && !npc.RedemptionNPCBuff().iceFrozen && item.HasElement(ElementID.Ice))
                    {
                        SoundEngine.PlaySound(SoundID.Item30);
                        npc.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 1780)));
                    }
                }
                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && item.HasElement(ElementID.Fire))
                        npc.AddBuff(BuffID.OnFire, 180);
                }
                if ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && item.HasElement(ElementID.Thunder))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (!npc.noTileCollide && npc.collideY && npc.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && item.HasElement(ElementID.Earth))
                        npc.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && item.HasElement(ElementID.Water))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (item.HasElement(ElementID.Shadow))
                {
                    int c = player.HasBuff<EvilJellyBuff>() ? 3 : 6;
                    if (Main.rand.NextBool(c) && npc.life <= 0 && npc.lifeMax > 5)
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<ShadowFuel>(), noGrabDelay: true);
                }
                if (item.HasElement(ElementID.Nature) && npc.NPCHasAnyDebuff() && !RedeHelper.HasFireDebuff(npc))
                {
                    int c = 6;
                    if (player.RedemptionPlayerBuff().shellNecklace)
                        c = (int)(c * 0.75f);
                    if (player.RedemptionPlayerBuff().forestCore)
                        c = (int)(c * 0.75f);
                    if (Main.rand.NextBool(c) && npc.CanBeChasedBy())
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<NaturePickup>(), noGrabDelay: true);
                }
                if (item.HasElement(ElementID.Celestial))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(6) && npc.CanBeChasedBy())
                        Projectile.NewProjectile(npc.GetSource_OnHurt(player), npc.Center + RedeHelper.Spread(400), Vector2.Zero, ModContent.ProjectileType<CelestialStar>(), 0, 0, player.whoAmI, npc.whoAmI);
                }
                #endregion
            }

            attacker = player;
        }
        public override void OnHitByProjectile(Terraria.NPC npc, Projectile projectile, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (!RedeConfigClient.Instance.ElementDisable && !ProjectileLists.NoElement.Contains(projectile.type))
            {
                #region Elemental Attributes
                if (NPCLists.Infected.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && npc.life < npc.lifeMax && projectile.HasElement(ElementID.Ice))
                        npc.AddBuff(ModContent.BuffType<PureChillDebuff>(), 600);
                }
                if (NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(8) && npc.life < npc.lifeMax && npc.knockBackResist > 0 && !npc.RedemptionNPCBuff().iceFrozen && projectile.HasElement(ElementID.Ice))
                    {
                        SoundEngine.PlaySound(SoundID.Item30);
                        npc.AddBuff(ModContent.BuffType<IceFrozen>(), 1800 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 1780)));
                    }
                }
                if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && projectile.HasElement(ElementID.Fire))
                        npc.AddBuff(BuffID.OnFire, 180);
                }
                if ((npc.wet && !npc.lavaWet) || npc.HasBuff(BuffID.Wet))
                {
                    if (Main.rand.NextBool(2) && projectile.HasElement(ElementID.Thunder))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (!npc.noTileCollide && npc.collideY && npc.knockBackResist > 0)
                {
                    if (Main.rand.NextBool(8) && projectile.HasElement(ElementID.Earth))
                        npc.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                }
                if (NPCLists.Robotic.Contains(npc.type))
                {
                    if (Main.rand.NextBool(4) && projectile.HasElement(ElementID.Water))
                        npc.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 120);
                }
                if (projectile.HasElement(ElementID.Shadow))
                {
                    int c = Main.player[projectile.owner].HasBuff<EvilJellyBuff>() ? 3 : 6;
                    if (Main.rand.NextBool(c) && npc.life <= 0 && npc.lifeMax > 5)
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<ShadowFuel>(), noGrabDelay: true);
                }
                if (projectile.HasElement(ElementID.Nature) && npc.NPCHasAnyDebuff() && !RedeHelper.HasFireDebuff(npc))
                {
                    int c = 6;
                    if (Main.player[projectile.owner].RedemptionPlayerBuff().shellNecklace)
                        c = (int)(c * 0.75f);
                    if (Main.player[projectile.owner].RedemptionPlayerBuff().forestCore)
                        c = (int)(c * 0.75f);
                    if (Main.rand.NextBool(c) && npc.CanBeChasedBy())
                        Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ModContent.ItemType<NaturePickup>(), noGrabDelay: true);
                }
                if (projectile.HasElement(ElementID.Celestial))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(6) && npc.CanBeChasedBy())
                        Projectile.NewProjectile(npc.GetSource_OnHurt(Main.player[projectile.owner]), npc.Center + RedeHelper.Spread(400), Vector2.Zero, ModContent.ProjectileType<CelestialStar>(), 0, 0, projectile.owner, npc.whoAmI);
                }
                #endregion
            }

            if (RedeProjectile.projOwners.TryGetValue(projectile.whoAmI, out (Entity entity, IEntitySource source) value))
            {
                bool g = false;
                if (value.entity is Terraria.NPC valueNPC && valueNPC.whoAmI == npc.whoAmI)
                    g = true;

                if (!g)
                    attacker = value.entity;
            }
        }
        public override void OnKill(Terraria.NPC npc)
        {
            if (NPCID.Sets.Skeletons[npc.type] && Main.rand.NextBool(3) && !npc.SpawnedFromStatue)
                RedeHelper.SpawnNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
        }
        public override void ModifyNPCLoot(Terraria.NPC npc, NPCLoot npcLoot)
        {
            if (NPCLists.SkeletonHumanoid.Contains(npc.type) || NPCLists.Humanoid.Contains(npc.type))
            {
                DecapitationCondition decapitationDropCondition = new();
                IItemDropRule conditionalRule = new LeadingConditionRule(decapitationDropCondition);
                int itemType = ItemID.None;
                if (NPCLists.SkeletonHumanoid.Contains(npc.type))
                    itemType = ItemID.Skull;
                if (npc.type == ModContent.NPCType<CorpseWalkerPriest>())
                    itemType = ModContent.ItemType<CorpseWalkerSkullVanity>();
                else if (npc.type == ModContent.NPCType<EpidotrianSkeleton>() || npc.type == ModContent.NPCType<SkeletonAssassin>() ||
                    npc.type == ModContent.NPCType<SkeletonDuelist>() || npc.type == ModContent.NPCType<SkeletonFlagbearer>() ||
                    npc.type == ModContent.NPCType<SkeletonNoble>() || npc.type == ModContent.NPCType<SkeletonWanderer>() ||
                    npc.type == ModContent.NPCType<SkeletonWarden>())
                    itemType = ModContent.ItemType<EpidotrianSkull>();
                else if (npc.type is NPCID.RockGolem)
                    itemType = ItemID.RockGolemHead;
                else if (npc.type is NPCID.Medusa)
                    itemType = ItemID.MedusaHead;
                else if (npc.type is NPCID.DesertLamiaLight or NPCID.DesertLamiaDark)
                    itemType = ItemID.LamiaHat;
                else if (npc.type is NPCID.Mummy or NPCID.BloodMummy or NPCID.DarkMummy or NPCID.LightMummy)
                    itemType = ItemID.MummyMask;

                if (itemType is not ItemID.None)
                {
                    IItemDropRule rule = ItemDropRule.Common(itemType);
                    conditionalRule.OnSuccess(rule);
                    npcLoot.Add(conditionalRule);
                }
            }
            if (npc.type is NPCID.BoneSerpentHead)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmolderedScale>(), 20));
            if (npc.type is NPCID.Ghost or NPCID.Wraith)
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<Soulshake>(), 150));
            //if (npc.type is NPCID.AngryBones or NPCID.AngryBonesBig or NPCID.AngryBonesBigHelmet or NPCID.AngryBonesBigMuscle or NPCID.CursedSkull or NPCID.DarkCaster)
            //    npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<Incisor>(), 100));
            if (npc.type is NPCID.Demon or NPCID.VoodooDemon or NPCID.FireImp or NPCID.RedDevil)
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<ForgottenSword>(), 100));
            if (npc.type is NPCID.GraniteFlyer or NPCID.GraniteGolem)
            {
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<GaucheStaff>(), 30));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LegoBrick>(), 200));
            }
            if (npc.type is NPCID.Dandelion)
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<GiantDandelion>(), 5));
            if (npc.type is NPCID.Golem)
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GolemStaff>(), 7));
            if (npc.type is NPCID.IceGolem or NPCID.RockGolem)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LegoBrick>(), 50));
            if (NPCLists.Dark.Contains(npc.type) && !npc.boss)
                npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<EldritchRoot>(), 400));
        }
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(ItemDropRule.ByCondition(new YoyosTidalWake(), ModContent.ItemType<TidalWake>(), 200));
        }
        public override void EditSpawnRate(Terraria.Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (maxSpawns <= 0)
                return;
            if (FowlMorningWorld.FowlMorningActive)
            {
                maxSpawns = 8;
                spawnRate = 40;
            }
            if (RedeWorld.blobbleSwarm)
            {
                spawnRate = 10;
                maxSpawns = 20;
                return;
            }
            if (RedeWorld.SkeletonInvasion)
            {
                spawnRate = 19;
                maxSpawns = 12;
            }
            if (player.InModBiome<LabBiome>())
            {
                spawnRate = 20;
                maxSpawns = 12;
            }
            if (player.InModBiome<WastelandPurityBiome>())
            {
                spawnRate = 35;
                maxSpawns = 10;
            }
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if ((spawnInfo.Player.RedemptionScreen().cutscene && !RedeConfigClient.Instance.CameraLockDisable) || SubworldSystem.IsActive<CSub>())
            {
                pool.Clear();
                return;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                Terraria.NPC safe = Main.npc[i];
                if (!safe.active || !NPCLists.DisablesSpawnsWhenNear.Contains(safe.type))
                    continue;
                Vector2 spawnPos = BaseUtility.TileToPos(new Vector2(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY));
                if (safe.DistanceSQ(spawnPos) < 1600 * 1600)
                {
                    pool.Clear();
                    return;
                }
            }
            if (RedeWorld.blobbleSwarm)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<Blobble>(), 10);
                return;
            }

            if (RedeWorld.SkeletonInvasion && spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<RaveyardSkeletonSpawner>(), 2);
                pool.Add(ModContent.NPCType<EpidotrianSkeleton>(), 5);
                pool.Add(ModContent.NPCType<CavernSkeletonSpawner>(), 5);
                pool.Add(ModContent.NPCType<SurfaceSkeletonSpawner>(), 2);
                pool.Add(ModContent.NPCType<CorpseWalkerPriest>(), 0.5f);
                pool.Add(ModContent.NPCType<JollyMadman>(), 0.02f);
            }
            if (FowlMorningWorld.FowlMorningActive && spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
            {
                pool.Clear();
                if (Framing.GetTileSafely(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1).WallType is not WallID.DirtUnsafe)
                {
                    IDictionary<int, float> spawnpool = FowlMorningNPC.SpawnPool.ElementAt(FowlMorningWorld.ChickWave);
                    foreach (KeyValuePair<int, float> key in spawnpool)
                    {
                        pool.Add(key.Key, key.Value);
                    }
                }
                return;
            }
            if (spawnInfo.Player.InModBiome<LabBiome>())
            {
                if (!RedeWorld.labSafe)
                {
                    pool.Clear();
                    pool.Add(ModContent.NPCType<LabSentryDrone>(), 10);
                }
                else
                {
                    int[] LabTileArray = { ModContent.TileType<LabPlatingTileUnsafe>(), ModContent.TileType<DangerTapeTile>(), ModContent.TileType<HardenedSludgeTile>(), ModContent.TileType<BlackHardenedSludgeTile>() };
                    bool tileCheck = LabTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType);

                    pool.Clear();
                    pool.Add(ModContent.NPCType<BlisteredScientist>(), tileCheck ? 1 : 0);
                    pool.Add(ModContent.NPCType<OozingScientist>(), tileCheck ? 0.7f : 0);
                    pool.Add(ModContent.NPCType<BloatedScientist>(), tileCheck ? 0.2f : 0);
                    if (spawnInfo.Water)
                        pool.Add(ModContent.NPCType<BlisteredFish>(), 0.4f);
                }
            }
            if (spawnInfo.Player.InModBiome<WastelandPurityBiome>() && !spawnInfo.Player.ZoneDungeon && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
            {
                int[] GrassTileArray = { ModContent.TileType<IrradiatedCorruptGrassTile>(), ModContent.TileType<IrradiatedCrimsonGrassTile>(), ModContent.TileType<IrradiatedGrassTile>() };
                bool tileCheck = GrassTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType);

                pool.Clear();
                pool.Add(NPCID.ToxicSludge, !Terraria.NPC.downedMechBossAny ? 0.8f : 0.3f);
                pool.Add(NPCID.GreenJellyfish, spawnInfo.Water && !Terraria.NPC.downedMechBossAny ? 0.6f : 0);
                pool.Add(ModContent.NPCType<BloatedGoldfish>(), spawnInfo.Water ? 2f : 0);
                pool.Add(ModContent.NPCType<RadioactiveJelly>(), spawnInfo.Water && Terraria.NPC.downedMechBossAny ? 1f : 0);
                pool.Add(ModContent.NPCType<HazmatZombie>(), 1f);
                pool.Add(ModContent.NPCType<BobTheBlob>(), 0.05f);
                pool.Add(ModContent.NPCType<RadioactiveSlime>(), 0.9f);
                pool.Add(ModContent.NPCType<NuclearSlime>(), 0.07f);
                pool.Add(ModContent.NPCType<HazmatBunny>(), Main.dayTime ? 0.1f : 0);
                pool.Add(ModContent.NPCType<SickenedBunny>(), Main.dayTime ? 0.6f : 0);
                pool.Add(ModContent.NPCType<SickenedDemonEye>(), !Main.dayTime ? 0.6f : 0);
                pool.Add(ModContent.NPCType<NuclearShadow>(), 0.2f);
                pool.Add(ModContent.NPCType<MutatedLivingBloom>(), tileCheck ? (Main.raining ? 0.4f : 0.2f) : 0f);
                if (spawnInfo.Player.InModBiome<WastelandSnowBiome>())
                {
                    pool.Add(ModContent.NPCType<SneezyFlinx>(), 0.8f);
                    pool.Add(ModContent.NPCType<SicklyWolf>(), 0.7f);
                    pool.Add(ModContent.NPCType<SicklyPenguin>(), 0.6f);
                }
                if (spawnInfo.Player.InModBiome<WastelandDesertBiome>())
                {
                    pool.Add(ModContent.NPCType<BloatedGhoul>(), 1f);
                    pool.Add(ModContent.NPCType<BloatedSwarmer>(), 0.3f);
                }
            }
            if (spawnInfo.Player.InModBiome<BlazingBastionBiome>())
            {
                pool.Clear();
                pool.Add(NPCID.Demon, 1f);
                pool.Add(NPCID.FireImp, 0.3f);
                pool.Add(NPCID.VoodooDemon, 0.3f);
                if (Terraria.NPC.downedMechBossAny)
                    pool.Add(NPCID.RedDevil, 0.2f);
                pool.Add(NPCID.HellButterfly, 0.1f);
                pool.Add(NPCID.Lavafly, 0.1f);
                pool.Add(NPCID.MagmaSnail, 0.1f);
                if (Terraria.NPC.downedPlantBoss)
                {
                    pool.Add(NPCID.HellArmoredBones, 0.2f);
                    pool.Add(NPCID.HellArmoredBonesMace, 0.2f);
                    pool.Add(NPCID.HellArmoredBonesSpikeShield, 0.2f);
                    pool.Add(NPCID.HellArmoredBonesSword, 0.2f);
                }
            }
        }
    }
}
