using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class AncientAltarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16,
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.addTile(Type);
            DustType = 7;
            MinPick = 1000;
            MineResist = 3f;
            HitSound = CustomSounds.StoneHit;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Ancient Altar");
            AddMapEntry(new Color(120, 190, 40), name);
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => Main.tile[i, j].TileFrameX < 36;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!Main.rand.NextBool(10))
                return;
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                int d = Dust.NewDust(new Vector2(i * 16 + 10, j * 16 + 6), 12, 12, DustID.TreasureSparkle, Alpha: 20);
                Main.dust[d].velocity *= 0;
                Main.dust[d].noGravity = true;
            }
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            if (Main.tile[i, j].TileFrameX < 36)
                player.cursorItemIconID = ModContent.ItemType<CursedGem>();
            else
                player.cursorItemIconID = -1;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool CanExplode(int i, int j) => false;

        public override bool RightClick(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            if (Main.tile[left, top].TileFrameX == 0)
            {
                Player player = Main.LocalPlayer;
                player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ModContent.ItemType<CursedGem>());
                for (int x = left; x < left + 2; x++)
                {
                    for (int y = top; y < top + 3; y++)
                    {
                        if (Main.tile[x, y].TileFrameX < 36)
                            Main.tile[x, y].TileFrameX += 36;
                    }
                }
                return true;
            }
            return false;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D flare = Redemption.WhiteFlare.Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(211, 232, 169), new Color(247, 247, 169), new Color(211, 232, 169));
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            Vector2 origin = new(flare.Width / 2f, flare.Height / 2f);
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

                spriteBatch.Draw(flare, new Vector2(((i + 1f) * 16) - (int)Main.screenPosition.X, ((j + 1f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), color, Main.GlobalTimeWrappedHourly, origin, 1, 0, 1f);
                spriteBatch.Draw(flare, new Vector2(((i + 1f) * 16) - (int)Main.screenPosition.X, ((j + 1f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), color, -Main.GlobalTimeWrappedHourly, origin, 1, 0, 1f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            }
            return true;
        }
    }
    public class AncientAltar : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Ancient Altar");
            /* Tooltip.SetDefault("Gives the Cursed Gem" +
                "\n[c/ff0000:Unbreakable]"); */
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<AncientAltarTile>();
        }
    }
}