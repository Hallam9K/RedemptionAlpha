using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class GathuramPortalTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            RedeTileHelper.CannotMineTileBelow[Type] = true;
            TileObjectData.newTile.Width = 11;
            TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(5, 10);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            HitSound = CustomSounds.StoneHit;
            DustType = ModContent.DustType<SlateDust>();
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Gathuram Gateway");
            AddMapEntry(new Color(81, 72, 65), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanExplode(int i, int j) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
    }
    public class GathuramPortalItem : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Gathuram Portal");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<GathuramPortalTile>();
        }
    }
}