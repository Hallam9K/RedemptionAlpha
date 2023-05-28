//TODO:
//Better start on thorns
//Lighting on thorns
//Make thorns end on tiles
//Stabbing
//Slowing effect on player
//Better landing
//Sound effects

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Redemption.Helpers;
using Terraria.ModLoader;
using System.Linq;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class BlightedRoot : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blighted Root");
            // Tooltip.SetDefault("i am BEGGING to be updated");
            Item.staff[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = 2500;
            Item.channel = true;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item21;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BlightedRootProj>();
            Item.shootSpeed = 8f;
            Item.noUseGraphic = true;
        }
    }

    public class BlightedRootProj : ModProjectile
    {
        private Player owner => Main.player[Projectile.owner];

        private int timer;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = -1.57f;
            Projectile.Center = owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f); 
            owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Vector2.Zero;

            if (owner.channel)
            {
                owner.itemTime = owner.itemAnimation = 5;
                Projectile.timeLeft = 2;
                timer++;
                if (timer == 40)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -70), Main.rand.NextVector2Circular(6, 6), ModContent.ProjectileType<BlightedRootThorn>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
                    }
                }

                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - 1.57f);
            }
            else
            {
                var projs = Main.projectile.Where(n => n.active && n.owner == Projectile.owner && n.type == ModContent.ProjectileType<BlightedRootThorn>()).ToList();
                projs.ForEach(n => (n.ModProjectile as BlightedRootThorn).doomed = true);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float rot = Projectile.rotation + 0.78f;
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 gOffset = new Vector2(0, owner.gfxOffY);

            Main.spriteBatch.Draw(tex, Projectile.Center + gOffset - Main.screenPosition, null, lightColor, rot, new Vector2(0, tex.Height), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class BlightedRootThorn : ModProjectile
    {

        private int numPoints = 90;
        private Player owner => Main.player[Projectile.owner];

        public List<Vector2> cache;
        public List<Vector2> realCache;
        public Trail trail;

        private bool growing = true;

        public bool doomed = false;

        int attackTimer = 0;

        public NPC target;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 3;
            Projectile.tileCollide = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y += 0.3f;
            UpdateCache();
            if (doomed)
            {
                for (int i = 0; i < 2; i++)
                {
                    numPoints--;
                    realCache.RemoveAt(0);
                    if (realCache.Count < 3)
                    {
                        Projectile.active = false;
                        return;
                    }
                }
            }
            else if (!growing)
            {
                attackTimer++;
                if (attackTimer > 70 && attackTimer % 20 == 0)
                {
                    var otherThorns = Main.projectile.Where(n => n.active && n != Projectile && Projectile.type == n.type).ToList();
                    List<NPC> forbiddenTargets = new List<NPC>();
                    foreach (Projectile thorn in otherThorns)
                    {
                        var thornMP = (thorn.ModProjectile as BlightedRootThorn);
                        if (thornMP.target != null && thornMP.target != default)
                        {
                            forbiddenTargets.Add(thornMP.target);
                        }
                    }

                    target = Main.npc.Where(n => n.active && n.CanBeChasedBy() && !forbiddenTargets.Contains(n) && n.Distance(Projectile.Center) < 700).OrderBy(n => n.Distance(Projectile.Center)).FirstOrDefault();

                    if (target != default)
                    {
                        SpawnStabberOnEnemy(target.Center);
                    }
                }
            }
                ManageTrail();
        }

        private void SpawnStabberOnEnemy(Vector2 pos)
        {
            Vector2 startPos = pos;
            startPos.X += Main.rand.Next(-32, 32);

            Tile tile = Framing.GetTileSafely((int)(startPos.X / 16), (int)(startPos.Y / 16));
            int tries = 0;
            while (!tile.HasTile || !Main.tileSolid[tile.TileType])
            {
                startPos.Y += 16;
                tile = Framing.GetTileSafely((int)(startPos.X / 16), (int)(startPos.Y / 16));
                if (tries++ > 60)
                    return;
            }

            float speed = 4;

            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), startPos, startPos.DirectionTo(pos) * speed, ModContent.ProjectileType<BlightedRootStabber>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            (proj.ModProjectile as BlightedRootStabber).distanceTimer = (int)(startPos.Distance(pos) / speed) + 20;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (growing)
            {
                UpdateCache();
            }
            ManageTrail();
            Projectile.velocity = Vector2.Zero;
            growing = false;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (trail == null || trail == default)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene[Redemption.Abbreviation + ":" + "BlightedRootThorns"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(tex);
            effect.Parameters["flip"].SetValue(false);
            effect.Parameters["repeats"].SetValue(TotalLength(realCache) / (float)tex.Width);

            trail.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void UpdateCache()
        {
            if (growing)
            {
                if (cache == null)
                {
                    cache = new List<Vector2>();
                    for (int k = 0; k < numPoints; k++)
                    {
                        cache.Add(Projectile.Center + Main.rand.NextVector2Circular(2, 2));
                    }
                }
                cache.Add(Projectile.Center);

                if (cache.Count > numPoints)
                    cache.RemoveAt(0);
            }

            if (doomed)
                return;


            realCache = new List<Vector2>
            {
                cache[0]
            };

            float pointLength = TotalLength(cache) / numPoints;

            float pointCounter = 0;

            int presision = 100; //This normalizes length between points so it doesnt squash super weirdly on certain parts
            for (int i = 0; i < numPoints - 2; i++)
            {
                for (int j = 0; j < presision; j++)
                {
                    pointCounter += (cache[i] - cache[i + 1]).Length() / presision;

                    while (pointCounter > pointLength)
                    {
                        float lerper = j / (float)presision;
                        realCache.Add(Vector2.Lerp(cache[i], cache[i + 1], lerper));
                        pointCounter -= pointLength;
                    }
                }
            }

            while (realCache.Count > numPoints)
                realCache.RemoveAt(0);
        }

        private void ManageTrail()
        {
            trail = new Trail(Main.graphics.GraphicsDevice, realCache.Count, new TriangularTip(4), factor => 11, factor => Color.White);
            trail.Positions = realCache.ToArray();
            trail.NextPosition = Projectile.Center;
        }

        public static float TotalLength(List<Vector2> points)
        {
            float ret = 0;

            for (int i = 1; i < points.Count; i++)
            {
                ret += (points[i] - points[i - 1]).Length();
            }

            return ret;
        }

    }

    public class BlightedRootStabber : ModProjectile
    {
        private int numPoints = 90;
        private Player owner => Main.player[Projectile.owner];

        public List<Vector2> cache;
        public Trail trail;

        private float alpha = 1;

        private bool hit = false;

        private List<NPC> alreadyHit = new List<NPC>();

        public int distanceTimer;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 3;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            if (hit)
            {
                Projectile.velocity = Vector2.Zero;
                alpha -= 0.01f;
                if (alpha <= 0)
                    Projectile.active = false;
            }
            else
            {
                UpdateCache();
                distanceTimer--;
                if (distanceTimer <= 0)
                {
                    Projectile.extraUpdates = 0;
                    hit = true;
                }
            }

            ManageTrail();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (alreadyHit.Contains(target))
                return false;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.penetrate++;
            alreadyHit.Add(target);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (trail == null || trail == default)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene[Redemption.Abbreviation + ":" + "BlightedRootThorns"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(tex);
            effect.Parameters["flip"].SetValue(false);
            effect.Parameters["repeats"].SetValue(BlightedRootThorn.TotalLength(cache) / (float)tex.Width);

            trail.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void UpdateCache()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int k = 0; k < numPoints; k++)
                {
                    cache.Add(Projectile.Center + Main.rand.NextVector2Circular(2, 2));
                }
            }
            cache.Add(Projectile.Center);

            if (cache.Count > numPoints)
                cache.RemoveAt(0);
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.graphics.GraphicsDevice, numPoints, new TriangularTip(4), factor => 11, factor => Color.White * alpha);
            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
        }
    }
}