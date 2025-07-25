using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Armor.PreHM.PureIron;
using Redemption.Items.Armor.Single;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Friendly;
using Redemption.Textures;
using Redemption.Tiles.Furniture.Misc;
using Redemption.UI;
using Redemption.UI.ChatUI;
using Redemption.UI.Dialect;
using Redemption.WorldGeneration;
using ReLogic.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Redemption.NPCs.Minibosses.Calavia
{
    [AutoloadHead]
    public class Calavia_NPC : ModRedeNPC
    {
        public override string Texture => "Redemption/NPCs/Minibosses/Calavia/Calavia";
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Calavia");
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[Type] = 20;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 48;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.friendly = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.npcSlots = 0;
            NPC.townNPC = true;
            TownNPCStayingHomeless = true;
            NPC.dontTakeDamage = true;

            DialogueBoxStyle = EPIDOTRA;
        }
        public override bool HasCruxButton(Player player) => RedeQuest.calaviaVar is 16 && !player.HasItem(ItemType<CruxCardCalavia>());
        public override string CruxButtonText(Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Crux");
        public override void CruxButton(Player player)
        {
            RequestCruxButton.RequestCrux(NPC, player, ItemType<CruxCardCalavia>(), "Calavia.NoCruxDialogue", "Calavia.CruxDialogue");
        }
        public override bool HasLeftHangingButton(Player player) => !RedeGlobalButton.talkActive && RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20;
        public override bool HasRightHangingButton(Player player) => !RedeGlobalButton.talkActive && RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20;

        private static Texture2D Bubble => !Main.dedServ ? CommonTextures.TextBubble_Epidotra.Value : null;
        private static Texture2D Bubble2 => !Main.dedServ ? CommonTextures.TextBubble_Cave.Value : null;
        private static readonly SoundStyle voice = CustomSounds.Voice1 with { Pitch = 0.6f };

        public bool HasShield;
        public int HasHelmet;
        public override void LoadData(TagCompound tag)
        {
            HasShield = tag.GetBool("HasShield");
            HasHelmet = tag.GetInt("HasHelmet");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["HasShield"] = HasShield;
            tag["HasHelmet"] = HasHelmet;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasShield);
            writer.Write((byte)HasHelmet);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasShield = reader.ReadBoolean();
            HasHelmet = reader.ReadByte();
        }

        public override bool UsesPartyHat() => false;
        public override bool CanChat() => RedeQuest.calaviaVar < 21 && RedeQuest.calaviaVar != 15;
        public override bool CheckActive() => false;
        readonly DialogueChain chain = new();
        public override void AI()
        {
            if (++NPC.breath <= 0)
                NPC.breath = 9000;

            if (NPC.target < 0 || NPC.target >= 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            int spiritWhoAmI = NPC.FindFirstNPC(NPCType<SpiritWalkerMan>());
            if (RedeQuest.calaviaVar == 14 && Main.LocalPlayer.talkNPC == -1)
            {
                if (spiritWhoAmI > -1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    RedeQuest.calaviaVar = 15;
                    RedeQuest.SyncData();
                }
            }
            else if (RedeQuest.calaviaVar is 15)
            {
                if (spiritWhoAmI > -1)
                {
                    NPC spirit = Main.npc[spiritWhoAmI];
                    SoundStyle spiritVoice = CustomSounds.Voice2;

                    if (AITimer++ == 0)
                    {
                        spirit.LookAtEntity(NPC);
                        EmoteBubble.NewBubble(3, new WorldUIAnchor(spirit), 60);
                    }
                    if (AITimer == 60)
                        NPC.LookAtEntity(spirit);

                    if (AITimer == 60 && chain.Dialogue.Count == 0)
                    {
                        string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.1");
                        string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.2");
                        string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.3");
                        string s4 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.4");
                        string s5 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.5");
                        string s6 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.6");
                        string s7 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.7");
                        string s8 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.8");
                        string s9 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.9");
                        string s10 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.10");
                        string s11 = Language.GetTextValue("Mods.Redemption.Cutscene.Calavia.Talk.11");

                        chain.Add(new(spirit, s1, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble2))
                             .Add(new(spirit, s2, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble2))
                             .Add(new(NPC, s3, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s4, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble2))
                             .Add(new(NPC, s5, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s6, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble2))
                             .Add(new(spirit, s7, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble2))
                             .Add(new(NPC, s8, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s9, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble2))
                             .Add(new(NPC, s10, Color.White, Color.Gray, voice, .05f, 2f, 0, false, bubble: Bubble))
                             .Add(new(spirit, s11, Color.LightBlue, Color.DarkBlue, spiritVoice, .05f, 2f, 0, false, bubble: Bubble2, endID: 1));
                        chain.OnSymbolTrigger += Chain_OnSymbolTrigger;
                        chain.OnEndTrigger += Chain_OnEndTrigger;
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer >= 10000)
                    {
                        AITimer = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            RedeQuest.calaviaVar = 16;
                            RedeQuest.SyncData();
                        }
                        NPC.netUpdate = true;
                    }
                    if (AITimer >= 60)
                    {
                        ScreenPlayer.CutsceneLock(Main.LocalPlayer, NPC, ScreenPlayer.CutscenePriority.Low, 800, 1200, 800);
                    }
                }
                else
                {
                    ChatUI.Remove(chain);
                    chain.Dialogue.Clear();
                    ChatUI.Visible = false;
                    AITimer = 0;
                    NPC.netUpdate = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        RedeQuest.calaviaVar = 13;
                        RedeQuest.SyncData();
                    }
                }
                if (player.DistanceSQ(NPC.Center) > 2000 * 2000)
                {
                    ChatUI.Remove(chain);
                    chain.Dialogue.Clear();
                    ChatUI.Visible = false;
                    AITimer = 0;
                    NPC.netUpdate = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        RedeQuest.calaviaVar = 13;
                        RedeQuest.SyncData();
                    }
                }
            }
            else if (RedeQuest.calaviaVar == 20 && Main.LocalPlayer.talkNPC == -1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (HasShield && HasHelmet > 0)
                        RedeQuest.calaviaVar = 22;
                    else
                        RedeQuest.calaviaVar = 21;

                    RedeQuest.SyncData();
                }
            }
            else if (RedeQuest.calaviaVar > 20)
            {
                Vector2 gathicPortalPos = new(((RedeGen.gathicPortalVector.X + 51) * 16) - 8, (RedeGen.gathicPortalVector.Y + 18) * 16);
                if (AITimer++ == 60)
                {
                    NPC.velocity.Y = -7;
                    NPC.velocity.X = 2;
                }
                if (AITimer >= 120)
                {
                    NPC.noTileCollide = true;
                    NPC.Move(gathicPortalPos, 20, 30);
                    NPC.alpha += 5;
                }
                if (AITimer >= 60)
                {
                    NPC.rotation += 0.1f;
                    NPC.velocity.X *= 0.99f;

                    if (NPC.DistanceSQ(gathicPortalPos) < 60 * 60)
                        NPC.alpha += 10;
                    if (NPC.alpha >= 255)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, NPC.position);
                        for (int i = 0; i < 30; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustType<GlowDust>(), 1, 0, 0, default, 0.5f);
                            Main.dust[dust].noGravity = true;
                            Color dustColor = new(Color.LightBlue.R, Color.LightBlue.G, Color.LightBlue.B) { A = 0 };
                            Main.dust[dust].color = dustColor;
                            Main.dust[dust].velocity *= 3f;
                        }

                        if (HasHelmet > 0 && HasShield)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                RedeWorld.Alignment++;
                        }
                        NPC.active = false;
                    }
                }
                return;
            }
            if (NPC.Sight(player, 600, false, true) && RedeQuest.calaviaVar != 15)
                NPC.LookAtEntity(player);
        }
        private void Chain_OnSymbolTrigger(Dialogue dialogue, string signature)
        {
            int spiritWhoAmI = NPC.FindFirstNPC(NPCType<SpiritWalkerMan>());
            if (spiritWhoAmI == -1)
                return;
            NPC spirit = Main.npc[spiritWhoAmI];
            switch (signature)
            {
                case "b":
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
                    break;
                case "c":
                    EmoteBubble.NewBubble(2, new WorldUIAnchor(spirit), 120);
                    break;
                case "d":
                    spirit.spriteDirection *= -1;
                    break;
                case "g":
                    spirit.spriteDirection *= -1;
                    break;
                case "e":
                    NPC.spriteDirection *= -1;
                    break;
                case "f":
                    NPC.spriteDirection *= -1;
                    break;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 10000;
        }
        public override string GetChat()
        {
            Player player = Main.LocalPlayer;
            if (RedeQuest.calaviaVar > 10)
            {
                WeightedRandom<string> chat = new();
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat2"));
                if (!player.RedemptionAbility().Spiritwalker)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat3"));
                else if (!NPC.AnyNPCs(NPCType<SpiritWalkerMan>()))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat4"));
                if (BasePlayer.HasArmorSet(Mod, player, "Iron Realm", true) || BasePlayer.HasArmorSet(Mod, player, "Iron Realm", false))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat5"));
                else if (BasePlayer.HasArmorSet(Mod, player, "Common Guard", true) || BasePlayer.HasArmorSet(Mod, player, "Common Guard", false))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat6"));
                else if (BasePlayer.HasArmorSet(Mod, player, "Dragon-Lead", true) || BasePlayer.HasArmorSet(Mod, player, "Dragon-Lead", false))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat7"));

                return chat;
            }
            return Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.Chat1");
        }
        public override bool CanGoToStatue(bool toKingStatue) => false;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
            {
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < 6 * frameHeight)
                        NPC.frame.Y = 6 * frameHeight;
                    NPC.frameCounter += NPC.velocity.X * 0.5f;
                    if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 19 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                }
            }
            else
                NPC.frame.Y = 5 * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Request<Texture2D>(Texture + "2").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(Calavia.CloakTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Calavia.LegsTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (HasHelmet > 0)
            {
                var effects2 = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Texture2D helmet = Request<Texture2D>("Redemption/Items/Armor/PreHM/PureIron/PureIronHelmet_Head").Value;
                if (HasHelmet == 2)
                    helmet = Request<Texture2D>("Redemption/Items/Armor/Single/AntiquePureIronHelmet_Head").Value;
                spriteBatch.Draw(helmet, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects2, 0);
            }
            if (HasShield)
                spriteBatch.Draw(Calavia.ShieldTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(Calavia.ArmTex.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
    public class CalaviaIntroButton1 : ChatButton
    {
        public override double Priority => 1.0;
        public override string Text(NPC npc, Player player)
        {
            return RedeQuest.calaviaVar switch
            {
                4 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.2"),
                5 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.3"),
                6 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.4"),
                7 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.5"),
                8 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.6"),
                9 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.7"),
                10 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.8"),
                20 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.9"),
                _ => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.1"),
            };
        }
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Calavia_NPC>() && (RedeQuest.calaviaVar < 11 || RedeQuest.calaviaVar == 20);
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            switch (RedeQuest.calaviaVar)
            {
                default:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine1");
                    RedeQuest.calaviaVar = 4;
                    RedeQuest.SyncData();
                    break;
                case 4:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine2");
                    RedeQuest.calaviaVar = 6;
                    RedeQuest.SyncData();
                    break;
                case 5:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine2");
                    RedeQuest.calaviaVar = 6;
                    RedeQuest.SyncData();
                    break;
                case 6:
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(npc), 120);
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine4");
                    RedeQuest.calaviaVar = 7;
                    RedeQuest.SyncData();
                    break;
                case 7:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine5");
                    if (RedeGen.cryoCrystalSpawn)
                        Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine5B");

                    if (RedeGen.cryoCrystalSpawn)
                        RedeQuest.calaviaVar = 8;
                    else
                        RedeQuest.calaviaVar = 11;

                    RedeQuest.SyncData();
                    break;
                case 8:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine6");
                    RedeQuest.calaviaVar = 9;
                    RedeQuest.SyncData();
                    break;
                case 9:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine8");
                    RedeQuest.calaviaVar = 11;
                    RedeQuest.SyncData();
                    break;
                case 10:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine10");
                    RedeQuest.calaviaVar = 11;
                    RedeQuest.SyncData();
                    break;
                case 20:
                    Main.CloseNPCChatOrSign();
                    if (npc.ModNPC is Calavia_NPC calavia && calavia.HasShield && calavia.HasHelmet > 0)
                        RedeQuest.calaviaVar = 22;
                    else
                        RedeQuest.calaviaVar = 21;
                    RedeQuest.SyncData();
                    break;
            }
        }
    }
    public class CalaviaIntroButton2 : ChatButton
    {
        public override double Priority => 2.0;
        public override string Text(NPC npc, Player player)
        {
            return RedeQuest.calaviaVar switch
            {
                6 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.4Alt"),
                8 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.6Alt"),
                9 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.7Alt"),
                10 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Leave.Name"),
                20 => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.9Alt"),
                _ => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.2Alt"),
            };
        }
        public override string Description(NPC npc, Player player) => RedeQuest.calaviaVar != 10 ? string.Empty : Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Leave.Description");

        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Calavia_NPC>() && RedeQuest.calaviaVar is 4 or 6 or 8 or 9 or 10 or 20;

        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.calaviaVar != 10 ? null : RedeColor.TextCaution;

        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            switch (RedeQuest.calaviaVar)
            {
                default:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine3");
                    RedeQuest.calaviaVar = 5;
                    RedeQuest.SyncData();
                    break;
                case 6:
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(npc), 120);
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine4");
                    RedeQuest.calaviaVar = 7;
                    RedeQuest.SyncData();
                    break;
                case 8:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine7");
                    RedeQuest.calaviaVar = 9;
                    RedeQuest.SyncData();
                    break;
                case 9:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine9");
                    RedeQuest.calaviaVar = 10;
                    RedeQuest.SyncData();
                    break;
                case 10:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine11");
                    RedeQuest.calaviaVar = 20;
                    RedeQuest.SyncData();
                    break;
                case 20:
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine12");
                    RedeQuest.calaviaVar = 11;
                    RedeQuest.SyncData();
                    break;
            }
        }
    }
    public class RequireButton_Calavia : ChatButton
    {
        public override double Priority => 1.0;
        public override string Text(NPC npc, Player player)
        {
            if (npc.ModNPC is Calavia_NPC cal)
            {
                if (cal.HasHelmet > 0 && cal.HasShield)
                    return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Ready");

                if (!cal.HasShield && Main.LocalPlayer.CountItem(ItemType<PureIronAlloy>()) >= 6)
                    return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Alloy");
                if (cal.HasHelmet == 0 && Main.LocalPlayer.HasItem(ItemType<AntiquePureIronHelmet>()))
                    return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.AntiqueHelmet");
                if (cal.HasHelmet == 0 && Main.LocalPlayer.HasItem(ItemType<PureIronHelmet>()))
                    return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Helmet");
            }
            return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Require");
        }
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Calavia_NPC>() && RedeGen.cryoCrystalSpawn && RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            if (npc.ModNPC is Calavia_NPC cal)
            {
                if (cal.HasHelmet > 0 && cal.HasShield)
                {
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ReadyLine");
                    return;
                }
                int pureIronAlloy = Main.LocalPlayer.FindItem(ItemType<PureIronAlloy>());
                int pureIronHelm = Main.LocalPlayer.FindItem(ItemType<PureIronHelmet>());
                int pureIronHelm2 = Main.LocalPlayer.FindItem(ItemType<AntiquePureIronHelmet>());
                if (cal.HasHelmet == 0 && (pureIronHelm >= 0 || pureIronHelm2 >= 0))
                {
                    if (pureIronHelm >= 0)
                    {
                        Main.LocalPlayer.inventory[pureIronHelm].stack--;
                        if (Main.LocalPlayer.inventory[pureIronHelm].stack <= 0)
                            Main.LocalPlayer.inventory[pureIronHelm] = new Item();
                        cal.HasHelmet = 1;
                    }
                    else
                    {
                        Main.LocalPlayer.inventory[pureIronHelm2].stack--;
                        if (Main.LocalPlayer.inventory[pureIronHelm2].stack <= 0)
                            Main.LocalPlayer.inventory[pureIronHelm2] = new Item();
                        cal.HasHelmet = 2;
                    }
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.HelmetLine");
                    if (pureIronHelm < 0 && pureIronHelm2 >= 0)
                        Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.HelmetLineB");
                    SoundEngine.PlaySound(SoundID.Grab, npc.position);
                    npc.netUpdate = true;
                    return;
                }
                if (!cal.HasShield && pureIronAlloy >= 0 && Main.LocalPlayer.inventory[pureIronAlloy].stack >= 6)
                {
                    Main.LocalPlayer.inventory[pureIronAlloy].stack -= 6;
                    if (Main.LocalPlayer.inventory[pureIronAlloy].stack <= 0)
                        Main.LocalPlayer.inventory[pureIronAlloy] = new Item();

                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ShieldLine");
                    SoundEngine.PlaySound(SoundID.Grab, npc.position);
                    cal.HasShield = true;
                    npc.netUpdate = true;
                    return;
                }
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.RequirementsLine");
                if (cal.HasHelmet > 0 && !cal.HasShield)
                    Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.RequirementsLineB");
                else if (cal.HasHelmet == 0 && cal.HasShield)
                    Main.npcChatText += Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.RequirementsLineC");
            }
        }
    }
    public class ForgeButton_Calavia : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;

            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56;
        }
        public bool NearFurnace;
        public void FurnaceNearMe(NPC npc)
        {
            NearFurnace = false;
            for (int x = -6; x <= 6; x++)
            {
                for (int y = -6; y <= 6; y++)
                {
                    Point npcPos = npc.Center.ToTileCoordinates();
                    Tile tile = Framing.GetTileSafely(npcPos.X + x, npcPos.Y + y);
                    if (tile.TileType == TileType<GathicCryoFurnaceTile>())
                    {
                        NearFurnace = true;
                        break;
                    }
                }
            }
        }
        public override string Text(NPC npc, Player player)
        {
            FurnaceNearMe(npc);
            if (NearFurnace)
                return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Forge");
            return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Furnace");
        }
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Calavia_NPC>() && RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20;
        public override void OnClick(NPC npc, Player player)
        {
            FurnaceNearMe(npc);
            if (NearFurnace)
            {
                if (Main.LocalPlayer.HasItem(ItemType<Mistfall>()))
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.MistfallLine");
                if (Main.LocalPlayer.HasItem(ItemType<Zweihander>()))
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ZweihanderLine");
                SoundEngine.PlaySound(SoundID.MenuOpen);
                TradeUI.Visible = true;
            }
            else
            {
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.FurnaceLine");
                SoundEngine.PlaySound(SoundID.Chat);
            }
        }
    }
    public class AboutButton_Calavia : ChatButton
    {
        public override double Priority => 3.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.AboutYou");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Calavia_NPC>() && RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.AboutMeLine");
        }
    }
    public class SpiritButton_Calavia : ChatButton
    {
        public override double Priority => 4.0;
        public override string Text(NPC npc, Player player)
        {
            if (RedeQuest.calaviaVar < 12 && RedeQuest.calaviaVar != 20)
                return "???";
            if (RedeQuest.calaviaVar is 13 or 14)
                return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.11");
            return Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.10");
        }
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Calavia_NPC>() && RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20 && RedeQuest.calaviaVar != 16;
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.calaviaVar < 12 ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.calaviaVar < 12 && RedeQuest.calaviaVar != 20)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.calaviaVar is 13 or 14)
            {
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ConfrontLine2");
                RedeQuest.calaviaVar = 14;
                RedeQuest.SyncData();
            }
            else
            {
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.ConfrontLine1");
                if (RedeQuest.calaviaVar < 13)
                {
                    RedeQuest.calaviaVar = 13;
                    RedeQuest.SyncData();
                }
            }
        }
    }
    public class LeaveButton_Calavia : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;

            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Leave.Name");

        public override string Description(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Calavia.Leave.Description");

        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<Calavia_NPC>() && RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20;
        public override Color? OverrideColor(NPC npc, Player player) => (npc.ModNPC is Calavia_NPC cal && cal.HasHelmet > 0 && cal.HasShield) ? RedeColor.TextPositive : RedeColor.TextCaution;
        public override void OnClick(NPC npc, Player player)
        {
            if (npc.ModNPC is Calavia_NPC cal && cal.HasHelmet > 0 && cal.HasShield)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.LeaveLine");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.Calavia.IntroLine11");

            RedeQuest.calaviaVar = 20;
            RedeQuest.SyncData();
        }
    }
}