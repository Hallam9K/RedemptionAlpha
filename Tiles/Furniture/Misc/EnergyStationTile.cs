using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class EnergyStationTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Energy Station");
            AddMapEntry(new Color(189, 191, 200), name);
            DustType = DustID.Electric;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.6f;
            g = 0.6f;
            b = 0.6f;
        }
        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(CustomSounds.Spark1 with { Pitch = 0.1f }, Main.LocalPlayer.position);
            Main.LocalPlayer.AddBuff(ModContent.BuffType<EnergyStationBuff>(), 300);
            return true;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                if (Main.rand.NextBool(5))
                {
                    int d = Dust.NewDust(new Vector2(i * 16 + 12, j * 16), 8, 2, DustID.Smoke);
                    Main.dust[d].velocity.Y -= 2f;
                    Main.dust[d].velocity.X *= 0.2f;
                    Main.dust[d].noGravity = true;
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
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16 + 2) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.EnergyPulse, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}