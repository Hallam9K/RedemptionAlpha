using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Pets;
using Redemption.Items.Critters;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PreHM.Ranged;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class HalPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Halm");
            Main.projFrames[Projectile.type] = 16;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 7, 5)
                .WithOffset(2, 0).WithSpriteDirection(-1);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 20;
            Projectile.height = 34;
            AIType = ProjectileID.BabyDino;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;

            if (Projectile.ai[0] == 1)
            {
                if (frameY < 8)
                    frameY = 8;
                if (++frameCounter >= 5)
                {
                    frameCounter = 0;
                    if (++frameY >= 15)
                        frameY = 8;
                }
            }
            else
            {
                if (Projectile.velocity.X > -1 && Projectile.velocity.X < 1)
                    frameY = 0;
                else
                {
                    if (frameY < 1)
                        frameY = 1;

                    frameCounter += (int)(Projectile.velocity.X * 0.5f);
                    if (frameCounter >= 2 || frameCounter <= -2)
                    {
                        frameCounter = 0;
                        if (++frameY >= 7)
                            frameY = 1;
                    }
                }
            }
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);

            if (Main.rand.NextBool(1000000))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Main.screenPosition.X, player.Center.Y + Main.rand.Next(-500, 500)), new Vector2(6, 0), ModContent.ProjectileType<HalPetSPEEN>(), 9999, 20, Projectile.owner);
                        break;
                    case 1:
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Main.screenPosition.X + Main.screenWidth, player.Center.Y + Main.rand.Next(-500, 500)), new Vector2(-6, 0), ModContent.ProjectileType<HalPetSPEEN>(), 9999, 20, Projectile.owner);
                        break;
                }
            }
            if (player.HeldItem.type == ItemID.BowlofSoup)
            {
                if (Main.rand.NextBool(900))
                {
                    EmoteBubble.NewBubble(75, new WorldUIAnchor(Projectile), 120);
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "soup?", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "yum yum", false, false);
                            break;
                    }
                }
            }
            if (player.HeldItem.type == ItemID.CookedFish)
            {
                if (Main.rand.NextBool(2))
                {
                    EmoteBubble.NewBubble(76, new WorldUIAnchor(Projectile), 120);
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "phish", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "yum yum", false, false);
                            break;
                        case 2:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "fishy", false, false);
                            break;
                        case 3:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "gimmi phish", false, false);
                            break;
                        case 4:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "feed me", false, false);
                            break;
                    }
                }
            }
            if (player.HeldItem.type == ItemID.GingerbreadCookie || player.HeldItem.type == ItemID.SugarCookie)
            {
                if (Main.rand.NextBool(500))
                {
                    EmoteBubble.NewBubble(94, new WorldUIAnchor(Projectile), 120);
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "cookie", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "yum yum", false, false);
                            break;
                        case 2:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "gimmi cookie", false, false);
                            break;
                    }
                }
            }
            if (player.HeldItem.type == ItemID.WhoopieCushion)
            {
                if (Main.rand.NextBool(1200))
                {
                    CombatText.NewText(Projectile.getRect(), Color.DeepPink, "*toot*", false, false);
                }
            }
            if (player.HeldItem.type == ModContent.ItemType<ChickenEgg>() || player.HeldItem.type == ModContent.ItemType<FriedChickenEgg>() || player.HeldItem.type == ModContent.ItemType<LongEgg>() || player.HeldItem.type == ModContent.ItemType<GoldChickenEgg>())
            {
                if (Main.rand.NextBool(300))
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "egg", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "chikcen", false, false);
                            break;
                    }
                }
            }
            if (player.HeldItem.type == ModContent.ItemType<ChickenGoldItem>() || player.HeldItem.type == ModContent.ItemType<ChickenItem>() || player.HeldItem.type == ModContent.ItemType<LeghornChickenItem>() || player.HeldItem.type == ModContent.ItemType<LongChickenItem>() || player.HeldItem.type == ModContent.ItemType<RedChickenItem>() || player.HeldItem.type == ModContent.ItemType<BlackChickenItem>())
            {
                if (Main.rand.NextBool(300))
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "chkcien funni", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "chikcen", false, false);
                            break;
                        case 2:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "i laugh at chieken", false, false);
                            break;
                    }
                }
            }
            if (player.HeldItem.type == ModContent.ItemType<FriedChicken>())
            {
                if (Main.rand.NextBool(2))
                {
                    CombatText.NewText(Projectile.getRect(), Color.DeepPink, "gimmi chciken", false, false);
                }
            }
            if (player.HasBuff(BuffID.Invisibility))
            {
                if (Main.rand.NextBool(400))
                {
                    EmoteBubble.NewBubble(87, new WorldUIAnchor(Projectile), 120);
                    switch (Main.rand.Next(6))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "hello?", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "where is hoomun?", false, false);
                            break;
                        case 2:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "mamaa", false, false);
                            break;
                        case 3:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "where you at?", false, false);
                            break;
                        case 4:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "why you leave?", false, false);
                            break;
                        case 5:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, ":<", false, false);
                            break;
                    }
                }
            }
            if (player.HasBuff(BuffID.Bleeding))
            {
                if (Main.rand.NextBool(3000))
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "yummy blood", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "you bleeding", false, false);
                            break;
                        case 2:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "blod", false, false);
                            break;
                    }
                }
            }
            if (player.HasBuff(BuffID.Slow))
            {
                if (Main.rand.NextBool(1000))
                {
                    EmoteBubble.NewBubble(91, new WorldUIAnchor(Projectile), 120);
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "hurry up", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "you're too slow", false, false);
                            break;
                        case 2:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "move move move", false, false);
                            break;
                    }
                }
            }
            if (player.HasBuff(BuffID.Frozen))
            {
                if (Main.rand.NextBool(1000))
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "is it cold in there?", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "i'll get you out!", false, false);
                            break;
                        case 2:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "brrr", false, false);
                            break;
                    }
                }
            }
            if (player.HasBuff(BuffID.Rabies))
            {
                if (Main.rand.NextBool(5000))
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "that bite wasn't from me", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "*nom*", false, false);
                            break;
                    }
                }
            }
            if (player.HasBuff(BuffID.Stinky))
            {
                if (Main.rand.NextBool(900))
                {
                    EmoteBubble.NewBubble(8, new WorldUIAnchor(Projectile), 120);
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "stinky", false, false);
                            break;
                        case 1:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "you stinky", false, false);
                            break;
                        case 2:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "uh oh stinky", false, false);
                            break;
                        case 3:
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "ew", false, false);
                            break;
                    }
                }
            }
            if (Main.rand.NextBool(40000))
            {
                switch (Main.rand.Next(15))
                {
                    case 0:
                        CombatText.NewText(Projectile.getRect(), Color.DeepPink, "Always watching...", false, false);
                        break;
                    case 1:
                        CombatText.NewText(Projectile.getRect(), Color.DeepPink, "You got any pasta?", false, false);
                        break;
                    case 2:
                        CombatText.NewText(Projectile.getRect(), Color.DeepPink, "You think you're safe?", false, false);
                        break;
                    case 3:
                        CombatText.NewText(Projectile.getRect(), Color.DeepPink, "yum yum", false, false);
                        break;
                    case 4:
                        CombatText.NewText(Projectile.getRect(), Color.DeepPink, "Pitiful fool...", false, false);
                        break;
                    case 5:
                        CombatText.NewText(Projectile.getRect(), Color.DeepPink, "cheese", false, false);
                        break;
                    case 6:
                        CombatText.NewText(Projectile.getRect(), Color.DeepPink, "*nom*", false, false);
                        break;
                    case 7:
                        CombatText.NewText(Projectile.getRect(), Color.DeepPink, "I have too much swagger for the dagger", false, false);
                        break;
                    case 10:
                        if (RedeSystem.Silence)
                        {
                            CombatText.NewText(Projectile.getRect(), Color.DeepPink, "spooky", false, false);
                        }
                        else { goto case 2; }
                        break;
                }
            }
        }
        private int frameY;
        private int frameCounter;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 16;
            int y = height * frameY;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<HalPetBuff>()))
                Projectile.timeLeft = 2;
        }
    }
}