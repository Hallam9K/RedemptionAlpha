using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Kingdom
{
    public class GiantEvergoldChain : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.IsBeam[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = DustID.GoldCoin;
            AddMapEntry(new Color(230, 230, 50));
            MinPick = 1000;
            MineResist = 10f;
            HitSound = CustomSounds.ChainHit;
        }
        public override bool CanExplode(int i, int j) => false;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class GiantEvergoldChainItem : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetSafeStaticDefaults()
        {
            //DisplayName.SetDefault("Giant Evergold Chain");
            //Tooltip.SetDefault("[c/ff0000:Unbreakable (1000% Pickaxe Power)]");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<GiantEvergoldChain>();
        }
    }
}