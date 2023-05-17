using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Plants;
using Redemption.Tiles.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Plants
{
    public class RadRootTile : ModTile
    {
        private const int FrameWidth = 18;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;
            TileID.Sets.IgnoredInHouseScore[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            AddMapEntry(Color.DarkOliveGreen);

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.AnchorValidTiles = new int[] {
                ModContent.TileType<IrradiatedGrassTile>(),
                ModContent.TileType<IrradiatedCorruptGrassTile>(),
                ModContent.TileType<IrradiatedCrimsonGrassTile>()
            };
            TileObjectData.newTile.AnchorAlternateTiles = new int[] {
                TileID.ClayPot,
                TileID.PlanterBox
            };
            TileObjectData.addTile(Type);

            HitSound = SoundID.Grass;
            DustType = DustID.GreenBlood;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
        }
        public override bool CanPlace(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (tile.HasTile)
            {
                int tileType = tile.TileType;
                if (tileType == Type)
                {
                    PlantStage stage = GetStage(i, j);
                    return stage == PlantStage.Grown;
                }
                else
                {
                    if (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip)
                    {
                        bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
                        bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
                        bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);
                        if (foliageGrass || moddedFoliage || harvestableVanillaHerb)
                        {
                            WorldGen.KillTile(i, j);
                            if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                            return true;
                        }
                    }
                    return false;
                }
            }
            return true;
        }
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
        public override bool CanDrop(int i, int j)
        {
            PlantStage stage = GetStage(i, j);

            if (stage == PlantStage.Planted)
                return false;
            return true;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            PlantStage stage = GetStage(i, j);

            Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
            Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

            int herbItemType = ModContent.ItemType<RadRoot>();
            int herbItemStack = 1;

            if (nearestPlayer.active && nearestPlayer.HeldItem.type == ItemID.StaffofRegrowth)
            {
                // Increased yields with Staff of Regrowth, even when not fully grown
                herbItemStack = Main.rand.Next(1, 3);
            }
            else if (stage == PlantStage.Grown)
            {
                // Default yields, only when fully grown
                herbItemStack = 1;
            }

            if (herbItemType > 0 && herbItemStack > 0)
            {
                yield return new Item(herbItemType, herbItemStack);
            }
        }
        public override bool IsTileSpelunkable(int i, int j)
        {
            PlantStage stage = GetStage(i, j);
            return stage == PlantStage.Grown;
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            PlantStage stage = GetStage(i, j);

            if (stage != PlantStage.Grown)
            {
                tile.TileFrameX += FrameWidth;

                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }

        private static PlantStage GetStage(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            return (PlantStage)(tile.TileFrameX / FrameWidth);
        }
    }
}
