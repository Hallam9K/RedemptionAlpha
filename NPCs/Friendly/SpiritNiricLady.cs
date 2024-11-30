using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Globals;
using Redemption.Items.Quest;
using Redemption.Items.Tools.PreHM;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.Textures;
using Redemption.UI.ChatUI;
using Redemption.UI.Dialect;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;
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
    public class SpiritNiricLady : ModRedeNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Old Spirit Lady");
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
            NPC.width = 44;
            NPC.height = 48;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;

            DialogueBoxStyle = CAVERN;
        }
        public override bool HasTalkButton() => true;
        public override bool HasCruxButton(Player player) => !player.HasItem(ItemType<CruxCardGladestone>());
        public override string CruxButtonText(Player player)
        {
            bool offering = player.HasItem(ItemID.Diamond);
            return request && offering ? Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritNiricLady.Offer") : Language.GetTextValue("Mods.Redemption.DialogueBox.Crux");
        }
        public override void CruxButton(Player player)
        {
            RequestCruxButton.RequestCrux(NPC, player, ItemType<CruxCardGladestone>(), "SpiritNiricLady.NoCruxDialogue", "SpiritNiricLady.CruxDialogue", "SpiritNiricLady.OfferCruxDialogue", ref request, ItemID.Diamond, 6);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            NPC.velocity *= 0.94f;

            if (AITimer == 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.Center, 2, 2, DustID.DungeonSpirit, 0f, 0f, 100, default, 2);
                    Main.dust[dustIndex].velocity *= 2f;
                    Main.dust[dustIndex].noGravity = true;
                }
                DustHelper.DrawDustImage(NPC.Center, DustID.DungeonSpirit, 0.5f, "Redemption/Effects/DustImages/DeadRingerDust", 2, true, 0);
                AITimer = 1;
            }
            NPC.alpha += Main.rand.Next(-10, 11);
            NPC.alpha = (int)MathHelper.Clamp(NPC.alpha, 80, 120);

            NPC.position.Y += (float)Math.Sin(NPC.localAI[0]++ / 15) / 3;
            NPC.LookAtEntity(player);
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
            return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritNiricLady.Dialogue");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White with { A = 100 }), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
    public class AboutButton_SpiritNiricLady : ChatButton
    {
        public override double Priority => 1.0;
        public static bool clicked;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.AboutYou");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritNiricLady>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritNiricLady.Chat1");
        }
    }
    public class NewKingdomButton_SpiritNiricLady : ChatButton
    {
        public override double Priority => 2.0;
        public static bool clicked;
        public override string Text(NPC npc, Player player) => !AboutButton_SpiritNiricLady.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritNiricLady.1");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritNiricLady>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !AboutButton_SpiritNiricLady.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!AboutButton_SpiritNiricLady.clicked)
                return;
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritNiricLady.Chat2");
        }
    }
    public class CorpseButton_SpiritNiricLady : ChatButton
    {
        public override double Priority => 3.0;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritNiricLady.2");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritNiricLady>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritNiricLady.Chat3");
        }
    }
    public class NirinButton_SpiritNiricLady : ChatButton
    {
        public override double Priority => 4.0;
        public static bool clicked;
        public override string Text(NPC npc, Player player) => !NewKingdomButton_SpiritNiricLady.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritNiricLady.3");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritNiricLady>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !NewKingdomButton_SpiritNiricLady.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!NewKingdomButton_SpiritNiricLady.clicked)
                return;
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritNiricLady.Chat4");
        }
    }
    public class DisasterButton_SpiritNiricLady : ChatButton
    {
        public override double Priority => 5.0;
        public override string Text(NPC npc, Player player) => !NirinButton_SpiritNiricLady.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritNiricLady.4");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritNiricLady>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !NirinButton_SpiritNiricLady.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!NirinButton_SpiritNiricLady.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritNiricLady.Chat5");
        }
    }
}