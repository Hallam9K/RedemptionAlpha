using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.NPCs.Friendly;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Cooldowns;
using Terraria.GameContent.Creative;
using Redemption.Rarities;
using Terraria.DataStructures;
using Redemption.Items.Materials.PreHM;
using Terraria.Audio;
using Terraria.GameContent;
using System.Collections.Generic;
using Redemption.Globals;

namespace Redemption.Items.Usable
{
    public class SoulScroll : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Scroll");
            Tooltip.SetDefault("Converts all soulless enemies on screen into a normal lost soul"
                + "\n1 minute cooldown\n" +
                "'It's blank...'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 80;
            Item.useTime = 80;
            Item.width = 38;
            Item.height = 22;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.shoot = ModContent.ProjectileType<SoulScroll_Proj>();
            Item.shootSpeed = 0;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<SoulScrollCooldown>());
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(ModContent.BuffType<SoulScrollCooldown>(), 3600, true);
            return true;
        }
    }
    public class SoulScroll_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Scroll");
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        NPC target;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity *= 0;
            Projectile.Center = new Vector2(player.Center.X, player.Center.Y - 100);
            if (Projectile.localAI[0] < 50)
            {
                if (++Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 8)
                        Projectile.frame = 7;
                }
            }
            else
            {
                Projectile.alpha -= 4;
                if (++Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    if (--Projectile.frame < 0)
                        Projectile.Kill();
                }
            }
            if (Projectile.frame >= 5)
            {
                if (Projectile.localAI[0]++ == 1 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlindingLight>(), 0, 0, Main.myPlayer);
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/NebSound2").WithVolume(.9f), (int)Projectile.position.X, (int)Projectile.position.Y);
                }
                if (Projectile.localAI[0] == 25)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath39, Projectile.position);
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        target = Main.npc[n];
                        if (!target.active || target.boss || !NPCLists.Soulless.Contains(target.type))
                            continue;

                        target.Transform(ModContent.NPCType<LostSoulNPC>());
                    }
                }
            }
        }
    }
    public class BlindingLight : ModProjectile
    {
        public override string Texture => "Redemption/Textures/TransitionTex";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blinding Light");
        }
        public override void SetDefaults()
        {
            Projectile.width = 1000;
            Projectile.height = 1000;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 500;
            Projectile.scale = 1f;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void AI()
        {
            Projectile.scale += 0.3f;
            if (Projectile.localAI[0] == 1f)
            {
                Projectile.alpha += 30;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha <= 0)
                    Projectile.localAI[0] = 1f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
