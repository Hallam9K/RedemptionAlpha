using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Redemption
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
}