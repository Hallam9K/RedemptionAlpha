using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Friendly;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class AlignmentButton : UIState
    {
        private readonly UIImage AlignmentButtonTexture = new(ModContent.Request<Texture2D>("Redemption/UI/AlignmentButton", AssetRequestMode.ImmediateLoad));
        private readonly Asset<Texture2D> AlignmentButton_MouseOverTexture = ModContent.Request<Texture2D>("Redemption/UI/AlignmentButton_MouseOver", AssetRequestMode.ImmediateLoad);

        public UIImage Icon;
        public UIHoverTextImageButton IconHighlight;
        public static bool Glowing = false;

        public override void OnActivate()
        {
            Icon = AlignmentButtonTexture;
            Icon.Left.Set(570, 0f);
            Icon.Top.Set(20, 0f);
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
            IconHighlight.Text = Language.GetTextValue("Mods.Redemption.UI.Chalice.Alignment") + ": " + RedeWorld.alignment;
            if (ItemLists.AlignmentInterest.Contains(Main.LocalPlayer.HeldItem.type))
                IconHighlight.Text += "\n" + Language.GetTextValue("Mods.Redemption.UI.Chalice.ItemInterested");
        }
        public override void Update(GameTime gameTime)
        {
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
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.HeartOfThorns"), 300, 30, 0, Color.DarkGoldenrod);
                        else
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.HeartOfThorns2"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<DemonScroll>())
                    {
                        string fight = RedeBossDowned.erhanDeath < 3 ? Language.GetTextValue("Mods.Redemption.GenericTerms.Words.Fighting") : Language.GetTextValue("Mods.Redemption.GenericTerms.Words.Slaying");
                        if (!RedeBossDowned.downedErhan)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.DemonScroll"), 300, 30, 0, Color.DarkGoldenrod);
                        else
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.DemonScroll2", fight), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<WeddingRing>())
                    {
                        if (RedeBossDowned.downedKeeper && !RedeBossDowned.keeperSaved && !RedeBossDowned.skullDiggerSaved)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.WeddingRing"), 300, 30, 0, Color.DarkGoldenrod);
                        else if (RedeBossDowned.keeperSaved && !RedeBossDowned.skullDiggerSaved)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.WeddingRing2"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<SorrowfulEssence>())
                    {
                        if (!RedeBossDowned.downedSkullDigger)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.SorrowfulEssence"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<AbandonedTeddy>())
                    {
                        if (!RedeBossDowned.keeperSaved)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.AbandonedTeddy"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<CyberTech>())
                    {
                        if (!RedeBossDowned.downedSlayer)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.CyberTech"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<SlayerShipEngine>())
                    {
                        if (RedeWorld.slayerRep < 4)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.SlayerShipEngine"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<MemoryChip>())
                    {
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.MemoryChip"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<AnglonicMysticBlossom>())
                    {
                        if (RedeWorld.alignment > 0 && RedeQuest.forestNymphVar < 2)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.AnglonicMysticBlossom"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<KingsOakStaff>())
                    {
                        if (RedeWorld.alignment > 0 && RedeQuest.forestNymphVar == 0)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.KingsOakStaff"), 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<NebSummon>())
                    {
                        string s = "";
                        if (RedeWorld.alignment < 0)
                            s = Language.GetTextValue("Mods.Redemption.UI.Chalice.NebSummon2");
                        if (!RedeBossDowned.downedNebuleus)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.NebSummon") + s, RedeWorld.alignment < 0 ? 500 : 300, 30, 0, Color.DarkGoldenrod);
                        else
                        {
                            if (RedeBossDowned.nebDeath >= 7)
                                RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.NebSummon3"), 300, 30, 0, Color.DarkGoldenrod);
                        }
                    }
                    return;
                }
                if (RedeWorld.alignment == 0)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A0"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment == -1)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A1N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment == 1)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A1"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 2 && RedeWorld.alignment <= 3)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A2"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -3 && RedeWorld.alignment <= -2)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A2N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -5 && RedeWorld.alignment <= -4)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A3N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 4 && RedeWorld.alignment <= 5)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A3"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -7 && RedeWorld.alignment <= -6)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A4N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 6 && RedeWorld.alignment <= 7)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A4"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -9 && RedeWorld.alignment <= -8)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A5N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 8 && RedeWorld.alignment <= 9)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A5"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment <= -10)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A6N"), 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 10)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.UI.Chalice.A6"), 120, 30, 0, Color.DarkGoldenrod);
            }
        }
        private float fade;
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
                Texture2D glowTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
                Vector2 drawOrigin = new(glowTex.Width / 2, glowTex.Height / 2);
                Color c = Color.Orange;
                c.A = 0;
                float cAlpha = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, .8f, 1f, .8f);
                float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, .9f, 1f, .9f);

                spriteBatch.Draw(glowTex, new Vector2(570 + 41, 20 + 31), null, c * cAlpha * fade, 0, drawOrigin, 0.6f * scale, 0, 0);
            }
            fade = MathHelper.Clamp(fade, 0, 1);
            base.Draw(spriteBatch);
        }
    }
}
