using BetterDialogue.UI;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Lab;
using ReLogic.Content;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;
using static Redemption.NPCs.ModRedeNPC;

namespace Redemption.UI.Dialect
{
    public class RedeGlobalDialogue : GlobalDialogueStyle
    {
        public override void PostDraw(DialogueStyle activeStyle, NPC npc, Player player, List<ChatButton> activeChatButtons)
        {
            foreach (ChatButton button in activeChatButtons)
            {
                if (npc == null)
                    return;
                if (!button.IsHovered || button.Description(npc, player) == null || npc.ModNPC?.Mod is not Redemption || button == ChatButton.Shop || button == ChatButton.TownNPCHappiness || button == ChatButton.Exit)
                    continue;

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, ChatButtonLoader.GetDescription(button, npc, player), Main.MouseWorld + new Vector2(20) - Main.screenPosition, activeStyle.ChatButtonColor, Color.Black, 0f, Vector2.Zero, Vector2.One);
            }
        }
    }
    public abstract class DialogueBoxBase : DialogueStyle
    {
        protected abstract int BoxStyleID { get; }
        protected abstract string BoxStyleName { get; }

        public override string DisplayName => BoxStyleName;
        public override Texture2D DialogueBoxTileSheet => Request<Texture2D>("Redemption/UI/Dialect/DialogueBox_" + BoxStyleName, AssetRequestMode.ImmediateLoad).Value;
        public override string Description => string.Empty;
        public override int BoxWidthModifier => 5;
        protected readonly Color someShadeOfGray = new(200, 200, 200, 200);

        public virtual bool SafeForceActive(NPC npc) => true;
        public override bool ForceActive(NPC npc, Player player)
        {
            ModRedeNPC redeNPC = null;
            return npc != null && SafeForceActive(npc) && npc.IsRedeNPC(ref redeNPC) && redeNPC.DialogueBoxStyle == BoxStyleID;
        }
        public virtual string ModifyNameText(NPC npc) => null;
        public override void PostDraw(NPC npc, Player player, List<ChatButton> activeChatButtons)
        {
            if (npc is null)
                return;

            if (npc.ModNPC == null || string.IsNullOrEmpty(npc.ModNPC.Name))
                return;

            string customName = string.Empty;
            string entry = Language.GetTextValue("Mods.Redemption.NPCs." + npc.ModNPC.Name + ".ShortName");
            if (!string.IsNullOrEmpty(entry) && !entry.Contains("Mods."))
                customName = entry;

            if (!string.IsNullOrEmpty(ModifyNameText(npc)))
                customName = ModifyNameText(npc);

            if (npc.IsRedeNPC(ref RedeGlobalButton.redeNPC) && RedeGlobalButton.redeNPC.HasReviveButton())
                return;
            DrawNameCard(player, npc, someShadeOfGray, ChatButtonColor, BoxStyleName, customName);
        }
        public override bool PreDraw(NPC npc, Player player, List<ChatButton> activeChatButtons)
        {
            if (npc is null)
                return base.PreDraw(npc, player, activeChatButtons);

            ModRedeNPC redeNPC = null;
            if (!npc.IsRedeNPC(ref redeNPC))
                return base.PreDraw(npc, player, activeChatButtons);

            if (redeNPC.HasLeftHangingButton(player))
            {
                HangingButtonParams button = redeNPC.LeftHangingButton(player);
                for (int i = 0; i < button.Count; i++)
                    DrawHangingBox(player, someShadeOfGray, new Vector2(0, (46 * i) + button.PositionY), BoxStyleName, i > 0, button.Glow && RedeGlobalButton.talkID == i + 1);
            }
            if (redeNPC.HasRightHangingButton(player))
            {
                HangingButtonParams button = redeNPC.RightHangingButton(player);
                for (int i = 0; i < button.Count; i++)
                    DrawHangingBox(player, someShadeOfGray, new Vector2(300, (46 * i) + button.PositionY), BoxStyleName, i > 0, button.Glow && RedeGlobalButton.talkID == i + 1);
            }
            return base.PreDraw(npc, player, activeChatButtons);
        }
        #region Draw Methods
        public static void DrawPortrait(Player player, Color color, string name)
        {
            Rectangle rectangle = new((Main.screenWidth / 2) - 384 - 45, 154 - 45, 90, 90);
            if (rectangle.Contains(Main.MouseScreen.ToPoint()))
                player.mouseInterface = true;

            Texture2D portrait = Request<Texture2D>("Redemption/UI/Dialect/Portrait_" + name, AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(portrait, new Vector2((Main.screenWidth / 2) - 384, 154), null, color, 0, portrait.Size() / 2f, 1, 0, 0);
        }
        public static void DrawNameCard(Player player, NPC npc, Color color, Color buttonColor, string name, string customName = "")
        {
            if (npc.GivenOrTypeName == string.Empty && customName == string.Empty)
                return;

            DynamicSpriteFont font = FontAssets.MouseText.Value;
            int textLength = (int)font.MeasureString(!string.IsNullOrEmpty(customName) ? customName : npc.GivenOrTypeName).X;
            int textHeight = (int)font.MeasureString(!string.IsNullOrEmpty(customName) ? customName : npc.GivenOrTypeName).Y;
            Rectangle rectangle = new((Main.screenWidth / 2) - 255 - 75, 77 - 23, 150, 46);
            if (rectangle.Contains(Main.MouseScreen.ToPoint()))
                player.mouseInterface = true;

            Texture2D nameCard = Request<Texture2D>("Redemption/UI/Dialect/NameCard_" + name, AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(nameCard, new Vector2((Main.screenWidth / 2) - 255, 77), null, color, 0, nameCard.Size() / 2f, 1, 0, 0);
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, !string.IsNullOrEmpty(customName) ? customName : npc.GivenOrTypeName,
                new Vector2((Main.screenWidth / 2) - 255 - (textLength / 2), 81 - (textHeight / 2)), buttonColor, Color.Black, 0f, Vector2.Zero, Vector2.One);
        }
        public static void DrawHangingBox(Player player, Color color, Vector2 position, string name, bool noChain = false, bool glow = false)
        {
            int mouseTextColor = (Main.mouseTextColor * 2 + 255) / 3;
            Color baseTextColor = new(mouseTextColor, mouseTextColor, mouseTextColor, mouseTextColor);
            Cache.PrepareCache(Main.npcChatText, baseTextColor);
            int amountOfLines = Cache.AmountOfLines;

            Rectangle rectangle = new((Main.screenWidth / 2) - 150 - 120 + (int)position.X, 190 - 30 + (amountOfLines * 30) + (int)position.Y, 240, 60);
            if (rectangle.Contains(Main.MouseScreen.ToPoint()))
                player.mouseInterface = true;

            Texture2D nameCard = Request<Texture2D>("Redemption/UI/Dialect/HangBox_" + name, AssetRequestMode.ImmediateLoad).Value;
            Rectangle? texRect = null;
            Vector2 origin = nameCard.Size() / 2f;
            if (noChain)
            {
                texRect = new Rectangle?(new Rectangle(0, 14, nameCard.Width, nameCard.Height - 14));
                origin.Y -= 14;
            }
            Main.spriteBatch.Draw(nameCard, new Vector2((Main.screenWidth / 2) - 150, 190 + (amountOfLines * 30)) + position, texRect, color, 0, origin, 1, 0, 0);
            if (!glow)
                return;
            Texture2D nameCardGlow = Request<Texture2D>("Redemption/UI/Dialect/HangBox_" + name + "_Glow", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(nameCardGlow, new Vector2((Main.screenWidth / 2) - 150, 190 + (amountOfLines * 30)) + position, texRect, Color.White, 0, origin, 1, 0, 0);
        }
        #endregion
    }
    public class DialogueBox_Epidotra : DialogueBoxBase
    {
        protected override int BoxStyleID => EPIDOTRA;
        protected override string BoxStyleName => "Epidotra";

        public override string ModifyNameText(NPC npc)
        {
            if (npc.ModNPC != null && npc.ModNPC is TreebarkDryad dryad)
                return dryad.setName;
            return null;
        }
    }
    public class DialogueBox_Kingdom : DialogueBoxBase
    {
        protected override int BoxStyleID => KINGDOM;
        protected override string BoxStyleName => "Kingdom";

        public override Color ChatButtonColor => new((byte)(255f * Main.mouseTextColor / 255f), (byte)(150f * Main.mouseTextColor / 255f), (byte)(0f * Main.mouseTextColor / 255f), Main.mouseTextColor);
    }
    public class DialogueBox_Cave : DialogueBoxBase
    {
        protected override int BoxStyleID => CAVERN;
        protected override string BoxStyleName => "Cave";

        public override string DisplayName => "Cavern";
        public override Color ChatButtonColor => new((byte)(160f * Main.mouseTextColor / 255f), (byte)(180f * Main.mouseTextColor / 255f), (byte)(210f * Main.mouseTextColor / 255f), Main.mouseTextColor);

        public override string ModifyNameText(NPC npc)
        {
            if (npc.type == NPCType<SkullDiggerFriendly_Spirit>())
                return Language.GetTextValue("Mods.Redemption.NPCs.SkullDiggerFriendly.DisplayName");
            return null;
        }
    }
    public class DialogueBox_Demon : DialogueBoxBase
    {
        protected override int BoxStyleID => DEMON;
        protected override string BoxStyleName => "Demon";

        //public override Color ChatButtonColor => new((byte)(160f * Main.mouseTextColor / 255f), (byte)(180f * Main.mouseTextColor / 255f), (byte)(210f * Main.mouseTextColor / 255f), Main.mouseTextColor);
    }
    public class DialogueBox_Liden : DialogueBoxBase
    {
        protected override int BoxStyleID => LIDEN;
        protected override string BoxStyleName => "Liden";

        public override bool SafeForceActive(NPC npc)
        {
            if (npc.type == NPCType<JustANormalToaster>() && RedeBossDowned.downedOmega3)
                return false;
            return base.SafeForceActive(npc);
        }
    }
    public class DialogueBox_Omega : DialogueBoxBase
    {
        protected override int BoxStyleID => OMEGA;
        protected override string BoxStyleName => "Omega";

        public override bool SafeForceActive(NPC npc)
        {
            if (npc.type == NPCType<JustANormalToaster>() && !RedeBossDowned.downedOmega3)
                return false;
            return base.SafeForceActive(npc);
        }
        public override Color ChatButtonColor => new((byte)(255 * Main.mouseTextColor / 255f), (byte)(150 * Main.mouseTextColor / 255f), (byte)(150 * Main.mouseTextColor / 255f), Main.mouseTextColor);
    }
    public class DialogueBox_Slayer : DialogueBoxBase
    {
        protected override int BoxStyleID => SLAYER;
        protected override string BoxStyleName => "Slayer";

        public override Color ChatButtonColor => new((byte)(170 * Main.mouseTextColor / 255f), (byte)(255 * Main.mouseTextColor / 255f), (byte)(255 * Main.mouseTextColor / 255f), Main.mouseTextColor);
        public override void PostDraw(NPC npc, Player player, List<ChatButton> activeChatButtons)
        {
            if (npc is null)
                return;
            if (npc.type == NPCType<KS3Sitting>())
                DrawPortrait(player, someShadeOfGray, Redemption.AprilFools ? "Slayer2" : "Slayer");
            DrawNameCard(player, npc, someShadeOfGray, ChatButtonColor, BoxStyleName);
        }
    }
    public class DialogueBox_Neb : DialogueBoxBase
    {
        protected override int BoxStyleID => NEBULEUS;
        protected override string BoxStyleName => "Neb";

        public override string DisplayName => "Nebuleus";
        public override Color ChatButtonColor => new((byte)(255f * Main.mouseTextColor / 255f), (byte)(100f * Main.mouseTextColor / 255f), (byte)(174f * Main.mouseTextColor / 255f), Main.mouseTextColor);
    }
    public class DialogueBox_Black : DialogueBoxBase
    {
        protected override int BoxStyleID => BLACK;
        protected override string BoxStyleName => "Black";

        public override void PostDraw(NPC npc, Player player, List<ChatButton> activeChatButtons) { }
    }
}