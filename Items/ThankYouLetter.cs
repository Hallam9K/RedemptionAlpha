using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Localization;
using Redemption.Globals;
using Redemption.WorldGeneration;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework;

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
                    TooltipLine missLine = new(Mod, "LetterMissedLine", "\nThings you didn't do this playthrough, but can do for the next:") { OverrideColor = Colors.RarityPink };
                    tooltips.Insert(tooltipLocation + 1, missLine);
                    if (!Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Gaining a spiritual ability"));
                    if (!RedeWorld.alignmentGiven)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Gaining an important object of judgement"));
                    if (!RedeBossDowned.foundNewb)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Finding a suspicious entity beneath a portal"));
                    if (!RedeBossDowned.downedFowlEmperor)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting a mighty rooster"));
                    if (!RedeBossDowned.downedThorn)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting a blighted bane of the forest [c/bbf160:(Good Route)]"));
                    if (!RedeBossDowned.downedErhan)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting a high priest [c/ff5533:(Bad Route)]"));
                    if (!RedeBossDowned.downedKeeper)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting a sorrowful undead"));
                    if (RedeQuest.forestNymphVar <= 0)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Befriending a being of nature and giving it a tree-home [c/bbf160:(Good Route)]"));
                    if (!RedeBossDowned.downedSkullDigger)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting an undead protector underground [c/ff5533:(Bad Route)]"));
                    if (!RedeBossDowned.keeperSaved)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Bringing peace to an undead mistress [c/bbf160:(Good Route)]"));
                    if (!RedeBossDowned.skullDiggerSaved)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Bringing peace to an undead protector [c/bbf160:(Good Route)]"));
                    if (!RedeBossDowned.downedEaglecrestGolem)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting a strange boulder in the overworld"));
                    if (!RedeBossDowned.downedSeed)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting an infectious anomaly"));
                    if (!RedeBossDowned.nukeDropped)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Detonating a nuclear warhead"));
                    if (!RedeBossDowned.downedJanitor)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Discovering an underground laboratory"));
                    if (!RedeBossDowned.downedSlayer)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting a depressed robot"));
                    if (RedeWorld.slayerRep < 4)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Helping a depressed robot preform ship repairs [c/bbf160:(Good Route)]"));
                    if (!RedeBossDowned.downedOmega1 && !RedeBossDowned.downedOmega2 && !RedeBossDowned.downedOmega3)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting a trio of deadly machines"));
                    if (!RedeBossDowned.downedPZ)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting the laboratory's final challenge"));
                    if (!RedeBossDowned.downedADD && RedeBossDowned.downedEaglecrestGolem)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Figuring out the Eaglecrest Golem's secret"));
                    if (RedeBossDowned.nebDeath < 7)
                        tooltips.Insert(tooltipLocation + 2, new(Mod, "LetterLine", "-Fighting the Angel of the Cosmos' last stand [c/ff5533:(Bad Route)]"));
                }
                else
                {
                    TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view what you've missed")
                    {
                        OverrideColor = Color.Gray,
                    };
                    tooltips.Insert(tooltipLocation + 1, line);
                }
            }
        }
    }
}
