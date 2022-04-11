using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.StructureHelper
{
    class NullBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
        }
    }
    class NullWall : ModWall { }

    class NullBlockItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Null Block");
            Tooltip.SetDefault("Use these in a structure to indicate where the generator\n should leave whatever already exists in the world untouched\n ignores walls, use null walls for that :3");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 2;
            Item.useTime = 2;
            Item.useStyle = 1;
            Item.rare = 8;
            Item.createTile = ModContent.TileType<NullBlock>();
        }
    }

    class NullWallItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Null Wall");
            Tooltip.SetDefault("Use these in a structure to indicate where the generator\n should leave walls that already exists in the world untouched\n for walls only, use null blocks for other things");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 2;
            Item.useTime = 2;
            Item.useStyle = 1;
            Item.rare = 8;
            Item.createWall = ModContent.WallType<NullWall>();
        }
    }

    class NullBoth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Null Tile Place-O-Matic");
            Tooltip.SetDefault("Places a null tile and null wall at the same time!");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 2;
            Item.useTime = 2;
            Item.useStyle = 1;
            Item.rare = 8;
            Item.createTile = ModContent.TileType<NullBlock>();
            Item.createWall = ModContent.WallType<NullWall>();
        }
    }
}
