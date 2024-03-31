using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Usable;
using Redemption.NPCs.Lab;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Redemption.Globals.RedeNet;

namespace Redemption.Tiles.Furniture.Lab
{
    public class Stage3CorpseTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.GreenTorch;
            HitSound = SoundID.Item27;
            MinPick = 200;
            MineResist = 7f;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Crystallized Corpse");
            AddMapEntry(new Color(54, 193, 59), name);
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            if (player.HeldItem.type == ModContent.ItemType<DeadRinger>())
                player.cursorItemIconID = ModContent.ItemType<DeadRinger>();
            else
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => Main.LocalPlayer.HeldItem.type == ModContent.ItemType<DeadRinger>();
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            if (player.HeldItem.type == ModContent.ItemType<DeadRinger>())
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<Stage3Scientist>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC(new EntitySource_TileInteraction(player, i, j), left * 16 + 24, top * 16 + 82, ModContent.NPCType<Stage3Scientist>());
                    }
                    else
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                            return false;

                        Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.NPCSpawnFromClient, ModContent.NPCType<Stage3Scientist>(), new Vector2(left * 16 + 24, top * 16 + 82)).Send(-1);
                    }
                    _ringer = true;
                    WorldGen.KillTile(i, j, noItem: true);
                }
                return true;
            }
            return false;
        }
        bool _ringer;
        public override bool CanDrop(int i, int j) => !_ringer;
        public override bool CreateDust(int i, int j, ref int type) => !_ringer;
        public override void PlaceInWorld(int i, int j, Item item)
        {
            _ringer = false;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            RedeTileHelper.DrawSpiritFlare(spriteBatch, i, j, 0, 1.5f, 1.5f);
            return true;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 1f;
            b = 0.3f;
        }
        public override bool CanExplode(int i, int j) => false;
    }
}