using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Redemption.Dusts;
using Redemption.Tiles.Plants;
using Redemption.Items.Placeable.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Redemption.Biomes;

namespace Redemption.Tiles.Tiles
{
    public class ShadestoneMossyTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneRubbleTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneSlabTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickMossyTile>()] = true;
            DustType = ModContent.DustType<VoidFlame>();
            RegisterItemDrop(ModContent.ItemType<Shadestone>());
            MinPick = 350;
            MineResist = 11f;
            HitSound = CustomSounds.StoneHit;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Mossy Shadestone");
            AddMapEntry(new Color(22, 26, 35));
        }
        public override void FloorVisuals(Player player)
        {
            if (player.velocity.X != 0f && Main.rand.NextBool(20))
            {
                Dust dust = Dust.NewDustDirect(player.Bottom, 0, 0, ModContent.DustType<VoidFlame>(), 0f, -Main.rand.NextFloat(2f));
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(40000) && Main.LocalPlayer.InModBiome<SoullessBiome>())
                Dust.NewDust(new Vector2(i * 16, j * 16), 0, 0, ModContent.DustType<SoullessScreenDust>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                fail = true;
                Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<ShadestoneTile>();
            }
        }
        public override void RandomUpdate(int i, int j)
        {
            if (!Framing.GetTileSafely(i, j + 1).HasTile && Framing.GetTileSafely(i, j).HasTile)
            {
                if (Main.rand.NextBool(5))
                {
                    WorldGen.PlaceObject(i, j + 1, ModContent.TileType<Nooseroot_Small>(), true, Main.rand.Next(3));
                    NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<Nooseroot_Small>(), Main.rand.Next(3), 0, -1, -1);
                }
                if (Main.rand.NextBool(7))
                {
                    WorldGen.PlaceObject(i, j + 1, ModContent.TileType<Nooseroot_Medium>(), true, Main.rand.Next(3));
                    NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<Nooseroot_Medium>(), Main.rand.Next(3), 0, -1, -1);
                }
                if (Main.rand.NextBool(12))
                {
                    WorldGen.PlaceObject(i, j + 1, ModContent.TileType<Nooseroot_Large>(), true, Main.rand.Next(3));
                    NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<Nooseroot_Large>(), Main.rand.Next(3), 0, -1, -1);
                }
            }
            if (Main.rand.NextBool(8))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<ShadestoneTile>(), Type, false);
        }
    }
}