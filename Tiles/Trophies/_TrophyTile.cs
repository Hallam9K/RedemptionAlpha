using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Trophies
{
    public abstract class BaseTrophyTile : ModTile
    {
        public virtual void SetSafeStaticDefaults() { }
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
            DustType = DustID.WoodFurniture;
            SetSafeStaticDefaults();
        }
    }
    public class AkanKirvesTrophyTile : BaseTrophyTile { }
    public class BasanTrophyTile : BaseTrophyTile
    {
        public override void SetSafeStaticDefaults()
        {
            Main.tileLighted[Type] = true;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.4f;
            g = 0.2f;
            b = 0.1f;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile))
                return;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.COLOR_GLOWPULSE, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
    public class CockatriceTrophyTile : BaseTrophyTile { }
    public class EaglecrestGolemTrophyTile : BaseTrophyTile { }
    public class ErhanTrophyTile : BaseTrophyTile { }
    public class FowlEmperorTrophyTile : BaseTrophyTile { }
    public class KeeperTrophyTile : BaseTrophyTile { }
    public class KS3TrophyTile : BaseTrophyTile { }
    public class NebuleusTrophyTile : BaseTrophyTile { }
    public class OmegaCleaverTrophyTile : BaseTrophyTile { }
    public class OmegaGigaporaTrophyTile : BaseTrophyTile { }
    public class OmegaObliteratorTrophyTile : BaseTrophyTile { }
    public class PZTrophyTile : BaseTrophyTile { }
    public class SkullDiggerTrophyTile : BaseTrophyTile { }
    public class SoITrophyTile : BaseTrophyTile { }
    public class ThornTrophyTile : BaseTrophyTile { }
    public class UkonKirvesTrophyTile : BaseTrophyTile { }
}