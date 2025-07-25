using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Redemption.Globals.RedeNet;

namespace Redemption.Tiles.Natural
{
    public class NiricAutomatonRemainsTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.HasOutlines[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            RedeTileHelper.CannotMineTileBelow[Type] = true;
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(2, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.DungeonSpirit;
            MinPick = 5000;
            MineResist = 50;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(117, 117, 126));
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                if (Main.rand.NextBool(20))
                {
                    int d = Dust.NewDust(new Vector2(i * 16 + 22, j * 16 + 8), 50, 24, DustID.DungeonSpirit);
                    Main.dust[d].velocity.Y -= 3f;
                    Main.dust[d].noGravity = true;
                }
            }
        }
        public override bool CanExplode(int i, int j) => false;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            if (RedeTileHelper.CanDeadRing(player))
                player.cursorItemIconID = ModContent.ItemType<DeadRinger>();
            else
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => RedeTileHelper.CanDeadRing(Main.LocalPlayer);
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (RedeTileHelper.CanDeadRing(player))
            {
                static int GetNPCIndex() => NPCType<SpiritNiricLady>();

                if (NPC.AnyNPCs(GetNPCIndex()))
                    return false;

                int offset = Main.tile[i, j].TileFrameX / 18;
                Vector2 pos = new Vector2(i - offset + 3, j + 1).ToWorldCoordinates();

                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Bell, pos);

                if (Main.netMode == NetmodeID.SinglePlayer)
                    NPC.NewNPC(new EntitySource_TileUpdate(i, j), (int)pos.X, (int)pos.Y, GetNPCIndex());
                else
                    SpawnNPCFromClient(GetNPCIndex(), pos);

                SoundEngine.PlaySound(SoundID.Item74, pos);
            }

            return true;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            RedeTileHelper.DrawSpiritFlare(spriteBatch, i, j, 0, 2.8f, 1.1f);
            return true;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class NiricAutomatonRemains : PlaceholderTile
    {
        public override string Texture => "Redemption/Tiles/Placeholder/NiricAutomatonRemains";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<NiricAutomatonRemainsTile>();
        }
    }
}