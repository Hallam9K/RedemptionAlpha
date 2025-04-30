using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Utilities;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Friendly;
using Redemption.Textures;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class AlignmentButton : UIState
    {
        private readonly UIImage AlignmentButtonTexture = new(Request<Texture2D>("Redemption/UI/AlignmentButton", AssetRequestMode.ImmediateLoad));
        private readonly Asset<Texture2D> AlignmentButton_MouseOverTexture = Request<Texture2D>("Redemption/UI/AlignmentButton_MouseOver", AssetRequestMode.ImmediateLoad);

        public UIImage Icon;
        public UIHoverTextImageButton IconHighlight;
        public static bool Glowing = false;

        public override void OnActivate()
        {
            Icon = AlignmentButtonTexture;
            Icon.Left.Set(570, 0f);
            Icon.Top.Set(10, 0f);
            Append(Icon);

            IconHighlight = new UIHoverTextImageButton(AlignmentButton_MouseOverTexture, "Alignment");
            IconHighlight.Left.Set(-2, 0f);
            IconHighlight.Top.Set(-2, 0f);
            IconHighlight.SetVisibility(1f, 0f);
            IconHighlight.OnLeftClick += IconHighlight_OnClick;
            Icon.Append(IconHighlight);

            base.OnActivate();
        }
        public override void MouseOver(UIMouseEvent evt)
        {
            IconHighlight.Text = Language.GetTextValue("Mods.Redemption.UI.Chalice.Alignment") + ": " + RedeWorld.Alignment;
            if (ItemLists.AlignmentInterest.Contains(Main.LocalPlayer.HeldItem.type))
                IconHighlight.Text += "\n" + Language.GetTextValue("Mods.Redemption.UI.Chalice.ItemInterested");
        }
        public override void Update(GameTime gameTime)
        {
            float passedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Main.FrameSkipMode == FrameSkipMode.Subtle)
                passedTime = 1f / 60f;

            frameCounter += (int)(passedTime * 60);
            if (frameCounter >= 12)
            {
                frameCounter = 0;
                if (++eyeFrame > 5)
                    eyeFrame = 0;
            }
            int type = Main.LocalPlayer.HeldItem.type;
            Glowing = false;
            if (RedeItem.ChaliceInterest(type))
                Glowing = true;
        }
        private void IconHighlight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!RedeWorld.alignmentGiven || !Main.playerInventory || NPC.AnyNPCs(ModContent.NPCType<Chalice_Intro>()))
                return;

            SoundEngine.PlaySound(SoundID.Chat);
            if (!Main.dedServ)
            {
                int type = Main.LocalPlayer.HeldItem.type;
                if (ItemLists.AlignmentInterest.Contains(type))
                {
                    if (type == ModContent.ItemType<HeartOfThorns>())
                    {
                        if (!RedeBossDowned.downedThorn)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.HeartOfThorns"), 300, 30, 0, Color.DarkGoldenrod);
                        else
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.HeartOfThorns2"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<DemonScroll>())
                    {
                        string fight = RedeBossDowned.erhanDeath < 3 ? Language.GetTextValue("Mods.Redemption.GenericTerms.Words.Fighting") : Language.GetTextValue("Mods.Redemption.GenericTerms.Words.Slaying");
                        if (!RedeBossDowned.downedErhan)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.DemonScroll"), 300, 30, 0, Color.DarkGoldenrod);
                        else
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.DemonScroll2", fight), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<WeddingRing>())
                    {
                        if (RedeBossDowned.downedKeeper && !RedeBossDowned.keeperSaved && !RedeBossDowned.skullDiggerSaved)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.WeddingRing"), 300, 30, 0, Color.DarkGoldenrod);
                        else if (RedeBossDowned.keeperSaved && !RedeBossDowned.skullDiggerSaved)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.WeddingRing2"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<SorrowfulEssence>())
                    {
                        if (!RedeBossDowned.downedSkullDigger && !RedeBossDowned.keeperSaved)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.SorrowfulEssence"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<AbandonedTeddy>())
                    {
                        if (!RedeBossDowned.keeperSaved)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.AbandonedTeddy"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<CyberTech>())
                    {
                        if (!RedeBossDowned.downedSlayer)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.CyberTech"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<SlayerShipEngine>())
                    {
                        if (RedeQuest.slayerRep < 4)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.SlayerShipEngine"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<MemoryChip>())
                    {
                        ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.MemoryChip"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<AnglonicMysticBlossom>())
                    {
                        if (RedeWorld.Alignment > 0 && RedeQuest.forestNymphVar < 2)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.AnglonicMysticBlossom"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<KingsOakStaff>())
                    {
                        if (RedeWorld.Alignment > 0 && RedeQuest.forestNymphVar == 0)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.KingsOakStaff"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<NebSummon>())
                    {
                        string s = "";
                        if (RedeWorld.Alignment < 0)
                            s = Language.GetTextValue("Mods.Redemption.UI.Chalice.NebSummon2");
                        if (!RedeBossDowned.downedNebuleus)
                            ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.NebSummon") + s, RedeWorld.Alignment < 0 ? 500 : 300, 30, 0, Color.DarkGoldenrod);
                        else
                        {
                            if (RedeBossDowned.nebDeath >= 7)
                                ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.NebSummon3"), 300, 30, 0, Color.DarkGoldenrod);
                        }
                    }
                    return;
                }

                if (RedeWorld.Alignment == 0)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A0"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment == -1)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A1N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment == 1)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A1"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= 2 && RedeWorld.Alignment <= 3)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A2"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= -3 && RedeWorld.Alignment <= -2)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A2N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= -5 && RedeWorld.Alignment <= -4)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A3N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= 4 && RedeWorld.Alignment <= 5)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A3"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= -7 && RedeWorld.Alignment <= -6)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A4N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= 6 && RedeWorld.Alignment <= 7)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A4"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= -9 && RedeWorld.Alignment <= -8)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A5N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= 8 && RedeWorld.Alignment <= 9)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A5"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment <= -10)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A6N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.Alignment >= 10)
                    ChaliceAlignmentUI.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A6"), 120, 30, 0, Color.DarkGoldenrod);
            }
        }
        private float fade;
        private int frameCounter;
        private int eyeFrame;
        Asset<Texture2D> eyeTex;
        Asset<Texture2D> eyeFlameTex;
        Asset<Texture2D> flameTex;
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!RedeWorld.alignmentGiven || !Main.playerInventory)
                return;

            if (Glowing)
                fade += 0.1f;
            else
                fade -= 0.1f;

            if (fade > 0)
            {
                Asset<Texture2D> glowTex = CommonTextures.WhiteGlow;
                Vector2 drawOrigin = glowTex.Size() / 2;
                Color c = Color.Orange;
                c.A = 0;
                float cAlpha = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, .8f, 1f, .8f);
                float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, .9f, 1f, .9f);

                spriteBatch.Draw(glowTex.Value, new Vector2(570 + 33, 10 + 46), null, c * cAlpha * fade, 0, drawOrigin, 0.6f * scale, 0, 0);
            }
            fade = MathHelper.Clamp(fade, 0, 1);

            Color color = Color.Lerp(Color.White, Color.Red.WithAlpha(0.9f), -(RedeWorld.Alignment / 10f));
            Color color2 = Color.Lerp(Color.White, Color.Red.WithAlpha(0f), -(RedeWorld.Alignment / 10f));
            if (RedeWorld.Alignment >= 0)
            {
                color = Color.Lerp(Color.White, Color.Cyan.WithAlpha(0), RedeWorld.Alignment / 10f);
                color2 = Color.Lerp(Color.White, Color.Cyan.WithAlpha(0), RedeWorld.Alignment / 10f);
            }

            Asset<Texture2D> tex = TextureAssets.Npc[NPCType<Chalice_Intro>()];
            Rectangle rect = tex.Frame(1, 6, 0, eyeFrame);
            spriteBatch.Draw(tex.Value, new Vector2(570, 10), rect, Color.White, 0, Vector2.Zero, 1, 0, 0);

            flameTex ??= Request<Texture2D>("Redemption/Items/Usable/AlignmentTeller2_Flame");
            spriteBatch.Draw(flameTex.Value, new Vector2(570, 10), rect, color2, 0, Vector2.Zero, 1, 0, 0);

            base.Draw(spriteBatch);

            eyeTex ??= Request<Texture2D>("Redemption/Items/Usable/AlignmentTeller2_Eye");
            rect = eyeTex.Frame(1, 6, 0, eyeFrame);
            Vector2 origin = rect.Size() / 2;

            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
            spriteBatch.Draw(eyeTex.Value, new Vector2(570 + 30, 10 + 11 + offset), rect, Color.White, 0, origin, 1, 0, 0);

            eyeFlameTex ??= Request<Texture2D>("Redemption/Items/Usable/AlignmentTeller2_EyeFlame");
            spriteBatch.Draw(eyeFlameTex.Value, new Vector2(570 + 30, 10 + 11 + offset), rect, color, 0, origin, 1, 0, 0);
        }
    }
}