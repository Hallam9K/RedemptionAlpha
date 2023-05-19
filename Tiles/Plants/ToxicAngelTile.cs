using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Placeable.Plants;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Plants
{
    public class ToxicAngelTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 5;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<AncientGrassTile>(), ModContent.TileType<AncientDirtTile>(), ModContent.TileType<AncientSlateTile>(), ModContent.TileType<AncientLushGrassTile>(), ModContent.TileType<OvergrownAncientSlateBeamTile>(), ModContent.TileType<OvergrownAncientSlateBrickTile>() };
            TileObjectData.addTile(Type);
            HitSound = SoundID.Grass;
            DustType = DustID.GreenFairy;
            RegisterItemDrop(ModContent.ItemType<ToxicAngel>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Toxic Angel");
            AddMapEntry(new Color(120, 240, 120), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .5f;
            g = 1f;
            b = .5f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Framing.GetTileSafely(i, j).LiquidAmount > 10 && Framing.GetTileSafely(i, j).LiquidType == LiquidID.Water)
            {
                Tile target = Main.tile[i, j];
                {
                    if (target.TileType == (ushort)ModContent.TileType<ToxicAngelTile>())
                        target.TileType = (ushort)ModContent.TileType<ToxicAngel2Tile>();
                }
            }
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (dist <= 4f && !player.IsRadiationProtected())
            {
                player.AddBuff(BuffID.Suffocation, 50);
                player.AddBuff(BuffID.Obstructed, 10);
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 10;
    }
}