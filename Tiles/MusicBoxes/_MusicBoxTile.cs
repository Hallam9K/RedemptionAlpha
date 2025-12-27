using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Placeable.MusicBoxes;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.MusicBoxes
{
    public abstract class BaseMusicBoxTile : ModTile
    {
        protected abstract int Item { get; }
        public virtual void SetSafeStaticDefaults() { }
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.MusicBox"));
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override bool CreateDust(int i, int j, ref int type) => false;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = Item;
        }
    }
    public class ErhanBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<ErhanBox>(); }
    public class ForestBossBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<ForestBossBox>(); }
    public class ForestBossBoxTile2 : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<ForestBossBox2>(); }
    public class FowlMorningBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<FowlMorningBox>(); }
    public class HallOfHeroesBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<HallOfHeroesBox>(); }
    public class KeeperBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<KeeperBox>(); }
    public class KSBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<KSBox>(); }
    public class LabBossMusicBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<LabBossMusicBox>(); }
    public class LabMusicBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<LabMusicBox>(); }
    public class NebBox2Tile : BaseMusicBoxTile
    {
        protected override int Item => ModContent.ItemType<NebBox2>();
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile))
                return;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.HallowBossDye);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, 0, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null);
        }
    }
    public class NebBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<NebBox>(); }
    public class OmegaBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<OmegaBox>(); }
    public class OmegaBoxTile2 : BaseMusicBoxTile
    {
        protected override int Item => ModContent.ItemType<OmegaBox2>();
        public override string Texture => "Redemption/Tiles/MusicBoxes/OmegaBoxTile";
        public override string HighlightTexture => "Redemption/Tiles/MusicBoxes/OmegaBoxTile_Highlight";
    }
    public class PZMusicBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<PZMusicBox>(); }
    public class RaveyardBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<RaveyardBox>(); }
    public class SkullDiggerBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<SkullDiggerBox>(); }
    public class SlayerShipBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<SlayerShipBox>(); }
    public class SoIBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<SoIBox>(); }
    public class WastelandBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<WastelandBox>(); }
    public class SpiritRealmBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<SpiritRealmBox>(); }
    public class FowlEmperorBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<FowlEmperorBox>(); }
    public class ThornBoxTile : BaseMusicBoxTile { protected override int Item => ModContent.ItemType<ThornBox>(); }
    public class BeyondSteelBoxTile : BaseMusicBoxTile { protected override int Item => ItemType<BeyondSteelBox>(); }
}