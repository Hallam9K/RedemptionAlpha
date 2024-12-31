using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals.Player;
using Redemption.Items.Weapons.HM.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Redemption.Items.Accessories.HM
{
    public class GeigerMuller : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Geiger-Muller");
            // Tooltip.SetDefault("Lab issued Geiger counter. The louder it gets, the higher the chance of you getting irradiated.");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.value = 0;
            Item.rare = ItemRarityID.LightRed;
            Item.width = 34;
            Item.height = 28;
            Item.accessory = true;
        }
        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<MullerEffect>().effect = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MullerEffect>().effect = true;
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.LocalPlayer.RedemptionRad().radiationLevel is 0)
                return;
            Color c = BaseUtility.MultiLerpColor((float)(Main.LocalPlayer.RedemptionRad().radiationLevel / 3), Color.White, Color.Yellow, Color.Orange, Color.Red);
            string radPercentage = ((int)(Main.LocalPlayer.RedemptionRad().radiationLevel * 100)).ToString() + "%";
            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, radPercentage, position, Color.Black * .6f, 0, Vector2.Zero - new Vector2(-24, 4), new Vector2(scale - .1f, scale - .1f));
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, radPercentage, position, c, 0, Vector2.Zero - new Vector2(-24, 4), new Vector2(scale - .1f, scale - .1f));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            Radiation modPlayer = player.RedemptionRad();
            string rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status1");
            string rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note1");
            if (modPlayer.radiationLevel is >= .5f and < 1f)
            {
                rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status2");
                rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note1");
            }
            else if (modPlayer.radiationLevel is >= 1f and < 1.5f)
            {
                rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status3");
                rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note2");
            }
            else if (modPlayer.radiationLevel is >= 1.5f and < 2f)
            {
                rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status4");
                rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note2");
            }
            else if (modPlayer.radiationLevel is >= 2f and < 2.5f)
            {
                rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status5");
                rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note3");
            }
            else if (modPlayer.radiationLevel is >= 2.5f)
            {
                rad = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Status6");
                rad2 = Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Note4");
            }

            Color c = BaseUtility.MultiLerpColor((float)(modPlayer.radiationLevel / 3), Color.LightGreen, Color.Yellow, Color.Orange, Color.Red);

            string text1 = rad + Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.StatusEnd") + rad2;
            TooltipLine line = new(Mod, "Geiger1", text1) { OverrideColor = Color.LightGreen };
            TooltipLine line2 = new(Mod, "Geiger2", ((int)(modPlayer.radiationLevel * 100)).ToString() + "%" + Language.GetTextValue("Mods.Redemption.Items.GeigerMuller.Irradiated")) { OverrideColor = c };
            tooltips.Insert(2, line);
            tooltips.Insert(3, line2);
        }
    }

    public class MullerEffect : ModPlayer
    {
        public bool effect;
        public override void ResetEffects()
        {
            effect = false;
        }
    }
}
