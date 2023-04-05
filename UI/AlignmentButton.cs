using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Quest.KingSlayer;
using Redemption.Items.Usable;
using Redemption.Items.Usable.Summons;
using Redemption.Items.Weapons.PreHM.Summon;
using ReLogic.Content;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent.Bestiary;
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
            IconHighlight.Text = "Alignment: " + RedeWorld.alignment;
            if (ItemLists.AlignmentInterest.Contains(Main.LocalPlayer.HeldItem.type))
                IconHighlight.Text += "\n[c/ffea9b:There is something to say about the item you hold]";
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
            if (!RedeWorld.alignmentGiven || !Main.playerInventory)
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
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("A piece of blight from Faywood forest, cursed by an ancient power.\nSlaying whatever this may attract will surely do good.", 300, 30, 0, Color.DarkGoldenrod);
                        else
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("The blight should quell with this unfortunate warden slain.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<DemonScroll>())
                    {
                        string fight = RedeBossDowned.erhanDeath < 3 ? "fighting" : "slaying";
                        if (!RedeBossDowned.downedErhan)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("A forbidden scroll, used to summon great demon terrors to the land.\nThese are risky things, do not be tempted by it; lest you desire the path of sin.", 300, 30, 0, Color.DarkGoldenrod);
                        else
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Attempting to summon a demon and " + fight + " a priest... Are you alright in the head?", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<WeddingRing>())
                    {
                        if (RedeBossDowned.downedKeeper && !RedeBossDowned.keeperSaved && !RedeBossDowned.skullDiggerSaved)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("An undead... Something tells me she isn't out of her misery just yet.", 300, 30, 0, Color.DarkGoldenrod);
                        else if (RedeBossDowned.keeperSaved && !RedeBossDowned.skullDiggerSaved)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("A part of the mistress's spirit remains in this ring. Mayhaps a certain protector may find solace in keeping it.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<SorrowfulEssence>())
                    {
                        if (!RedeBossDowned.downedSkullDigger)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Octavia's protector is in search of you. A conflict will be inevitable.\nHowever, mayhaps this may bring a chance for a courteous ending.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<AbandonedTeddy>())
                    {
                        if (!RedeBossDowned.keeperSaved)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("A ragged teddy, and a reminder of a hopeful future.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<CyberTech>())
                    {
                        if (!RedeBossDowned.downedSlayer)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("A signal-sender to one of the Heroes of the Golden Age.\nBe warned, he has a rather short temper.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<SlayerShipEngine>())
                    {
                        if (RedeWorld.slayerRep < 4)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Although a small thing, this shows your willingness to help.\nI'm sure King Slayer will appreciate it.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<MemoryChip>())
                    {
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("On first glance, it seems like a useless gift to someone like you.\nAnd although it may be true, I don't doubt this may play a crucial role in the future.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<AnglonicMysticBlossom>())
                    {
                        if (RedeWorld.alignment > 0 && RedeQuest.forestNymphVar < 2)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("A beauty of nature, often gifted to those whom wish eternal blessings.\nPerhaps you'll find a worthy friend to gift this to one day.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<KingsOakStaff>())
                    {
                        if (RedeWorld.alignment > 0 && RedeQuest.forestNymphVar == 0)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("A staff that calls to the critters of Nature.\nDo not be perturbed by their verbose spirit, for they may even convince a certain creature of your friendship.", 300, 30, 0, Color.DarkGoldenrod);
                    }
                    else if (type == ModContent.ItemType<NebSummon>())
                    {
                        string s = "";
                        if (RedeWorld.alignment < 0)
                            s = " Her past is a mystery,\nyet in my soul I know the importance of her role in this world's protection.\nBased on your past actions, I foresee a terrible future.";
                        if (!RedeBossDowned.downedNebuleus)
                            RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("This beckons a silent observer to test your strength." + s, RedeWorld.alignment < 0 ? 500 : 300, 30, 0, Color.DarkGoldenrod);
                        else
                        {
                            if (RedeBossDowned.nebDeath >= 7)
                                RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("... I see it now.", 300, 30, 0, Color.DarkGoldenrod);
                        }
                    }
                    return;
                }
                if (RedeWorld.alignment == 0)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are truly neutral...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment == -1)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You have done harm, but you are safe for now...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment == 1)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You have done good so far...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 2 && RedeWorld.alignment <= 3)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are choosing the right path. Please, continue.", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -3 && RedeWorld.alignment <= -2)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Be wary, you are straying from the path of good...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -5 && RedeWorld.alignment <= -4)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are really pushing it aren't you... If you continue down this road, justice will await you.", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 4 && RedeWorld.alignment <= 5)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("I am proud of you for keeping the light within you bright...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -7 && RedeWorld.alignment <= -6)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("... Listen, you are following the wrong path here... Please, go back.", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 6 && RedeWorld.alignment <= 7)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Vanquishing the evil of the world... You really are something.", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -9 && RedeWorld.alignment <= -8)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("I am sorry, you can't go back now...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 8 && RedeWorld.alignment <= 9)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Light shines within you, but I am sure more dangerous foes lie ahead...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment <= -10)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are past redemption...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 10)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are the redemption this world needed...", 120, 30, 0, Color.DarkGoldenrod);
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
