using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Tools.PreHM;
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
    public class SpiritGathicMan : ModRedeNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Old Spirit Man");
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
        public override bool HasCruxButton(Player player) => !player.HasItem(ItemType<CruxCardGathicSkeletons>());
        public override string CruxButtonText(Player player)
        {
            bool offering = player.HasItem(ItemType<GraveSteelBattleaxe>());
            return request && offering ? Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritGathicMan.Offer") : Language.GetTextValue("Mods.Redemption.DialogueBox.Crux");
        }
        public override void CruxButton(Player player)
        {
            RequestCruxButton.RequestCrux(NPC, player, ItemType<CruxCardGathicSkeletons>(), "SpiritGathicMan.NoCruxDialogue", "SpiritGathicMan.CruxDialogue", "SpiritGathicMan.OfferCruxDialogue", ref request, ItemType<GraveSteelBattleaxe>());
        }
        public override bool HasLeftHangingButton(Player player) => RedeGlobalButton.talkActive;
        public override bool HasRightHangingButton(Player player) => RedeGlobalButton.talkActive;
        public override HangingButtonParams LeftHangingButton(Player player) => new(5);
        public override HangingButtonParams RightHangingButton(Player player) => new(2);

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
            return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritGathicMan.Dialogue");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White with { A = 100 }), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
    public class AboutButton_SpiritGathicMan : TalkButtonBase
    {
        protected override int YOffset => 0;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritGathicMan.0";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<SpiritGathicMan>();

        public static bool clicked;
        public override void OnSafeClick(NPC npc, Player player) => clicked = true;
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.AboutYou");
    }
    public class OldenRuinsButton_SpiritGathicMan : TalkButtonBase
    {
        protected override int YOffset => 1;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritGathicMan.1";
        protected override bool VisibleRequirement => AboutButton_SpiritGathicMan.clicked;
        protected override int NPCType => NPCType<SpiritGathicMan>();

        public static bool clicked;
        public override void OnSafeClick(NPC npc, Player player) => clicked = true;
    }
    public class GodOfDecayButton_SpiritGathicMan : TalkButtonBase
    {
        protected override int YOffset => 2;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritGathicMan.2";
        protected override bool VisibleRequirement => OldenRuinsButton_SpiritGathicMan.clicked;
        protected override int NPCType => NPCType<SpiritGathicMan>();

        public static bool clicked;
        public override void OnSafeClick(NPC npc, Player player) => clicked = true;
    }
    public class FalseGodsButton_SpiritGathicMan : TalkButtonBase
    {
        protected override int YOffset => 3;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritGathicMan.3";
        protected override bool VisibleRequirement => GodOfDecayButton_SpiritGathicMan.clicked;
        protected override int NPCType => NPCType<SpiritGathicMan>();
    }
    public class DeadRingerButton_SpiritGathicMan : TalkButtonBase
    {
        protected override int YOffset => 4;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritGathicMan.4";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<SpiritGathicMan>();
    }
    public class SkullDiggerButton_SpiritGathicMan : TalkButtonBase
    {
        protected override int YOffset => 0;
        protected override bool LeftSide => false;
        protected override string DialogueType => "SpiritGathicMan.5";
        protected override bool VisibleRequirement => RedeBossDowned.downedSkullDigger;
        protected override int NPCType => NPCType<SpiritGathicMan>();

        public static bool clicked;
        public override string Text(NPC npc, Player player)
        {
            return !RedeBossDowned.downedSkullDigger ? "???" : Language.GetTextValue("Mods.Redemption.DialogueBox." + DialogueType);
        }
        public override void OnClick(NPC npc, Player player)
        {
            if (!RedeBossDowned.downedSkullDigger)
                return;
            clicked = true;
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritGathicMan.ChatSkullDigger");
        }
    }
    public class HowDidDieButton_SpiritGathicMan : TalkButtonBase
    {
        protected override int YOffset => 1;
        protected override bool LeftSide => false;
        protected override string DialogueType => "SpiritGathicMan.6";
        protected override bool VisibleRequirement => SkullDiggerButton_SpiritGathicMan.clicked;
        protected override int NPCType => NPCType<SpiritGathicMan>();
    }
}