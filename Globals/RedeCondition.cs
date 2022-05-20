using Redemption.NPCs.Minibosses.SkullDigger;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Globals
{
    public class DecapitationCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info)
		{
			if (!info.IsInSimulation && NPCLists.SkeletonHumanoid.Contains(info.npc.type))
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
}