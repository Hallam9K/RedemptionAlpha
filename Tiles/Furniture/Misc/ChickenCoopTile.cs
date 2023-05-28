using Microsoft.Xna.Framework;
using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Items.Weapons.PreHM.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class ChickenCoopTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
            MinPick = 0;
            MineResist = 1.2f;
			LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Chicken Coop");
            AddMapEntry(new Color(151, 107, 75), name);
		}
        public override void RandomUpdate(int i, int j)
        {
            if (Main.rand.NextBool(8))
            {
                int egg = ModContent.ItemType<ChickenEgg>();
                if (Main.rand.NextBool(200))
                    egg = ModContent.ItemType<GoldChickenEgg>();
                if (Main.rand.NextBool(10000))
                    egg = ModContent.ItemType<SussyEgg>();
                SoundEngine.PlaySound(SoundID.Item16, new Vector2(i * 16, j * 16));
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, egg);
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}