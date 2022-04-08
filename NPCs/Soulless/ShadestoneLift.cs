using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Soulless
{
    public class ShadestoneLift : ModNPC
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
            NPC.width = 96;
            NPC.height = 26;
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
            if (colliders == null || colliders.Length != 4)
            {
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 1, 1, 1, 1 }),
                    new CollisionSurface(NPC.TopLeft, NPC.BottomLeft, new int[] { 1, 1, 1, 1 }),
                    new CollisionSurface(NPC.TopRight, NPC.BottomRight, new int[] { 1, 1, 1, 1 }),
                    new CollisionSurface(NPC.BottomLeft, NPC.BottomRight, new int[] { 1, 1, 1, 1 }) };
            }
            return true;
        }
        private float buttonY = 14;
        private float velY;
        private int buttonPressed;
        public override void AI()
        {
            Rectangle buttonRect = new((int)NPC.position.X + 32, (int)NPC.position.Y - 14, 32, 18);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead || buttonPressed != 0 || buttonY < 14)
                    continue;

                if (!player.Hitbox.Intersects(buttonRect))
                    continue;

                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Switch1").WithVolume(0.5f), NPC.position);
                if (NPC.position.Y < 838 * 16)
                    buttonPressed = 1;
                else
                    buttonPressed = 2;
            }
            switch (buttonPressed)
            {
                case 0:
                    buttonY += 0.2f;
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    break;
                case 1:
                    buttonY -= 0.2f;
                    if (NPC.ai[0]++ == 50)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slam1").WithVolume(0.5f), NPC.position);
                        Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                    }
                    if (NPC.ai[0] >= 50)
                    {
                        velY += 0.01f;
                        NPC.velocity.Y += velY / 10;
                        if (NPC.position.Y >= 863 * 16)
                        {
                            if (NPC.ai[1]++ == 0)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slam2").WithVolume(0.5f), NPC.position);
                                Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 5 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                            }
                            NPC.velocity *= 0;
                            velY = 0;
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                Player player = Main.player[i];
                                if (!player.active || player.dead)
                                    continue;

                                if (player.Hitbox.Intersects(buttonRect))
                                    continue;

                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Switch1").WithVolume(0.5f), NPC.position);
                                buttonPressed = 0;
                            }
                        }
                    }
                    break;
                case 2:
                    buttonY -= 0.2f;
                    if (NPC.ai[0]++ == 50)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slam1").WithVolume(0.5f), NPC.position);
                        Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                    }
                    if (NPC.ai[0] >= 50)
                    {
                        velY += 0.01f;
                        NPC.velocity.Y -= velY / 10;
                        if (NPC.position.Y <= 821 * 16)
                        {
                            if (NPC.ai[1]++ == 0)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slam2").WithVolume(0.5f), NPC.position);
                                Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 5 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                            }
                            NPC.velocity *= 0;
                            velY = 0;
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                Player player = Main.player[i];
                                if (!player.active || player.dead)
                                    continue;

                                if (player.Hitbox.Intersects(buttonRect))
                                    continue;

                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Switch1").WithVolume(0.5f), NPC.position);
                                buttonPressed = 0;
                            }
                        }
                    }
                    break;
            }
            buttonY = MathHelper.Clamp(buttonY, 11, 14);
            velY = MathHelper.Clamp(velY, -0.03f, 0.03f);
            if (colliders != null && colliders.Length == 4)
            {
                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);

                colliders[1].Update();
                colliders[1].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[1].endPoints[1] = NPC.Center + (NPC.BottomLeft - NPC.Center).RotatedBy(NPC.rotation);

                colliders[2].Update();
                colliders[2].endPoints[0] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);
                colliders[2].endPoints[1] = NPC.Center + (NPC.BottomRight - NPC.Center).RotatedBy(NPC.rotation);

                colliders[3].Update();
                colliders[3].endPoints[0] = NPC.Center + (NPC.BottomLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[3].endPoints[1] = NPC.Center + (NPC.BottomRight - NPC.Center).RotatedBy(NPC.rotation);
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
            Texture2D button = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Button").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Main.EntitySpriteDraw(button, NPC.Center - new Vector2(-32, buttonY - 10) - new Vector2(0, NPC.velocity.Y * 2) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, NPC.Center - new Vector2(0, NPC.velocity.Y * 2) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}