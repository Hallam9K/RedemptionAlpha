using Redemption.Items.Placeable.Plants;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class PlantMatter : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Use at an Extractinator");
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;

            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PlantMatterTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            switch (Main.rand.Next(14))
            {
                case 0:
                    if (Main.rand.NextBool(200))
                    {
                        resultType = ModContent.ItemType<AnglonicMysticBlossom>();
                        resultStack = 1;
                    }
                    else
                    {
                        resultType = ItemID.Daybloom;
                        resultStack = 1;
                    }
                    break;
                case 1:
                    resultType = ItemID.Blinkroot;
                    resultStack = 1;
                    break;
                case 2:
                    resultType = ItemID.Moonglow;
                    resultStack = 1;
                    break;
                case 3:
                    resultType = ItemID.Waterleaf;
                    resultStack = 1;
                    break;
                case 4:
                    resultType = ItemID.Fireblossom;
                    resultStack = 1;
                    break;
                case 5:
                    resultType = ItemID.Shiverthorn;
                    resultStack = 1;
                    break;
                case 6:
                    resultType = ModContent.ItemType<Nightshade>();
                    resultStack = 1;
                    break;
                case 7:
                    resultType = ItemID.DaybloomSeeds;
                    resultStack = 1;
                    break;
                case 8:
                    resultType = ItemID.BlinkrootSeeds;
                    resultStack = 1;
                    break;
                case 9:
                    resultType = ItemID.MoonglowSeeds;
                    resultStack = 1;
                    break;
                case 10:
                    resultType = ItemID.WaterleafSeeds;
                    resultStack = 1;
                    break;
                case 11:
                    resultType = ItemID.FireblossomSeeds;
                    resultStack = 1;
                    break;
                case 12:
                    resultType = ItemID.ShiverthornSeeds;
                    resultStack = 1;
                    break;
                case 13:
                    resultType = ModContent.ItemType<NightshadeSeeds>();
                    resultStack = 1;
                    break;
            }
        }
    }
}
