using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.Items.Placeable.Furniture.Misc;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class SoulCandlesTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Soul Candles");
            AddMapEntry(new Color(204, 223, 224), name);
            DustType = ModContent.DustType<ShadestoneDust>();
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AnimationFrameHeight = 54;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 2)
                    frame = 0;
            }
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            if (!Main.projectile.Any(projectile => projectile.type == ModContent.ProjectileType<SoulCandles_Proj>() && (projectile.ModProjectile as SoulCandles_Proj).Parent == Main.tile[i, j] && projectile.active))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int aura = Projectile.NewProjectile(new EntitySource_TileInteraction(player, i, j), new Vector2(i * 16 + 8, j * 16 + 16), Vector2.Zero, ModContent.ProjectileType<SoulCandles_Proj>(), 0, 0, Main.myPlayer);
                    (Main.projectile[aura].ModProjectile as SoulCandles_Proj).Parent = Main.tile[i, j];
                }
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.7f;
            g = 0.7f;
            b = 0.8f;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<SoulCandles>());
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Tiles/Furniture/Misc/SoulCandlesTile_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
    public class SoulCandles_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public Tile Parent;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Candles");
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        Vector2 vector;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!Parent.HasTile)
                Projectile.Kill();
            Projectile.timeLeft = 10;
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC target = Main.npc[n];
                if (!target.boss && target.Distance(Projectile.Center) <= 100)
                {
                    //if (NPCLists.IsSoulless.Contains(target.type)) // TODO: Soul Candles kill enemy
                    //{
                    //    player.ApplyDamageToNPC(target, 9999, 0, 0, false);
                    //}
                }
            }
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player playerTarget = Main.player[p];
                if (playerTarget.active && !playerTarget.dead && playerTarget.Distance(Projectile.Center) <= 100)
                {
                    if (playerTarget.lifeRegen > 0)
                        playerTarget.lifeRegen = 0;

                    if (playerTarget.lifeRegenCount > 0)
                        playerTarget.lifeRegenCount = 0;

                    playerTarget.lifeRegenTime = 0;
                }
            }
            for (int k = 0; k < 2; k++)
            {
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * 100);
                vector.Y = (float)(Math.Cos(angle) * 100);
                Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, DustID.AncientLight, 0f, 0f, 100, default, 1f)];
                dust2.noGravity = true;
                dust2.velocity = -Projectile.DirectionTo(dust2.position) * 4f;
            }

        }
    }
}