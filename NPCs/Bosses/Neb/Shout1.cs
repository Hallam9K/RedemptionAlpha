using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class Shout1 : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout1";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }
        public override void SetDefaults()
        {
            Projectile.width = 258;
            Projectile.height = 46;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 60;
            DrawOriginOffsetY = -100;
        }
        public override void AI()
        {
            NPC npc2 = Main.npc[(int)Projectile.ai[0]];
            Projectile.Center = npc2.Center;
            if (Projectile.localAI[0] == 1f)
            {
                Projectile.alpha += 10;
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
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
    public class Shout2 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout2";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 184;
            Projectile.height = 46;
        }
    }
    public class Shout3 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout3";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 396;
            Projectile.height = 64;
        }
    }
    public class Shout4 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout4";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 285;
            Projectile.height = 46;
        }
    }
    public class Shout5 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout5";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 447;
            Projectile.height = 46;
        }
    }
    public class Shout6 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout6";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 556;
            Projectile.height = 64;
        }
    }
    public class Shout7 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout7";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 423;
            Projectile.height = 46;
        }
    }
    public class Shout8 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout8";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 347;
            Projectile.height = 61;
        }
    }
    public class Shout9 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout9";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 539;
            Projectile.height = 46;
        }
    }
    public class Shout10 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout10";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 489;
            Projectile.height = 62;
        }
    }
    public class Shout11 : Shout1
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Shout11";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shout!");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 324;
            Projectile.height = 62;
        }
    }
}