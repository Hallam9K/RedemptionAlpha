using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Lore;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.MusicBoxes;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Tools.PostML;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.Bosses.Obliterator;
using Redemption.NPCs.Bosses.PatientZero;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.NPCs.Bosses.Thorn;
using Redemption.NPCs.FowlMorning;
using Redemption.NPCs.HM;
using Redemption.NPCs.Lab;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.MACE;
using Redemption.NPCs.Lab.Volt;
using Redemption.NPCs.Minibosses.Calavia;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Redemption.NPCs.Minibosses.FowlEmperor;
using Redemption.NPCs.Minibosses.SkullDigger;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.CrossMod
{
    internal class WeakReferences
    {
        public static List<int> FargosInstas = new();
        public static void PerformModSupport()
        {
            Redemption mod = Redemption.Instance;
            ModLoader.TryGetMod("Wikithis", out Mod wikithis);
            if (wikithis != null && !Main.dedServ)
                wikithis.Call("AddModURL", mod, "https://modofredemption.wiki.gg/wiki/{}");

            PerformBossChecklistSupport();
            PerformFargosSupport();
            PerformMusicDisplaySupport();

            if (ModLoader.TryGetMod("Fargowiltas", out var fargo))
            {
                if (fargo.TryFind("AutoHouse", out ModItem AutoHouse)) FargosInstas.Add(AutoHouse.Type);
                if (fargo.TryFind("CityBuster", out ModItem CityBuster)) FargosInstas.Add(CityBuster.Type);
                if (fargo.TryFind("DoubleObsidianInstabridge", out ModItem DoubleObsidianInstabridge)) FargosInstas.Add(DoubleObsidianInstabridge.Type);
                if (fargo.TryFind("HalfInstavator", out ModItem HalfInstavator)) FargosInstas.Add(HalfInstavator.Type);
                if (fargo.TryFind("InstaBridge", out ModItem InstaBridge)) FargosInstas.Add(InstaBridge.Type);
                if (fargo.TryFind("InstaPond", out ModItem InstaPond)) FargosInstas.Add(InstaPond.Type);
                if (fargo.TryFind("InstaTrack", out ModItem InstaTrack)) FargosInstas.Add(InstaTrack.Type);
                if (fargo.TryFind("Instavator", out ModItem Instavator)) FargosInstas.Add(Instavator.Type);
                if (fargo.TryFind("ObsidianInstaBridge", out ModItem ObsidianInstaBridge)) FargosInstas.Add(ObsidianInstaBridge.Type);
            }
        }
        private static void PerformBossChecklistSupport()
        {
            Redemption mod = Redemption.Instance;
            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
            {
                #region Fowl Emperor
                bossChecklist.Call("LogMiniBoss", mod, nameof(FowlEmperor), 0.1f, () => RedeBossDowned.downedFowlEmperor, ModContent.NPCType<FowlEmperor>(),
                    new Dictionary<string, object>()
                    {
                        ["spawnItems"] = ModContent.ItemType<EggCrown>(),
                        ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<FowlEmperorRelic>(),
                            ModContent.ItemType<EggPet>(),
                            ModContent.ItemType<FowlEmperorTrophy>(),
                            ModContent.ItemType<FowlCrown>(),
                            ModContent.ItemType<FowlEmperorBox>()
                        },
                        ["availability"] = () => RedeBossDowned.downedFowlEmperor,
                        ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                        {
                            Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/FowlEmperor").Value;
                            Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                            sb.Draw(texture, centered, color);
                        }
                    });
                #endregion

                #region Fowl Morning
                bossChecklist.Call("LogEvent", mod, "FowlMorning", 0.11f, () => RedeBossDowned.downedFowlMorning, new List<int>()
                    {
                        ModContent.NPCType<ChickenScratcher>(),
                        ModContent.NPCType<ChickenBomber>(),
                        ModContent.NPCType<RoosterBooster>(),
                        ModContent.NPCType<Haymaker>(),
                        ModContent.NPCType<HeadlessChicken>(),
                        ModContent.NPCType<Cockatrice>(),
                        ModContent.NPCType<Basan>()
                    }, new Dictionary<string, object>()
                    {
                        ["spawnItems"] = ModContent.ItemType<FowlWarHorn>(),
                        ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<CockatriceRelic>(),
                            ModContent.ItemType<FowlFeather>(),
                            ModContent.ItemType<CockatriceTrophy>(),
                            ModContent.ItemType<BasanRelic>(),
                            ModContent.ItemType<SpicyDrumstick>(),
                            ModContent.ItemType<BasanTrophy>(),
                            ModContent.ItemType<FowlMorningBox>()
                        },
                        ["availability"] = () => RedeBossDowned.downedFowlEmperor,
                        ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                        {
                            Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/FowlMorning").Value;
                            Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                            sb.Draw(texture, centered, color);
                        },
                        ["overrideHeadTextures"] = "Redemption/Gores/Boss/FowlEmperor_Crown"
                    });
                #endregion

                #region Cockatrice
                bossChecklist.Call("LogMiniBoss", mod, nameof(Cockatrice), 0.111f, () => RedeBossDowned.downedFowlMorning, ModContent.NPCType<Cockatrice>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<FowlWarHorn>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<CockatriceRelic>(),
                            ModContent.ItemType<FowlFeather>(),
                            ModContent.ItemType<CockatriceTrophy>(),
                            ModContent.ItemType<FowlMorningBox>()
                        },
                    ["availability"] = () => RedeBossDowned.downedFowlEmperor,
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Cockatrice").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Basan
                bossChecklist.Call("LogMiniBoss", mod, nameof(Basan), 0.112f, () => RedeBossDowned.downedFowlMorning, ModContent.NPCType<Basan>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<FowlWarHorn>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<BasanRelic>(),
                            ModContent.ItemType<SpicyDrumstick>(),
                            ModContent.ItemType<BasanTrophy>(),
                            ModContent.ItemType<FowlMorningBox>()
                        },
                    ["availability"] = () => RedeBossDowned.downedFowlEmperor,
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Basan").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Thorn
                bossChecklist.Call("LogBoss", mod, nameof(Thorn), 1.5f, () => RedeBossDowned.downedThorn, ModContent.NPCType<Thorn>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<HeartOfThorns>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<ThornRelic>(),
                            ModContent.ItemType<BouquetOfThorns>(),
                            ModContent.ItemType<ThornTrophy>(),
                            ModContent.ItemType<ThornMask>(),
                            ModContent.ItemType<ThornBox>()
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Thorn").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Erhan
                bossChecklist.Call("LogBoss", mod, nameof(PalebatImp), 1.9f, () => RedeBossDowned.downedErhan, ModContent.NPCType<Erhan>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<DemonScroll>(),
                    ["availability"] = () => RedeBossDowned.erhanDeath == 0,
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/PalebatImp").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    },
                    ["overrideHeadTextures"] = "Redemption/Items/HintIcon",
                    ["displayName"] = Language.GetText("Mods.Redemption.NPCs.PalebatImp.DisplayName")
                });

                bossChecklist.Call("LogBoss", mod, nameof(Erhan), 1.9f, () => RedeBossDowned.downedErhan, ModContent.NPCType<Erhan>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<DemonScroll>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<ErhanRelic>(),
                            ModContent.ItemType<DevilsAdvocate>(),
                            ModContent.ItemType<ErhanTrophy>(),
                            ModContent.ItemType<ErhanHelmet>(),
                            ModContent.ItemType<ErhanBox>(),
                        },
                    ["availability"] = () => RedeBossDowned.erhanDeath > 0,
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Erhan").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region The Keeper
                bossChecklist.Call("LogBoss", mod, nameof(Keeper), 2.4f, () => RedeBossDowned.downedKeeper, ModContent.NPCType<Keeper>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<WeddingRing>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<KeeperRelic>(),
                            ModContent.ItemType<OcciesCollar>(),
                            ModContent.ItemType<KeeperTrophy>(),
                            ModContent.ItemType<KeepersVeil>(),
                            ModContent.ItemType<KeeperBox>(),
                            ModContent.ItemType<KeepersCirclet>()
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Keeper").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Skull Digger
                bossChecklist.Call("LogMiniBoss", mod, nameof(SkullDigger), 2.41f, () => RedeBossDowned.downedSkullDigger, ModContent.NPCType<SkullDigger>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<SorrowfulEssence>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<SkullDiggerMask>(),
                            ModContent.ItemType<AbandonedTeddy>(),
                            ModContent.ItemType<SkullDiggerBox>()
                        },
                    ["availability"] = () => RedeBossDowned.downedKeeper,
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/SkullDigger").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Eaglecrest Golem
                bossChecklist.Call("LogMiniBoss", mod, nameof(EaglecrestGolem), 3.48f, () => RedeBossDowned.downedEaglecrestGolem, ModContent.NPCType<EaglecrestGolem>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<EaglecrestSpelltome>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<StonePuppet>(),
                            ModContent.ItemType<ForestBossBox>(),
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/EaglecrestGolem").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Seed of Infection
                bossChecklist.Call("LogBoss", mod, nameof(SoI), 3.6f, () => RedeBossDowned.downedSeed, ModContent.NPCType<SoI>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<AnomalyDetector>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<SoIRelic>(),
                            ModContent.ItemType<CuddlyTeratoma>(),
                            ModContent.ItemType<SoITrophy>(),
                            ModContent.ItemType<InfectedMask>(),
                            ModContent.ItemType<SoIBox>()
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/SeedOfInfection").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Calavia
                bossChecklist.Call("LogMiniBoss", mod, nameof(Calavia), 5.2f, () => RedeBossDowned.downedCalavia || RedeQuest.calaviaVar >= 3, ModContent.NPCType<Calavia>(), new Dictionary<string, object>()
                {
                    ["availability"] = () => NPC.downedBoss3,
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Calavia").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region The Abandoned Lab
                bossChecklist.Call("LogEvent", mod, "AbandonedLaboratory", 11.1f, () => RedeBossDowned.downedBehemoth, new List<int>()
                    {
                        ModContent.NPCType<OozeBlob>(),
                        ModContent.NPCType<BlisteredScientist>(),
                        ModContent.NPCType<OozingScientist>(),
                        ModContent.NPCType<BloatedScientist>(),
                        ModContent.NPCType<JanitorBot>(),
                        ModContent.NPCType<IrradiatedBehemoth>()
                    }, new Dictionary<string, object>()
                    {
                        ["spawnItems"] = ModContent.ItemType<IOLocator>(),
                        ["collectibles"] = new List<int>
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
                        ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                        {
                            Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Lab").Value;
                            Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                            sb.Draw(texture, centered, color);
                        },
                        ["overrideHeadTextures"] = "Redemption/Textures/Bestiary/TeochromeIcon"
                    });
                #endregion

                #region King Slayer III
                bossChecklist.Call("LogBoss", mod, nameof(KS3), 11.999f, () => RedeBossDowned.downedSlayer, ModContent.NPCType<KS3>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<CyberTech>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<KS3Relic>(),
                            ModContent.ItemType<SlayerProjector>(),
                            ModContent.ItemType<KS3Trophy>(),
                            ModContent.ItemType<KingSlayerMask>(),
                            ModContent.ItemType<KSBox>(),
                            ModContent.ItemType<SlayerMedal>()
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/KingSlayer").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Omega Cleaver
                bossChecklist.Call("LogBoss", mod, nameof(OmegaCleaver), 12.5f, () => RedeBossDowned.downedOmega1, ModContent.NPCType<OmegaCleaver>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<OmegaTransmitter>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<CleaverRelic>(),
                            ModContent.ItemType<OmegaCleaverTrophy>(),
                            ModContent.ItemType<OmegaBox>(),
                            ModContent.ItemType<SwordRemote>()
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/OmegaCleaver").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Omega Gigapora
                bossChecklist.Call("LogBoss", mod, nameof(Gigapora), 14f, () => RedeBossDowned.downedOmega2, ModContent.NPCType<Gigapora>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<OmegaTransmitter>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<GigaporaRelic>(),
                            ModContent.ItemType<PowerDrill>(),
                            ModContent.ItemType<OmegaGigaporaTrophy>(),
                            ModContent.ItemType<OmegaBox>()
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/OmegaGigapora").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Omega Obliterator
                bossChecklist.Call("LogBoss", mod, nameof(OO), 18.1f, () => RedeBossDowned.downedOmega3, ModContent.NPCType<OO>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<OmegaTransmitter>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<ToasterPet>(),
                            ModContent.ItemType<OORelic>(),
                            ModContent.ItemType<OmegaObliteratorTrophy>(),
                            ModContent.ItemType<OmegaBox2>()
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/OmegaObliterator").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Patient Zero
                bossChecklist.Call("LogEvent", mod, "AbandonedLaboratory2", 18.05f, () => RedeBossDowned.downedMACE, new List<int>()
                {
                    ModContent.NPCType<Blisterface>(),
                    ModContent.NPCType<ProtectorVolt>(),
                    ModContent.NPCType<MACEProject>()
                }, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<Keycard>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<Keycard2>(),
                            ModContent.ItemType<NanoPickaxe>(),
                            ModContent.ItemType<Electronade>(),
                            ModContent.ItemType<FloppyDisk5>(),
                            ModContent.ItemType<FloppyDisk5_1>(),
                            ModContent.ItemType<FloppyDisk5_2>(),
                            ModContent.ItemType<FloppyDisk5_3>(),
                            ModContent.ItemType<FloppyDisk6>(),
                            ModContent.ItemType<FloppyDisk6_1>(),
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Lab2").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    },
                    ["overrideHeadTextures"] = "Redemption/Textures/Bestiary/TeochromeIcon"
                });

                bossChecklist.Call("LogBoss", mod, nameof(PZ), 19.1f, () => RedeBossDowned.downedPZ, ModContent.NPCType<PZ>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<Keycard>(),
                    ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<PZRelic>(),
                            ModContent.ItemType<Xenoemia>(),
                            ModContent.ItemType<PZTrophy>(),
                            ModContent.ItemType<PZMask>(),
                            ModContent.ItemType<FloppyDisk7>(),
                            ModContent.ItemType<FloppyDisk7_1>(),
                            ModContent.ItemType<PZMusicBox>()
                        },
                    ["availability"] = () => RedeBossDowned.downedMACE,
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/PatientZero").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                });
                #endregion

                #region Ancient Deity Duo
                bossChecklist.Call("LogBoss", mod, nameof(EaglecrestGolem2), 20f, () => RedeBossDowned.downedADD, ModContent.NPCType<EaglecrestGolem2>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<GolemEye>(),
                    ["availability"] = () => RedeBossDowned.downedEaglecrestGolem && RedeBossDowned.ADDDeath == 0,
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/EaglecrestGolem").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    },
                    ["overrideHeadTextures"] = "Redemption/NPCs/Minibosses/EaglecrestGolem/EaglecrestGolem_Head_Boss"
                });
                bossChecklist.Call("LogBoss", mod, "AncientDeityDuo", 20.001f, () => RedeBossDowned.downedADD, new List<int>
                    {
                        ModContent.NPCType<Ukko>(),
                        ModContent.NPCType<Akka>()
                    }, new Dictionary<string, object>()
                    {
                        ["spawnItems"] = ModContent.ItemType<AncientSigil>(),
                        ["collectibles"] = new List<int>
                        {
                            ModContent.ItemType<UkkoRelic>(),
                            ModContent.ItemType<AkkaRelic>(),
                            ModContent.ItemType<JyrinaMount>(),
                            ModContent.ItemType<UkonKirvesTrophy>(),
                            ModContent.ItemType<AkanKirvesTrophy>(),
                            ModContent.ItemType<UkkoMask>(),
                            ModContent.ItemType<AkkaMask>(),
                            ModContent.ItemType<ForestBossBox2>(),
                        },
                        ["availability"] = () => RedeBossDowned.ADDDeath > 0,
                        ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                        {
                            Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/UkkoAkka").Value;
                            Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                            sb.Draw(texture, centered, color);
                        },
                    });
                #endregion

                #region Nebuleus
                bossChecklist.Call("LogBoss", mod, nameof(Nebuleus), 23f, () => RedeBossDowned.downedNebuleus, ModContent.NPCType<Nebuleus>(), new Dictionary<string, object>()
                {
                    ["spawnItems"] = ModContent.ItemType<NebSummon>(),
                    ["collectibles"] = new List<int>
                        {
                        ModContent.ItemType<NebRelic>(),
                        ModContent.ItemType<GildedBonnet>(),
                        ModContent.ItemType<NebuleusTrophy>(),
                        ModContent.ItemType<NebuleusMask>(),
                        ModContent.ItemType<NebBox>()
                        },
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.HallowBossDye);
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Neb").Value;
                        Texture2D wingTex = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Neb_Wings").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
                        GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
                        sb.Draw(wingTex, centered, color);
                        sb.End();
                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
                    }
                });
                #endregion

                /*KingSlime = 1f;
                EyeOfCthulhu = 2f;
                EaterOfWorlds = 3f; // and Brain of Cthulhu
                QueenBee = 4f;
                Skeletron = 5f;
                DeerClops = 6f;
                WallOfFlesh = 7f;
                QueenSlime = 8f;
                TheTwins = 9f;
                TheDestroyer = 10f;
                SkeletronPrime = 11f;
                Plantera = 12f;
                Golem = 13f;
                Betsy = 14f;
                EmpressOfLight = 15f;
                DukeFishron = 16f;
                LunaticCultist = 17f;
                Moonlord = 18f;*/
            }
        }
        private static void PerformFargosSupport()
        {
            if (ModLoader.TryGetMod("Fargowiltas", out Mod fargos))
            {
                fargos.Call("AddSummon", 0.1f, "Redemption", "EggCrown", () => RedeBossDowned.downedFowlEmperor, Item.buyPrice(0, 4));
                fargos.Call("AddSummon", 1.5f, "Redemption", "HeartOfThorns", () => RedeBossDowned.downedThorn, Item.buyPrice(0, 6));
                fargos.Call("AddSummon", 1.9f, "Redemption", "DemonScroll", () => RedeBossDowned.downedErhan, Item.buyPrice(0, 6));
                fargos.Call("AddSummon", 2.4f, "Redemption", "WeddingRing", () => RedeBossDowned.downedKeeper, Item.buyPrice(0, 8));
                fargos.Call("AddSummon", 2.41f, "Redemption", "SorrowfulEssence", () => RedeBossDowned.downedSkullDigger, Item.buyPrice(0, 4));
                fargos.Call("AddSummon", 3.48f, "Redemption", "AnomalyDetector", () => RedeBossDowned.downedSeed, Item.buyPrice(0, 10));
                fargos.Call("AddSummon", 11.1f, "Redemption", "LabHologramDevice", () => RedeBossDowned.downedBehemoth, Item.buyPrice(0, 20));
                fargos.Call("AddSummon", 11.999f, "Redemption", "CyberTech", () => RedeBossDowned.downedSlayer, Item.buyPrice(0, 40));
                fargos.Call("AddSummon", 12.5f, "Redemption", "OmegaTransmitter", () => RedeBossDowned.downedOmega1 || RedeBossDowned.downedOmega2 || RedeBossDowned.downedOmega3, Item.buyPrice(0, 60));
                fargos.Call("AddSummon", 20f, "Redemption", "AncientSigil", () => RedeBossDowned.downedADD, Item.buyPrice(4));
                fargos.Call("AddSummon", 21f, "Redemption", "NebSummon", () => RedeBossDowned.downedNebuleus, Item.buyPrice(10));

                fargos.Call("AddEventSummon", 1f, "Redemption", "FowlWarHorn", () => RedeBossDowned.downedFowlMorning, Item.buyPrice(0, 4, 50));
            }
        }
        private static void PerformMusicDisplaySupport()
        {
            Redemption mod = Redemption.Instance;
            if (!ModLoader.TryGetMod("MusicDisplay", out Mod display))
                return;

            void AddMusic(string path, string name, string author = "musicman") => display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(mod, path), name, "by " + author, "Mod of Redemption");

            AddMusic("Sounds/Music/BossErhan", "Holy Inquisition (Erhan Theme)", "Sc0p3r");
            AddMusic("Sounds/Music/BossForest1", "Dramatic4 (Epidotrian Opponent Theme)", "Peritune");
            AddMusic("Sounds/Music/BossUkko", "Nature's Wrath (Ancient Deity Duo Theme)", "Yuri O");
            AddMusic("Sounds/Music/FowlMorning", "Dawn of the Coop (Fowl Morning Theme)", "Sc0p3r");
            AddMusic("Sounds/Music/HallofHeroes", "??? (Hall of Heroes Theme)", "???");
            AddMusic("Sounds/Music/BossKeeper", "Haunting Loneliness (The Keeper Theme)", "SpectralAves");
            AddMusic("Sounds/Music/BossSlayer", "Titanium Will (King Slayer III Theme)", "Sc0p3r");
            AddMusic("Sounds/Music/LabBossMusic", "Safety Violation (Abandoned Lab Minibosses Theme)", "inSignia");
            AddMusic("Sounds/Music/LabBossMusicIB", "Safety Violation (Abandoned Lab Minibosses Theme)", "inSignia");
            AddMusic("Sounds/Music/LabBossMusicMP", "Safety Violation (Abandoned Lab Minibosses Theme)", "inSignia");
            AddMusic("Sounds/Music/LabMusic", "Facility of Contagion (Abandoned Lab Theme)", "inSignia");
            AddMusic("Sounds/Music/BossStarGod1", "Interstellar Isolation (Nebuleus Theme)", "musicman");
            AddMusic("Sounds/Music/BossStarGod2", "Hypernova (Nebuleus' Final Form Theme)", "musicman");
            AddMusic("Sounds/Music/BossOmega1", "Armageddon Interface (Omega Prototypes Theme)", "musicman");
            AddMusic("Sounds/Music/BossOmega2", "Hailfire (Omega Obliterator Theme)", "Universe");
            AddMusic("Sounds/Music/LabBossMusic2", "Xenostate (Patient Zero Theme)", "Universe");
            AddMusic("Sounds/Music/SlayerShipMusic", "Waiting for Slayer to Respond (Slayer's Crashed Ship Theme)", "Kweequ");
            AddMusic("Sounds/Music/BossXeno1", "Virogenesis (Seed of Infection Theme)", "musicman");
            AddMusic("Sounds/Music/Wasteland", "The Wastelands (Wasteland Theme)", "Musearys");
            AddMusic("Sounds/Music/BossFowl", "King's Gambit (Fowl Emperor Theme)", "Sc0p3r");
            AddMusic("Sounds/Music/BossSlayer2", "Betrayal of Fear (TeslaX VIP Remix)", "Talurre");
            AddMusic("Sounds/Music/Epidotra", "Epidotra (Epidotra Menu Theme)", "musicman");
            AddMusic("Sounds/Music/ImpOfDoom", "Imp of Doom (Palebat Imp Theme)", "musicman");
            AddMusic("Sounds/Music/Island", "████ (The Island Theme)", "████");
            AddMusic("Sounds/Music/OmegaOverheat", "Meltdown (Obliterator Overheat Theme)", "Universe");
            AddMusic("Sounds/Music/SilentCaverns", "The Loudness of Silence (Skull Digger Theme)", "Yuri O");
            AddMusic("Sounds/Music/SpiritRealm", "Spectral Interloper (Spirit Realm Theme)", "Yuri O");
            AddMusic("Sounds/Music/Raveyard", "Rattle Thy Bones (Raveyard Theme)", "Zeus073");
            AddMusic("Sounds/Music/Warhead", "Detonation Sequence (Nuclear Countdown Theme)", "musicman");
            AddMusic("Sounds/Music/BossThorn", "Every Rose... (Thorn Theme)", "musicman");
        }
    }
}