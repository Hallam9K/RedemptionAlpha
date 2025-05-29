using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Donator.Lizzy;
using Redemption.Items.Lore;
using Redemption.Items.Materials.HM;
using Redemption.Items.Quest.KingSlayer;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.Tiles.Tiles;
using Redemption.UI.Dialect;
using Redemption.Walls;
using Redemption.WorldGeneration;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.NPCs.Friendly
{
    public class KS3Sitting : ModRedeNPC
    {
        public ref float AITimer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III");
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.width = 56;
            NPC.height = 82;
            NPC.lifeMax = 999;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;

            DialogueBoxStyle = SLAYER;
        }
        public override bool HasTalkButton() => true;
        public override bool HasLeftHangingButton(Player player) => RedeGlobalButton.talkActive;
        public override bool HasRightHangingButton(Player player) => RedeGlobalButton.talkActive;
        public override HangingButtonParams LeftHangingButton(Player player) => new(4);
        public override HangingButtonParams RightHangingButton(Player player) => new(4);

        public override bool UsesPartyHat() => true;
        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        public override bool CanChat() => true;
        public override bool CheckActive()
        {
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (Terraria.GameContent.Events.BirthdayParty.PartyIsUp)
            {
                Asset<Texture2D> hat = Request<Texture2D>("Terraria/Images/Item_" + ItemID.PartyHat);
                var offset = (NPC.frame.Y / 88) switch
                {
                    3 => 2,
                    4 => 2,
                    _ => 0,
                };
                Vector2 origin = new(hat.Value.Width / 2f, hat.Value.Height / 2f);
                spriteBatch.Draw(hat.Value, NPC.Center - new Vector2(1 - offset * NPC.spriteDirection, 48) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
        }
        public override void AI()
        {
            if (RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus)
                NPC.active = false;

            NPC.direction = 1;
            if (NPC.AnyNPCs(NPCType<KS3>()))
            {
                for (int i = 0; i < 15; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Frost, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dustIndex].velocity *= 3f;
                }
                NPC.active = false;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile lizzy = Main.projectile[i];
                if (!lizzy.active || lizzy.type != ProjectileType<LizzyPet>())
                    continue;

                if (lizzy.frame != 8)
                    continue;

                if (NPC.localAI[0]++ % 500 == 0)
                    EmoteBubble.NewBubble(1, new WorldUIAnchor(NPC), 120);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }
            AITimer++;
            if (AITimer >= 300 && AITimer <= 370)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 10)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += 88;
                    if (NPC.frame.Y > 6 * 88)
                        NPC.frame.Y = 0;
                }
                if (AITimer >= 370)
                {
                    NPC.frame.Y = 0;
                    AITimer = 0;
                }
                if (AITimer == 330)
                    NPC.Shoot(new Vector2(NPC.Center.X - 80, NPC.Center.Y - 50), ProjectileType<KS3Sitting_Hologram>(), 0, Vector2.Zero, Main.rand.Next(4));
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                AITimer++;
                if (AITimer >= 300 && AITimer <= 370)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 10)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += 88;
                        if (NPC.frame.Y > 6 * 88)
                            NPC.frame.Y = 0;
                    }
                    if (AITimer >= 370)
                    {
                        NPC.frame.Y = 0;
                        AITimer = 0;
                    }
                    if (AITimer == 330)
                        NPC.Shoot(new Vector2(NPC.Center.X - 80, NPC.Center.Y - 50), ProjectileType<KS3Sitting_Hologram>(), 0, Vector2.Zero, Main.rand.Next(4));
                }
            }
        }
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add<Datalog>()
                .Add<Datalog2>()
                .Add<Datalog3>()
                .Add<Datalog4>()
                .Add<Datalog5>()
                .Add<Datalog6>()
                .Add<Datalog7>()
                .Add<Datalog8>()
                .Add<Datalog9>()
                .Add<Datalog10>()
                .Add<Datalog11>()
                .Add<Datalog12>()
                .Add<Datalog13>()
                .Add<Datalog14>()
                .Add<Datalog15>()
                .Add<Datalog16>()
                .Add<Datalog17>()
                .Add<Datalog18>()
                .Add<Datalog19>()
                .Add<Datalog20>()
                .Add<Datalog21>()
                .Add<Datalog22>()
                .Add<Datalog24>()
                .Add<Datalog25>()
                .Add<Datalog26>()
                .Add<Datalog27>()
                .Add<Datalog28>();

            npcShop.Register();
        }

        public override string GetChat()
        {
            Player player = Main.LocalPlayer;
            WeightedRandom<string> chat = new(Main.rand);

            if (RedeBossDowned.downedSlayer && !Main.LocalPlayer.InModBiome<SlayerShipBiome>())
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.NotInShipDialogue"));
            else
            {
                if (RedeQuest.slayerRep >= 2)
                {
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue1"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue2"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue3"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue4"));
                }
                else if (RedeQuest.slayerRep >= 4)
                {
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue5"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue6"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue7"));
                }
                else
                {
                    if (player.IsFullTBot())
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue8TBot"));
                    else
                        chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue8"));

                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue9"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue10"));
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue11"));
                }
                if (NPC.downedMoonlord)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue12"), 3);

                if (BasePlayer.HasHelmet(player, ItemType<KingSlayerMask>(), true))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue13"));
                if (BasePlayer.HasHelmet(player, ItemType<AndroidHead>(), true) || BasePlayer.HasHelmet(player, ItemType<AndroidHead2>(), true) || BasePlayer.HasHelmet(player, ItemType<AndroidHead3>(), true))
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.DialogueAndroidHead"), 2);

                if (player.wellFed && RedeQuest.slayerRep < 2)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue14"));
                if (player.RedemptionPlayerBuff().ChickenForm)
                    chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Dialogue15"));
            }
            return chat;
        }
    }
    public class WhyAreHereButton_KS3 : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.0");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.slayerRep >= 1 && RedeQuest.slayerRep < 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat1B");
            else if (RedeQuest.slayerRep >= 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat1C");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat1");
        }
    }
    public class CrashedSpaceshipButton_KS3 : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + 46;
        }
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.1");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.slayerRep == 1)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat2B");
            else if (RedeQuest.slayerRep >= 2 && RedeQuest.slayerRep < 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat2C");
            else if (RedeQuest.slayerRep >= 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat2D");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat2");
        }
    }
    public class OfferUraniumButton_KS3 : ChatButton
    {
        public override double Priority => 8.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.2");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && !RedeGlobalButton.talkActive && RedeQuest.slayerRep == 0;
        public override Color? OverrideColor(NPC npc, Player player) => player.HasItem(ItemType<Uranium>()) ? null : Color.Gray;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (player.ConsumeItem(ItemType<Uranium>()))
            {
                WeightedRandom<string> chat = new(Main.rand);
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.UraniumDialogue1"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.UraniumDialogue2"));
                chat.Add(Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.UraniumDialogue3"));
                Main.npcChatText = chat;

                player.QuickSpawnItem(npc.GetSource_Loot(), ItemID.SilverCoin, 20);

                RedeQuest.slayerRep++;
                RedeQuest.SyncData();

                CombatText.NewText(npc.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.DialogueBox.New"), true, false);
                SoundEngine.PlaySound(SoundID.Chat);
            }
            else
            {
                Main.npcChatCornerItem = ItemType<Uranium>();
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.NoUraniumDialogue3");
            }
        }
    }
    public class ArentFightingButton_KS3 : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + (46 * 2);
        }
        public override string Text(NPC npc, Player player) => RedeQuest.slayerRep == 0 ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.3");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.slayerRep != 0 ? null : Color.Gray;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.slayerRep == 0)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.slayerRep == 3)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat3C");
            else if (RedeQuest.slayerRep >= 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat3D");
            else
                Main.npcChatText = Main.rand.NextBool(2) ? Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat3") : Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat3B");
        }
    }
    public class AreYouHumanButton_KS3 : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2);
            position.Y += 56 + (46 * 3);
        }
        public override string Text(NPC npc, Player player) => RedeQuest.slayerRep == 0 ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.4");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.slayerRep != 0 ? null : Color.Gray;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.slayerRep == 0)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.slayerRep == 2)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat4B");
            else if (RedeQuest.slayerRep >= 3)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat4C");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat4");
        }
    }
    public class HallOfHeroesButton_KS3 : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56;
        }
        public override string Text(NPC npc, Player player) => RedeQuest.slayerRep == 0 || !player.Redemption().foundHall ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.5");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.slayerRep != 0 && player.Redemption().foundHall ? null : Color.Gray;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.slayerRep == 0 || !player.Redemption().foundHall)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.slayerRep == 2)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5B");
            else if (RedeQuest.slayerRep == 3)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5C");
            else if (RedeQuest.slayerRep >= 4)
                Main.npcChatText = Main.rand.NextBool(3) ? Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5E") : Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5D");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat5");
        }
    }
    public class AbandonedLabButton_KS3 : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + 46;
        }
        public override string Text(NPC npc, Player player) => RedeQuest.slayerRep == 0 || !player.Redemption().foundLab ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.6");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.slayerRep != 0 && player.Redemption().foundLab ? null : Color.Gray;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.slayerRep == 0 || !player.Redemption().foundLab)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.slayerRep >= 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat6B");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat6");
        }
    }
    public class EpidotraButton_KS3 : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 2);
        }
        public override string Text(NPC npc, Player player) => RedeQuest.slayerRep == 0 ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.7");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.slayerRep != 0 ? null : Color.Gray;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.slayerRep == 0)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.slayerRep >= 2 && RedeQuest.slayerRep < 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat7B");
            else if (RedeQuest.slayerRep >= 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat7C");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat7");
        }
    }
    public class OtherWorldButton_KS3 : ChatButton
    {
        public override double Priority => 200.0;
        public override void ModifyPosition(NPC npc, Player player, ref Vector2 position)
        {
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(ChatButtonLoader.GetText(this, npc, player)).X;
            position.X = (Main.screenWidth / 2) - 150 - (textLength / 2) + 300;
            position.Y += 56 + (46 * 3);
        }
        public override string Text(NPC npc, Player player) => RedeQuest.slayerRep == 0 || !RedeBossDowned.downedSeed ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.KS3.8");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => RedeQuest.slayerRep != 0 && RedeBossDowned.downedSeed ? null : Color.Gray;
        public override void OnClick(NPC npc, Player player)
        {
            if (RedeQuest.slayerRep == 0 || !RedeBossDowned.downedSeed)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            if (RedeQuest.slayerRep >= 2 && RedeQuest.slayerRep < 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat8B");
            else if (RedeQuest.slayerRep >= 4)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat8C");
            else
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Chat8");
        }
    }
    public class QuestButton_KS3 : ChatButton
    {
        public override double Priority => 10.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.Quest");
        public override Color? OverrideColor(NPC npc, Player player)
        {
            if (RedeQuest.slayerRep == 3 && player.HasItem(ItemType<SlayerShipEngine>()))
                return RedeColor.TextPositive;

            return null;
        }
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<KS3Sitting>() && !RedeGlobalButton.talkActive && RedeQuest.slayerRep != 0;
        public override void OnClick(NPC npc, Player player)
        {
            switch (RedeQuest.slayerRep)
            {
                case 1:
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    if (player.ConsumeItem(ItemType<SlayerWiringKit>()))
                    {
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest1CompleteDialogue");

                        player.QuickSpawnItem(npc.GetSource_Loot(), ItemID.GoldCoin, 4);
                        CombatText.NewText(npc.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.DialogueBox.New"), true, false);
                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };
                        TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/SlayerShipFix1");
                        Point origin = RedeGen.slayerShipVector.ToPoint();
                        TexGen gen = TexGen.GetTexGenerator(tex, colorToTile);
                        gen.Generate(origin.X, origin.Y, false, true);

                        RedeQuest.slayerRep++;
                        RedeQuest.SyncData();

                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ItemType<SlayerWiringKit>();
                        Main.npcChatText = QuestChat();
                    }
                    break;
                case 2:
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    if (player.ConsumeItem(ItemType<SlayerHullPlating>()))
                    {
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest2CompleteDialogue");

                        player.QuickSpawnItem(npc.GetSource_Loot(), ItemID.GoldCoin, 8);
                        CombatText.NewText(npc.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.DialogueBox.New"), true, false);
                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(0, 255, 255)] = TileType<SlayerShipPanelTile>(),
                            [new Color(255, 0, 255)] = TileType<ShipGlassTile>(),
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };
                        TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/SlayerShipFix2");
                        Point origin = RedeGen.slayerShipVector.ToPoint();
                        TexGen gen = TexGen.GetTexGenerator(tex, colorToTile);
                        gen.Generate(origin.X, origin.Y, false, true);

                        RedeQuest.slayerRep++;
                        RedeQuest.SyncData();

                        return;
                    }
                    else
                    {
                        Main.npcChatCornerItem = ItemType<SlayerHullPlating>();
                        Main.npcChatText = QuestChat();
                    }
                    break;
                case 3:
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    if (player.ConsumeItem(ItemType<SlayerShipEngine>()))
                    {
                        Main.npcChatCornerItem = ItemType<MemoryChip>();
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest3CompleteDialogue");

                        player.QuickSpawnItem(npc.GetSource_Loot(), ItemID.GoldCoin, 12);
                        player.QuickSpawnItem(npc.GetSource_Loot(), ItemType<MemoryChip>());

                        CombatText.NewText(npc.getRect(), Color.LightCyan, Language.GetTextValue("Mods.Redemption.DialogueBox.New"), true, false);
                        SoundEngine.PlaySound(SoundID.Chat);

                        Dictionary<Color, int> colorToTile = new()
                        {
                            [new Color(0, 255, 255)] = TileType<SlayerShipPanelTile>(),
                            [new Color(255, 0, 255)] = TileType<ShipGlassTile>(),
                            [new Color(150, 150, 150)] = -2,
                            [Color.Black] = -1
                        };
                        Dictionary<Color, int> colorToWall = new()
                        {
                            [new Color(0, 255, 0)] = WallType<SlayerShipPanelWallTile>(),
                            [Color.Black] = -1
                        };

                        TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/SlayerShipFix2");
                        TexGenData texWalls = TexGen.GetTextureForGen("Redemption/WorldGeneration/SlayerShipWallsFix");
                        Point origin = RedeGen.slayerShipVector.ToPoint();
                        TexGen gen = TexGen.GetTexGenerator(tex, colorToTile, texWalls, colorToWall);
                        gen.Generate(origin.X, origin.Y, false, true);

                        RedeWorld.Alignment += 2;

                        RedeQuest.slayerRep++;
                        RedeQuest.SyncData();
                    }
                    else
                    {
                        Main.npcChatCornerItem = ItemType<SlayerShipEngine>();
                        Main.npcChatText = QuestChat();
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                    break;
                default:
                    Main.npcChatText = QuestChat();
                    SoundEngine.PlaySound(SoundID.Chat);
                    break;
            }
        }
        public static string QuestChat()
        {
            return RedeQuest.slayerRep switch
            {
                2 => Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest2Dialogue"),
                3 => Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest3Dialogue"),
                4 => Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest4Dialogue"),
                _ => Language.GetTextValue("Mods.Redemption.Dialogue.KingSlayer.Quest1Dialogue"),
            };
        }
    }
}