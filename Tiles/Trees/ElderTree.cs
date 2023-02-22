using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Terraria.GameContent;
using Terraria;
using Redemption.Tiles.Tiles;
using ReLogic.Content;
using Redemption.Items.Usable.Potions;
using Terraria.DataStructures;
using Redemption.Items.Usable;
using Terraria.Utilities;
using Redemption.Items.Materials.PreHM;
using Redemption.Globals;
using Redemption.NPCs.Critters;

namespace Redemption.Tiles.Trees
{
    public class ElderTree : ModTree
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
            drop.Add(4, 3.9);
            drop.Add(5, .099);
            drop.Add(6, 1.033);
            drop.Add(7, 2.38);
            drop.Add(8, 4.93);
            drop.Add(9, .08);

            if (drop > 0)
                createLeaves = true;
            else
                createLeaves = false;
            switch (drop)
            {
                case 1:
                    if (Main.rand.NextBool(2))
                        Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<Avocado>());
                    else
                        Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<Olives>());
                    break;
                case 2:
                    Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ItemID.Acorn, Main.rand.Next(1, 3));
                    break;
                case 3:
                    Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<ElderWood>(), Main.rand.Next(1, 4));
                    break;
                case 4:
                    Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<AncientGoldCoin>(), Main.rand.Next(1, 4));
                    break;
                case 5:
                    Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ItemID.EucaluptusSap);
                    break;
                case 6:
                    if (Main.rand.NextBool(2))
                        Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<ZweihanderFragment1>());
                    else
                        Item.NewItem(new EntitySource_ShakeTree(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<ZweihanderFragment2>());
                    break;
                case 7:
                    for (int l = 0; l < Main.rand.Next(3, 7); l++)
                    {
                        NPC npc2 = Main.npc[NPC.NewNPC(new EntitySource_ShakeTree(x, y), x * 16, y * 16, ModContent.NPCType<SpiderSwarmer>())];
                        npc2.velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                        npc2.netUpdate = true;
                    }
                    break;
                case 8:
                    NPC npc = Main.npc[NPC.NewNPC(new EntitySource_ShakeTree(x, y), x * 16, y * 16, ModContent.NPCType<BoneSpider>())];
                    npc.velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                    npc.netUpdate = true;
                    break;
                case 9:
                    NPC npc3 = Main.npc[NPC.NewNPC(new EntitySource_ShakeTree(x, y), x * 16, y * 16, ModContent.NPCType<JohnSnail>())];
                    npc3.velocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                    npc3.netUpdate = true;
                    break;
            }
            return false;
        }
        public override void SetStaticDefaults()
        {
            GrowsOnTileId = new int[2] { ModContent.TileType<AncientDirtTile>(), ModContent.TileType<AncientGrassTile>() };
        }
        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/ElderTree");
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<ElderSapling>();
        }
        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
        }
        public override Asset<Texture2D> GetBranchTextures()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/ElderTree_Branches");
        }
        public override Asset<Texture2D> GetTopTextures()
        {
            return ModContent.Request<Texture2D>("Redemption/Tiles/Trees/ElderTree_Top");
        }
        public override int DropWood() => ModContent.ItemType<ElderWood>();
        public override int CreateDust() => DustID.t_BorealWood;
        public override int TreeLeaf()
        {
            return ModContent.Find<ModGore>("Redemption/ElderTreeFX").Type;
        }
    }
}