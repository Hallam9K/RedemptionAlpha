using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Redemption.Items.Materials.PostML;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ParticleLibrary;
using Redemption.Globals;
using Redemption.Particles;

namespace Redemption.Tiles.Furniture.Kingdom
{
    public class CoalPileTile : ModTile
    {
        private Asset<Texture2D> flameTexture;
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            DustType = DustID.Asphalt;
            MinPick = 350;
            MineResist = 8f;
            AddMapEntry(new Color(45, 51, 56));
            if (!Main.dedServ)
            {
                flameTexture = ModContent.Request<Texture2D>(Texture + "_Glow2");
                glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
            }
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX == 54 && Main.tile[i, j].TileFrameY == 0)
            {
                if (closer && !Main.gamePaused && Main.rand.NextBool(20))
                {
                    ParticleManager.NewParticle(new Vector2(i * 16, j * 16) + new Vector2(Main.rand.Next(8, 44), Main.rand.Next(36, 50)), RedeHelper.SpreadUp(1), new EmberParticle(), Color.White, 1);
                }
            }
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(glowTexture.Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.HeatColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(flameTexture.Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.HeatColour * .3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<ScorchingCoal>(), 6);
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class CoalPileItem : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetSafeStaticDefaults()
        {
            //DisplayName.SetDefault("Scorching Coal Pile");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<CoalPileTile>();
        }
    }
}