using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Plants;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Plants
{
    public class NightshadeTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            DustType = DustID.GrassBlades;
            SoundType = SoundID.Grass;
            Main.tileLighted[Type] = true;
            Main.tileSpelunker[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Nightshade");
            AddMapEntry(Color.Purple, name);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.AnchorValidTiles = new int[]
            {
                TileID.Grass,
                TileID.HallowedGrass
            };
            TileObjectData.newTile.AnchorAlternateTiles = new int[]
            {
                TileID.ClayPot,
                TileID.PlanterBox
            };
            TileObjectData.addTile(Type);
        }
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            int stage = Main.tile[i, j].TileFrameX / 18;
            if (stage == 1 && !Main.dayTime)
            {
                Main.tile[i, j].TileFrameX += 18;
            }
            else if (stage == 2 && Main.dayTime)
            {
                Main.tile[i, j].TileFrameX -= 18;
            }
        }
        public override bool Drop(int i, int j)
        {
            int stage = Main.tile[i, j].TileFrameX / 18;
            switch (stage)
            {
                case 1:
                    if (Main.rand.NextBool(4))
                    {
                        Item.NewItem(i * 16, j * 16, 0, 0, ModContent.ItemType<NightshadeSeeds>());
                    }
                    break;
                case 2:
                    Item.NewItem(i * 16, j * 16, 0, 0, ModContent.ItemType<NightshadeSeeds>());
                    Item.NewItem(i * 16, j * 16, 0, 0, ModContent.ItemType<Nightshade>());
                    break;
            }
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX == 0 && !Main.dayTime)
            {
                Main.tile[i, j].TileFrameX += 18;
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            int stage = Main.tile[i, j].TileFrameX / 18;
            if (stage == 2)
            {
                r = 0.2f;
                g = 0.0f;
                b = 0.3f;
            }
            if (stage < 2)
            {
                r = 0.0f;
                g = 0.0f;
                b = 0.0f;
            }
        }

    }
}
