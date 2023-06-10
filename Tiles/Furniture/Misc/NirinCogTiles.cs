using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Furniture.Misc
{
    public class NirinCogMediumTile : ModTile
    {
        public override string Texture => "Redemption/Tiles/Furniture/Misc/CogTile";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            RegisterItemDrop(ModContent.ItemType<NirinCogMedium>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Niric Cog");
            AddMapEntry(new Color(117, 117, 126), name);
            DustType = ModContent.DustType<NiricBrassDust>();
            HitSound = SoundID.Tink;
            MinPick = 210;
            MineResist = 10f;
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Main.tile[i, j];
            if (Main.LocalPlayer.direction == 1)
                tile.TileFrameY += 18;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D cog = ModContent.Request<Texture2D>("Redemption/Tiles/Furniture/Misc/NirinCogMedium").Value;
            int height = cog.Height / 3;
            int y = height * Main.tile[i, j].TileFrameX / 18;
            Rectangle rect = new(0, y, cog.Width, height);
            Vector2 drawOrigin = new(cog.Width / 2, height / 2 - 2);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            float rotation = Main.GlobalTimeWrappedHourly;
            if (Main.tile[i, j].TileFrameY != 0)
                rotation = -Main.GlobalTimeWrappedHourly;
            if (Main.tile[i, j].TileFrameX % 18 == 0)
            {
                spriteBatch.Draw(cog, new Vector2(((i + 0.5f) * 16) - (int)Main.screenPosition.X, ((j + 0.5f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), Lighting.GetColor(i, j), rotation, drawOrigin, 1, 0, 1f);
            }
            return true;
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class NirinCogMedium : ModItem
    {
        public override string Texture => "Redemption/Tiles/Furniture/Misc/NirinCog";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Niric Cog");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<NirinCogMediumTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
    public class NirinCogBigTile : ModTile
    {
        public override string Texture => "Redemption/Tiles/Furniture/Misc/CogTile";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            RegisterItemDrop(ModContent.ItemType<NirinCogBig>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Niric Cog");
            AddMapEntry(new Color(117, 117, 126), name);
            DustType = DustID.Lead;
            HitSound = SoundID.Tink;
            MinPick = 210;
            MineResist = 15f;
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Main.tile[i, j];
            if (Main.LocalPlayer.direction == 1)
                tile.TileFrameY += 18;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D cog = ModContent.Request<Texture2D>("Redemption/Tiles/Furniture/Misc/NirinCogBig").Value;
            int height = cog.Height / 2;
            int y = height * Main.tile[i, j].TileFrameX / 18;
            Rectangle rect = new(0, y, cog.Width, height);
            Vector2 drawOrigin = new(cog.Width / 2, height / 2 - 2);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            float rotation = Main.GlobalTimeWrappedHourly;
            if (Main.tile[i, j].TileFrameY != 0)
                rotation = -Main.GlobalTimeWrappedHourly;
            if (Main.tile[i, j].TileFrameX % 18 == 0)
            {
                spriteBatch.Draw(cog, new Vector2(((i + 0.5f) * 16) - (int)Main.screenPosition.X, ((j + 0.5f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), Lighting.GetColor(i, j), rotation, drawOrigin, 1, 0, 1f);
            }
            return true;
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class NirinCogBig : ModItem
    {
        public override string Texture => "Redemption/Tiles/Furniture/Misc/NirinCog";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Big Niric Cog");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<NirinCogBigTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
