using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.NPCs.Bosses.Thorn;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;

namespace Redemption.Items.Usable.Summons
{
    public class HeartOfThorns : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Heart of Thorns");
			/* Tooltip.SetDefault("Summons an unfortunate curse-bearer"
				+ "\nOnly usable at day"
				+ "\nNot consumable" +
				"\n[i:" + ModContent.ItemType<GoodRoute>() + "][c/bbf160: This item may have a positive impact onto the world]"); */

			Item.ResearchUnlockCount = 1;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 40;
			Item.maxStack = 1;
			Item.value = Item.sellPrice(0, 1, 50, 0);
			Item.rare = ItemRarityID.Blue;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return !SubworldSystem.IsActive<SoullessSub>() && Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<Thorn>());
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				SoundEngine.PlaySound(SoundID.Roar, player.position);

				int type = ModContent.NPCType<Thorn>();

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else
				{
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
				}
			}
			return true;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Stinger, 2)
				.AddIngredient(ItemID.Vine)
				.AddIngredient(ItemID.LifeCrystal)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
