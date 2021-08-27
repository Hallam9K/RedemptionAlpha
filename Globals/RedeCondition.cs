using Redemption.Globals.NPC;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Redemption.Globals
{
	public class DecapitationCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation && NPCTags.SkeletonHumanoid.Has(info.npc.type))
			{
				return info.npc.GetGlobalNPC<RedeNPC>().decapitated;
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
		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation && info.npc.FindBuffIndex(BuffID.OnFire) != -1)
			{
				return true;
			}
			return false;
		}
		public bool CanShowItemDropInUI() => true;
		public string GetConditionDescription() => "Dropped while on fire";
	}
}