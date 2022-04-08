using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Redemption.Items;
using Terraria.ID;
using Redemption.NPCs.Bosses.Thorn;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.NPCs.Minibosses.SkullDigger;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Items.Lore;
using Redemption.Items.Placeable.MusicBoxes;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Materials.HM;
using Redemption.Items.Accessories.HM;
using Redemption.NPCs.Lab;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Items.Weapons.HM.Ranged;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.Items.Donator.Gonk;
using Redemption.Items.Donator.Uncon;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.NPCs.Bosses.PatientZero;
using Redemption.Items.Tools.PostML;
using Redemption.NPCs.Bosses.Erhan;

namespace Redemption.CrossMod
{
    internal class WeakReferences
    {
        public static void PerformModSupport()
        {
            PerformBossChecklistSupport();
        }
        private static void PerformBossChecklistSupport()
        {
            Redemption mod = Redemption.Instance;

            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
            {
                #region Thorn
                bossChecklist.Call("AddBoss", 1.5f, ModContent.NPCType<Thorn>(), mod,
                    "Thorn, Bane of the Forest",
                    (Func<bool>)(() => RedeBossDowned.downedThorn),
                    ModContent.ItemType<HeartOfThorns>(),
                    new List<int>
                    {
                        ModContent.ItemType<ThornRelic>(),
                        ModContent.ItemType<BouquetOfThorns>(),
                        ModContent.ItemType<ThornTrophy>(),
                        ModContent.ItemType<ThornMask>(),
                        ModContent.ItemType<ForestBossBox>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<ThornBag>(),
                        ModContent.ItemType<CircletOfBrambles>(),
                        ModContent.ItemType<CursedGrassBlade>(),
                        ModContent.ItemType<CursedThornBow>(),
                        ModContent.ItemType<RootTendril>(),
                    },
                    "Use a [i:" + ModContent.ItemType<HeartOfThorns>() + "] at day. Can be found on the surface near spawn.",
                    null,
                    "Redemption/CrossMod/BossChecklist/Thorn",
                    "Redemption/NPCs/Bosses/Thorn/Thorn_Head_Boss");
                #endregion

                #region Erhan
                bossChecklist.Call("AddBoss", 1.9f, ModContent.NPCType<Erhan>(), mod,
                    "Erhan, Anglonic High Priest",
                    (Func<bool>)(() => RedeBossDowned.downedErhan),
                    ModContent.ItemType<DemonScroll>(),
                    new List<int>
                    {
                        ModContent.ItemType<ErhanRelic>(),
                        ModContent.ItemType<DevilsAdvocate>(),
                        ModContent.ItemType<ErhanTrophy>(),
                        ModContent.ItemType<ErhanHelmet>(),
                    },
                    new List<int>
                    {
                        ModContent.ItemType<ErhanBag>(),
                        ModContent.ItemType<ErhanCross>(),
                        ModContent.ItemType<Bindeklinge>(),
                        ModContent.ItemType<HolyBible>(),
                        ModContent.ItemType<HallowedHandGrenade>(),
                        ModContent.ItemType<ErhanMagnifyingGlass>(),
                    },
                    "Use a [i:" + ModContent.ItemType<DemonScroll>() + "] at day. Can be found at the surface portal.",
                    null,
                    "Redemption/CrossMod/BossChecklist/Erhan",
                    "Redemption/NPCs/Bosses/Erhan/Erhan_Head_Boss");
                #endregion

                #region The Keeper
                bossChecklist.Call("AddBoss", 2.4f, ModContent.NPCType<Keeper>(), mod,
                    "The Keeper",
                    (Func<bool>)(() => RedeBossDowned.downedKeeper),
                    new List<int>
                    {
                        ModContent.ItemType<WeddingRing>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<KeeperRelic>(),
                        ModContent.ItemType<OcciesCollar>(),
                        ModContent.ItemType<KeeperTrophy>(),
                        ModContent.ItemType<KeepersVeil>(),
                        ModContent.ItemType<KeeperBox>(),
                        ModContent.ItemType<KeepersCirclet>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<KeeperBag>(),
                        ModContent.ItemType<HeartInsignia>(),
                        ModContent.ItemType<FanOShivs>(),
                        ModContent.ItemType<SoulScepter>(),
                        ModContent.ItemType<KeepersClaw>(),
                        ModContent.ItemType<GrimShard>()
                    },
                    "Use a [i:" + ModContent.ItemType<WeddingRing>() + "] at night.",
                    null,
                    "Redemption/CrossMod/BossChecklist/Keeper",
                    "Redemption/NPCs/Bosses/Keeper/Keeper_Head_Boss");
                #endregion

                #region Skull Digger
                bossChecklist.Call("AddMiniBoss", 2.41f, ModContent.NPCType<SkullDigger>(), mod,
                    "Skull Digger",
                    (Func<bool>)(() => RedeBossDowned.downedSkullDigger),
                    null,
                    ModContent.ItemType<SkullDiggerMask>(),
                    new List<int>
                    {
                        ModContent.ItemType<AbandonedTeddy>(),
                        ModContent.ItemType<LostSoul>(),
                        ModContent.ItemType<GraveSteelShards>(),
                        ModContent.ItemType<SkullDiggerFlail>()
                    },
                    "Roams the caverns, seeking revenge...",
                    null,
                    "Redemption/CrossMod/BossChecklist/SkullDigger",
                    "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Head_Boss",
                    (Func<bool>)(() => RedeBossDowned.downedSkullDigger));
                #endregion

                #region Seed of Infection
                bossChecklist.Call("AddBoss", 3.48f, ModContent.NPCType<SoI>(), mod,
                    "Seed of Infection",
                    (Func<bool>)(() => RedeBossDowned.downedSeed),
                    ModContent.ItemType<AnomalyDetector>(),
                    new List<int>
                    {
                        ModContent.ItemType<SoIRelic>(),
                        ModContent.ItemType<CuddlyTeratoma>(),
                        ModContent.ItemType<SoITrophy>(),
                        ModContent.ItemType<InfectedMask>(),
                        ModContent.ItemType<SoIBox>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<SoIBag>(),
                        ModContent.ItemType<XenomiteGlaive>(),
                        ModContent.ItemType<CystlingSummon>(),
                        ModContent.ItemType<XenomiteShard>()
                    },
                    "Use an [i:" + ModContent.ItemType<AnomalyDetector>() + "]. Begins the Infection Storyline.",
                    null,
                    "Redemption/CrossMod/BossChecklist/SeedOfInfection",
                    "Redemption/NPCs/Bosses/SeedOfInfection/SoI_Head_Boss");
                #endregion

                #region Eaglecrest Golem
                bossChecklist.Call("AddMiniBoss", 4.1f, ModContent.NPCType<EaglecrestGolem>(), mod,
                    "Eaglecrest Golem",
                    (Func<bool>)(() => RedeBossDowned.downedEaglecrestGolem),
                    ModContent.ItemType<EaglecrestSpelltome>(),
                    new List<int>
                    {
                        ModContent.ItemType<ForestBossBox>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<GolemEye>(),
                        ModContent.ItemType<EaglecrestJavelin>(),
                        ModContent.ItemType<EaglecrestSling>(),
                        ModContent.ItemType<GathicStone>()
                    },
                    "Naturally spawns at day after Eater of Worlds/Brain of Cthulhu is defeated.",
                    null,
                    "Redemption/CrossMod/BossChecklist/EaglecrestGolem",
                    "Redemption/NPCs/Bosses/EaglecrestGolem/EaglecrestGolem_Head_Boss");
                #endregion

                #region The Abandoned Lab
                bossChecklist.Call("AddEvent", 9.1f,
                    new List<int>()
                    {
                        ModContent.NPCType<OozeBlob>(),
                        ModContent.NPCType<BlisteredScientist>(),
                        ModContent.NPCType<OozingScientist>(),
                        ModContent.NPCType<BloatedScientist>(),
                        ModContent.NPCType<JanitorBot>(),
                        ModContent.NPCType<IrradiatedBehemoth>()
                    },
                    mod,
                    "The Abandoned Laboratory",
                    (Func<bool>)(() => RedeBossDowned.downedBehemoth),
                    ModContent.ItemType<IOLocator>(),
                    new List<int>
                    {
                        ModContent.ItemType<HazmatSuit2>(),
                        ModContent.ItemType<FloppyDisk1>(),
                        ModContent.ItemType<FloppyDisk2>(),
                        ModContent.ItemType<FloppyDisk2_1>(),
                        ModContent.ItemType<FloppyDisk3>(),
                        ModContent.ItemType<FloppyDisk3_1>(),
                        ModContent.ItemType<LabMusicBox>(),
                        ModContent.ItemType<LabBossMusicBox>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<XenomiteShard>(),
                        ModContent.ItemType<RadiationPill>(),
                        ModContent.ItemType<FirstAidKit>(),
                        ItemID.AdhesiveBandage,
                        ItemID.Vitamins,
                        ModContent.ItemType<ScrapMetal>(),
                        ModContent.ItemType<AIChip>(),
                        ModContent.ItemType<Capacitator>(),
                        ModContent.ItemType<Plating>(),
                        ModContent.ItemType<CrystalSerum>(),
                        ModContent.ItemType<CarbonMyofibre>(),
                        ModContent.ItemType<GasMask>(),
                        ModContent.ItemType<Holoshield>(),
                        ModContent.ItemType<PrototypeAtomRifle>(),
                        ModContent.ItemType<MiniWarhead>(),
                        ModContent.ItemType<GravityHammer>(),
                        ModContent.ItemType<TeslaGenerator>(),
                        ModContent.ItemType<LightningRod>()
                    },
                    "Find the Abandoned Lab far below the surface, defeat the first 2 minibosses within. Requires all mech bosses to be defeated.",
                    null,
                    "Redemption/CrossMod/BossChecklist/Lab",
                    null);
                #endregion

                #region King Slayer III
                bossChecklist.Call("AddBoss", 9.99999f, ModContent.NPCType<KS3>(), mod,
                    "King Slayer III",
                    (Func<bool>)(() => RedeBossDowned.downedSlayer),
                    ModContent.ItemType<CyberTech>(),
                    new List<int>
                    {
                        ModContent.ItemType<KS3Relic>(),
                        ModContent.ItemType<SlayerProjector>(),
                        ModContent.ItemType<KS3Trophy>(),
                        ModContent.ItemType<KingSlayerMask>(),
                        ModContent.ItemType<KSBox>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<SlayerBag>(),
                        ModContent.ItemType<SlayerGun>(),
                        ModContent.ItemType<SlayerMedal>(),
                        ModContent.ItemType<Holokey>(),
                        ModContent.ItemType<CyberPlating>()
                    },
                    "Use a [i:" + ModContent.ItemType<CyberTech>() + "] at day.",
                    null,
                    "Redemption/CrossMod/BossChecklist/KingSlayer",
                    "Redemption/NPCs/Bosses/KSIII/KS3_Head_Boss");
                #endregion

                #region Omega Cleaver
                bossChecklist.Call("AddBoss", 11.5f, ModContent.NPCType<OmegaCleaver>(), mod,
                    "1st Omega Prototype",
                    (Func<bool>)(() => RedeBossDowned.downedVlitch1),
                    ModContent.ItemType<CleaverSword>(),
                    new List<int>
                    {
                        ModContent.ItemType<OmegaTrophy>(),
                        ModContent.ItemType<OmegaBox>(),
                        ModContent.ItemType<SwordRemote>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<OmegaCleaverBag>(),
                        ModContent.ItemType<GonkPet>(),
                        ModContent.ItemType<UnconPetItem>(),
                        ModContent.ItemType<BrokenBlade>(),
                        ModContent.ItemType<OmegaBattery>()
                    },
                    "Use a [i:" + ModContent.ItemType<CleaverSword>() + "] at night.",
                    null,
                    "Redemption/CrossMod/BossChecklist/OmegaCleaver",
                    "Redemption/NPCs/Bosses/Cleaver/OmegaCleaver_Head_Boss",
                    (Func<bool>)(() => RedeBossDowned.downedSeed));
                #endregion

                #region Omega Gigapora
                bossChecklist.Call("AddBoss", 11.9f, ModContent.NPCType<Gigapora>(), mod,
                    "2nd Omega Prototype",
                    (Func<bool>)(() => RedeBossDowned.downedVlitch2),
                    ModContent.ItemType<OmegaPowerdrill>(),
                    new List<int>
                    {
                        ModContent.ItemType<OmegaTrophy>(),
                        ModContent.ItemType<OmegaBox>()
                    },
                    new List<int>
                    {
                    },
                    "Use a [i:" + ModContent.ItemType<OmegaPowerdrill>() + "] at night.",
                    null,
                    "Redemption/CrossMod/BossChecklist/OmegaGigapora",
                    "Redemption/NPCs/Bosses/Gigapora/Gigapora_Head_Boss",
                    (Func<bool>)(() => RedeBossDowned.downedSeed));
                #endregion

                #region Patient Zero
                bossChecklist.Call("AddBoss", 14.5f,
                    ModContent.NPCType<PZ>(),
                    mod,
                    "Patient Zero",
                    (Func<bool>)(() => RedeBossDowned.downedPZ),
                    null,
                    new List<int>
                    {
                        ModContent.ItemType<Keycard2>(),
                        ModContent.ItemType<NanoPickaxe>(),
                        ModContent.ItemType<Electronade>(),
                        ModContent.ItemType<PZTrophy>(),
                        ModContent.ItemType<PZMask>(),
                        ModContent.ItemType<FloppyDisk6>(),
                        ModContent.ItemType<FloppyDisk6_1>(),
                        ModContent.ItemType<FloppyDisk7>(),
                        ModContent.ItemType<FloppyDisk7_1>(),
                        ModContent.ItemType<PZMusicBox>()
                    },
                    new List<int>
                    {
                        ModContent.ItemType<PZBag>(),
                        ModContent.ItemType<MedicKit>()
                    },
                    "Use a [i:" + ModContent.ItemType<Keycard>() + "] to access further sections of the laboratory. Beware what awaits beyond.",
                    null,
                    "Redemption/CrossMod/BossChecklist/PatientZero",
                    "Redemption/NPCs/Bosses/PatientZero/PZ_Head_Boss",
                    (Func<bool>)(() => RedeBossDowned.downedSeed));
                #endregion

                // SlimeKing = 1f;
                // EyeOfCthulhu = 2f;
                // EaterOfWorlds = 3f;
                // QueenBee = 4f;
                // Skeletron = 5f;
                // WallOfFlesh = 6f;
                // TheTwins = 7f;
                // TheDestroyer = 8f;
                // SkeletronPrime = 9f;
                // Plantera = 10f;
                // Golem = 11f;
                // DukeFishron = 12f;
                // LunaticCultist = 13f;
                // Moonlord = 14f;
            }
        }
    }
}
