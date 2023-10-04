using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Items.Materials.PreHM;
using Terraria.Audio;
using Redemption.Items.Armor.Vanity;
using Redemption.Base;
using Terraria.Localization;
using Redemption.NPCs.Minibosses.Calavia;

namespace Redemption.NPCs.Friendly
{
    public class SpiritWalkerMan : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit Stranger");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
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
            NPC.noTileCollide = true;
            NPC.width = 24;
            NPC.height = 44;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public bool floatTimer;
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (RedeQuest.calaviaVar != 15)
                NPC.LookAtEntity(player);

            if (AITimer < 60)
                NPC.velocity *= 0.94f;

            if (AITimer++ == 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.Center, 2, 2, DustID.DungeonSpirit, 0f, 0f, 100, default, 2);
                    Main.dust[dustIndex].velocity *= 2f;
                    Main.dust[dustIndex].noGravity = true;
                }
                DustHelper.DrawDustImage(NPC.Center, DustID.DungeonSpirit, 0.5f, "Redemption/Effects/DustImages/DeadRingerDust", 2, true, 0);
            }
            NPC.alpha += Main.rand.Next(-10, 11);
            NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 40, 60);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= 4 * frameHeight)
                    NPC.frame.Y = 0;
            }
            if (!floatTimer)
            {
                NPC.velocity.Y += 0.03f;
                if (NPC.velocity.Y > .5f)
                {
                    floatTimer = true;
                    NPC.netUpdate = true;
                }
            }
            else if (floatTimer)
            {
                NPC.velocity.Y -= 0.03f;
                if (NPC.velocity.Y < -.5f)
                {
                    floatTimer = false;
                    NPC.netUpdate = true;
                }
            }
        }
        public static int ChatNumber = 0;
        public override void SetChatButtons(ref string button, ref string button2)
        {
            switch (ChatNumber)
            {
                case 0:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritWalkerMan.1");
                    break;
                case 1:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritWalkerMan.2");
                    break;
                case 2:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritWalkerMan.3");
                    break;
                case 3:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritWalkerMan.4");
                    break;
                case 4:
                    button = Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritWalkerMan.5");
                    break;
                case 5:
                    if (Main.LocalPlayer.HasItem(ModContent.ItemType<OldTophat>()))
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritWalkerMan.Tophat");
                    else
                        button = Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritWalkerMan.Crux");
                    break;
            }
            button2 = Language.GetTextValue("Mods.Redemption.DialogueBox.CycleD");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.npcChatText = ChitChat();
                if (ChatNumber == 5)
                {
                    if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                    {
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.NoRealmCruxDialogue");
                        ChatNumber = 4;
                        return;
                    }
                    int card = Main.LocalPlayer.FindItem(ModContent.ItemType<EmptyCruxCard>());
                    if (Main.LocalPlayer.HasItem(ModContent.ItemType<OldTophat>()))
                    {
                        int oldTophat = Main.LocalPlayer.FindItem(ModContent.ItemType<OldTophat>());
                        if (card >= 0 && oldTophat >= 0)
                        {
                            Main.LocalPlayer.inventory[oldTophat].stack--;
                            if (Main.LocalPlayer.inventory[oldTophat].stack <= 0)
                                Main.LocalPlayer.inventory[oldTophat] = new Item();

                            Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardTied>());
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.CruxAsherDialogue");
                            Main.npcChatCornerItem = ModContent.ItemType<CruxCardTied>();
                            SoundEngine.PlaySound(SoundID.Chat);
                        }
                        else
                        {
                            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.NoCruxAsherDialogue");
                            Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                        }
                        ChatNumber = 4;
                        return;
                    }
                    if (card >= 0)
                    {
                        Main.LocalPlayer.inventory[card].stack--;
                        if (Main.LocalPlayer.inventory[card].stack <= 0)
                            Main.LocalPlayer.inventory[card] = new Item();

                        Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), ModContent.ItemType<CruxCardSkeleton>());
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.CruxDialogue");
                        Main.npcChatCornerItem = ModContent.ItemType<CruxCardSkeleton>();
                        SoundEngine.PlaySound(SoundID.Chat);
                    }
                    else
                    {
                        Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.NoCruxDialogue");
                        Main.npcChatCornerItem = ModContent.ItemType<EmptyCruxCard>();
                    }
                    ChatNumber = 4;
                }
            }
            else
            {
                ChatNumber++;
                int max = 4;
                if (Main.LocalPlayer.HasItem(ModContent.ItemType<OldTophat>()))
                {
                    if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive && !Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardTied>()))
                        max = 5;
                }
                else
                {
                    if (Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive && !Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardSkeleton>()))
                        max = 5;
                }
                if (ChatNumber > max)
                    ChatNumber = 0;
            }
        }
        public static string ChitChat()
        {
            return ChatNumber switch
            {
                0 => Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Chat1"),
                1 => Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Chat2"),
                2 => Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Chat3"),
                3 => !Main.rand.NextBool(8) ? Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Chat4") : Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Chat4B"),
                4 => Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Chat5"),
                _ => "...",
            };
        }
        public override bool CanChat() => RedeQuest.calaviaVar != 15;
        public override string GetChat()
        {
            if (RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20 && NPC.AnyNPCs(ModContent.NPCType<Calavia_NPC>()))
            {
                if (RedeQuest.calaviaVar < 12)
                {
                    RedeQuest.calaviaVar = 12;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }

                if (!Main.LocalPlayer.HasItem(ModContent.ItemType<CruxCardCalavia>()))
                {
                    if (RedeQuest.calaviaVar is 16)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.CalaviaDialogue2");
                    return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.CalaviaDialogue1");
                }
            }
            bool wearingHat = BasePlayer.HasHelmet(Main.LocalPlayer, ModContent.ItemType<OldTophat>());
            string s = "";
            if (wearingHat)
                s = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Dialogue2Mid");
            if (Main.LocalPlayer.HasItem(ModContent.ItemType<OldTophat>()) || wearingHat)
                return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Dialogue2") + s + Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Dialogue2Cont");
            return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Dialogue1");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.MirageDye);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
