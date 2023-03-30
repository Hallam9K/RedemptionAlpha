using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Furniture.SlayerShip;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.SlayerShip
{
    public class BiocontainerTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.addTile(Type);
            DustType = DustID.Glass;
            MinPick = 100;
            MineResist = 5f;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Life Fruit Bio-Container");
            AddMapEntry(new Color(143, 215, 29), name);
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
        }
        public override void MouseOver(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;

            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if (Main.tile[left, top].TileFrameX >= 54)
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ItemID.LifeFruit;
            }
            else
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
        public override bool RightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            if (Main.tile[left, top].TileFrameX >= 54)
            {
                Player player = Main.LocalPlayer;
                player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ItemID.LifeFruit);
                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 3; y++)
                    {
                        while (Main.tile[x, y].TileFrameX >= 54)
                            Main.tile[x, y].TileFrameX -= 54;
                    }
                }
            }
            return true;
        }
        public override bool CanExplode(int i, int j) => false;
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Tile t = Main.tile[i, j];
            int style = t.TileFrameX / 54;
            if (style >= 1)
                yield return new Item(ItemID.LifeFruit);
            yield return new Item(ModContent.ItemType<Biocontainer>());

        }
        public override bool IsTileSpelunkable(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            return Main.tile[left, top].TileFrameX >= 54;
        }
        public override void RandomUpdate(int i, int j)
        {
            if (Main.rand.NextBool(15))
            {
                int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
                int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
                int variant = Main.rand.Next(3) + 1;
                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 3; y++)
                    {
                        if (Main.tile[x, y].TileFrameX < 54)
                            Main.tile[x, y].TileFrameX += (short)(54 * variant);
                    }
                }
            }
        }
    }
}