using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.HM;
using Redemption.NPCs.Bosses.Cleaver;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
	public class VlitchCleaverBag : ModItem
	{
		public override int BossBagNPC => ModContent.NPCType<VlitchCleaver>();

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Box");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
		public override void SetDefaults()
		{
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Expert;
			Item.expert = true;
            if (!Main.dedServ)
            {
                Item.GetGlobalItem<ItemUseGlow>().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            }
        }
		public override bool CanRightClick()
		{
			return true;
		}
		public override void OpenBossBag(Player player)
		{
			/*if (Main.rand.Next(20) == 0)
			{
				player.QuickSpawnItem(ModContent.ItemType<IntruderMask>(), 1);
				player.QuickSpawnItem(ModContent.ItemType<IntruderArmour>(), 1);
				player.QuickSpawnItem(ModContent.ItemType<IntruderPants>(), 1);
			}*/
			if (Main.rand.Next(14) == 0)
			{
				player.QuickSpawnItem(ModContent.ItemType<GirusMask>(), 1);
			}
			/*if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(ModContent.ItemType<GirusDagger>(), 1);
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(ModContent.ItemType<GirusLance>(), 1);
			}*/
			player.QuickSpawnItem(ModContent.ItemType<CorruptedXenomiteItem>(), Main.rand.Next(12, 24));
			player.QuickSpawnItem(ModContent.ItemType<VlitchBattery>(), Main.rand.Next(1, 3));
			//player.QuickSpawnItem(ModContent.ItemType<BrokenBlade>(), 1);
		}
	}
}
