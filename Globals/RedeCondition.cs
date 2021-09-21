using Redemption.Globals.NPC;
using Redemption.NPCs.Minibosses.SkullDigger;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

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
			if (!info.IsInSimulation && info.npc.FindBuffIndex(BuffID.OnFire) != -1 && info.npc.FindBuffIndex(BuffID.OnFire3) != -1)
				return true;

			return false;
		}
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