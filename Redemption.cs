using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Redemption.Backgrounds.Skies;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Effects.RenderTargets;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Armor.PostML.Shinkite;
using Redemption.Items.Armor.PreHM.DragonLead;
using Redemption.Items.Donator.Arche;
using Redemption.Items.Donator.Uncon;
using Redemption.Items.Usable;
using Redemption.UI;
using ReLogic.Content;
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
using static Redemption.Globals.RedeNet;

namespace Redemption
{
    public class Redemption : Mod
    {
        public static Redemption Instance { get; private set; }

        public const string Abbreviation = "MoR";
        public const string EMPTY_TEXTURE = "Redemption/Empty";
        public Vector2 cameraOffset;
        public static ModKeybind RedeSpecialAbility;
        public static ModKeybind RedeSpiritwalkerAbility;

        public static RenderTargetManager Targets;

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

        public Redemption()
        {
            Instance = this;
        }

        public override void Load()
        {
            LoadCache();

            if (!Main.dedServ)
            {
                dragonLeadCapeID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Armor/PreHM/DragonLead/DragonLeadRibplate_Back", EquipType.Back, ModContent.GetInstance<DragonLeadRibplate>());
                shinkiteCapeID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Armor/PostML/Shinkite/ShinkiteChestplate_Back", EquipType.Back, ModContent.GetInstance<ShinkiteChestplate>());
                archeMaleLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Arche/ArchePatreonVanityLegs_Legs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<ArchePatreonVanityLegs>()));
                archeFemLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Arche/ArchePatreonVanityLegs_FemaleLegs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<ArchePatreonVanityLegs>()));
                unconMaleLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Uncon/UnconLegs_Legs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs>()));
                unconFemLegID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Uncon/UnconLegs_FemaleLegs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs>()));
                unconMaleLeg2ID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Uncon/UnconLegs2_Legs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs2>()));
                unconFemLeg2ID = EquipLoader.AddEquipTexture(this, "Redemption/Items/Donator/Uncon/UnconLegs2_FemaleLegs", EquipType.Legs, ModContent.GetModItem(ModContent.ItemType<UnconLegs2>()));

                Main.QueueMainThreadAction(() =>
                {
                    Texture2D bubbleTex = ModContent.Request<Texture2D>("Redemption/Textures/BubbleShield", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref bubbleTex);
                    Texture2D portalTex = ModContent.Request<Texture2D>("Redemption/Textures/PortalTex", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref portalTex);
                    Texture2D soullessPortal = ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/SoullessPortal", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref soullessPortal);
                    Texture2D holyGlowTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref holyGlowTex);
                    Texture2D whiteFlareTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref whiteFlareTex);
                    Texture2D whiteOrbTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteOrb", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref whiteOrbTex);
                    Texture2D whiteLightBeamTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteLightBeam", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref whiteLightBeamTex);
                    Texture2D transitionTex = ModContent.Request<Texture2D>("Redemption/Textures/TransitionTex", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref transitionTex);
                    Texture2D staticBallTex = ModContent.Request<Texture2D>("Redemption/Textures/StaticBall", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref staticBallTex);
                    Texture2D iceMistTex = ModContent.Request<Texture2D>("Redemption/Textures/IceMist", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref iceMistTex);
                    Texture2D glowDustTex = ModContent.Request<Texture2D>("Redemption/Dusts/GlowDust", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref glowDustTex);

                    Texture2D purityWastelandBG3Tex = ModContent.Request<Texture2D>("Redemption/Backgrounds/PurityWastelandBG3", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref purityWastelandBG3Tex);
                    Texture2D wastelandCrimsonBG3Tex = ModContent.Request<Texture2D>("Redemption/Backgrounds/WastelandCrimsonBG3", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref wastelandCrimsonBG3Tex);
                    Texture2D wastelandCorruptionBG3Tex = ModContent.Request<Texture2D>("Redemption/Backgrounds/WastelandCorruptionBG3", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref wastelandCorruptionBG3Tex);
                    Texture2D ruinedKingdomSurfaceClose_MenuTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/RuinedKingdomSurfaceClose_Menu", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref ruinedKingdomSurfaceClose_MenuTex);
                    Texture2D ruinedKingdomSurfaceFar_MenuTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/RuinedKingdomSurfaceFar_Menu", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref ruinedKingdomSurfaceFar_MenuTex);
                    Texture2D ruinedKingdomSurfaceMid_MenuTex = ModContent.Request<Texture2D>("Redemption/Backgrounds/RuinedKingdomSurfaceMid_Menu", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref ruinedKingdomSurfaceMid_MenuTex);
                });
            }

            Filters.Scene["MoR:WastelandSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0.2f, 0f).UseOpacity(0.5f), EffectPriority.High);
            Filters.Scene["MoR:SpiritSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.8f, 0.8f), EffectPriority.VeryHigh);
            Filters.Scene["MoR:IslandEffect"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.4f, 0.4f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["MoR:RuinedKingdomSky"] = new RuinedKingdomSky();
            Filters.Scene["MoR:SoullessSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0.55f), EffectPriority.High);

            RedeSpecialAbility = KeybindLoader.RegisterKeybind(this, "Special Ability Key", Keys.R);
            RedeSpiritwalkerAbility = KeybindLoader.RegisterKeybind(this, "Spirit Walker Key", Keys.K);
            AntiqueDorulCurrencyId = CustomCurrencyManager.RegisterCurrency(new AntiqueDorulCurrency(ModContent.ItemType<AncientGoldCoin>(), 999L, "Antique Doruls"));
        }

        public override void PostSetupContent()
        {
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
                        {
                            return;
                        }

                        int npcID = NPC.NewNPC(null, npcCenterX, npcCenterY, bossType);
                        Main.npc[npcID].Center = new Vector2(npcCenterX, npcCenterY);
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
                        {
                            return;
                        }

                        int npcID = NPC.NewNPC(null, npcCenterX, npcCenterY, NPCType);
                        Main.npc[npcID].Center = new Vector2(npcCenterX, npcCenterY);
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    break;
                case ModMessageType.NPCSpawnFromClient2:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int NPCType = bb.ReadInt32();
                        int npcCenterX = bb.ReadInt32();
                        int npcCenterY = bb.ReadInt32();

                        int npcID = NPC.NewNPC(null, npcCenterX, npcCenterY, NPCType);
                        Main.npc[npcID].Center = new Vector2(npcCenterX, npcCenterY);
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    break;
                case ModMessageType.SpawnTrail:
                    int projindex = bb.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        WriteToPacket(GetPacket(), (byte)ModMessageType.SpawnTrail, projindex).Send();
                        break;
                    }

                    if (Main.projectile[projindex].ModProjectile is ITrailProjectile trailproj)
                        trailproj.DoTrailCreation(RedeSystem.TrailManager);

                    break;
                    /*case ModMessageType.StartChickArmy:
                        ChickWorld.chickArmy = true;
                        ChickWorld.ChickArmyStart();
                        break;
                    case ModMessageType.ChickArmyData:
                        ChickWorld.HandlePacket(bb);
                        break;*/
            }
        }
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
        public TextBubbleUI TextBubbleUIElement;

        public static TrailManager TrailManager;
        public bool Initialized;

        public override void Load()
        {
            RedeDetours.Initialize();
            if (!Main.dedServ)
            {
                On.Terraria.Main.Update += LoadTrailManager;

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
				TextBubbleUIElement = new TextBubbleUI();
                TextBubbleUILayer.SetState(TextBubbleUIElement);
			}
        }
        private void LoadTrailManager(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime)
        {
            if (!Initialized)
            {
                TrailManager = new TrailManager(Redemption.Instance);
                Initialized = true;
            }

            orig(self, gameTime);
        }

        public override void Unload()
        {
            TrailManager = null;
            On.Terraria.Main.Update -= LoadTrailManager;
        }

        public override void ModifyLightingBrightness(ref float scale)
        {
            if (ModContent.GetInstance<RedeTileCount>().WastelandCrimsonTileCount >= 50 || ModContent.GetInstance<RedeTileCount>().WastelandCorruptTileCount >= 50)
                scale = 0.9f;
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            RedeTileCount tileCount = ModContent.GetInstance<RedeTileCount>();
            if (tileCount.WastelandTileCount > 0)
            {
                float Strength = tileCount.WastelandTileCount / 200f;
                Strength = Math.Min(Strength, 1f);

                int sunR = backgroundColor.R;
                int sunG = backgroundColor.G;
                int sunB = backgroundColor.B;
                sunR -= (int)(200 * Strength * (backgroundColor.R / 255f));
                sunB -= (int)(200f * Strength * (backgroundColor.B / 255f));
                sunG -= (int)(170f * Strength * (backgroundColor.G / 255f));
                sunR = Utils.Clamp(sunR, 15, 255);
                sunG = Utils.Clamp(sunG, 15, 255);
                sunB = Utils.Clamp(sunB, 15, 255);
                backgroundColor.R = (byte)sunR;
                backgroundColor.G = (byte)sunG;
                backgroundColor.B = (byte)sunB;
            }
        }
        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (Main.gameMenu)
                return;

            Player player = Main.LocalPlayer;
            ScreenPlayer screenPlayer = player.GetModPlayer<ScreenPlayer>();

            if (screenPlayer.timedZoomDurationMax > 0 && screenPlayer.timedZoom != Vector2.Zero)
            {
                float lerpAmount = MathHelper.Lerp(0, MathHelper.PiOver2, screenPlayer.timedZoomTime / screenPlayer.timedZoomTimeMax);

                Vector2 idealScreenZoom = screenPlayer.timedZoom;
                Transform.Zoom = Vector2.Lerp(new Vector2(1), idealScreenZoom, (float)Math.Sin(lerpAmount));
            }
        }

        public override void PreUpdateProjectiles()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                TrailManager.UpdateTrails();
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (RedeWorld.SkeletonInvasion)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
                if (index >= 0)
                {
                    LegacyGameInterfaceLayer SkeleUI = new ("Redemption: SkeleInvasion",
                        delegate
                        {
                            DrawSkeletonInvasionUI(Main.spriteBatch);
                            return true;
                        },
                        InterfaceScaleType.UI);
                    layers.Insert(index, SkeleUI);
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
				AddInterfaceLayer(layers, TextBubbleUILayer, TextBubbleUIElement, MouseTextIndex + 5, TextBubbleUI.Visible, "Text Bubble");
			}
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

        #region Skele Invasion UI
        public static void DrawSkeletonInvasionUI(SpriteBatch spriteBatch)
        {
            if (RedeWorld.SkeletonInvasion)
            {
                float alpha = 0.5f;
                Texture2D backGround1 = TextureAssets.ColorBar.Value;
                Texture2D progressColor = TextureAssets.ColorBar.Value;
                Texture2D InvIcon = ModContent.Request<Texture2D>("Redemption/Items/Armor/Vanity/EpidotrianSkull").Value;
                float scmp = 0.5f + 0.75f * 0.5f;
                Color descColor = new (77, 39, 135);
                Color waveColor = new (255, 241, 51);
                const int offsetX = 20;
                const int offsetY = 20;
                int width = (int)(200f * scmp);
                int height = (int)(46f * scmp);
                Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 23f), new Vector2(width, height));
                Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);
                float cleared = (float)Main.time / 16200;
                string waveText = "Cleared " + Math.Round(100 * cleared) + "%";
                Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.X + waveBackground.Width / 2, waveBackground.Y + 5), Color.White, scmp * 0.8f, 0.5f, -0.1f);
                Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X + waveBackground.Width * 0.5f, waveBackground.Y + waveBackground.Height * 0.75f), new Vector2(progressColor.Width, progressColor.Height));
                Rectangle waveProgressAmount = new (0, 0, (int)(progressColor.Width * MathHelper.Clamp(cleared, 0f, 1f)), progressColor.Height);
                Vector2 offset = new ((waveProgressBar.Width - (int)(waveProgressBar.Width * scmp)) *0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scmp)) * 0.5f);
                spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, null, Color.White * alpha, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
                spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
                const int internalOffset = 6;
                Vector2 descSize = new Vector2(154, 40) * scmp;
                Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 19f), new Vector2(width, height));
                Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), descSize * .8f);
                Utils.DrawInvBG(spriteBatch, descBackground, descColor * alpha);
                int descOffset = (descBackground.Height - (int)(32f * scmp)) / 2;
                Rectangle icon = new (descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * scmp), (int)(32 * scmp));
                spriteBatch.Draw(InvIcon, icon, Color.White);
                Utils.DrawBorderString(spriteBatch, "Raveyard", new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
            }
        }
        #endregion
    }
}
