using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Sounds.Custom;
using ReLogic.Utilities;
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
        public float buttonY = 14;
        public float velY;
        public int buttonPressed;
        private SlotId loop;
        private float loopVolume;
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
                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/Switch1") { Volume = .5f }, NPC.position);
                if (NPC.position.Y < (NPC.ai[2] - ((NPC.ai[2] / 2) - (NPC.ai[3] / 2))) * 16)
                    buttonPressed = 1;
                else
                    buttonPressed = 2;
            }
            switch (buttonPressed)
            {
                case 0:
                    loopVolume = 0;
                    buttonY += 0.2f;
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    break;
                case 1:
                    buttonY -= 0.2f;
                    if (NPC.ai[0]++ == 50)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(new("Redemption/Sounds/Custom/ElevatorStart"), NPC.position);
                        Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                    }
                    if (NPC.ai[0] >= 50)
                    {
                        velY += 0.01f;
                        NPC.velocity.Y += velY / 10;
                        if (NPC.ai[0] >= 160)
                        {
                            loopVolume = 1;
                        }
                        if (NPC.position.Y >= NPC.ai[2] * 16)
                        {
                            loopVolume = 0;
                            if (NPC.ai[1]++ == 0)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/Slam2") { Volume = .5f }, NPC.position);
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
                                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/Switch1") { Volume = .5f }, NPC.position);
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
                            SoundEngine.PlaySound(new("Redemption/Sounds/Custom/ElevatorStart"), NPC.position);
                        Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                    }
                    if (NPC.ai[0] >= 50)
                    {
                        velY += 0.01f;
                        NPC.velocity.Y -= velY / 10;
                        if (NPC.ai[0] >= 160)
                        {
                            loopVolume = 1;
                        }
                        if (NPC.position.Y <= NPC.ai[3] * 16)
                        {
                            loopVolume = 0;
                            if (NPC.ai[1]++ == 0)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/Slam2") { Volume = .5f }, NPC.position);
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
                                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/Switch1") { Volume = .5f }, NPC.position);
                                buttonPressed = 0;
                            }
                        }
                    }
                    break;
            }
            CustomSounds.UpdateLoopingSound(ref loop, CustomSounds.LiftLoop, loopVolume, 0, NPC.position);

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
    public class ShadestoneLift2 : ShadestoneLift
    {
        public override string Texture => "Redemption/NPCs/Soulless/ShadestoneLift";
    }
    public class ShadestoneLift3 : ShadestoneLift
    {
        public override string Texture => "Redemption/NPCs/Soulless/ShadestoneLift";
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
                    SoundEngine.PlaySound(new("Redemption/Sounds/Custom/Switch1") { Volume = .5f }, NPC.position);
                buttonPressed = 1;
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
                            SoundEngine.PlaySound(new("Redemption/Sounds/Custom/ElevatorStart"), NPC.position);
                        Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 8 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                    }
                    if (NPC.ai[0] >= 50)
                    {
                        velY += 0.01f;
                        NPC.velocity.Y += velY / 10;
                        if (NPC.ai[0] == 160 && NPC.ai[0] < 260)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(new("Redemption/Sounds/Custom/ElevatorLoop"), NPC.position);
                        }
                        if (NPC.ai[0] == 260 && !Main.dedServ)
                            SoundEngine.PlaySound(new("Redemption/Sounds/Custom/ElevatorBreak"), NPC.position);
                        if (NPC.ai[0] == 480)
                        {
                            Rectangle fallRect = new((int)NPC.position.X, (int)NPC.position.Y - 40, NPC.width, NPC.height + 40);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                Player player = Main.player[i];
                                if (!player.active || player.dead)
                                    continue;

                                if (!player.Hitbox.Intersects(fallRect))
                                    continue;

                                player.RedemptionScreen().cutscene = true;
                                player.AddBuff(ModContent.BuffType<StunnedDebuff>(), 260);
                            }
                            if (Main.netMode != NetmodeID.Server)
                            {
                                for (int g = 0; g < 6; g++)
                                {
                                    int goreIndex = Gore.NewGore(NPC.GetSource_FromThis(), RedeHelper.RandAreaInEntity(NPC), default, Main.rand.Next(61, 64));
                                    Main.gore[goreIndex].velocity *= 0.1f;
                                }
                            }
                            NPC.velocity.Y += 7;
                            Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 12 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                        }
                        if (NPC.ai[0] >= 480)
                        {
                            NPC.velocity.Y += 0.1f;
                            NPC.rotation += 0.004f;
                        }
                        if (NPC.position.Y >= NPC.ai[2] * 16)
                        {
                            SoullessArea.soullessBools[2] = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.WorldData);

                            if (!Main.dedServ)
                                SoundEngine.PlaySound(new("Redemption/Sounds/Custom/ElevatorImpact"), NPC.position);
                            Main.player[Main.myPlayer].RedemptionScreen().ScreenShakeIntensity = 14 - (Main.player[Main.myPlayer].Distance(NPC.Center) / 64);
                            if (Main.netMode != NetmodeID.Server)
                            {
                                for (int g = 0; g < 6; g++)
                                {
                                    int goreIndex = Gore.NewGore(NPC.GetSource_FromThis(), RedeHelper.RandAreaInEntity(NPC), default, Main.rand.Next(61, 64));
                                    Main.gore[goreIndex].velocity *= 2f;
                                }
                            }
                            NPC.active = false;
                        }
                    }
                    break;
            }
            buttonY = MathHelper.Clamp(buttonY, 11, 14);
            velY = MathHelper.Clamp(velY, -0.03f, 0.03f);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead)
                    continue;

                if (!player.HasBuff<StunnedDebuff>())
                    continue;

                player.RedemptionScreen().cutscene = true;
            }
            if (NPC.ai[0] < 480)
            {
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
        }
        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                    collider.PostUpdate();
            }
        }
    }
}