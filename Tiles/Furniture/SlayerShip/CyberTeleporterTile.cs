using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.Items;
using Redemption.Items.Placeable.Furniture.SlayerShip;
using Redemption.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.SlayerShip
{
    public class CyberTeleporterTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cyber Teleporter");
            AddMapEntry(new Color(0, 160, 170), name);
            DustType = ModContent.DustType<LabPlatingDust>();
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => RedeWorld.slayerRep >= 4 && (RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus);
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if (RedeWorld.slayerRep >= 4 && (RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus))
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ModContent.ItemType<HintIcon>();
            }
            else
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
        public override bool RightClick(int i, int j)
        {
            if (RedeWorld.slayerRep >= 4 && (RedeBossDowned.downedOmega3 || RedeBossDowned.downedNebuleus))
            {
                if (!Main.dedServ)
                {
                    RedeSystem.Instance.CyberTeleporterUIElement.DisplayCyberTeleporterUI("Teleport");
                }
            }
            return true;

        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, ModContent.ItemType<CyberTeleporter>());
        public override bool CanExplode(int i, int j) => false;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f;
            g = 0.5f;
            b = 0.8f;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Tiles/Furniture/SlayerShip/CyberTeleporterTile_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.COLOR_GLOWPULSE, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}