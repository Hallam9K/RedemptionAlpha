using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.UI.Dialect;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly
{
    public class SpiritAssassin : ModRedeNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit Assassin");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
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
            NPC.noTileCollide = true;
            NPC.width = 40;
            NPC.height = 54;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;

            DialogueBoxStyle = CAVERN;
        }
        public override bool HasTalkButton() => AboutButton_SpiritAssassin.what;
        public override bool HasCruxButton(Player player) => !player.HasItem(ItemType<CruxCardSkeletonAssassin>());
        public override string CruxButtonText(Player player)
        {
            bool offering = player.HasItem(ItemType<Nightshade>());
            return request && offering ? Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritAssassin.Offer") : Language.GetTextValue("Mods.Redemption.DialogueBox.Crux");
        }
        public override void CruxButton(Player player)
        {
            RequestCruxButton.RequestCrux(NPC, player, ItemType<CruxCardSkeletonAssassin>(), "SpiritAssassin.NoCruxDialogue", "SpiritAssassin.CruxDialogue", "SpiritAssassin.OfferCruxDialogue", ref request, ItemType<Nightshade>(), 3);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            NPC.LookAtEntity(player);

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
            NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 15) / 3;
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
        }
        public static bool request;
        public override bool CanChat() => true;
        public override string GetChat()
        {
            if (AboutButton_SpiritAssassin.what)
                return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritAssassin.Dialogue1");
            return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritAssassin.Dialogue2"); // (Word of great surprise)! (Word to make this sentence a question) you summoned me? (ba- past tense) Do tell your reason. (Mu- Possession of you. Starts with 'your reason' and ends with 'tell')
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White with { A = 100 }), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
    public class AboutButton_SpiritAssassin : ChatButton
    {
        public override double Priority => 1.0;
        public static bool clicked;
        public static bool what;
        public override string Text(NPC npc, Player player)
        {
            if (what)
                return Language.GetTextValue("Mods.Redemption.DialogueBox.AboutYou");
            return Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritAssassin.1");
        }
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritAssassin>() && (!what ? !RedeGlobalButton.talkActive : RedeGlobalButton.talkActive);
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            if (!what)
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritAssassin.WhatDialogue");
            else
            {
                clicked = true;
                Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritAssassin.Chat1");
            }
            what = true;
        }
    }
    public class GathuramButton_SpiritAssassin : ChatButton
    {
        public override double Priority => 2.0;
        public override string Text(NPC npc, Player player) => !AboutButton_SpiritAssassin.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritAssassin.2");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritAssassin>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !AboutButton_SpiritAssassin.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!AboutButton_SpiritAssassin.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritAssassin.Chat2");
        }
    }
    public class GothrioneButton_SpiritAssassin : ChatButton
    {
        public override double Priority => 3.0;
        public static bool clicked;
        public override string Text(NPC npc, Player player) => !AboutButton_SpiritAssassin.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritAssassin.3");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritAssassin>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !AboutButton_SpiritAssassin.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!AboutButton_SpiritAssassin.clicked)
                return;
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritAssassin.Chat3");
        }
    }
    public class DemonButton_SpiritAssassin : ChatButton
    {
        public override double Priority => 4.0;
        public override string Text(NPC npc, Player player) => !GothrioneButton_SpiritAssassin.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritAssassin.4");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritAssassin>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !GothrioneButton_SpiritAssassin.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!GothrioneButton_SpiritAssassin.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritAssassin.Chat4");
        }
    }
}