using Microsoft.Xna.Framework;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PostML.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class SongOfTheAbyssTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            DustType = DustID.AncientLight;
            HitSound = SoundID.Tink;
            RegisterItemDrop(ModContent.ItemType<SongOfTheAbyss>(), 0);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Song of the Abyss");
            AddMapEntry(new Color(250, 250, 250), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .5f;
            g = .5f;
            b = .5f;
        }
        public override bool RightClick(int i, int j)
        {
            Main.player[Main.myPlayer].PickTile(i, j, 100);
            return true;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<SongOfTheAbyss>();
        }
        public override bool CanExplode(int i, int j) => false;
    }
}