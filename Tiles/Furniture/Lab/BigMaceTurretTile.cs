using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class BigMaceTurretTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorRight = AnchorData.Empty;
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            DustType = DustID.Electric;
            MinPick = 1000;
            MineResist = 20f;
            LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Laboratory MACE Turret");
			AddMapEntry(new Color(110, 106, 120), name);
		}
        public override bool CanExplode(int i, int j) => false;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if (!Main.projectile.Any(projectile => projectile.type == ModContent.ProjectileType<BigMaceTurret_NPC>() && (projectile.ModProjectile as BigMaceTurret_NPC).Parent == tile && projectile.active))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int turret = Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), new Vector2(i * 16, j * 16 + 32), Vector2.Zero, ModContent.ProjectileType<BigMaceTurret_NPC>(), 0, 0, Main.myPlayer);
                        (Main.projectile[turret].ModProjectile as BigMaceTurret_NPC).Parent = tile;
                    }
                }
            }
            if (tile.TileFrameX == 18 && tile.TileFrameY == 0)
            {
                if (!Main.projectile.Any(projectile => projectile.type == ModContent.ProjectileType<BigMaceTurret_NPC>() && (projectile.ModProjectile as BigMaceTurret_NPC).Parent == tile && projectile.active))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int turret = Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), new Vector2(i * 16 + 16, j * 16 + 32), Vector2.Zero, ModContent.ProjectileType<BigMaceTurret_NPC>(), 0, 0, Main.myPlayer);
                        (Main.projectile[turret].ModProjectile as BigMaceTurret_NPC).Parent = tile;
                    }
                }
            }
        }
    }
    public class BigMaceTurret : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory MACE Turret");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<BigMaceTurretTile>();
        }
    }
}