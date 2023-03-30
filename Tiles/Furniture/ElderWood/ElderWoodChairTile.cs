using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.ElderWood;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameContent;

namespace Redemption.Tiles.Furniture.ElderWood
{
    public class ElderWoodChairTile : ModTile
	{
		public const int NextStyleHeight = 40; // Calculated by adding all CoordinateHeights + CoordinatePaddingFix.Y applied to all of them + 2
		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.CanBeSatOnForNPCs[Type] = true;
			TileID.Sets.CanBeSatOnForPlayers[Type] = true;

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

			DustType = DustID.t_BorealWood;
			AdjTiles = new int[] { TileID.Chairs };

			AddMapEntry(new Color(109, 87, 78), Language.GetText("MapObject.Chair"));

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
		}

		public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			info.TargetDirection = -1;
			if (tile.TileFrameX != 0)
			{
				info.TargetDirection = 1;
			}

			info.AnchorTilePosition.X = i;
			info.AnchorTilePosition.Y = j;
			if (tile.TileFrameY % NextStyleHeight == 0)
			{
				info.AnchorTilePosition.Y++;
			}
		}

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
			if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
			{
				return;
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<ElderWoodChair>();

			if (Main.tile[i, j].TileFrameX / 18 < 1)
			{
				player.cursorItemIconReversed = true;
			}
		}
	}
}
