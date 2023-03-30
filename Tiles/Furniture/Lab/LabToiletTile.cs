using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Lab;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameContent;

namespace Redemption.Tiles.Furniture.Lab
{
    public class LabToiletTile : ModTile
	{
		public const int NextStyleHeight = 40;
		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.CanBeSatOnForNPCs[Type] = true;
			TileID.Sets.CanBeSatOnForPlayers[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

			DustType = ModContent.DustType<LabPlatingDust>();
			AdjTiles = new int[] { TileID.Toilets };

			// Names
			AddMapEntry(new Color(189, 191, 200), Language.GetText("MapObject.Toilet"));

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			// The following 3 lines are needed if you decide to add more styles and stack them vertically
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
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
			if (info.RestingEntity is Player player && player.HasBuff(BuffID.Stinky))
			{
				info.VisualOffset = Main.rand.NextVector2Circular(2, 2);
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
			player.cursorItemIconID = ModContent.ItemType<LabToilet>();
			if (Main.tile[i, j].TileFrameX / 18 < 1)
			{
				player.cursorItemIconReversed = true;
			}
		}

		public override void HitWire(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			int spawnX = i;
			int spawnY = j - (tile.TileFrameY % NextStyleHeight) / 18;

			Wiring.SkipWire(spawnX, spawnY);
			Wiring.SkipWire(spawnX, spawnY + 1);

			if (Wiring.CheckMech(spawnX, spawnY, 60))
			{
				Projectile.NewProjectile(Wiring.GetProjectileSource(spawnX, spawnY), spawnX * 16 + 8, spawnY * 16 + 12, 0f, 0f, ProjectileID.ToiletEffect, 0, 0f, Main.myPlayer);
			}
		}
	}
}