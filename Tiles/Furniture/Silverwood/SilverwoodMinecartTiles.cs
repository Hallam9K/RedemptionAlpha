using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Globals;
using Redemption.Items.Materials.PostML;
using Redemption.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Silverwood
{
    public class SilverwoodMinecartTiles : ModTile
    {
        private Asset<Texture2D> flameTexture;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 54;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.Pearlwood;
            AddMapEntry(new Color(228, 213, 173));
            MinPick = 350;
            MineResist = 8f;
            if (!Main.dedServ)
                flameTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX / 54 < 1)
            {
                if (closer)
                {
                    if (Main.rand.NextBool(40))
                    {
                        ParticleManager.NewParticle(new Vector2(i * 16, j * 16) + new Vector2(Main.rand.Next(8, 42), Main.rand.Next(0, 6)), RedeHelper.SpreadUp(1), new EmberParticle(), Color.White, 1);
                    }
                }
            }
        }
        private float drawTimer;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            RedeDraw.DrawTreasureBagEffect(spriteBatch, flameTexture.Value, ref drawTimer, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.HeatColour, 0, Vector2.Zero, 1f, 0);
            return true;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if (Main.tile[i, j].TileFrameX / 54 < 2)
                player.cursorItemIconEnabled = true;
            else
                player.cursorItemIconEnabled = false;

            switch (Main.tile[i, j].TileFrameX / 54)
            {
                case 0:
                    player.cursorItemIconID = ModContent.ItemType<ScorchingCoal>();
                    break;
                case 1:
                    player.cursorItemIconID = ModContent.ItemType<EvergoldOre>();
                    break;
            }
        }
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            int item = 0;
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            switch (Main.tile[i, j].TileFrameX / 54)
            {
                case 0:
                    item = ModContent.ItemType<ScorchingCoal>();
                    break;
                case 1:
                    item = ModContent.ItemType<EvergoldOre>();
                    break;
            }
            if (item > 0 && Main.tile[i, j].TileFrameX / 54 < 2)
                player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), item, 6);
            for (int x = left; x < left + 3; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    if (Main.tile[x, y].TileFrameX < 108 && Main.tile[x, y].TileFrameX >= 54)
                        Main.tile[x, y].TileFrameX += 54;
                    else if (Main.tile[x, y].TileFrameX < 54)
                        Main.tile[x, y].TileFrameX += 108;
                }
            }
            return true;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int item = 0;
            switch (frameX / 54)
            {
                case 0:
                    item = ModContent.ItemType<ScorchingCoal>();
                    break;
                case 1:
                    item = ModContent.ItemType<EvergoldOre>();
                    break;
            }
            if (item > 0 && frameX / 54 < 2)
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 54, 36, item, 6);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class SilverwoodMinecartCoal : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Minecart (Scorched Coal)");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<SilverwoodMinecartTiles>();
        }
    }
    public class SilverwoodMinecartEvergold : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Minecart (Evergold Ore)");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<SilverwoodMinecartTiles>();
            Item.placeStyle = 1;
        }
    }
    public class SilverwoodMinecartItem : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Minecart");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<SilverwoodMinecartTiles>();
            Item.placeStyle = 2;
        }
    }
}