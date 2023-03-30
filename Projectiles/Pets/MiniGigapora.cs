using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.Buffs.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class MiniGigapora : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 104;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        private bool spawned;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            if (!spawned)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int latestProj = Projectile.whoAmI;
                    int[] Type = { 0, 0, 1, 0, 0, 1, 0, 0, 2 };
                    for (int i = 0; i < Type.Length; ++i)
                    {
                        int bodyType = ModContent.ProjectileType<MiniGigapora_Body>();
                        switch (Type[i])
                        {
                            case 1:
                                bodyType = ModContent.ProjectileType<MiniGigapora_Core>();
                                break;
                            case 2:
                                bodyType = ModContent.ProjectileType<MiniGigapora_Tail>();
                                break;
                        }
                        latestProj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, bodyType, 0, 0, player.whoAmI, Projectile.whoAmI, latestProj);
                    }
                }
                spawned = true;
            }
            float num1047 = 0.2f;
            Vector2 vector133 = player.Center - Projectile.Center;
            if (vector133.Length() < 200f) num1047 = 0.12f;
            if (vector133.Length() < 140f) num1047 = 0.06f;
            if (vector133.Length() > 100f)
            {
                if (Math.Abs(player.Center.X - Projectile.Center.X) > 20f) Projectile.velocity.X = Projectile.velocity.X + num1047 * Math.Sign(player.Center.X - Projectile.Center.X);
                if (Math.Abs(player.Center.Y - Projectile.Center.Y) > 10f) Projectile.velocity.Y = Projectile.velocity.Y + num1047 * Math.Sign(player.Center.Y - Projectile.Center.Y);
            }
            else if (Projectile.velocity.Length() > 2f)
                Projectile.velocity *= 0.96f;

            if (Math.Abs(Projectile.velocity.Y) < 1f) Projectile.velocity.Y = Projectile.velocity.Y - 0.1f;
            float num1048 = 15f;
            if (Projectile.velocity.Length() > num1048) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * num1048;

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        public void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<MiniGigaporaPetBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            else Projectile.active = false;
        }
    }
    public class MiniGigapora_Body : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<MiniGigaporaPetBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile proj = Main.projectile[(int)Projectile.ai[0]];
            CheckActive(player);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!proj.active || proj.type != ModContent.ProjectileType<MiniGigapora>())
                    Projectile.active = false;
            }
            if (Projectile.ai[1] < (double)Main.projectile.Length)
            {
                float dirX = Main.projectile[(int)Projectile.ai[1]].Center.X - Projectile.Center.X;
                float dirY = Main.projectile[(int)Projectile.ai[1]].Center.Y - Projectile.Center.Y;
                Projectile.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

                float dist = (length - Projectile.width / 1.8f) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                if (dirX < 0f)
                    Projectile.spriteDirection = 1;
                else
                    Projectile.spriteDirection = -1;

                Projectile.velocity = Vector2.Zero;
                Projectile.position.X = Projectile.position.X + posX;
                Projectile.position.Y = Projectile.position.Y + posY;
            }
        }
    }
    public class MiniGigapora_Core : MiniGigapora_Body
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
    public class MiniGigapora_Tail : MiniGigapora_Body
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
}