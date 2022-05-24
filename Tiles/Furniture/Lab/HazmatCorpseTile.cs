using Microsoft.Xna.Framework;
using Redemption.Items;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Redemption.Globals.RedeNet;

namespace Redemption.Tiles.Furniture.Lab
{
    public class HazmatCorpseTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 20, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.GreenBlood;
            MinPick = 500;
            MineResist = 8f;
            HitSound = SoundID.NPCHit13;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Hazmat Corpse");
            AddMapEntry(new Color(242, 183, 111), name);
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            if (player.HeldItem.type == ModContent.ItemType<DeadRinger>())
                player.cursorItemIconID = ModContent.ItemType<DeadRinger>();
            else
                player.cursorItemIconID = ModContent.ItemType<HintIcon>();
        }
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            if (player.HeldItem.type == ModContent.ItemType<DeadRinger>())
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<HazmatCorpse_Ghost>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int index1 = NPC.NewNPC(new EntitySource_TileInteraction(player, i, j), i * 16, (j + 1) * 16, ModContent.NPCType<HazmatCorpse_Ghost>());
                        SoundEngine.PlaySound(SoundID.Item74, Main.npc[index1].position);
                        Main.npc[index1].velocity.Y -= 4;
                        Main.npc[index1].netUpdate2 = true;
                    }
                    else
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                            return false;

                        Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.NPCSpawnFromClient, (byte)player.whoAmI, ModContent.NPCType<HazmatCorpse_Ghost>(), i * 16, (j + 1) * 16).Send(-1);
                    }
                }
            }
            else
            {
                if (Main.tile[left, top].TileFrameX == 0)
                {
                    player.QuickSpawnItem(new EntitySource_TileInteraction(player, i, j), ModContent.ItemType<HazmatSuit2>());
                }
                for (int x = left; x < left + 3; x++)
                {
                    for (int y = top; y < top + 2; y++)
                    {
                        if (Main.tile[x, y].TileFrameX < 54)
                            Main.tile[x, y].TileFrameX += 54;
                    }
                }
            }
            return true;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            if (Main.tile[left, top].TileFrameX == 0)
            {
                Player player = Main.LocalPlayer;
                //player.QuickSpawnItem(ModContent.ItemType<Crowbar>());
                player.QuickSpawnItem(new EntitySource_TileBreak(i, j), ModContent.ItemType<HazmatSuit2>());
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class HazmatCorpse : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hazmat Corpse");
            Tooltip.SetDefault("Gives Crowbar and Hazmat Suit" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<HazmatCorpseTile>();
        }
    }
}
