using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Usable;
using Redemption.NPCs.Critters;
using Redemption.Tiles.Tiles;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Utilities;
using Terraria;

namespace Redemption.Tiles.Trees
{
    public class SilverwoodTree : ModTree
    {
        public override TreePaintingSettings TreeShaderSettings => new()
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };
        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            WeightedRandom<int> drop = new(Main.rand);
            drop.Add(0, 2);
            drop.Add(1, 5.4);
            drop.Add(2, 14.17);
            drop.Add(3, 7.08);
            drop.Add(4, .099);

            if (drop > 0)
                createLeaves = true;
            else
                createLeaves = false;
            switch (drop)
            {
                case 1:
                    Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<ClockworkOrange>());
                    break;
                case 2:
                    Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ItemID.Acorn, Main.rand.Next(1, 3));
                    break;
                case 3:
                    Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<Silverwood>(), Main.rand.Next(1, 4));
                    break;
                case 4:
                    Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ItemID.EucaluptusSap);
                    break;
            }
            return false;
        }
        public override void SetStaticDefaults()
        {
            GrowsOnTileId = new int[2] { ModContent.TileType<OvergrownAncientSlateBeamTile>(), ModContent.TileType<OvergrownAncientSlateBrickTile>() };
        }
        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/SilverwoodTree");
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<SilverwoodSapling>();
        }
        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            xoffset = 48;
            topTextureFrameWidth = 114;
            topTextureFrameHeight = 96;
        }
        public override Asset<Texture2D> GetBranchTextures()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/SilverwoodTree_Branches");
        }
        public override Asset<Texture2D> GetTopTextures()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/SilverwoodTree_Top");
        }
        public override int DropWood() => ModContent.ItemType<Silverwood>();
        public override int CreateDust() => DustID.t_PearlWood;
        public override int TreeLeaf()
        {
            return ModContent.Find<ModGore>("Redemption/SilverwoodLeafFX").Type;
        }
    }
}