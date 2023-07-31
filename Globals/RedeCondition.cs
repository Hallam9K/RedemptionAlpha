using Redemption.NPCs.Minibosses.SkullDigger;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.NPCs.Minibosses.FowlEmperor;
using Terraria;
using Redemption.Base;
using Redemption.Items.Armor.Vanity.TBot;

namespace Redemption.Globals
{
    public static class RedeConditions
    {
        public static Condition DownedThorn = new("Mods.Redemption.Conditions.DownedThorn", () => RedeBossDowned.downedThorn);
        public static Condition DownedErhan = new("Mods.Redemption.Conditions.DownedErhan", () => RedeBossDowned.downedErhan);
        public static Condition DownedADD = new("Mods.Redemption.Conditions.DownedADD", () => RedeBossDowned.downedADD);
        public static Condition DownedBehemoth = new("Mods.Redemption.Conditions.DownedBehemoth", () => RedeBossDowned.downedBehemoth);
        public static Condition DownedBlisterface = new("Mods.Redemption.Conditions.DownedBlisterface", () => RedeBossDowned.downedBlisterface);
        public static Condition DownedEaglecrestGolem = new("Mods.Redemption.Conditions.DownedEaglecrestGolem", () => RedeBossDowned.downedEaglecrestGolem);
        public static Condition DownedFowlEmperor = new("Mods.Redemption.Conditions.DownedFowlEmperor", () => RedeBossDowned.downedFowlEmperor);
        public static Condition DownedFowlMorning = new("Mods.Redemption.Conditions.DownedFowlMorning", () => RedeBossDowned.downedFowlMorning);
        public static Condition DownedJanitor = new("Mods.Redemption.Conditions.DownedJanitor", () => RedeBossDowned.downedJanitor);
        public static Condition DownedKeeper = new("Mods.Redemption.Conditions.DownedKeeper", () => RedeBossDowned.downedKeeper);
        public static Condition DownedMACE = new("Mods.Redemption.Conditions.DownedMACE", () => RedeBossDowned.downedMACE);
        public static Condition DownedNebuleus = new("Mods.Redemption.Conditions.DownedNebuleus", () => RedeBossDowned.downedNebuleus);
        public static Condition DownedOmega1 = new("Mods.Redemption.Conditions.DownedOmega1", () => RedeBossDowned.downedOmega1);
        public static Condition DownedOmega2 = new("Mods.Redemption.Conditions.DownedOmega2", () => RedeBossDowned.downedOmega2);
        public static Condition DownedOmega3 = new("Mods.Redemption.Conditions.DownedOmega3", () => RedeBossDowned.downedOmega3);
        public static Condition DownedPZ = new("Mods.Redemption.Conditions.DownedPZ", () => RedeBossDowned.downedPZ);
        public static Condition DownedSeed = new("Mods.Redemption.Conditions.DownedSeed", () => RedeBossDowned.downedSeed);
        public static Condition DownedSkullDigger = new("Mods.Redemption.Conditions.DownedSkullDigger", () => RedeBossDowned.downedSkullDigger);
        public static Condition DownedVolt = new("Mods.Redemption.Conditions.DownedVolt", () => RedeBossDowned.downedVolt);
        public static Condition DownedSlayer = new("Mods.Redemption.Conditions.DownedSlayer", () => RedeBossDowned.downedSlayer);
        public static Condition BroughtCat = new("Mods.Redemption.Conditions.BroughtCat", () => Terraria.NPC.boughtCat);
        public static Condition IsTBotHead = new("Mods.Redemption.Conditions.IsTBotHead", () => Main.LocalPlayer.IsTBotHead());
        public static Condition IsJanitor = new("Mods.Redemption.Conditions.IsJanitor", () => BasePlayer.HasChestplate(Main.LocalPlayer, ModContent.ItemType<JanitorOutfit>(), true) && BasePlayer.HasLeggings(Main.LocalPlayer, ModContent.ItemType<JanitorPants>(), true));
        public static Condition KeycardGiven = new("Mods.Redemption.Conditions.KeycardGiven", () => RedeWorld.keycardGiven);
        public static Condition NukeDropped = new("Mods.Redemption.Conditions.NukeDropped", () => RedeBossDowned.nukeDropped);
        public static Condition NukeDroppedOrDownedMechBossAll = new("Mods.Redemption.Conditions.NukeDroppedOrDownedMechBossAll", () => RedeBossDowned.nukeDropped || Condition.DownedMechBossAny.IsMet());
        public static Condition IsFinlandDay = new("Mods.Redemption.Conditions.IsFinlandDay", () => Redemption.FinlandDay);
        public static Condition IsNotFinlandDay = new("Mods.Redemption.Conditions.IsNotFinlandDay", () => !Redemption.FinlandDay);
        public static Condition InMoonlight = new("Mods.Redemption.Conditions.InMoonlight", () => !Main.dayTime && Main.moonPhase != 4);
        public static Condition RepairedByFallen = new("Mods.Redemption.Conditions.RepairedByFallen", () => false);
        public static Condition DownedEarlyGameBossAndMoR = new("Mods.Redemption.Conditions.DownedEarlyGameBossAndMoR", () => Terraria.NPC.downedBoss1 || Terraria.NPC.downedSlimeKing || RedeBossDowned.downedThorn || RedeBossDowned.downedErhan);
        public static Condition DownedEoCOrBoCOrKeeper = new("Mods.Redemption.Conditions.DownedEoCOrBoCOrKeeper", () => Terraria.NPC.downedBoss2 || RedeBossDowned.downedKeeper);
        public static Condition DownedSkeletronOrSeed = new("Mods.Redemption.Conditions.DownedSkeletronOrSeed", () => Terraria.NPC.downedBoss3 || RedeBossDowned.downedSeed);
        public static Condition HasSpiritWalker = new("Mods.Redemption.Conditions.HasSpiritWalker", () => Main.LocalPlayer.RedemptionAbility().Spiritwalker);
        public static Condition DeadRingerGiven = new("Mods.Redemption.Conditions.DeadRingerGiven", () => RedeWorld.deadRingerGiven);
        public static Condition ForestNymphTrust = new("Mods.Redemption.Conditions.ForestNymphTrust", () => RedeQuest.forestNymphVar > 0);
    }
    public class DecapitationCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation && (NPCLists.SkeletonHumanoid.Contains(info.npc.type) || NPCLists.Humanoid.Contains(info.npc.type)))
			{
				return info.npc.Redemption().decapitated;
			}
			return false;
		}
		public bool CanShowItemDropInUI() => false;
		public string GetConditionDescription() => "Drops when decapitated";
	}
	public class LostSoulCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) => false;
		public bool CanShowItemDropInUI() => true;
		public string GetConditionDescription() => "Appears as an NPC";
	}
	public class OnFireCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) => false;
		public bool CanShowItemDropInUI() => true;
		public string GetConditionDescription() => "Dropped while on fire";
	}
	public class TeddyCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation && info.npc.type == ModContent.NPCType<SkullDigger>() && !(info.npc.ModNPC as SkullDigger).KeeperSpawn)
				return true;

			return false;
		}
		public bool CanShowItemDropInUI() => true;
		public string GetConditionDescription() => "Dropped when spawned naturally in the caverns";
	}
	public class YoyosTidalWake : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation && !info.npc.SpawnedFromStatue && info.player.ZoneBeach && Terraria.NPC.downedMechBossAny)
				return true;

			return false;
		}
		public bool CanShowItemDropInUI() => true;
		public string GetConditionDescription() => "Dropped from enemies at the beach after any mech boss is defeated";
	}
	public class EggCrackerCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation && Terraria.NPC.AnyNPCs(ModContent.NPCType<FowlEmperor>()))
				return true;

			return false;
		}
		public bool CanShowItemDropInUI() => true;
		public string GetConditionDescription() => "Dropped while the Fowl Emperor is alive";
	}
}
