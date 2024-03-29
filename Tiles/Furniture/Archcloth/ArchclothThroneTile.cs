using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Redemption.Items.Placeable.Furniture.Archcloth;

namespace Redemption.Tiles.Furniture.Archcloth
{
    public class ArchclothThroneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new(1, 3);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.Platinum;
            MinPick = 0;
            MineResist = 1.2f;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Archcloth Throne");
            AddMapEntry(new Color(123, 44, 122), name);
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
            => settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconID = ModContent.ItemType<ArchclothThrone>();
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }
        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            int left = i - tile.TileFrameX / 16 % 3;
            int top = j - tile.TileFrameY / 16 % 4;

            info.AnchorTilePosition.X = left + 1;
            info.AnchorTilePosition.Y = top + 3;
            info.VisualOffset.Y -= 4;
            info.VisualOffset.X -= 4;
        }
    }
}