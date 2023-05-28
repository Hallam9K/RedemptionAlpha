using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Redemption.UI;
using Redemption.Items;
using Redemption.Globals;
using Redemption.Items.Placeable.Furniture.Misc;

namespace Redemption.Tiles.Furniture.Misc
{
    public class NuclearWarheadTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 4;
            
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            DustType = DustID.Electric;
            MinPick = 10;
            MineResist = 7f;

            RegisterItemDrop(ModContent.ItemType<NuclearWarhead>());
            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Nuclear Warhead");
            AddMapEntry(new Color(62, 88, 90), name);
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<HintIcon>();
        }
        public override bool CanDrop(int i, int j)
        {
            return !RedeWorld.nukeCountdownActive;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (RedeWorld.nukeCountdownActive)
            {
                RedeWorld.nukeTimerInternal = 2;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
        public override bool CanExplode(int i, int j)
        {
            return true;
        }
        public override bool RightClick(int i, int j)
        {
            if (!RedeWorld.nukeCountdownActive)
            {
                if (!Main.dedServ)
                    NukeDetonationUI.Visible = true;

                RedeWorld.nukeGroundZero = new Vector2(i * 16, j * 16);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
            
        }
    }
}