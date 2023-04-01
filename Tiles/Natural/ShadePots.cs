using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Shade;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Natural
{
    public class ShadePots : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileCut[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<ShadestoneDust>();
            HitSound = SoundID.Shatter;
            AddMapEntry(new Color(140, 140, 170), Language.GetText("MapObject.Pot"));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            EntitySource_TileBreak source = new(i, j);
            if (Main.rand.NextBool(250))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(source, (i + 1.5f) * 16f, j * 16f, 0f, 0f, ProjectileID.CoinPortal, 0, 0, Main.myPlayer);
            }
            else
            {
                if (Main.expertMode ? Main.rand.Next(45) < 2 : Main.rand.NextBool(45))
                {
                    switch (Main.rand.Next(11))
                    {
                        case 0:
                            //Item.NewItem(source, i * 16, j * 16, 32, 16, ModContent.ItemType<ElectrifiedPotion>());
                            break;
                        case 1:
                            //Item.NewItem(source, i * 16, j * 16, 32, 16, ModContent.ItemType<LurkingKetredPotion>());
                            break;
                        case 2:
                            //Item.NewItem(source, i * 16, j * 16, 32, 16, ModContent.ItemType<ChakrogAnglerPotion>());
                            break;
                        case 3:
                            //Item.NewItem(source, i * 16, j * 16, 32, 16, ModContent.ItemType<AbyssBloskusPotion>());
                            break;
                        case 4:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.BattlePotion);
                            break;
                        case 5:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.EndurancePotion);
                            break;
                        case 6:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.InvisibilityPotion);
                            break;
                        case 7:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.ManaRegenerationPotion);
                            break;
                        case 8:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.MagicPowerPotion);
                            break;
                        case 9:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.TitanPotion);
                            break;
                        case 10:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.WrathPotion);
                            break;
                    }
                }
                else
                {
                    switch (Main.rand.Next(8))
                    {
                        case 0:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.Heart);
                            if (Main.rand.NextBool(2))
                                Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.Heart);
                            break;
                        case 1:
                            if (Main.tile[i, j].LiquidAmount == 255 && Main.tile[i, j].LiquidType == LiquidID.Water)
                                Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.SpelunkerGlowstick, Main.rand.Next(Main.expertMode ? 5 : 4, Main.expertMode ? 18 : 12));
                            else
                                Item.NewItem(source, i * 16, j * 16, 32, 16, ModContent.ItemType<ShadeTorch>(), Main.rand.Next(Main.expertMode ? 5 : 4, Main.expertMode ? 18 : 12));
                            break;
                        case 2:
                            //Item.NewItem(source, i * 16, j * 16, 32, 16, ModContent.ItemType<ShadeKnife>(), Main.rand.Next(10, 20));
                            break;
                        case 3:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.SuperHealingPotion);
                            if (Main.rand.NextBool(3)) { Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.SuperHealingPotion); }
                            break;
                        case 4:
                            Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.Bomb, Main.rand.Next(1, Main.expertMode ? 7 : 4));
                            break;
                        case 5:
                            for (int k = 0; k < Main.rand.Next(1, 4); k++)
                            {
                                if (Main.rand.NextBool(2)) { Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.CopperCoin, Main.rand.Next(1, 99)); }
                            }
                            for (int k = 0; k < Main.rand.Next(1, 4); k++)
                            {
                                if (Main.rand.NextBool(2)) { Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.SilverCoin, Main.rand.Next(1, 50)); }
                            }
                            for (int k = 0; k < Main.rand.Next(1, 3); k++)
                            {
                                if (Main.rand.NextBool(4)) { Item.NewItem(source, i * 16, j * 16, 32, 16, ItemID.GoldCoin, Main.rand.Next(1, 3)); }
                            }
                            break;
                        case 6:
                            goto case 5;
                        case 7:
                            //int index1 = NPC.NewNPC(source, (int)((i + 1.5f) * 16), (int)((j + 1.5f) * 16), ModContent.NPCType<LaughingMaskSmall>());
                            //if (index1 < Main.maxNPCs && Main.netMode == NetmodeID.MultiplayerClient)
                            //    NetMessage.SendData(MessageID.SyncNPC, number: index1);
                            break;
                    }
                }
            }
            if (Main.netMode == NetmodeID.Server)
                return;

            Vector2 v = new(Main.rand.Next(-4, 5), Main.rand.Next(-4, 5));
            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), v, ModContent.Find<ModGore>("Redemption/ShadePotGore1").Type);
            if (Main.rand.NextBool(2))
                Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), v, ModContent.Find<ModGore>("Redemption/ShadePotGore2").Type);
            if (Main.rand.NextBool(2))
                Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), v, ModContent.Find<ModGore>("Redemption/ShadePotGore3").Type);
            if (Main.rand.NextBool(2))
                Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), v, ModContent.Find<ModGore>("Redemption/ShadePotGore4").Type);
        }
    }
    public class ShadePotsItem : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Shadestone Pot");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<ShadePots>();
        }
    }
}