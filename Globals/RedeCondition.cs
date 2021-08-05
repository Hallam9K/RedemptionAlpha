using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Redemption
{
	public class DecapitationCondition : IItemDropRuleCondition, IProvideItemConditionDescription
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			if (info.IsInSimulation)
			{
				return false;
			}
			return info.npc.GetGlobalNPC<RedeNPC>().decapitated;
		}
		public bool CanShowItemDropInUI() => false;
		public string GetConditionDescription() => "Drops when decapitated";
	}
}