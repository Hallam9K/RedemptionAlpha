using Microsoft.Xna.Framework;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class DreambinderElixirTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            DustType = DustID.AncientLight;
            HitSound = SoundID.Tink;
			ModTranslation name = CreateMapEntryName();
            name.SetDefault("Dreambinder Elixir");
            AddMapEntry(new Color(223, 230, 238), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 1f;
            b = 1f;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<DreambinderElixir>();
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<DreambinderElixir>());
        }
        public override bool CanExplode(int i, int j) => false;
    }
}