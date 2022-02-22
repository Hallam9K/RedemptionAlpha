using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Redemption.Buffs.Debuffs;

namespace Redemption.Tiles.Plants
{
    public class ThornsTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileCut[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 24 };
            TileObjectData.newTile.CoordinateWidth = 26;
            TileObjectData.newTile.DrawYOffset = -4;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(159, 208, 159));
            DustType = DustID.GrassBlades;
            SoundType = SoundID.Grass;
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (player.active && !player.dead && dist <= 0.6f)
                player.AddBuff(ModContent.BuffType<EnsnaredDebuff>(), 20);
        }
    }
}
