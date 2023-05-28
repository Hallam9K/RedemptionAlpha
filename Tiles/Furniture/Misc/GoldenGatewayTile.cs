using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using System;
using ReLogic.Content;
using Redemption.Globals;

namespace Redemption.Tiles.Furniture.Misc
{
    public class GoldenGatewayTile : ModTile
	{
		public const int FrameWidth = 16 * 12;
		public const int FrameHeight = 16 * 3;
		public const int HorizontalFrames = 1;
		public const int VerticalFrames = 2;

		public Asset<Texture2D> GatewayTexture;

		public virtual string GatewayTextureName => "Redemption/Tiles/Furniture/Misc/GoldenGateway_Frame";

		public override void Load()
		{
			if (!Main.dedServ)
				GatewayTexture = ModContent.Request<Texture2D>(GatewayTextureName);
		}

		public override void Unload()
		{
			GatewayTexture = null;
		}

		public override void SetStaticDefaults()
		{
			Main.tileShine[Type] = 400;
			Main.tileFrameImportant[Type] = true;
            RedeTileHelper.CannotMineTileBelow[Type] = true;
            TileObjectData.newTile.Width = 12;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.Origin = new Point16(5, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            HitSound = CustomSounds.MetalHit;
            DustType = DustID.GoldCoin;
            LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Golden Gateway");
			AddMapEntry(new Color(203, 179, 73), name);
		}
        public override bool CanExplode(int i, int j) => false;
		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
			{
				Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
			}
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Vector2 offScreen = new(Main.offScreenRange);
			if (Main.drawToScreen)
				offScreen = Vector2.Zero;

			Point p = new(i, j);
			Tile tile = Framing.GetTileSafely(p.X, p.Y);
			if (tile == null || !tile.HasTile)
				return;

			Texture2D texture = GatewayTexture.Value;

			Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, 0);

			Vector2 origin = frame.Size() / 2f;
			Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

			Color color = Lighting.GetColor(p.X, p.Y);

			bool direction = tile.TileFrameY / FrameHeight != 0;
			SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			const float TwoPi = (float)Math.PI * 2f;
			float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
			Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(72f, -208f) + new Vector2(0f, offset * 4f);

			spriteBatch.Draw(texture, drawPos, frame, color, Main.GlobalTimeWrappedHourly / 30f, origin, 1f, effects, 0f);

			float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
			Color effectColor = color;
			effectColor.A = 0;
			effectColor = effectColor * 0.1f * scale;
			for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
			{
				spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, Main.GlobalTimeWrappedHourly / 30f, origin, 1f, effects, 0f);
			}
		}
	}
	public class GoldenGatewayItem : PlaceholderTile
	{
		public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
		public override void SetSafeStaticDefaults()
		{
			// DisplayName.SetDefault("Golden Gateway");
			// Tooltip.SetDefault("[c/ff0000:Unbreakable]");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.createTile = ModContent.TileType<GoldenGatewayTile>();
		}
	}
}
