using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Space
{
    public class SlayerBaseLift : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Platform");
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public CollisionSurface[] colliders = null;
        public override void SetDefaults()
        {
            NPC.width = 128;
            NPC.height = 32;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
        }
        public override bool CheckActive() => false;
        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 1)
            {
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 2, 0, 0, 0 }) };
            }
            return true;
        }
        public float standTimer;
        public float velY;
        public int activated;
        public override void AI()
        {
            Rectangle activeRect = new((int)NPC.position.X, (int)NPC.position.Y - 14, 128, 16);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead || activated != 0)
                    continue;

                if (!player.Hitbox.Intersects(activeRect))
                    continue;

                standTimer += 2;
                if (standTimer < 60)
                    continue;

                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Spark1, NPC.position);
                if (NPC.position.Y < (NPC.ai[2] - ((NPC.ai[2] / 2) - (NPC.ai[3] / 2))) * 16)
                    activated = 1;
                else
                    activated = 2;
            }
            switch (activated)
            {
                case 0:
                    standTimer--;
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    break;
                case 1:
                    if (NPC.ai[0]++ == 50)
                    {
                        //if (!Main.dedServ)
                        //    SoundEngine.PlaySound(CustomSounds.ElevatorStart, NPC.position);
                        //Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                    }
                    if (NPC.ai[0] >= 50)
                    {
                        velY += 0.1f;
                        NPC.velocity.Y += velY / 10;
                        if (NPC.position.Y >= NPC.ai[2] * 16)
                        {
                            if (NPC.ai[1]++ == 0)
                            {
                                //if (!Main.dedServ)
                                //    SoundEngine.PlaySound(CustomSounds.Slam2, NPC.position);
                                //Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 5 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                            }
                            NPC.velocity *= 0;
                            velY = 0;
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                Player player = Main.player[i];
                                if (!player.active || player.dead)
                                    continue;

                                if (player.Hitbox.Intersects(activeRect))
                                    continue;

                                standTimer--;
                                if (standTimer > 0)
                                    continue;

                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Spark1, NPC.position);
                                activated = 0;
                            }
                        }
                    }
                    break;
                case 2:
                    if (NPC.ai[0]++ == 50)
                    {
                        //if (!Main.dedServ)
                        //    SoundEngine.PlaySound(CustomSounds.ElevatorStart, NPC.position);
                        //Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                    }
                    if (NPC.ai[0] >= 50)
                    {
                        velY += 0.1f;
                        NPC.velocity.Y -= velY / 10;
                        if (NPC.position.Y <= NPC.ai[3] * 16)
                        {
                            if (NPC.ai[1]++ == 0)
                            {
                                //if (!Main.dedServ)
                                //    SoundEngine.PlaySound(CustomSounds.Slam2, NPC.position);
                                //Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 5 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                            }
                            NPC.velocity *= 0;
                            velY = 0;
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                Player player = Main.player[i];
                                if (!player.active || player.dead)
                                    continue;

                                if (player.Hitbox.Intersects(activeRect))
                                    continue;

                                standTimer--;
                                if (standTimer > 0)
                                    continue;

                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Spark1, NPC.position);
                                activated = 0;
                            }
                        }
                    }
                    break;
            }

            velY = MathHelper.Clamp(velY, -0.04f, 0.04f);
            standTimer = MathHelper.Clamp(standTimer, 0, 60);
            if (colliders != null && colliders.Length == 1)
            {
                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);
            }
        }
        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                    collider.PostUpdate();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Main.EntitySpriteDraw(texture, NPC.Center - new Vector2(0, NPC.velocity.Y * 2) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(glow, NPC.Center - new Vector2(0, NPC.velocity.Y * 2) - screenPos, null, NPC.GetAlpha(Color.White) * (standTimer / 60), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class SlayerBaseLift2 : SlayerBaseLift
    {
        public override string Texture => "Redemption/NPCs/Space/SlayerBaseLift";
    }
    public class SlayerBaseLift3 : SlayerBaseLift
    {
        public override string Texture => "Redemption/NPCs/Space/SlayerBaseLift";
    }
}