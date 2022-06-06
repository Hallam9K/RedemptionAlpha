using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Redemption.Buffs.Debuffs;
using Terraria.GameContent.Metadata;

namespace Redemption.Tiles.Plants
{
    public class ThornsTile2 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileCut[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 32 };
            TileObjectData.newTile.CoordinateWidth = 42;
            TileObjectData.newTile.DrawYOffset = -8;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(159, 208, 159));
            DustType = DustID.GrassBlades;
            HitSound = SoundID.Grass;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (player.active && !player.dead && dist <= 1)
                player.AddBuff(ModContent.BuffType<EnsnaredDebuff>(), 20);
        }
    }
}
