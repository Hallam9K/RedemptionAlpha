using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Redemption.Globals;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Redemption.Items
{
    public class ThankYouLetter : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Letter");
            /* Tooltip.SetDefault("It reads - '[c/E5F3FB:Thank you for playing the mod, hope you had fun.]" +
                "\n[c/E5F3FB:The mod is still in development so many things can change, and new content will be added, so stay tuned.]" +
                "\n[c/E5F3FB:  -] [c/F9E5FB:Halm] & [c/E5FBE5:Tied]'");*/
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 32;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 0;
            Item.rare = ItemRarityID.Blue;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip2"));
            if (tooltipLocation != -1)
            {
                if (Main.keyState.PressingShift())
                {
                    TooltipLine missLine = new(Mod, "LetterMissedLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Missed")) { OverrideColor = Colors.RarityPink };
                    tooltips.Insert(tooltipLocation + 1, missLine);
                    if (!Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.SpiritWalker")));
                    if (!RedeWorld.alignmentGiven)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Chalice")));
                    if (!RedeBossDowned.foundNewb)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Newb")));
                    if (!RedeBossDowned.downedFowlEmperor)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.FowlEmperor")));
                    if (!RedeBossDowned.downedThorn)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Thorn")));
                    if (!RedeBossDowned.downedErhan)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Erhan")));
                    if (!RedeBossDowned.downedKeeper)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Keeper")));
                    if (RedeQuest.forestNymphVar <= 0)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.ForestNymph")));
                    if (!RedeBossDowned.downedSkullDigger)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.SkullDigger")));
                    if (!RedeBossDowned.keeperSaved)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.KeeperSaved")));
                    if (!RedeBossDowned.skullDiggerSaved)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.SkullDiggerSaved")));
                    if (!RedeBossDowned.downedEaglecrestGolem)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Golem")));
                    if (!RedeBossDowned.downedSeed)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.SoI")));
                    if (!RedeBossDowned.nukeDropped)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Nuke")));
                    if (!RedeBossDowned.downedJanitor)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Lab")));
                    if (!RedeBossDowned.downedSlayer)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.KS3")));
                    if (RedeWorld.slayerRep < 4)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.KS3Quest")));
                    if (!RedeBossDowned.downedOmega1 && !RedeBossDowned.downedOmega2 && !RedeBossDowned.downedOmega3)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.Omega")));
                    if (!RedeBossDowned.downedPZ)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.PatientZero")));
                    if (!RedeBossDowned.downedADD && RedeBossDowned.downedEaglecrestGolem)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.ADD")));
                    if (RedeBossDowned.nebDeath < 7)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.UltimateNeb")));
                }
                else
                {
                    TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.ThankYouLetter.HoldShift"))
                    {
                        OverrideColor = Color.Gray,
                    };
                    tooltips.Insert(tooltipLocation + 1, line);
                }
            }
        }
    }
}
