using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Redemption.Backgrounds.Skies;
using Redemption.Biomes;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.CrossMod;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Effects.RenderTargets;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Globals.World;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.PostML.Shinkite;
using Redemption.Items.Armor.PostML.Vorti;
using Redemption.Items.Armor.PreHM.DragonLead;
using Redemption.Items.Donator.Arche;
using Redemption.Items.Donator.Uncon;
using Redemption.Items.Usable;
using Redemption.UI;
using Redemption.UI.ChatUI;
using ReLogic.Content;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static Redemption.Globals.RedeNet;
using Redemption.WorldGeneration.Misc;
using Redemption.Items.Usable.Summons;
using Redemption.Helpers;

namespace Redemption
{
    public partial class Redemption : Mod
    {
        public static Redemption Instance { get; private set; }

        public const string Abbreviation = "MoR";
        public const string EMPTY_TEXTURE = "Redemption/Empty";
        public const string PLACEHOLDER_TEXTURE = "Redemption/Placeholder";
        public Vector2 cameraOffset;
        public Rectangle currentScreen;
        public static int grooveTimer;
        public static ModKeybind RedeSpecialAbility;
        public static ModKeybind RedeSpiritwalkerAbility;
        public static bool AprilFools => DateTime.Now is DateTime { Month: 4, Day: 1 };
        public static bool FinlandDay => DateTime.Now is DateTime { Month: 12, Day: 6 };

        public static BasicEffect basicEffect;
        public static RenderTargetManager Targets;
        public static Effect GlowTrailShader;
        public static TrailManager TrailManager;

        private List<ILoadable> _loadCache;

        public static int AntiqueDorulCurrencyId;
        public static int dragonLeadCapeID;
        public static int shinkiteCapeID;
        public static int archeFemLegID;
        public static int archeMaleLegID;
        public static int unconFemLegID;
        public static int unconMaleLegID;
        public static int unconFemLeg2ID;
        public static int unconMaleLeg2ID;
        public static int halmFemLegID;
        public static int halmMaleLegID;

        public static Asset<Texture2D> Circle;
        public static Asset<Texture2D> SoftGlow;
        public static Asset<Texture2D> EmberParticle;
        public static Asset<Texture2D> GlowParticle;
        public static Asset<Texture2D> WhiteGlow;
        public static Asset<Texture2D> WhiteFlare;
        public static Asset<Texture2D> WhiteOrb;
        public static Asset<Texture2D> IceMist;
        public static Asset<Texture2D> HolyGlow2;
        public static Asset<Texture2D> HolyGlow3;

        public static Asset<Texture2D> GlowTrail;

        public Redemption()
        {
            Instance = this;
        }

        public override void Load()
        {
            LoadCache();

            if (!Main.dedServ)
            {
                TrailManager = new TrailManager(this);
                AdditiveCallManager.Load();

                Circle = ModContent.Request<Texture2D>("Redemption/Textures/Circle");
                SoftGlow = ModContent.Request<Texture2D>("Redemption/Textures/SoftGlow");
                EmberParticle = ModContent.Request<Texture2D>("Redemption/Particles/EmberParticle");
                GlowParticle = ModContent.Request<Texture2D>("Redemption/Particles/GlowParticle");
                WhiteGlow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow");
                WhiteFlare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare");
                WhiteOrb = ModContent.Request<Texture2D>("Redemption/Textures/WhiteOrb");
                IceMist = ModContent.Request<Texture2D>("Redemption/Textures/IceMist");
                HolyGlow2 = ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2");
                HolyGlow3 = ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow3");

                GlowTrail = ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail", AssetRequestMode.ImmediateLoad);

                dragonLeadCapeID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Armor/PreHM/DragonLead/DragonLeadRibplate_Back", EquipType.Back, ModContent.GetInstance<DragonLeadRibplate>());
                shinkiteCapeID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Armor/PostML/Shinkite/ShinkiteChestplate_Back", EquipType.Back, ModContent.GetInstance<ShinkiteChestplate>());
                archeMaleLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Arche/ArchePatreonVanityLegs_Legs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<ArchePatreonVanityLegs>()));
                archeFemLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Arche/ArchePatreonVanityLegs_FemaleLegs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<ArchePatreonVanityLegs>()));
                unconMaleLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Uncon/UnconLegs_Legs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs>()));
                unconFemLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Uncon/UnconLegs_FemaleLegs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs>()));
                unconMaleLeg2ID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Uncon/UnconLegs2_Legs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs2>()));
                unconFemLeg2ID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Uncon/UnconLegs2_FemaleLegs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs2>()));
                halmMaleLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Armor/Vanity/Dev/HallamLeggings_Legs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs2>()));
                halmFemLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Armor/Vanity/Dev/HallamLeggings_FemaleLegs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs2>()));

                int width = Main.graphics.GraphicsDevice.Viewport.Width;
                int height = Main.graphics.GraphicsDevice.Viewport.Height;
                Vector2 zoom = Main.GameViewMatrix.Zoom;
                Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);

                AssetRequestMode immLoad = AssetRequestMode.ImmediateLoad;
                Main.QueueMainThreadAction(() =>
                {
                    basicEffect = new BasicEffect(Main.graphics.GraphicsDevice)
                    {
                        VertexColorEnabled = true,
                        View = view,
                        Projection = projection
                    };
                    Texture2D bubbleTex = ModContent.Request<Texture2D>("Redemption/Textures/BubbleShield", immLoad).Value;
                    PremultiplyTexture(ref bubbleTex);
                    Texture2D portalTex = ModContent.Request<Texture2D>("Redemption/Textures/PortalTex", immLoad).Value;
                    PremultiplyTexture(ref portalTex);
                    Texture2D soullessPortal = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/SoullessPortal", immLoad).Value;
                    PremultiplyTexture(ref soullessPortal);
                    Texture2D holyGlowTex = ModContent.Request<Texture2D>("Redemption/" + WhiteGlow.Name, immLoad).Value;
                    PremultiplyTexture(ref holyGlowTex);
                    Texture2D whiteFlareTex = ModContent.Request<Texture2D>("Redemption/" + WhiteFlare.Name, immLoad).Value;
                    PremultiplyTexture(ref whiteFlareTex);
                    Texture2D whiteOrbTex = ModContent.Request<Texture2D>("Redemption/" + WhiteOrb.Name, immLoad).Value;
                    PremultiplyTexture(ref whiteOrbTex);
                    Texture2D whiteLightBeamTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteLightBeam", immLoad).Value;
                    PremultiplyTexture(ref whiteLightBeamTex);
                    Texture2D transitionTex = ModContent.Request<Texture2D>("Redemption/Textures/TransitionTex", immLoad).Value;
                    PremultiplyTexture(ref transitionTex);
                    Texture2D staticBallTex = ModContent.Request<Texture2D>("Redemption/Textures/StaticBall", immLoad).Value;
                    PremultiplyTexture(ref staticBallTex);
                    Texture2D iceMistTex = ModContent.Request<Texture2D>("Redemption/" + IceMist.Name, immLoad).Value;
                    PremultiplyTexture(ref iceMistTex);
                    Texture2D glowDustTex = ModContent.Request<Texture2D>("Redemption/Dusts/GlowDust", immLoad).Value;
                    PremultiplyTexture(ref glowDustTex);
                    Texture2D AkkaHealingSpiritTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/ADD/AkkaHealingSpirit", immLoad).Value;
                    PremultiplyTexture(ref AkkaHealingSpiritTex);
                    Texture2D AkkaIslandWarningTex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/ADD/AkkaIslandWarning", immLoad).Value;
                    PremultiplyTexture(ref AkkaIslandWarningTex);
                    Texture2D SunTex = ModContent.Request<Texture2D>("Redemption/Textures/Sun", immLoad).Value;
                    PremultiplyTexture(ref SunTex);
                    Texture2D DarkSoulTex = ModContent.Request<Texture2D>("Redemption/Textures/DarkSoulTex", immLoad).Value;
                    PremultiplyTexture(ref DarkSoulTex);
                    Texture2D TornadoTex = ModContent.Request<Texture2D>("Redemption/Textures/TornadoTex", immLoad).Value;
                    PremultiplyTexture(ref TornadoTex);
                    Texture2D SpiritPortalTex = ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex", immLoad).Value;
                    PremultiplyTexture(ref SpiritPortalTex);

                    Texture2D purityWastelandBG3Tex = ModContent.Request<Texture2D>("Redemption/Backgrounds/PurityWastelandBG3", immLoad).Value;
                    PremultiplyTexture(ref purityWastelandBG3Tex);
                    Texture2D wastelandCrimsonBG3Tex = ModContent.Request<Texture2D>("Redemption/Backgrounds/WastelandCrimsonBG3", immLoad).Value;
                    PremultiplyTexture(ref wastelandCrimsonBG3Tex);
                    Texture2D wastelandCorruptionBG3Tex = ModContent.Request<Texture2D>("Redemption/Backgrounds/WastelandCorruptionBG3", immLoad).Value;
                    PremultiplyTexture(ref wastelandCorruptionBG3Tex);
                    Texture2D ruinedKingdomSurfaceClose_MenuTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/RuinedKingdomSurfaceClose_Menu", immLoad).Value;
                    PremultiplyTexture(ref ruinedKingdomSurfaceClose_MenuTex);
                    Texture2D ruinedKingdomSurfaceFar_MenuTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/RuinedKingdomSurfaceFar_Menu", immLoad).Value;
                    PremultiplyTexture(ref ruinedKingdomSurfaceFar_MenuTex);
                    Texture2D ruinedKingdomSurfaceMid_MenuTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/RuinedKingdomSurfaceMid_Menu", immLoad).Value;
                    PremultiplyTexture(ref ruinedKingdomSurfaceMid_MenuTex);
                    Texture2D UkkoCloudsTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/UkkoClouds", immLoad).Value;
                    PremultiplyTexture(ref UkkoCloudsTex);
                    Texture2D UkkoSkyBeamTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/UkkoSkyBeam", immLoad).Value;
                    PremultiplyTexture(ref UkkoSkyBeamTex);
                    Texture2D UkkoSkyBoltTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/UkkoSkyBolt", immLoad).Value;
                    PremultiplyTexture(ref UkkoSkyBoltTex);
                    Texture2D UkkoSkyFlashTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/UkkoSkyFlash", immLoad).Value;
                    PremultiplyTexture(ref UkkoSkyFlashTex);
                    Texture2D SkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/SkyTex", immLoad).Value;
                    PremultiplyTexture(ref SkyTex);
                    Texture2D SkyTex2 = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/SkyTex2", immLoad).Value;
                    PremultiplyTexture(ref SkyTex2);
                    Texture2D WastelandCorruptSkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/WastelandCorruptSkyTex", immLoad).Value;
                    PremultiplyTexture(ref WastelandCorruptSkyTex);
                    Texture2D WastelandCrimsonSkyTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/Skies/WastelandCrimsonSkyTex", immLoad).Value;
                    PremultiplyTexture(ref WastelandCrimsonSkyTex);
                });
                Filters.Scene["MoR:OOSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.2f, 0f, 0f).UseOpacity(0.2f), EffectPriority.VeryHigh);
                SkyManager.Instance["MoR:OOSky"] = new OOSky();
                Filters.Scene["MoR:NebP1"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.2f, 0f, 0.3f).UseOpacity(0.5f), EffectPriority.VeryHigh);
                SkyManager.Instance["MoR:NebP1"] = new NebSky();
                Filters.Scene["MoR:NebP2"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.2f, 0f, 0.3f).UseOpacity(0.5f), EffectPriority.VeryHigh);
                SkyManager.Instance["MoR:NebP2"] = new NebSky2();
                Filters.Scene["MoR:Ukko"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.2f, 0.1f, 0f).UseOpacity(0.3f), EffectPriority.VeryHigh);
                SkyManager.Instance["MoR:Ukko"] = new UkkoClouds();
            }
            Filters.Scene["MoR:WastelandSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0.2f, 0f).UseOpacity(0.5f), EffectPriority.High);
            SkyManager.Instance["MoR:WastelandSky"] = new WastelandSky();
            SkyManager.Instance["MoR:WastelandSnowSky"] = new WastelandSnowSky();
            SkyManager.Instance["MoR:WastelandCorruptSky"] = new WastelandCorruptSky();
            SkyManager.Instance["MoR:WastelandCrimsonSky"] = new WastelandCrimsonSky();

            Filters.Scene["MoR:SpiritSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.8f, 0.8f), EffectPriority.VeryHigh);
            Filters.Scene["MoR:IslandEffect"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.4f, 0.4f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["MoR:RuinedKingdomSky"] = new RuinedKingdomSky();
            Filters.Scene["MoR:SoullessSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0.55f), EffectPriority.High);
            Filters.Scene["MoR:FowlMorningSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.7f, 0.3f, 0.02f).UseOpacity(0.3f), EffectPriority.High);

            RedeSpecialAbility = KeybindLoader.RegisterKeybind(this, "Special Ability Key", Keys.F);
            RedeSpiritwalkerAbility = KeybindLoader.RegisterKeybind(this, "Spirit Walker Key", Keys.K);
            AntiqueDorulCurrencyId = CustomCurrencyManager.RegisterCurrency(new AntiqueDorulCurrency(ModContent.ItemType<AncientGoldCoin>(), 999L, "Antique Doruls"));
        }
        public override void Unload()
        {
            Circle = null;
            SoftGlow = null;
            EmberParticle = null;
            GlowParticle = null;
            WhiteGlow = null;
            WhiteFlare = null;
            WhiteOrb = null;
            IceMist = null;
            HolyGlow2 = null;
            HolyGlow3 = null;

            GlowTrail = null;

            TrailManager = null;
            AdditiveCallManager.Unload();
        }
        public override void PostSetupContent()
        {
            WeakReferences.PerformModSupport();
            if (!Main.dedServ)
            {
                Main.QueueMainThreadAction(() =>
                {
                    OnHeadDraw.RegisterHeads();
                    OnLegDraw.RegisterLegs();
                    OnBodyDraw.RegisterBodies();
                });
            }
        }
        public static void PremultiplyTexture(ref Texture2D texture)
        {
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(
                        buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            texture.SetData(buffer);
        }
        private void LoadCache()
        {
            _loadCache = new List<ILoadable>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(ILoadable)))
                {
                    _loadCache.Add(Activator.CreateInstance(type) as ILoadable);
                }
            }

            _loadCache.Sort((x, y) => x.Priority > y.Priority ? 1 : -1);

            for (int i = 0; i < _loadCache.Count; ++i)
            {
                if (Main.dedServ && !_loadCache[i].LoadOnDedServer)
                {
                    continue;
                }

                _loadCache[i].Load();
            }
        }

        public ModPacket GetPacket(ModMessageType type, int capacity)
        {
            ModPacket packet = GetPacket(capacity + 1);
            packet.Write((byte)type);
            return packet;
        }
        public static ModPacket WriteToPacket(ModPacket packet, byte msg, params object[] param)
        {
            packet.Write(msg);

            for (int m = 0; m < param.Length; m++)
            {
                object obj = param[m];
                if (obj is bool boolean) packet.Write(boolean);
                else
                if (obj is byte @byte) packet.Write(@byte);
                else
                if (obj is int @int) packet.Write(@int);
                else
                if (obj is float single) packet.Write(single);
                else
                if (obj is Vector2 vector) { packet.Write(vector.X); packet.Write(vector.Y); }
            }
            return packet;
        }
        public override void HandlePacket(BinaryReader bb, int whoAmI)
        {
            ModMessageType msgType = (ModMessageType)bb.ReadByte();
            //byte player;
            switch (msgType)
            {
                case ModMessageType.BossSpawnFromClient:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int bossType = bb.ReadInt32();
                        int npcCenterX = bb.ReadInt32();
                        int npcCenterY = bb.ReadInt32();

                        if (NPC.AnyNPCs(bossType))
                            return;

                        int npcID = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), npcCenterX, npcCenterY, bossType);
                        Main.npc[npcID].netUpdate2 = true;
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[npcID].GetTypeNetName()), new Color(175, 75, 255));
                    }
                    break;
                case ModMessageType.NPCSpawnFromClient:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int NPCType = bb.ReadInt32();
                        int npcCenterX = bb.ReadInt32();
                        int npcCenterY = bb.ReadInt32();

                        if (NPC.AnyNPCs(NPCType))
                            return;

                        int npcID = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), npcCenterX, npcCenterY, NPCType);
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    break;
                case ModMessageType.SpawnNPCFromClient:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int npcIndex = bb.ReadInt32();
                        int npcCenterX = bb.ReadInt32();
                        int npcCenterY = bb.ReadInt32();

                        int npcID = NPC.NewNPC(Entity.GetSource_NaturalSpawn(), npcCenterX, npcCenterY, npcIndex);
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    break;
                case ModMessageType.SpawnTrail:
                    int projindex = bb.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        //If received by the server, send to all clients instead
                        WriteToPacket(Instance.GetPacket(), (byte)ModMessageType.SpawnTrail, projindex).Send();
                        break;
                    }

                    if (Main.projectile[projindex].ModProjectile is IManualTrailProjectile trailProj)
                        trailProj.DoTrailCreation(TrailManager);
                    break;
                case ModMessageType.StartFowlMorning:
                    FowlMorningWorld.FowlMorningActive = true;
                    FowlMorningWorld.ChickArmyStart();
                    break;
                case ModMessageType.FowlMorningData:
                    FowlMorningWorld.HandlePacket(bb);
                    break;
            }
        }
        public static void SpawnBossFromClient(byte whoAmI, int type, int x, int y) => WriteToPacket(Instance.GetPacket(), (byte)ModMessageType.BossSpawnFromClient, whoAmI, type, x, y).Send();
    }
    public class RedeSystem : ModSystem
    {
        public static RedeSystem Instance { get; private set; }
        public RedeSystem()
        {
            Instance = this;
        }

        public static bool Silence;

        public override void PostUpdatePlayers()
        {
            Silence = false;
        }

        public UserInterface DialogueUILayer;
        public MoRDialogueUI DialogueUIElement;

        public UserInterface ChaliceUILayer;
        public ChaliceAlignmentUI ChaliceUIElement;

        public UserInterface TitleUILayer;
        public TitleCard TitleCardUIElement;

        public UserInterface NukeUILayer;
        public NukeDetonationUI NukeUIElement;

        public UserInterface AMemoryUILayer;
        public AMemoryUIState AMemoryUIElement;

        public UserInterface TextBubbleUILayer;
        public ChatUI TextBubbleUIElement;

        public UserInterface YesNoUILayer;
        public YesNoUI YesNoUIElement;

        public UserInterface TradeUILayer;
        public TradeUI TradeUIElement;

        public UserInterface AlignmentButtonUILayer;
        public AlignmentButton AlignmentButtonUIElement;

        public UserInterface SpiritWalkerButtonUILayer;
        public SpiritWalkerButton SpiritWalkerButtonUIElement;

        public override void Load()
        {
            RedeDetours.Initialize();
            if (!Main.dedServ)
            {
                TitleUILayer = new UserInterface();
                TitleCardUIElement = new TitleCard();
                TitleUILayer.SetState(TitleCardUIElement);

                DialogueUILayer = new UserInterface();
                DialogueUIElement = new MoRDialogueUI();
                DialogueUILayer.SetState(DialogueUIElement);

                ChaliceUILayer = new UserInterface();
                ChaliceUIElement = new ChaliceAlignmentUI();
                ChaliceUILayer.SetState(ChaliceUIElement);

                NukeUILayer = new UserInterface();
                NukeUIElement = new NukeDetonationUI();
                NukeUILayer.SetState(NukeUIElement);

                AMemoryUILayer = new UserInterface();
                AMemoryUIElement = new AMemoryUIState();
                AMemoryUILayer.SetState(AMemoryUIElement);

                TextBubbleUILayer = new UserInterface();
                TextBubbleUIElement = new ChatUI();
                TextBubbleUILayer.SetState(TextBubbleUIElement);

                YesNoUILayer = new UserInterface();
                YesNoUIElement = new YesNoUI();
                YesNoUILayer.SetState(YesNoUIElement);

                TradeUILayer = new UserInterface();
                TradeUIElement = new TradeUI();
                TradeUILayer.SetState(TradeUIElement);

                AlignmentButtonUILayer = new UserInterface();
                AlignmentButtonUIElement = new AlignmentButton();
                AlignmentButtonUILayer.SetState(AlignmentButtonUIElement);

                SpiritWalkerButtonUILayer = new UserInterface();
                SpiritWalkerButtonUIElement = new SpiritWalkerButton();
                SpiritWalkerButtonUILayer.SetState(SpiritWalkerButtonUIElement);
            }
        }
        public override void ModifyLightingBrightness(ref float scale)
        {
            if (ModContent.GetInstance<RedeTileCount>().WastelandCrimsonTileCount >= 50 || ModContent.GetInstance<RedeTileCount>().WastelandCorruptTileCount >= 50)
                scale = .9f;
        }
        public override void PreUpdateItems()
        {
            if (Main.netMode != NetmodeID.Server)
                Redemption.TrailManager.UpdateTrails();
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            RedeTileCount tileCount = ModContent.GetInstance<RedeTileCount>();
            if (NPC.downedMechBossAny && tileCount.WastelandTileCount > 0)
            {
                float Strength = tileCount.WastelandTileCount / 200f;
                Strength = Math.Min(Strength, 1f);

                int sunR = backgroundColor.R;
                int sunG = backgroundColor.G;
                int sunB = backgroundColor.B;
                sunR -= (int)(40f * Strength * (backgroundColor.R / 255f));
                sunB -= (int)(40f * Strength * (backgroundColor.B / 255f));
                sunG -= (int)(30f * Strength * (backgroundColor.G / 255f));
                sunR = Utils.Clamp(sunR, 15, 255);
                sunG = Utils.Clamp(sunG, 15, 255);
                sunB = Utils.Clamp(sunB, 15, 255);
                backgroundColor.R = (byte)sunR;
                backgroundColor.G = (byte)sunG;
                backgroundColor.B = (byte)sunB;
            }
            if (SubworldSystem.IsActive<CSub>())
            {
                backgroundColor.R = 15;
                backgroundColor.G = 15;
                backgroundColor.B = 15;
                tileColor.R = 15;
                tileColor.G = 15;
                tileColor.B = 15;
            }
        }
        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (Main.gameMenu || RedeConfigClient.Instance.CameraLockDisable)
                return;

            Player player = Main.LocalPlayer;
            ScreenPlayer screenPlayer = player.GetModPlayer<ScreenPlayer>();

            if (screenPlayer.timedZoomDurationMax > 0 && screenPlayer.timedZoom != Vector2.Zero)
            {
                float lerpAmount = MathHelper.Lerp(0, MathHelper.PiOver2, screenPlayer.timedZoomTime / screenPlayer.timedZoomTimeMax);

                Vector2 idealScreenZoom = screenPlayer.timedZoom;
                Transform.Zoom = Vector2.Lerp(new Vector2(1), idealScreenZoom, (float)Math.Sin(lerpAmount));
            }
            if (screenPlayer.customZoom > 0)
                Transform.Zoom = new Vector2(screenPlayer.customZoom);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            BuffPlayer bP = Main.LocalPlayer.GetModPlayer<BuffPlayer>();
            if (Main.LocalPlayer.HasBuff<StunnedDebuff>())
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
                LegacyGameInterfaceLayer StunUI = new("Redemption: Stun UI",
                    delegate
                    {
                        DrawStunStars(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI);
                layers.Insert(index, StunUI);
            }
            if (BasePlayer.HasAccessory(Main.LocalPlayer, ModContent.ItemType<PocketShieldGenerator>(), true, true))
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
                LegacyGameInterfaceLayer ShieldGaugeUI = new("Redemption: Shield Gauge UI",
                    delegate
                    {
                        DrawShieldGenGauge(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI);
                layers.Insert(index, ShieldGaugeUI);
            }
            EnergyPlayer eP = Main.LocalPlayer.GetModPlayer<EnergyPlayer>();
            if (eP.statEnergy < eP.energyMax)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
                LegacyGameInterfaceLayer EnergyGaugeUI = new("Redemption: Energy Gauge UI",
                    delegate
                    {
                        DrawEnergyGauge(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI);
                layers.Insert(index, EnergyGaugeUI);
            }
            if (Main.LocalPlayer.HeldItem.CountsAsClass<DamageClasses.RitualistClass>())
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
                LegacyGameInterfaceLayer SpiritGaugeUI = new("Redemption: Spirit Gauge UI",
                    delegate
                    {
                        DrawSpiritGauge(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI);
                layers.Insert(index, SpiritGaugeUI);
            }
            if (NPC.downedGolemBoss && Main.LocalPlayer.HeldItem.type == ModContent.ItemType<OmegaTransmitter>())
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
                LegacyGameInterfaceLayer OmegaTransmitterUI = new("Redemption: Omega Transmitter UI",
                    delegate
                    {
                        DrawOmegaTransmitterText(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI);
                layers.Insert(index, OmegaTransmitterUI);
            }
            if (YesNoUI.Visible && !Main.playerInventory)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
                LegacyGameInterfaceLayer ChoiceTextUI = new("Redemption: Choice Text UI",
                    delegate
                    {
                        DrawChoiceText(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI);
                layers.Insert(index, ChoiceTextUI);
            }
            if (Main.LocalPlayer.Redemption().slayerCursor)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 4"));
                LegacyGameInterfaceLayer SlayerCursorUI = new("Redemption: Slayer Cursor UI",
                    delegate
                    {
                        DrawSlayerCursor(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI);
                layers.Insert(index, SlayerCursorUI);
            }
            if (RedeWorld.SkeletonInvasion)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
                if (index >= 0)
                {
                    LegacyGameInterfaceLayer SkeleUI = new("Redemption: SkeleInvasion",
                        delegate
                        {
                            DrawSkeletonInvasionUI(Main.spriteBatch);
                            return true;
                        },
                        InterfaceScaleType.UI);
                    layers.Insert(index, SkeleUI);
                }
            }
            if (FowlMorningWorld.FowlMorningActive)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
                if (index >= 0)
                {
                    LegacyGameInterfaceLayer FowlUI = new("Redemption: FowlMorning",
                        delegate
                        {
                            DrawFowlMorningUI(Main.spriteBatch);
                            return true;
                        },
                        InterfaceScaleType.UI);
                    layers.Insert(index, FowlUI);
                }
            }
            layers.Insert(layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text")), new LegacyGameInterfaceLayer("GUI Menus",
                delegate
                {
                    return true;
                }, InterfaceScaleType.UI));
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                AddInterfaceLayer(layers, AMemoryUILayer, AMemoryUIElement, MouseTextIndex, AMemoryUIState.Visible, "Lab Photo");
                AddInterfaceLayer(layers, ChaliceUILayer, ChaliceUIElement, MouseTextIndex + 1, ChaliceAlignmentUI.Visible, "Chalice");
                AddInterfaceLayer(layers, DialogueUILayer, DialogueUIElement, MouseTextIndex + 2, MoRDialogueUI.Visible, "Dialogue");
                AddInterfaceLayer(layers, TitleUILayer, TitleCardUIElement, MouseTextIndex + 3, TitleCard.Showing, "Title Card");
                AddInterfaceLayer(layers, NukeUILayer, NukeUIElement, MouseTextIndex + 4, NukeDetonationUI.Visible, "Nuke UI");
                AddInterfaceLayer(layers, TextBubbleUILayer, TextBubbleUIElement, MouseTextIndex + 5, ChatUI.Visible, "Text Bubble");
                AddInterfaceLayer(layers, YesNoUILayer, YesNoUIElement, MouseTextIndex + 6, YesNoUI.Visible, "Yes No Choice");
                AddInterfaceLayer(layers, TradeUILayer, TradeUIElement, MouseTextIndex + 7, TradeUI.Visible, "Trade");
                AddInterfaceLayer(layers, SpiritWalkerButtonUILayer, SpiritWalkerButtonUIElement, MouseTextIndex + 8, Main.LocalPlayer.RedemptionAbility().Spiritwalker && Main.playerInventory, "Spirit Walker Button");
                AddInterfaceLayer(layers, AlignmentButtonUILayer, AlignmentButtonUIElement, MouseTextIndex + 8, RedeWorld.alignmentGiven && Main.playerInventory, "Alignment Button");
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (AMemoryUILayer?.CurrentState != null && AMemoryUIState.Visible)
                AMemoryUILayer.Update(gameTime);
            if (NukeUILayer?.CurrentState != null && NukeDetonationUI.Visible)
                NukeUILayer.Update(gameTime);
            if (TradeUILayer?.CurrentState != null && TradeUI.Visible)
                TradeUILayer.Update(gameTime);
            if (YesNoUILayer?.CurrentState != null && YesNoUI.Visible)
                YesNoUILayer.Update(gameTime);
            if (AlignmentButtonUILayer?.CurrentState != null && RedeWorld.alignmentGiven && Main.playerInventory)
                AlignmentButtonUILayer.Update(gameTime);
        }
        public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, string customName = null) //Code created by Scalie
        {
            string name;
            if (customName == null)
            {
                name = state.ToString();
            }
            else
            {
                name = customName;
            }
            layers.Insert(index, new LegacyGameInterfaceLayer("Redemption: " + name,
                delegate
                {
                    if (visible)
                    {
                        userInterface.Update(Main._drawInterfaceGameTime);
                        state.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.UI));
        }

        public static void DrawSpiritGauge(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            RitualistPlayer rP = player.GetModPlayer<RitualistPlayer>();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D timerBar = ModContent.Request<Texture2D>("Redemption/UI/SpiritGauge").Value;
            Texture2D timerBarInner = ModContent.Request<Texture2D>("Redemption/UI/SpiritGauge_Fill").Value;
            float timerMax = rP.SpiritGaugeMax;
            int timerProgress = (int)(timerBarInner.Width * (rP.SpiritGauge / timerMax));
            Vector2 drawPos = player.Center + new Vector2(0, 32) - Main.screenPosition;
            spriteBatch.Draw(timerBar, drawPos, null, Color.White, 0f, timerBar.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(timerBarInner, drawPos, new Rectangle?(new Rectangle(0, 0, timerProgress, timerBarInner.Height)), Color.White, 0f, timerBarInner.Size() / 2f, 1f, SpriteEffects.None, 0f);

            Texture2D timerBar2 = ModContent.Request<Texture2D>("Redemption/UI/SpiritGaugeSmall").Value;
            Texture2D timerBarInner2 = ModContent.Request<Texture2D>("Redemption/UI/SpiritGaugeSmall_Fill").Value;
            float timerMax2 = rP.SpiritGaugeCDMax;
            int timerProgress2 = (int)(timerBarInner2.Width * (rP.SpiritGaugeCD / timerMax2));
            Vector2 drawPos2 = player.Center + new Vector2(0, 41) - Main.screenPosition;
            spriteBatch.Draw(timerBar2, drawPos2, null, Color.White, 0f, timerBar2.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(timerBarInner2, drawPos2, new Rectangle?(new Rectangle(0, 0, timerProgress2, timerBarInner2.Height)), Color.White, 0f, timerBarInner2.Size() / 2f, 1f, SpriteEffects.None, 0f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, (rP.SpiritLevel + 1).ToString(), player.Center + new Vector2(-46, 36) - Main.screenPosition, Color.White, 0, Vector2.Zero, Vector2.One);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static void DrawShieldGenGauge(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            BuffPlayer bP = player.GetModPlayer<BuffPlayer>();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D timerBar = ModContent.Request<Texture2D>("Redemption/UI/ShieldGauge").Value;
            Texture2D timerBarInner = ModContent.Request<Texture2D>("Redemption/UI/ShieldGauge_Fill").Value;
            Texture2D timerBarInner2 = ModContent.Request<Texture2D>("Redemption/UI/ShieldGauge_Fill2").Value;
            float timerMax = 200;
            int timerProgress = (int)(timerBarInner.Width * (bP.shieldGeneratorLife / timerMax));
            int timerProgress2 = (int)(timerBarInner.Width * (bP.shieldGeneratorCD / 3600f));
            Vector2 drawPos = player.Center - new Vector2(0, 60) - Main.screenPosition;
            spriteBatch.Draw(timerBar, drawPos, null, Color.White, 0f, timerBar.Size() / 2f, 1f, SpriteEffects.None, 0f);
            if (bP.shieldGeneratorCD <= 0)
                spriteBatch.Draw(timerBarInner, drawPos, new Rectangle?(new Rectangle(0, 0, timerProgress, timerBarInner.Height)), Color.White, 0f, timerBarInner.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(timerBarInner2, drawPos, new Rectangle?(new Rectangle(0, 0, timerProgress2, timerBarInner.Height)), Color.White * .5f, 0f, timerBarInner.Size() / 2f, 1f, SpriteEffects.None, 0f);

            Texture2D shieldTex = ModContent.Request<Texture2D>("Redemption/Textures/BubbleShield").Value;
            Vector2 drawOrigin = new(shieldTex.Width / 2, shieldTex.Height / 2);

            if (bP.shieldGeneratorCD <= 0)
                spriteBatch.Draw(shieldTex, player.Center - Main.screenPosition, null, Color.White * ((float)bP.shieldGeneratorLife / 200) * (bP.shieldGeneratorAlpha + 0.3f), 0, drawOrigin, 0.5f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static void DrawEnergyGauge(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            EnergyPlayer eP = player.GetModPlayer<EnergyPlayer>();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D timerBar = ModContent.Request<Texture2D>("Redemption/UI/EnergyGauge").Value;
            Texture2D timerBarInner = ModContent.Request<Texture2D>("Redemption/UI/EnergyGauge_Fill").Value;
            float timerMax = eP.energyMax;
            int timerProgress = (int)(timerBarInner.Height * (eP.statEnergy / timerMax));
            Vector2 drawPos = player.Center + new Vector2(40, 0) - Main.screenPosition;
            spriteBatch.Draw(timerBar, drawPos, null, Color.White * 0.75f, 0f, timerBar.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(timerBarInner, drawPos, new Rectangle?(new Rectangle(0, 0, timerBarInner.Width, timerProgress)), RedeColor.EnergyPulse * 0.75f, MathHelper.Pi, timerBarInner.Size() / 2f, 1f, SpriteEffects.None, 0f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, ((int)(eP.statEnergy / timerMax * 100)).ToString() + "%", player.Center + new Vector2(30, -36) - Main.screenPosition, Color.White * 0.75f, 0, Vector2.Zero, Vector2.One * 0.75f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static void DrawStunStars(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D starTex = ModContent.Request<Texture2D>("Redemption/Textures/StunVisual").Value;
            int height = starTex.Height / 4;
            int y = height * player.RedemptionPlayerBuff().stunFrame;
            Vector2 drawOrigin = new(starTex.Width / 2, height / 2);

            spriteBatch.Draw(starTex, player.Center - new Vector2(0, 34) - Main.screenPosition, new Rectangle?(new Rectangle(0, y, starTex.Width, height)), Color.White * ((255 - Main.BlackFadeIn) / 255f), 0, drawOrigin, 1, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static void DrawOmegaTransmitterText(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Right-click to switch Prototype", player.Center + new Vector2(-118, 36) - Main.screenPosition, Color.Red, 0, Vector2.Zero, Vector2.One);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
        public static void DrawChoiceText(SpriteBatch spriteBatch)
        {
            string text = "Open Inventory to make your choice";
            int textLength = (int)(FontAssets.DeathText.Value.MeasureString(text).X * .5f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, new Vector2((Main.screenWidth / 2) - (textLength / 2), Main.screenHeight / 4), Color.White, 0, Vector2.Zero, Vector2.One * .5f);
        }
        public static void DrawSlayerCursor(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Redemption/Textures/SlayerCursor").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 0.9f, 1f);

            spriteBatch.Draw(texture, Main.MouseWorld - Main.screenPosition, null, Color.White, 0, drawOrigin, scale, SpriteEffects.None, 0);
        }
        #region Skele Invasion UI
        public static void DrawSkeletonInvasionUI(SpriteBatch spriteBatch)
        {
            float alpha = .5f;
            Texture2D backGround1 = TextureAssets.ColorBar.Value;
            Texture2D progressColor = TextureAssets.ColorBar.Value;
            Texture2D InvIcon = ModContent.Request<Texture2D>("Redemption/Items/Armor/Vanity/EpidotrianSkull").Value;
            float scmp = .875f;
            Color descColor = new(77, 39, 135);
            Color waveColor = new(255, 241, 51);
            const int offsetX = 20;
            const int offsetY = 20;
            int width = (int)(200f * scmp);
            int height = (int)(52f * scmp);
            Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 23f), new Vector2(width, height));
            Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);
            float cleared = (float)Main.time / 16200;
            string waveText = "Until Party's Over: " + Math.Round(100 * cleared) + "%";
            Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.X + waveBackground.Width / 2, waveBackground.Y + 1), Color.White, scmp, 0.5f, -0.1f);
            Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X + waveBackground.Width * 0.5f, waveBackground.Y + waveBackground.Height * 0.75f), new Vector2(progressColor.Width, progressColor.Height));
            Rectangle waveProgressAmount = new(0, 0, (int)(progressColor.Width * MathHelper.Clamp(cleared, 0f, 1f)), progressColor.Height);
            Vector2 offset = new((waveProgressBar.Width - (int)(waveProgressBar.Width * scmp)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scmp)) * 0.5f);
            spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, null, Color.White * alpha, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
            spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
            const int internalOffset = 6;
            Vector2 descSize = new Vector2(154, 40) * scmp;
            Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 19f), new Vector2(width, height));
            Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), descSize * .8f);
            Utils.DrawInvBG(spriteBatch, descBackground, descColor * alpha);
            int descOffset = (descBackground.Height - 20) / 2;
            Rectangle icon = new(descBackground.X + descOffset + 5, descBackground.Y + descOffset, 18, 20);
            spriteBatch.Draw(InvIcon, icon, Color.White);
            Utils.DrawBorderString(spriteBatch, "Raveyard", new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), Color.White, scmp, 0.4f, 0.4f);
        }
        #endregion
        #region Fowl Morning UI
        public static void DrawFowlMorningUI(SpriteBatch spriteBatch)
        {
            float alpha = .5f;
            Texture2D backGround1 = TextureAssets.ColorBar.Value;
            Texture2D progressColor = TextureAssets.ColorBar.Value;
            Texture2D InvIcon = ModContent.Request<Texture2D>("Redemption/Gores/Boss/FowlEmperor_Crown").Value;
            float scmp = .875f;
            Color descColor = new(104, 70, 6);
            Color waveColor = new(255, 241, 51);
            const int offsetX = 20;
            const int offsetY = 40;
            int width = (int)(200f * scmp);
            int height = (int)(52f * scmp);
            Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 23f), new Vector2(width, height));
            Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);
            float cleared = FowlMorningWorld.ChickPoints / (float)FowlMorningNPC.maxPoints;
            string waveText = "Wave " + (FowlMorningWorld.ChickWave + 1) + ": " + Math.Round(100 * cleared) + "%";
            Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.X + waveBackground.Width / 2, waveBackground.Y + 3), Color.White, 1, 0.5f, -0.1f);
            Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X + waveBackground.Width * 0.5f, waveBackground.Y + waveBackground.Height * 0.75f), new Vector2(progressColor.Width, progressColor.Height));
            Rectangle waveProgressAmount = new(0, 0, (int)(progressColor.Width * MathHelper.Clamp(cleared, 0f, 1f)), progressColor.Height);
            Vector2 offset = new((waveProgressBar.Width - (int)(waveProgressBar.Width * scmp)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scmp)) * 0.5f);
            spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, null, Color.Black, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
            spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
            const int internalOffset = -1;
            Vector2 descSize = new Vector2(188, 50) * scmp;
            Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 19f), new Vector2(width, height));
            Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), descSize * .8f);
            Utils.DrawInvBG(spriteBatch, descBackground, descColor * alpha);
            int descOffset = (descBackground.Height - (int)(32f * scmp)) / 2;
            Rectangle icon = new(descBackground.X + descOffset, descBackground.Y + descOffset, 22, 24);
            spriteBatch.Draw(InvIcon, icon, Color.White);
            Utils.DrawBorderString(spriteBatch, "Fowl Morning", new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), Color.White, scmp, 0.4f, 0.4f);
        }
        #endregion
    }
}
