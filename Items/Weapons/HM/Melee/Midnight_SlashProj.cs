using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.BaseExtension;
using Terraria.ModLoader;
using Redemption.Projectiles.Melee;
using ParticleLibrary;
using Redemption.Particles;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Midnight_SlashProj : TrueMeleeProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Midnight, Defiler of the Prince");
			Main.projFrames[Projectile.type] = 9;
		}
		public override bool ShouldUpdatePosition() => false;
		public override void SetSafeDefaults()
		{
			Projectile.width = 78;
			Projectile.height = 94;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.Redemption().IsAxe = true;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public float SwingSpeed;
		int directionLock = 0;
		private float glow;
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			player.heldProj = Projectile.whoAmI;
			Rectangle projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 140 : Projectile.Center.X), (int)(Projectile.Center.Y - 100), 140, 130);
			Point tileBelow = new Vector2(projHitbox.Center.X, projHitbox.Bottom).ToTileCoordinates();
			Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);

			SwingSpeed = SetSwingSpeed(25);

			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();
			if (Main.myPlayer == Projectile.owner)
			{
				if (Projectile.ai[0] == 0)
				{
					player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
					player.bodyFrame.Y = 5 * player.bodyFrame.Height;
					if (++Projectile.frameCounter >= SwingSpeed / 9)
					{
						Projectile.frameCounter = 0;
						Projectile.frame++;
						if (Projectile.frame >= 3)
						{
							glow += 0.08f;
							glow = MathHelper.Clamp(glow, 0, 0.8f);
							if (glow >= 0.8 && Projectile.localAI[0] == 0)
							{
								RedeDraw.SpawnRing(Projectile.Center, new Color(195, 100, 255), 0.14f, 0.87f, 4);
								RedeDraw.SpawnRing(Projectile.Center, new Color(102, 0, 255), 0.2f, 0.7f);
								if (!Main.dedServ)
									SoundEngine.PlaySound(CustomSounds.NebSound1 with { Pitch = 0.2f }, player.position);
								Projectile.localAI[0] = 1;
							}
							if (!player.channel)
							{
								Projectile.ai[0] = 1;
								directionLock = player.direction;
							}
							Projectile.frame = 3;
						}
					}
				}
				if (Projectile.ai[0] >= 1)
				{
					player.direction = directionLock;
					Projectile.ai[0]++;
					if (Projectile.frame > 3)
						player.itemRotation -= MathHelper.ToRadians(-9f * player.direction);
					else
						player.bodyFrame.Y = 5 * player.bodyFrame.Height;
					if (++Projectile.frameCounter >= SwingSpeed / 9)
					{
						Projectile.frameCounter = 0;
						Projectile.frame++;
						if (Projectile.frame is 5)
						{
							SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
							if (tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType])
							{
								SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
								player.RedemptionScreen().ScreenShakeIntensity = 5;
								if (!hitOnce)
									SpawnNebulaSparks();
							}
							else
							{
								SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
							}
							player.velocity.X += 2 * player.direction;

							for (int i = 0; i < 12; i++)
								ParticleManager.NewParticle(RedeHelper.RandomPointInArea(projHitbox), RedeHelper.Spread(4), new RainbowParticle(), Color.White, 0.2f);
						}
						if (Projectile.frame > 8)
						{
							Projectile.Kill();
						}
					}
				}
			}

			Projectile.spriteDirection = player.direction;

			Projectile.Center = player.Center;
			player.itemTime = 2;
			player.itemAnimation = 2;
		}
		private void SpawnNebulaSparks()
		{
			Player player = Main.player[Projectile.owner];
			Rectangle projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 140 : Projectile.Center.X), (int)(Projectile.Center.Y - 100), 140, 130);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<NebulaStar>()] < 4)
			{
				if (!Main.dedServ)
					SoundEngine.PlaySound(CustomSounds.Teleport1, Projectile.position);
				int num = 0;
				if (glow >= 0.8f)
					num = 2;
				else if (glow >= 0.5f)
					num = 1;
				for (int i = 0; i < num; i++)
				{
					if (Projectile.owner == Main.myPlayer)
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), RedeHelper.RandomPointInArea(projHitbox), RedeHelper.SpreadUp(10), ModContent.ProjectileType<NebulaStar>(), (int)(Projectile.damage * 0.75f), 2, player.whoAmI);
				}
			}
		}
		private bool hitOnce;
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (!hitOnce)
			{
				SpawnNebulaSparks();
				hitOnce = true;
			}
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			damage = (int)(damage * ((glow * 2) + 1));
			RedeProjectile.Decapitation(target, ref damage, ref crit, 80);
		}

		private float drawTimer;
		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			Texture2D axe = TextureAssets.Projectile[Projectile.type].Value;
			Texture2D mask = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Melee/Midnight_SlashProjMask").Value;
			int height = axe.Height / 9;
			int y = height * Projectile.frame;
			Rectangle rect = new(0, y, axe.Width, height);
			Vector2 drawOrigin = new(axe.Width / 2, Projectile.height / 2);
			Vector2 pos = Projectile.Center - Main.screenPosition - new Vector2(-62 * Projectile.spriteDirection, 80) + Vector2.UnitY * Projectile.gfxOffY;
			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, axe, ref drawTimer, pos, new Rectangle?(rect), RedeColor.NebColour * Projectile.Opacity * glow, Projectile.rotation, drawOrigin, Projectile.scale, effects);

			Texture2D space = ModContent.Request<Texture2D>("Redemption/Textures/SpacePlaceholder").Value;
			Effect midnight = ModContent.Request<Effect>("Redemption/Effects/Midnight").Value;
			midnight.Parameters["sampleTexture"].SetValue(space);
			midnight.Parameters["border"].SetValue(new Color(1f, 0.41960788f, 0.5921569f, 1f).ToVector4());
			midnight.Parameters["mask"].SetValue(new Vector4(0f, 1f, 0f, 1f));
			midnight.Parameters["offset"].SetValue(Main.player[Main.myPlayer].position * 0.21f / new Vector2(space.Width, space.Height));
			// The division stuff here will need changing once the actual space texture is made.
			midnight.Parameters["spriteRatio"].SetValue(new Vector2(Main.screenWidth / 8f / mask.Width, Main.screenHeight / 2f / mask.Height * 25f));
			midnight.Parameters["conversion"].SetValue(new Vector2(1f / (Main.screenWidth / 2f), 1f / (Main.screenHeight / 2f)));

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
			Main.spriteBatch.Draw(axe, pos, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			if (!player.channel || Projectile.ai[0] >= 1)
			{
				midnight.CurrentTechnique.Passes[0].Apply();
				Main.spriteBatch.Draw(mask, pos, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			projHitbox = new((int)(Projectile.spriteDirection == -1 ? Projectile.Center.X - 140 : Projectile.Center.X), (int)(Projectile.Center.Y - 100), 140, 130);
			return Projectile.frame is 5 && projHitbox.Intersects(targetHitbox);
		}
	}
}