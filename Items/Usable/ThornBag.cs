using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.Items.Weapons.PreHM.Summon;
using Redemption.NPCs.Bosses.Thorn;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ThornBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Treasure Bag (Thorn)");
            // Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");

            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 34;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }

        public override bool CanRightClick() => true;
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ItemType<ThornMask>(), 7));
            itemLoot.Add(ItemDropRule.FewFromOptions(2, 1, ItemType<AldersStaff>(), ItemType<CursedGrassBlade>(), ItemType<RootTendril>(), ItemType<CursedThornBow>()));
            itemLoot.Add(ItemDropRule.Common(ItemType<CircletOfBrambles>()));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<Thorn>()));
        }
    }
}