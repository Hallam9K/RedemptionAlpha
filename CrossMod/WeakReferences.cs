using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.NPCs.Bosses.Thorn;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Items.Usable;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.NPCs.Minibosses.SkullDigger;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Lore;
using Redemption.Items.Placeable.MusicBoxes;
using Redemption.Items.Accessories.HM;
using Redemption.NPCs.Lab;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Weapons.HM.Melee;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.NPCs.Bosses.PatientZero;
using Redemption.Items.Tools.PostML;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.Obliterator;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Redemption.NPCs.Bosses.Neb;
using Terraria.Graphics.Shaders;
using Terraria;
using Redemption.NPCs.Bosses.ADD;

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
                bossChecklist.Call("AddBoss", mod, "Thorn", ModContent.NPCType<Thorn>(), 1.5f, () => RedeBossDowned.downedThorn, () => true,
                    new List<int>
                    {
                        ModContent.ItemType<ThornRelic>(),
                        ModContent.ItemType<BouquetOfThorns>(),
                        ModContent.ItemType<ThornTrophy>(),
                        ModContent.ItemType<ThornMask>(),
                        ModContent.ItemType<ForestBossBox>()
                    },
                    ModContent.ItemType<HeartOfThorns>(), "Use a [i:" + ModContent.ItemType<HeartOfThorns>() + "] at day. Can be found on the surface near spawn.",
                    "Thorn returned to his blighted forest...",
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Thorn").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Erhan
                bossChecklist.Call("AddBoss", mod, "Palebat Imp", ModContent.NPCType<Erhan>(), 1.9f, () => RedeBossDowned.downedErhan, () => RedeBossDowned.erhanDeath == 0, null, ModContent.ItemType<DemonScroll>(), "Use a [i:" + ModContent.ItemType<DemonScroll>() + "] at day. Can be found at the surface portal.", null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/PalebatImp").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, "Redemption/Items/HintIcon");

                bossChecklist.Call("AddBoss", mod, "Erhan", ModContent.NPCType<Erhan>(), 1.9f, () => RedeBossDowned.downedErhan, () => RedeBossDowned.erhanDeath > 0,
                    new List<int>
                    {
                        ModContent.ItemType<ErhanRelic>(),
                        ModContent.ItemType<DevilsAdvocate>(),
                        ModContent.ItemType<ErhanTrophy>(),
                        ModContent.ItemType<ErhanHelmet>(),
                    },
                    ModContent.ItemType<DemonScroll>(), "Use a [i:" + ModContent.ItemType<DemonScroll>() + "] at day. Can be found at the surface portal.",
                    "Erhan bravely flew away...",
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Erhan").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region The Keeper
                bossChecklist.Call("AddBoss", mod, "The Keeper", ModContent.NPCType<Keeper>(), 2.4f, () => RedeBossDowned.downedKeeper, () => true,
                    new List<int>
                    {
                        ModContent.ItemType<KeeperRelic>(),
                        ModContent.ItemType<OcciesCollar>(),
                        ModContent.ItemType<KeeperTrophy>(),
                        ModContent.ItemType<KeepersVeil>(),
                        ModContent.ItemType<KeeperBox>(),
                        ModContent.ItemType<KeepersCirclet>()
                    },
                    ModContent.ItemType<WeddingRing>(), "Use a [i:" + ModContent.ItemType<WeddingRing>() + "] at night.",
                    null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Keeper").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Skull Digger
                bossChecklist.Call("AddMiniBoss", mod, "Skull Digger", ModContent.NPCType<SkullDigger>(), 2.41f, () => RedeBossDowned.downedSkullDigger, () => RedeBossDowned.downedKeeper,
                    new List<int>
                    {
                        ModContent.ItemType<SkullDiggerMask>(),
                        ModContent.ItemType<AbandonedTeddy>()
                    },
                    ModContent.ItemType<SorrowfulEssence>(), "Roams the caverns, seeking revenge...",
                    null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/SkullDigger").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Seed of Infection
                bossChecklist.Call("AddBoss", mod, "Seed of Infection", ModContent.NPCType<SoI>(), 3.48f, () => RedeBossDowned.downedSeed, () => true,
                    new List<int>
                    {
                        ModContent.ItemType<SoIRelic>(),
                        ModContent.ItemType<CuddlyTeratoma>(),
                        ModContent.ItemType<SoITrophy>(),
                        ModContent.ItemType<InfectedMask>(),
                        ModContent.ItemType<SoIBox>()
                    },
                    ModContent.ItemType<AnomalyDetector>(), "Use an [i:" + ModContent.ItemType<AnomalyDetector>() + "]. Begins the Xenomite Infection.",
                    null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/SeedOfInfection").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Eaglecrest Golem
                bossChecklist.Call("AddMiniBoss", mod, "Eaglecrest Golem", ModContent.NPCType<EaglecrestGolem>(), 3.6f, () => RedeBossDowned.downedEaglecrestGolem, () => true,
                    new List<int>
                    {
                        ModContent.ItemType<StonePuppet>(),
                        ModContent.ItemType<ForestBossBox>(),
                    },
                    ModContent.ItemType<EaglecrestSpelltome>(), "Naturally spawns at day after Eater of Worlds/Brain of Cthulhu is defeated.",
                    null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/EaglecrestGolem").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region The Abandoned Lab
                bossChecklist.Call("AddEvent", mod, "The Abandoned Laboratory",
                    new List<int>()
                    {
                        ModContent.NPCType<OozeBlob>(),
                        ModContent.NPCType<BlisteredScientist>(),
                        ModContent.NPCType<OozingScientist>(),
                        ModContent.NPCType<BloatedScientist>(),
                        ModContent.NPCType<JanitorBot>(),
                        ModContent.NPCType<IrradiatedBehemoth>()
                    },
                    11.1f, () => RedeBossDowned.downedBehemoth, () => true,
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
                    ModContent.ItemType<IOLocator>(), "Find the Abandoned Lab far below the surface, defeat the first 2 minibosses within. Requires all mech bosses to be defeated.",
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Lab").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2) - 1, rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, "Redemption/Textures/Bestiary/TeochromeIcon");
                #endregion

                #region King Slayer III
                bossChecklist.Call("AddBoss", mod, "King Slayer III", ModContent.NPCType<KS3>(), 11.999f, () => RedeBossDowned.downedSlayer, () => true,
                    new List<int>
                    {
                        ModContent.ItemType<KS3Relic>(),
                        ModContent.ItemType<SlayerProjector>(),
                        ModContent.ItemType<KS3Trophy>(),
                        ModContent.ItemType<KingSlayerMask>(),
                        ModContent.ItemType<KSBox>()
                    },
                    ModContent.ItemType<CyberTech>(), "Use a [i:" + ModContent.ItemType<CyberTech>() + "] at day, or attack Androids on the surface and allow them to teleport away.", null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/KingSlayer").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Omega Cleaver
                bossChecklist.Call("AddBoss", mod, "1st Omega Prototype", ModContent.NPCType<OmegaCleaver>(), 12.5f, () => RedeBossDowned.downedOmega1, () => true,
                    new List<int>
                    {
                        ModContent.ItemType<CleaverRelic>(),
                        ModContent.ItemType<OmegaTrophy>(),
                        ModContent.ItemType<OmegaBox>(),
                        ModContent.ItemType<SwordRemote>()
                    },
                    ModContent.ItemType<OmegaTransmitter>(), "Use a [i:" + ModContent.ItemType<OmegaTransmitter>() + "] at night after Plantera has been defeated.", null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/OmegaCleaver").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Omega Gigapora
                bossChecklist.Call("AddBoss", mod, "2nd Omega Prototype", ModContent.NPCType<Gigapora>(), 13.5f, () => RedeBossDowned.downedOmega2, () => true,
                    new List<int>
                    {
                        ModContent.ItemType<GigaporaRelic>(),
                        ModContent.ItemType<OmegaTrophy>(),
                        ModContent.ItemType<OmegaBox>()
                    },
                    ModContent.ItemType<OmegaTransmitter>(), "Use a [i:" + ModContent.ItemType<OmegaTransmitter>() + "] at night after Golem has been defeated.", null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/OmegaGigapora").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Omega Obliterator
                bossChecklist.Call("AddBoss", mod, "3rd Omega Prototype", ModContent.NPCType<OO>(), 18.05f, () => RedeBossDowned.downedOmega3, () => true,
                    new List<int>
                    {
                        ModContent.ItemType<OORelic>(),
                        ModContent.ItemType<OmegaTrophy>(),
                        ModContent.ItemType<OmegaBox2>()
                    },
                    ModContent.ItemType<OmegaTransmitter>(), "Use a [i:" + ModContent.ItemType<OmegaTransmitter>() + "] at night after Moon Lord has been defeated.", null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/OmegaObliterator").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Patient Zero
                bossChecklist.Call("AddBoss", mod, "Patient Zero", ModContent.NPCType<PZ>(), 19f, () => RedeBossDowned.downedPZ, () => true,
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
                    ModContent.ItemType<Keycard>(), "Use a [i:" + ModContent.ItemType<Keycard>() + "] to access further sections of the laboratory. Beware what awaits beyond.", null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/PatientZero").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Ancient Deity Duo
                bossChecklist.Call("AddBoss", mod, "Eaglecrest Golem Rematch", ModContent.NPCType<EaglecrestGolem2>(), 20f, () => RedeBossDowned.downedADD, () => RedeBossDowned.ADDDeath == 0, null, ModContent.ItemType<GolemEye>(), "Encase the [i:" + ModContent.ItemType<GolemEye>() + "] within the stones of its origins, and it's true power will present itself.", null,
                (SpriteBatch sb, Rectangle rect, Color color) =>
                {
                    Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/EaglecrestGolem").Value;
                    Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                }, "Redemption/NPCs/Minibosses/EaglecrestGolem/EaglecrestGolem_Head_Boss");

                bossChecklist.Call("AddBoss", mod, "Ancient Deity Duo",
                    new List<int>
                    {
                        ModContent.NPCType<Ukko>(),
                        ModContent.NPCType<Akka>()
                    }, 20.001f, () => RedeBossDowned.downedADD, () => RedeBossDowned.ADDDeath > 0,
                    new List<int>
                    {
                        //ModContent.ItemType<ErhanRelic>(),
                        //ModContent.ItemType<DevilsAdvocate>(),
                        //ModContent.ItemType<ErhanTrophy>(),
                        //ModContent.ItemType<ErhanHelmet>(),
                    },
                    ModContent.ItemType<AncientSigil>(), "Use an [i:" + ModContent.ItemType<AncientSigil>() + "] at day.", null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/UkkoAkka").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }, null);
                #endregion

                #region Nebuleus
                bossChecklist.Call("AddBoss", mod, "Nebuleus", ModContent.NPCType<Nebuleus>(), 21f, () => RedeBossDowned.downedNebuleus, () => true,
                    new List<int>
                    {
                        //ModContent.ItemType<NebTrophy>(),
                        //ModContent.ItemType<NebMask>(),
                        //ModContent.ItemType<NebBox>()
                    },
                    ModContent.ItemType<NebSummon>(), "Use a [i:" + ModContent.ItemType<NebSummon>() + "] at night, dropped from Star Serpents in the sky.", null,
                    (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.LivingRainbowDye);
                        Texture2D texture = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Neb").Value;
                        Texture2D wingTex = ModContent.Request<Texture2D>("Redemption/CrossMod/BossChecklist/Neb_Wings").Value;
                        Vector2 centered = new(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                        GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                        sb.Draw(wingTex, centered, color);
                        sb.End();
                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    }, null);
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
    }
}
