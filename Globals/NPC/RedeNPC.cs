using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Buffs.Cooldowns;
using Redemption.Globals.World;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Armor.Vanity.Dev;
using Redemption.Items.Critters;
using Redemption.Items.Donator.BLT;
using Redemption.Items.Donator.Lordfunnyman;
using Redemption.Items.Donator.Megaswave;
using Redemption.Items.Donator.Spoopy;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Furniture.Misc;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Friendly.TownNPCs;
using Redemption.NPCs.HM;
using Redemption.NPCs.Lab;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.PreHM;
using Redemption.NPCs.Wasteland;
using Redemption.Tiles.Tiles;
using Redemption.UI.Dialect;
using Redemption.WorldGeneration.Misc;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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

        public bool HideAllButtons;

        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                case NPCID.Demolitionist:
                    shop.Add<EggBomb>(Condition.PlayerCarriesItem(ItemType<GreneggLauncher>()));
                    break;
                case NPCID.SkeletonMerchant:
                    shop.Add<CalciteWand>();
                    break;
                case NPCID.Clothier:
                    shop.Add<ThornPlush>(RedeConditions.DownedThorn);
                    shop.Add<EaglecrestGolemPlush>(RedeConditions.DownedEaglecrestGolem);
                    break;
                case NPCID.Wizard:
                    shop.Add<Taikasauva>();
                    break;
                case NPCID.Princess:
                    shop.Add<HamPatPainting>();
                    shop.Add<TiedBoiPainting>();
                    shop.Add<HallamHoodie>(Condition.InExpertMode, RedeConditions.BroughtCat);
                    shop.Add<HallamLeggings>(Condition.InExpertMode, RedeConditions.BroughtCat);
                    break;
                case NPCID.Mechanic:
                    shop.Add<SpringtrapHead>();
                    shop.Add<SpringtrapBody>();
                    shop.Add<SpringtrapLegs>();
                    break;
                case NPCID.BestiaryGirl:
                    shop.Add<HairyCloak>();
                    break;
                case NPCID.WitchDoctor:
                    shop.Add<ErleasFlower>();
                    break;
                case NPCID.Merchant:
                    shop.Add<DAVEPainting>(Condition.PlayerCarriesItem(ItemType<JohnSnailItem>()));
                    break;
            }
        }
        private static bool TalkedDryad;
        public override void GetChat(Terraria.NPC npc, ref string chat)
        {
            if (npc.type == NPCID.Dryad && !TalkedDryad && RedeBossDowned.downedKeeper)
            {
                RedeQuest.adviceUnlocked[(int)RedeQuest.Advice.ForestNymph] = true;
                if (RedeQuest.forestNymphVar > 0)
                    RedeQuest.adviceSeen[(int)RedeQuest.Advice.ForestNymph] = true;
                RedeQuest.SyncData();

                TalkedDryad = true;
            }
            RedeGlobalButton.talkActive = false;
            RedeGlobalButton.talkID = 0;
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
                if (apply && npc.ai[3] >= 0)
                    Main.player[(int)npc.ai[3]].AddBuff(BuffType<CruxCardCooldown>(), 3600);
            }
            int FallenID = Terraria.NPC.FindFirstNPC(NPCType<Fallen>());
            if (npc.type is NPCID.EaterofWorldsHead or NPCID.BrainofCthulhu && !Terraria.NPC.downedBoss2)
            {
                RedeQuest.adviceUnlocked[(int)RedeQuest.Advice.EaglecrestGolem] = true;
                if (RedeBossDowned.downedEaglecrestGolem)
                    RedeQuest.adviceSeen[(int)RedeQuest.Advice.EaglecrestGolem] = true;
                RedeQuest.SyncData();
            }
            else if (npc.type is NPCID.WallofFlesh && !Main.hardMode)
            {
                RedeQuest.adviceUnlocked[(int)RedeQuest.Advice.Androids] = true;
                if (RedeBossDowned.downedSlayer)
                    RedeQuest.adviceSeen[(int)RedeQuest.Advice.Androids] = true;
                RedeQuest.SyncData();
            }
            else if (npc.type is NPCID.MoonLordCore && !Terraria.NPC.downedMoonlord)
            {
                RedeQuest.adviceUnlocked[(int)RedeQuest.Advice.UkkoEye] = true;
                if (FallenID >= 0 && !RedeBossDowned.downedADD)
                    Main.npc[FallenID].GetGlobalNPC<ExclaimMarkNPC>().exclaimationMark[4] = true;
                if (RedeBossDowned.downedADD)
                    RedeQuest.adviceSeen[(int)RedeQuest.Advice.UkkoEye] = true;
                RedeQuest.SyncData();
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
            if (npc.type is NPCID.Bee or NPCID.BeeSmall)
                return !target.RedemptionPlayerBuff().erleasFlower;

            return true;
        }

        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref Terraria.NPC.HitModifiers modifiers)
        {
            // Decapitation
            bool humanoid = NPCLists.SkeletonHumanoid.Contains(npc.type) || NPCLists.Humanoid.Contains(npc.type);
            if (npc.life < npc.lifeMax && npc.life < item.damage * 100 && item.CountsAsClass(DamageClass.Melee) && !item.noUseGraphic && item.damage > 0 && item.useStyle == ItemUseStyleID.Swing && humanoid)
            {
                if (Main.rand.NextBool(200) && !ItemLists.BluntSwing.Contains(item.type) && item.pick == 0 && item.hammer == 0)
                    RedeProjectile.DecapitationEffect(npc, ref modifiers);
                else if (Main.rand.NextBool(80) && (item.axe > 0 || item.Redemption().TechnicallyAxe))
                    RedeProjectile.DecapitationEffect(npc, ref modifiers);
            }
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref Terraria.NPC.HitModifiers modifiers)
        {
            if (spiritSummon && projectile.hostile && !projectile.Redemption().friendlyHostile)
                modifiers.FinalDamage *= NPCHelper.HostileProjDamageMultiplier();
        }
        public override void OnHitNPC(Terraria.NPC npc, Terraria.NPC target, Terraria.NPC.HitInfo hit)
        {
            target.Redemption().attacker = npc;
        }
        public override void OnHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, Terraria.NPC.HitInfo hit, int damageDone)
        {
            attacker = player;
        }
        public override void OnHitByProjectile(Terraria.NPC npc, Projectile projectile, Terraria.NPC.HitInfo hit, int damageDone)
        {
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
                RedeHelper.SpawnNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCType<LostSoulNPC>(), Main.rand.NextFloat(0, 0.4f));
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
                if (npc.type == NPCType<CorpseWalkerPriest>())
                    itemType = ItemType<CorpseWalkerSkullVanity>();
                else if (npc.type == NPCType<EpidotrianSkeleton>() || npc.type == NPCType<SkeletonAssassin>() ||
                    npc.type == NPCType<SkeletonDuelist>() || npc.type == NPCType<SkeletonFlagbearer>() ||
                    npc.type == NPCType<SkeletonNoble>() || npc.type == NPCType<SkeletonWanderer>() ||
                    npc.type == NPCType<SkeletonWarden>() || npc.type == NPCType<RaveyardSkeleton>() || npc.type == NPCType<MoonflareSkeleton>())
                    itemType = ItemType<EpidotrianSkull>();
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
                npcLoot.Add(ItemDropRule.Common(ItemType<SmolderedScale>(), 20));
            if (npc.type is NPCID.Ghost or NPCID.Wraith)
                npcLoot.Add(ItemDropRule.Food(ItemType<Soulshake>(), 150));
            if (npc.type is NPCID.AngryBones or NPCID.AngryBonesBig or NPCID.AngryBonesBigHelmet or NPCID.AngryBonesBigMuscle or NPCID.CursedSkull or NPCID.DarkCaster)
                npcLoot.Add(ItemDropRule.Common(ItemType<Incisor>(), 100));
            if (npc.type is NPCID.Demon or NPCID.VoodooDemon or NPCID.FireImp or NPCID.RedDevil)
                npcLoot.Add(ItemDropRule.Common(ItemType<ForgottenSword>(), 100));
            if (npc.type is NPCID.GraniteFlyer or NPCID.GraniteGolem)
            {
                npcLoot.Add(ItemDropRule.Common(ItemType<GaucheStaff>(), 30));
                npcLoot.Add(ItemDropRule.Common(ItemType<LegoBrick>(), 200));
            }
            if (npc.type is NPCID.Dandelion)
                npcLoot.Add(ItemDropRule.Common(ItemType<GiantDandelion>(), 5));
            if (npc.type is NPCID.Golem)
            {
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ItemType<GolemStaff>(), 7));
            }
            if (npc.type is NPCID.IceGolem or NPCID.RockGolem)
                npcLoot.Add(ItemDropRule.Common(ItemType<LegoBrick>(), 50));
            if (npc.type is NPCID.Pumpking or NPCID.HoppinJack or NPCID.HeadlessHorseman)
                npcLoot.Add(ItemDropRule.Common(ItemType<DuskyBall>(), 50));
            if (NPCLists.Dark.Contains(npc.type) && !npc.boss)
                npcLoot.Add(ItemDropRule.Common(ItemType<EldritchRoot>(), 400));
            if (npc.type is NPCID.WallCreeper or NPCID.WallCreeperWall or NPCID.BlackRecluse or NPCID.BlackRecluseWall)
                npcLoot.Add(ItemDropRule.Common(ItemType<SpiderSerum>(), 40));
        }
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(ItemDropRule.ByCondition(new YoyosTidalWake(), ItemType<TidalWake>(), 200));
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
            if (RedeWorld.SkeletonInvasion && player.ZoneOverworldHeight && !player.ZoneTowerNebula && !player.ZoneTowerSolar && !player.ZoneTowerStardust && !player.ZoneTowerVortex)
            {
                spawnRate = 19;
                maxSpawns = 12;
            }
            if (player.InModBiome<LabBiome>())
            {
                spawnRate /= 20;
                maxSpawns = (int)(maxSpawns * 1.5f);
            }
            if (player.InModBiome<WastelandPurityBiome>())
            {
                spawnRate /= 15;
                maxSpawns = (int)(maxSpawns * 1.3f);
            }
            if (player.RedemptionPlayerBuff().skirmish)
            {
                spawnRate = (int)(spawnRate * .25f);
                maxSpawns = (int)(maxSpawns * 3f);
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
                pool.Add(NPCType<Blobble>(), 10);
                return;
            }
            if (RedeWorld.SkeletonInvasion && spawnInfo.Player.ZoneOverworldHeight && !spawnInfo.Player.ZoneTowerNebula && !spawnInfo.Player.ZoneTowerSolar && !spawnInfo.Player.ZoneTowerStardust && !spawnInfo.Player.ZoneTowerVortex)
            {
                pool.Clear();
                pool.Add(NPCType<RaveyardSkeletonSpawner>(), 2);
                pool.Add(NPCType<EpidotrianSkeleton>(), 5);
                pool.Add(NPCType<CavernSkeletonSpawner>(), 5);
                pool.Add(NPCType<SurfaceSkeletonSpawner>(), 2);
                pool.Add(NPCType<CorpseWalkerPriest>(), 0.5f);
                pool.Add(NPCType<MoonflareSkeleton>(), 0.5f);
                pool.Add(NPCType<JollyMadman>(), 0.025f);
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
                int[] GrassTileArray = { TileType<IrradiatedCorruptGrassTile>(), TileType<IrradiatedCrimsonGrassTile>(), TileType<IrradiatedGrassTile>() };
                bool tileCheck = GrassTileArray.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType);

                pool.Clear();
                pool.Add(NPCID.ToxicSludge, !Terraria.NPC.downedMechBossAny ? 0.8f : 0.3f);
                pool.Add(NPCID.GreenJellyfish, spawnInfo.Water && !Terraria.NPC.downedMechBossAny ? 0.6f : 0);
                pool.Add(NPCType<BloatedGoldfish>(), spawnInfo.Water ? 2f : 0);
                pool.Add(NPCType<RadioactiveJelly>(), spawnInfo.Water && Terraria.NPC.downedMechBossAny ? 1f : 0);
                pool.Add(NPCType<HazmatZombie>(), 1f);
                pool.Add(NPCType<BobTheBlob>(), 0.05f);
                pool.Add(NPCType<RadioactiveSlime>(), 0.9f);
                pool.Add(NPCType<NuclearSlime>(), 0.07f);
                pool.Add(NPCType<HazmatBunny>(), Main.dayTime ? 0.1f : 0);
                pool.Add(NPCType<SickenedBunny>(), Main.dayTime ? 0.6f : 0);
                pool.Add(NPCType<SickenedDemonEye>(), !Main.dayTime ? 0.6f : 0);
                pool.Add(NPCType<NuclearShadow>(), 0.2f);
                if (!spawnInfo.PlayerSafe)
                    pool.Add(NPCType<BloatedDiggerHead>(), 0.14f);

                pool.Add(NPCType<MutatedLivingBloom>(), tileCheck ? (Main.raining ? 0.4f : 0.2f) : 0f);
                if (spawnInfo.Player.InModBiome<WastelandSnowBiome>())
                {
                    pool.Add(NPCType<SneezyFlinx>(), 0.8f);
                    pool.Add(NPCType<SicklyWolf>(), 0.7f);
                    pool.Add(NPCType<SicklyPenguin>(), 0.6f);
                }
                if (spawnInfo.Player.InModBiome<WastelandDesertBiome>())
                {
                    pool.Add(NPCType<BloatedGhoul>(), 1f);
                    pool.Add(NPCType<BloatedSwarmer>(), 0.3f);
                }
                if (spawnInfo.Player.InModBiome<WastelandCorruptionBiome>())
                    pool.Add(NPCType<BloatedClinger>(), .4f);
                if (spawnInfo.Player.InModBiome<WastelandCrimsonBiome>())
                    pool.Add(NPCType<BloatedFaceMonster>(), 1f);
            }
            if (spawnInfo.Player.InModBiome<BlazingBastionBiome>())
            {
                pool.Clear();
                if (!spawnInfo.PlayerSafe)
                    pool.Add(NPCID.FireImp, 0.3f);

                pool.Add(NPCID.Demon, 1f);
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
