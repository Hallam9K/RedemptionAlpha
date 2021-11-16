using Redemption.Items.Accessories.HM;
using Redemption.Items.Accessories.PostML;
using Redemption.Items.Lore;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabCrate : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laboratory Crate");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabCrateTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Lime;
            Item.maxStack = 999;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            // TODO: Lab crate loot
            /*
            if (NPC.downedMoonlord)
            {
                int choice = Main.rand.Next(22);
                if (choice == 0)
                {
                    player.QuickSpawnItem(ModContent.ItemType<GasMask>());
                }
                if (choice == 1)
                {
                    player.QuickSpawnItem(ModContent.ItemType<PlasmaShield>());
                }
                if (choice == 2)
                {
                    player.QuickSpawnItem(ModContent.ItemType<MiniNuke>());
                }
                if (choice == 3)
                {
                    player.QuickSpawnItem(ModContent.ItemType<PlasmaSaber>());
                }
                if (choice == 4)
                {
                    player.QuickSpawnItem(ModContent.ItemType<RadioactiveLauncher>());
                }
                if (choice == 5)
                {
                    player.QuickSpawnItem(ModContent.ItemType<SludgeSpoon>());
                }
                if (choice == 6)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk1>());
                }
                if (choice == 7)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk3>());
                }
                if (choice == 8)
                {
                    player.QuickSpawnItem(ModContent.ItemType<HazmatSuit>());
                }
                if (choice == 9)
                {
                    player.QuickSpawnItem(ModContent.ItemType<SuspiciousXenomiteShard>());
                }
                if (choice == 10)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Petridish>());
                }
                if (choice == 11)
                {
                    player.QuickSpawnItem(ModContent.ItemType<DNAgger>());
                }
                if (choice == 12)
                {
                    player.QuickSpawnItem(ModContent.ItemType<EmptyMutagen>());
                }
                if (choice == 13)
                {
                    player.QuickSpawnItem(ModContent.ItemType<TeslaManipulatorPrototype>());
                }
                if (choice == 14)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk5>());
                }
                if (choice == 15)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk5>());
                }
                if (choice == 16)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk5_1>());
                }
                if (choice == 17)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk5_2>());
                }
                if (choice == 18)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk5_3>());
                }
                if (choice == 19)
                {
                    player.QuickSpawnItem(ModContent.ItemType<TerraBombaPart1>());
                }
                if (choice == 20)
                {
                    player.QuickSpawnItem(ModContent.ItemType<TerraBombaPart2>());
                }
                if (choice == 21)
                {
                    player.QuickSpawnItem(ModContent.ItemType<TerraBombaPart3>());
                }

                int choice2 = Main.rand.Next(5);
                if (choice2 == 0)
                {
                    player.QuickSpawnItem(ModContent.ItemType<ScrapMetal>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 1)
                {
                    player.QuickSpawnItem(ModContent.ItemType<AIChip>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 2)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Mk3Capacitator>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 3)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Mk3Plating>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 4)
                {
                    player.QuickSpawnItem(ModContent.ItemType<RawXenium>(), Main.rand.Next(1, 3));
                }

                int choice3 = Main.rand.Next(4);
                if (choice3 == 0)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Starlite>(), Main.rand.Next(8, 12));
                }
                if (choice3 == 1)
                {
                    player.QuickSpawnItem(ModContent.ItemType<XenomiteShard>(), Main.rand.Next(8, 12));
                }
                if (choice3 == 2)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Electronade>(), Main.rand.Next(8, 12));
                }
                if (choice3 == 3)
                {
                    player.QuickSpawnItem(ItemID.LunarOre, Main.rand.Next(8, 12));
                }
            }
            else
            {
                int choice = Main.rand.Next(8);
                if (choice == 0)
                {
                    player.QuickSpawnItem(ModContent.ItemType<GasMask>());
                }
                if (choice == 1)
                {
                    player.QuickSpawnItem(ModContent.ItemType<PlasmaShield>());
                }
                if (choice == 2)
                {
                    player.QuickSpawnItem(ModContent.ItemType<MiniNuke>());
                }
                if (choice == 3)
                {
                    player.QuickSpawnItem(ModContent.ItemType<PlasmaSaber>());
                }
                if (choice == 4)
                {
                    player.QuickSpawnItem(ModContent.ItemType<RadioactiveLauncher>());
                }
                if (choice == 5)
                {
                    player.QuickSpawnItem(ModContent.ItemType<SludgeSpoon>());
                }
                if (choice == 6)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk1>());
                }
                if (choice == 7)
                {
                    player.QuickSpawnItem(ModContent.ItemType<FloppyDisk3>());
                }

                int choice2 = Main.rand.Next(8);
                if (choice2 == 0)
                {
                    player.QuickSpawnItem(ModContent.ItemType<ScrapMetal>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 1)
                {
                    player.QuickSpawnItem(ModContent.ItemType<AIChip>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 2)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Mk1Capacitator>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 3)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Mk2Capacitator>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 4)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Mk3Capacitator>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 5)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Mk1Plating>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 6)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Mk2Plating>(), Main.rand.Next(1, 3));
                }
                if (choice2 == 7)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Mk3Plating>(), Main.rand.Next(1, 3));
                }

                int choice3 = Main.rand.Next(4);
                if (choice3 == 0)
                {
                    player.QuickSpawnItem(ModContent.ItemType<AntiXenomiteApplier>(), Main.rand.Next(2, 8));
                }
                if (choice3 == 1)
                {
                    player.QuickSpawnItem(ModContent.ItemType<CarbonMyofibre>(), Main.rand.Next(2, 8));
                }
                if (choice3 == 2)
                {
                    player.QuickSpawnItem(ModContent.ItemType<Starlite>(), Main.rand.Next(2, 8));
                }
                if (choice3 == 3)
                {
                    player.QuickSpawnItem(ModContent.ItemType<XenomiteShard>(), Main.rand.Next(2, 8));
                }
            }*/
        }
    }
}
