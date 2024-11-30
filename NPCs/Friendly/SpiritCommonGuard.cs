using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Weapons.PreHM.Melee;
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
    public class SpiritCommonGuard : ModRedeNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit Common Guard");
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
            NPC.width = 30;
            NPC.height = 48;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;

            DialogueBoxStyle = CAVERN;
        }
        public override bool HasTalkButton() => true;
        public override bool HasCruxButton(Player player) => !player.HasItem(ItemType<CruxCardAnglonSkeletons>());
        public override string CruxButtonText(Player player)
        {
            bool offering = player.HasItem(ItemType<NoblesHalberd>());
            return request && offering ? Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritCommonGuard.Offer") : Language.GetTextValue("Mods.Redemption.DialogueBox.Crux");
        }
        public override void CruxButton(Player player)
        {
            RequestCruxButton.RequestCrux(NPC, player, ItemType<CruxCardAnglonSkeletons>(), "SpiritCommonGuard.NoCruxDialogue", "SpiritCommonGuard.CruxDialogue", "SpiritCommonGuard.OfferCruxDialogue", ref request, ItemType<NoblesHalberd>());
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
            return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritCommonGuard.Dialogue");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White with { A = 100 }), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
    public class AboutButton_SpiritCommonGuard : ChatButton
    {
        public override double Priority => 1.0;
        public static bool clicked;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.AboutYou");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritCommonGuard>() && RedeGlobalButton.talkActive;
        public override void OnClick(NPC npc, Player player)
        {
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritCommonGuard.Chat1");
        }
    }
    public class AnglonButton_SpiritCommonGuard : ChatButton
    {
        public override double Priority => 2.0;
        public override string Text(NPC npc, Player player) => !AboutButton_SpiritCommonGuard.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritCommonGuard.1");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritCommonGuard>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !AboutButton_SpiritCommonGuard.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!AboutButton_SpiritCommonGuard.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritCommonGuard.Chat2");
        }
    }
    public class RicusaButton_SpiritCommonGuard : ChatButton
    {
        public override double Priority => 3.0;
        public static bool clicked;
        public override string Text(NPC npc, Player player) => !AboutButton_SpiritCommonGuard.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritCommonGuard.2");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritCommonGuard>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !AboutButton_SpiritCommonGuard.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!AboutButton_SpiritCommonGuard.clicked)
                return;
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritCommonGuard.Chat3");
        }
    }
    public class DemonsButton_SpiritCommonGuard : ChatButton
    {
        public override double Priority => 4.0;
        public override string Text(NPC npc, Player player) => !RicusaButton_SpiritCommonGuard.clicked ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritCommonGuard.3");
        public override bool IsActive(NPC npc, Player player) => npc.type == NPCType<SpiritCommonGuard>() && RedeGlobalButton.talkActive;
        public override Color? OverrideColor(NPC npc, Player player) => !RicusaButton_SpiritCommonGuard.clicked ? Color.Gray : null;
        public override void OnClick(NPC npc, Player player)
        {
            if (!RicusaButton_SpiritCommonGuard.clicked)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritCommonGuard.Chat4");
        }
    }
}
