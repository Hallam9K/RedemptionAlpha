using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Minibosses.Calavia;
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
    public class SpiritWalkerMan : ModRedeNPC
    {
        public ref float AITimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit Stranger");
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
            NPC.width = 24;
            NPC.height = 44;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0;

            DialogueBoxStyle = CAVERN;
        }
        public override bool HasTalkButton() => true;
        public override bool HasCruxButton(Player player) => !player.HasItem(ItemType<CruxCardSkeleton>()) || player.HasItem(ItemType<OldTophat>());
        public override string CruxButtonText(Player player)
        {
            if (player.HasItem(ItemType<OldTophat>()))
                return Language.GetTextValue("Mods.Redemption.DialogueBox.SpiritWalkerMan.Tophat");
            return Language.GetTextValue("Mods.Redemption.DialogueBox.Crux");
        }
        public override void CruxButton(Player player)
        {
            if (player.HasItem(ItemType<OldTophat>()))
            {
                if (player.ConsumeItem(ItemType<EmptyCruxCard>()) && player.ConsumeItem(ItemType<OldTophat>()))
                {
                    player.QuickSpawnItem(NPC.GetSource_Loot(), ItemType<CruxCardTied>());
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.CruxAsherDialogue");
                    Main.npcChatCornerItem = ItemType<CruxCardTied>();
                }
                else
                {
                    Main.npcChatText = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.NoCruxAsherDialogue");
                    Main.npcChatCornerItem = ItemType<EmptyCruxCard>();
                }
                return;
            }
            RequestCruxButton.RequestCrux(NPC, player, ItemType<CruxCardSkeleton>(), "SpiritWalkerMan.NoCruxDialogue", "SpiritWalkerMan.CruxDialogue");
        }
        public override bool HasLeftHangingButton(Player player) => RedeGlobalButton.talkActive;
        public override HangingButtonParams LeftHangingButton(Player player) => new(5);

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (RedeQuest.calaviaVar != 15)
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
        public override bool CanChat() => RedeQuest.calaviaVar != 15;
        public override string GetChat()
        {
            if (RedeQuest.calaviaVar >= 11 && RedeQuest.calaviaVar != 20 && NPC.AnyNPCs(NPCType<Calavia_NPC>()))
            {
                if (RedeQuest.calaviaVar < 12)
                {
                    RedeQuest.calaviaVar = 12;
                    RedeQuest.SyncData();
                }

                if (!Main.LocalPlayer.HasItem(ItemType<CruxCardCalavia>()))
                {
                    if (RedeQuest.calaviaVar is 16)
                        return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.CalaviaDialogue2");
                    return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.CalaviaDialogue1");
                }
            }
            bool wearingHat = BasePlayer.HasHelmet(Main.LocalPlayer, ItemType<OldTophat>());
            string s = "";
            if (wearingHat)
                s = Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Dialogue2Mid");
            if (Main.LocalPlayer.HasItem(ItemType<OldTophat>()) || wearingHat)
                return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Dialogue2") + s + Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Dialogue2Cont");
            return Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.Dialogue1");
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White with { A = 100 }), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
    }
    public class AboutButton_SpiritWalkerMan : TalkButtonBase
    {
        protected override int YOffset => 0;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritWalkerMan.0";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<SpiritWalkerMan>();
        public override string Text(NPC npc, Player player) => Language.GetTextValue("Mods.Redemption.DialogueBox.AboutYou");
    }
    public class SpiritWalkingButton_SpiritWalkerMan : TalkButtonBase
    {
        protected override int YOffset => 1;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritWalkerMan.1";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<SpiritWalkerMan>();
    }
    public class DeadRingerButton_SpiritWalkerMan : TalkButtonBase
    {
        protected override int YOffset => 2;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritWalkerMan.2";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<SpiritWalkerMan>();
    }
    public class LostSoulsButton_SpiritWalkerMan : TalkButtonBase
    {
        protected override int YOffset => 3;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritWalkerMan.3";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<SpiritWalkerMan>();
    }
    public class RingerUsesButton_SpiritWalkerMan : TalkButtonBase
    {
        protected override int YOffset => 4;
        protected override bool LeftSide => true;
        protected override string DialogueType => "SpiritWalkerMan.4";
        protected override bool VisibleRequirement => true;
        protected override int NPCType => NPCType<SpiritWalkerMan>();
        public override void OnClick(NPC npc, Player player)
        {
            SoundEngine.PlaySound(SoundID.Chat);
            Main.npcChatText = !Main.rand.NextBool(10) ? Language.GetTextValue("Mods.Redemption.Dialogue." + DialogueType) : Language.GetTextValue("Mods.Redemption.Dialogue.SpiritWalkerMan.4B");
        }
    }
}