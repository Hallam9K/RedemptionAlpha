using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Base;
using Terraria.GameContent;

namespace Redemption.NPCs.Bosses.Thorn
{
    [AutoloadBossHead]
    public class Thorn : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn, Bane of the Forest");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 2100;
            NPC.defense = 6;
            NPC.damage = 14;
            NPC.width = 62;
            NPC.height = 60;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 0;
            NPC.noGravity = false;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.noTileCollide = false;
            //bossBag = ModContent.ItemType<ThornBag>();
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 40; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Grass, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Grass, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public float[] customAI = new float[4];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(customAI[0]);
                writer.Write(customAI[1]);
                writer.Write(customAI[2]);
                writer.Write(customAI[3]);

                writer.Write(teleportTimer);
                writer.Write(choice);

                writer.Write(beginFight);
                writer.Write(teleport);
                writer.Write(attacking);
                writer.Write(appearing);
                writer.Write(disappearing);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                customAI[0] = (float)reader.ReadDouble();
                customAI[1] = (float)reader.ReadDouble();
                customAI[2] = (float)reader.ReadDouble();
                customAI[3] = (float)reader.ReadDouble();

                teleportTimer = reader.ReadInt32();
                choice = reader.ReadInt32();

                beginFight = reader.ReadBoolean();
                teleport = reader.ReadBoolean();
                attacking = reader.ReadBoolean();
                appearing = reader.ReadBoolean();
                disappearing = reader.ReadBoolean();
            }
        }
        public int choice;
        public Vector2 vector;
        bool title = false;

        public override void AI()
        {
            /*if (!title)
            {
                if (!Main.dedServ) Redemption.Inst.TitleCardUIElement.DisplayTitle("Thorn", 60, 90, 0.8f, 0, Color.ForestGreen, "Bane of the Forest");
                title = true;
            }*/
            if (Main.xMas)
                NPC.GivenName = "Everthorn, Bane of the Holidays";

            Target();

            DespawnHandler();

            NPC.frameCounter++;
            if (NPC.frameCounter >= 15)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += 68;
                if (NPC.frame.Y > (68 * 1))
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 0;
                }
            }
            if (attacking == true)
            {
                attackCounter++;
                if (attackCounter > 15)
                {
                    attackFrame++;
                    attackCounter = 0;
                }
                if (attackFrame >= 4)
                {
                    attackFrame = 2;
                }
            }
            if (appearing == true)
            {
                appearCounter++;
                if (appearCounter > 5)
                {
                    appearFrame++;
                    appearCounter = 0;
                }
                if (appearFrame >= 12)
                {
                    appearFrame = 11;
                }
            }
            if (disappearing == true)
            {
                disappearCounter++;
                if (disappearCounter > 5)
                {
                    disappearFrame++;
                    disappearCounter = 0;
                }
                if (disappearFrame >= 11)
                {
                    disappearFrame = 10;
                }
            }
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            if (player.Center.X > NPC.Center.X)
            {
                NPC.spriteDirection = 1;
            }
            else
            {
                NPC.spriteDirection = -1;
            }

            if (customAI[1] == 0)
            {
                teleport = true;
                NPC.netUpdate = true;
            }
            else if (customAI[1] == 1)
            {
                teleport = true;
                NPC.netUpdate = true;
                customAI[3] = 1;
            }
            else if (customAI[1] == 2)
            {
                customAI[1] = 3;
                choice = Main.rand.Next(5);
                NPC.netUpdate = true;
            }
            else if (customAI[1] == 3)
            {
                NPC.netUpdate = true;
                switch (choice)
                {
                    // Thorns
                    #region Thorns
                    case 0:
                        attacking = true;
                        customAI[0]++;
                        if (NPC.life < (int)(NPC.lifeMax * 0.5f))
                        {
                            if (customAI[0] == 60)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 500, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -500, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 80)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 400, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -400, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 100)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 300, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -300, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 120)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 200, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -200, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 140)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 100, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -100, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 160)
                            {
                                int p = Projectile.NewProjectile(player.Center.X + 0, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p].netUpdate = true;
                            }
                            if (customAI[0] >= 240)
                            {
                                customAI[0] = 0;
                                customAI[1] = 4;
                                teleportTimer = 0;
                                attacking = false;
                                teleport = true;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (customAI[0] == 60)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 500, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -500, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 90)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 400, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -400, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 120)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 300, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -300, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 150)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 200, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -200, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 180)
                            {
                                int p1 = Projectile.NewProjectile(player.Center.X + 100, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                int p2 = Projectile.NewProjectile(player.Center.X + -100, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p1].netUpdate = true;
                                Main.projectile[p2].netUpdate = true;
                            }
                            if (customAI[0] == 210)
                            {
                                int p = Projectile.NewProjectile(player.Center.X + 0, player.Center.Y - 200, 0f, 0f, ModContent.ProjectileType<ThornSeed>(), 10, 1, Main.myPlayer, 0, 0);
                                Main.projectile[p].netUpdate = true;
                            }
                            if (customAI[0] >= 320)
                            {
                                customAI[0] = 0;
                                customAI[1] = 4;
                                teleportTimer = 0;
                                attacking = false;
                                teleport = true;
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                    #endregion

                    // Vine Whip
                    #region Vine Whip
                    case 1:
                        if (Vector2.Distance(NPC.Center, player.Center) < 300)
                        {
                            customAI[0]++;
                            if (NPC.life < (int)(NPC.lifeMax * 0.5f))
                            {
                                if (customAI[0] == 60 || customAI[0] == 90 || customAI[0] == 120 || customAI[0] == 150 || customAI[0] == 180)
                                {
                                    float Speed = 19f;
                                    Vector2 vector8 = new Vector2(NPC.Center.X, NPC.Center.Y);
                                    int damage = 12;
                                    int type = ModContent.ProjectileType<CursedThornVile>();
                                    float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                                    int num54 = Projectile.NewProjectile(vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), type, damage, 0f, 0);
                                    Main.projectile[num54].netUpdate = true;
                                }
                            }
                            else
                            {
                                if (customAI[0] == 60 || customAI[0] == 90 || customAI[0] == 120)
                                {
                                    float Speed = 16f;
                                    Vector2 vector8 = new Vector2(NPC.Center.X, NPC.Center.Y);
                                    int damage = 10;
                                    int type = ModContent.ProjectileType<CursedThornVile>();
                                    float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                                    int num54 = Projectile.NewProjectile(vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), type, damage, 0f, 0);
                                    Main.projectile[num54].netUpdate = true;
                                }
                            }
                            if (customAI[0] >= 200)
                            {
                                customAI[0] = 0;
                                customAI[1] = 4;
                                teleportTimer = 0;
                                teleport = true;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.netUpdate = true;
                            customAI[0] = 0;
                            choice = Main.rand.Next(5);
                        }
                        break;
                    #endregion

                    // Poison
                    #region Poison
                    case 2:
                        customAI[0]++;
                        if (NPC.life < (int)(NPC.lifeMax * 0.5f))
                        {
                            if (customAI[0] == 10)
                            {
                                for (int k = 0; k < 40; k++)
                                {
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.GrassBlades, 0f, 0f, 100, default, 3f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -NPC.DirectionTo(dust2.position) * 10f;
                                }
                            }
                            if (customAI[0] == 80 || customAI[0] == 140)
                            {
                                int pieCut = 4;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    int projID = Projectile.NewProjectile(NPC.Center.X, NPC.Center.Y, 0, 0, ProjectileID.Stinger, 10, 3);
                                    Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(10f, 0f), m / (float)pieCut * 6.28f);
                                    Main.projectile[projID].netUpdate = true;
                                    Main.projectile[projID].timeLeft = 180;
                                }
                            }
                            if (customAI[0] == 110)
                            {
                                int pieCut = 8;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    int projID = Projectile.NewProjectile(NPC.Center.X, NPC.Center.Y, 0, 0, ProjectileID.Stinger, 10, 3);
                                    Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(10f, 0f), m / (float)pieCut * 6.28f);
                                    Main.projectile[projID].netUpdate = true;
                                    Main.projectile[projID].timeLeft = 180;
                                }
                            }
                            if (customAI[0] == 170)
                            {
                                int pieCut = 16;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    int projID = Projectile.NewProjectile(NPC.Center.X, NPC.Center.Y, 0, 0, ProjectileID.Stinger, 10, 3);
                                    Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(10f, 0f), m / (float)pieCut * 6.28f);
                                    Main.projectile[projID].netUpdate = true;
                                    Main.projectile[projID].timeLeft = 180;
                                }
                            }
                            if (customAI[0] >= 200)
                            {
                                if (Main.rand.Next(3) == 0)
                                {
                                    NPC.netUpdate = true;
                                    customAI[0] = 0;
                                    choice = Main.rand.Next(5);
                                }
                                else
                                {
                                    customAI[0] = 0;
                                    customAI[1] = 4;
                                    teleportTimer = 0;
                                    teleport = true;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            if (customAI[0] == 10)
                            {
                                for (int k = 0; k < 40; k++)
                                {
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    vector.X = (float)(Math.Sin(angle) * 100);
                                    vector.Y = (float)(Math.Cos(angle) * 100);
                                    Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.GrassBlades, 0f, 0f, 100, default, 3f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -NPC.DirectionTo(dust2.position) * 10f;

                                }
                            }
                            if (customAI[0] == 80)
                            {
                                int pieCut = 4;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    int projID = Projectile.NewProjectile(NPC.Center.X, NPC.Center.Y, 0, 0, ProjectileID.Stinger, 10, 3);
                                    Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(10f, 0f), m / (float)pieCut * 6.28f);
                                    Main.projectile[projID].netUpdate = true;
                                    Main.projectile[projID].timeLeft = 180;
                                }
                            }
                            if (customAI[0] == 120)
                            {
                                int pieCut = 8;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    int projID = Projectile.NewProjectile(NPC.Center.X, NPC.Center.Y, 0, 0, ProjectileID.Stinger, 10, 3);
                                    Main.projectile[projID].velocity = BaseUtility.RotateVector(default, new Vector2(10f, 0f), m / (float)pieCut * 6.28f);
                                    Main.projectile[projID].netUpdate = true;
                                    Main.projectile[projID].timeLeft = 180;
                                }
                            }
                            if (customAI[0] >= 180)
                            {
                                if (Main.rand.Next(3) == 0)
                                {
                                    NPC.netUpdate = true;
                                    customAI[0] = 0;
                                    choice = Main.rand.Next(5);
                                }
                                else
                                {
                                    customAI[0] = 0;
                                    customAI[1] = 4;
                                    teleportTimer = 0;
                                    teleport = true;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        break;
                    #endregion

                    // Life Drain
                    #region Life Drain
                    case 3:
                        if (NPC.life < (int)(NPC.lifeMax * 0.9f))
                        {
                            customAI[0]++;
                            if (NPC.life < (int)(NPC.lifeMax * 0.5f))
                            {
                                if (customAI[0] < 60)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                        vector.X = (float)(Math.Sin(angle) * 100);
                                        vector.Y = (float)(Math.Cos(angle) * 100);
                                        Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.LifeDrain, 0f, 0f, 100, default, 1f)];
                                        dust2.noGravity = true;
                                        dust2.velocity = -NPC.DirectionTo(dust2.position) * 10f;
                                    }
                                }
                                if (customAI[0] >= 60 && customAI[0] <= 140)
                                {
                                    if (Main.rand.Next(2) == 0)
                                    {
                                        float Speed = 9f;  //projectile speed
                                        Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                                        int damage = 10;  //projectile damage
                                        int type = ModContent.ProjectileType<LeechingThornSeed>();  //put your projectile
                                        float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                                        int num54 = Projectile.NewProjectile(vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1) + Main.rand.Next(-1, 1), (float)(Math.Sin(rotation) * Speed * -1) + Main.rand.Next(-1, 1), type, damage, 0f, 0, NPC.whoAmI);
                                        Main.projectile[num54].netUpdate = true;
                                    }
                                }
                                if (customAI[0] >= 240)
                                {
                                    if (Main.rand.Next(3) == 0)
                                    {
                                        NPC.netUpdate = true;
                                        customAI[0] = 0;
                                        choice = Main.rand.Next(5);
                                    }
                                    else
                                    {
                                        customAI[0] = 0;
                                        customAI[1] = 4;
                                        teleportTimer = 0;
                                        teleport = true;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            else
                            {
                                if (customAI[0] < 60)
                                {
                                    for (int k = 0; k < 3; k++)
                                    {
                                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                        vector.X = (float)(Math.Sin(angle) * 100);
                                        vector.Y = (float)(Math.Cos(angle) * 100);
                                        Dust dust2 = Main.dust[Dust.NewDust(NPC.Center + vector, 2, 2, DustID.LifeDrain, 0f, 0f, 100, default, 1f)];
                                        dust2.noGravity = true;
                                        dust2.velocity = -NPC.DirectionTo(dust2.position) * 10f;

                                    }
                                }
                                if (customAI[0] >= 60 && customAI[0] <= 90)
                                {
                                    if (Main.rand.Next(2) == 0)
                                    {
                                        float Speed = 11f;  //projectile speed
                                        Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                                        int damage = 10;  //projectile damage
                                        int type = ModContent.ProjectileType<LeechingThornSeed>();  //put your projectile
                                        float rotation = (float)Math.Atan2(vector8.Y - (player.position.Y + (player.height * 0.5f)), vector8.X - (player.position.X + (player.width * 0.5f)));
                                        int num54 = Projectile.NewProjectile(vector8.X, vector8.Y, (float)(Math.Cos(rotation) * Speed * -1) + Main.rand.Next(-1, 1), (float)(Math.Sin(rotation) * Speed * -1) + Main.rand.Next(-1, 1), type, damage, 0f, 0, NPC.whoAmI);
                                        Main.projectile[num54].netUpdate = true;
                                    }
                                }
                                if (customAI[0] >= 180)
                                {
                                    if (Main.rand.Next(3) == 0)
                                    {
                                        NPC.netUpdate = true;
                                        customAI[0] = 0;
                                        choice = Main.rand.Next(5);
                                    }
                                    else
                                    {
                                        customAI[0] = 0;
                                        customAI[1] = 4;
                                        teleportTimer = 0;
                                        teleport = true;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NPC.netUpdate = true;
                            customAI[0] = 0;
                            choice = Main.rand.Next(5);
                        }
                        break;
                    #endregion

                    // Cleave
                    #region Cleave
                    case 4:
                        customAI[0]++;
                        if (NPC.life < (int)(NPC.lifeMax * 0.5f))
                        {
                            if (customAI[0] == 60 || customAI[0] == 90 || customAI[0] == 120 || customAI[0] == 150 || customAI[0] == 180)
                            {
                                for (int index1 = 0; index1 < 8; ++index1)
                                {
                                    Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Sandnado, 0.0f, 0.0f, 100, new Color(), 2f);
                                    dust.velocity = -player.DirectionTo(dust.position) * 10;
                                    dust.noGravity = true;
                                }
                                for (int k = 0; k < 16; k++)
                                {
                                    double angle = k * (Math.PI * 2 / 16);
                                    vector.X = (float)(Math.Sin(angle) * 60);
                                    vector.Y = (float)(Math.Cos(angle) * 60);
                                    Dust dust2 = Main.dust[Dust.NewDust(player.Center + vector - new Vector2(4, 4), 1, 1, DustID.Sandnado, 0f, 0f, 100, default, 2f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -player.DirectionTo(dust2.position) * 10;
                                }
                                int ProjID = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<SlashFlashPro>(), 10, 3);
                                Main.projectile[ProjID].netUpdate = true;
                            }
                            if (customAI[0] >= 230)
                            {
                                if (Main.rand.Next(4) == 0)
                                {
                                    NPC.netUpdate = true;
                                    customAI[0] = 0;
                                    choice = Main.rand.Next(5);
                                }
                                else
                                {
                                    customAI[0] = 0;
                                    customAI[1] = 4;
                                    teleportTimer = 0;
                                    teleport = true;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            if (customAI[0] == 50 || customAI[0] == 100 || customAI[0] == 150 || customAI[0] == 200)
                            {
                                for (int index1 = 0; index1 < 8; ++index1)
                                {
                                    Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Sandnado, 0.0f, 0.0f, 100, new Color(), 2f);
                                    dust.velocity = -player.DirectionTo(dust.position) * 10;
                                    dust.noGravity = true;
                                }
                                for (int k = 0; k < 16; k++)
                                {
                                    double angle = k * (Math.PI * 2 / 16);
                                    vector.X = (float)(Math.Sin(angle) * 60);
                                    vector.Y = (float)(Math.Cos(angle) * 60);
                                    Dust dust2 = Main.dust[Dust.NewDust(player.Center + vector - new Vector2(4, 4), 1, 1, DustID.Sandnado, 0f, 0f, 100, default, 2f)];
                                    dust2.noGravity = true;
                                    dust2.velocity = -player.DirectionTo(dust2.position) * 10;
                                }
                                int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<SlashFlashPro>(), 10, 3);
                                Main.projectile[p].netUpdate = true;
                            }
                            if (customAI[0] >= 250)
                            {
                                if (Main.rand.Next(4) == 0)
                                {
                                    NPC.netUpdate = true;
                                    customAI[0] = 0;
                                    choice = Main.rand.Next(5);
                                }
                                else
                                {
                                    customAI[0] = 0;
                                    customAI[1] = 4;
                                    teleportTimer = 0;
                                    teleport = true;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        break;
                        #endregion
                }
            }
            /*if (Main.expertMode ? NPC.life < (int)(NPC.lifeMax * 0.5f) : NPC.life < (int)(NPC.lifeMax * 0.35f))
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<ManaBarrierPro>()))
                {
                    int dustType = 20;
                    int pieCut = 16;
                    for (int m = 0; m < pieCut; m++)
                    {
                        int dustID = Dust.NewDust(new Vector2(NPC.Center.X - 1, NPC.Center.Y - 1), 2, 2, dustType, 0f, 0f, 100, Color.White, 3f);
                        Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)pieCut * 6.28f);
                        Main.dust[dustID].noLight = false;
                        Main.dust[dustID].noGravity = true;
                    }
                    float distance = 2f;
                    float k = 0.4f;
                    for (int count = 0; count < 1; count++)
                    {
                        Vector2 spawn = NPC.Center + distance * (count * k).ToRotationVector2();
                        int Minion = NPC.NewNPC((int)spawn.X, (int)spawn.Y, ModContent.NPCType<ManaBarrierPro>(), 0, NPC.whoAmI, 0.0f, count, 0.0f, 255);
                        Main.NPC[Minion].netUpdate = true;
                    }
                }
            }*/
            // Teleporting
            if (teleport)
            {
                teleportTimer++;
                if (teleportTimer >= 2 && teleportTimer <= 62)
                {
                    disappearing = true;
                }
                if (teleportTimer == 62)
                {
                    if (NPC.ai[2] != 0f && NPC.ai[3] != 0f)
                    {
                        NPC.position.X = NPC.ai[2] * 16f - NPC.width / 2 + 8f;
                        NPC.position.Y = NPC.ai[3] * 16f - NPC.height;
                        NPC.velocity.X = 0f;
                        NPC.velocity.Y = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                    }
                    //NPC.ai[0] += 1f;

                    NPC.ai[0] = 1f;
                    int playerTilePositionX = (int)Main.player[NPC.target].position.X / 16;
                    int playerTilePositionY = (int)Main.player[NPC.target].position.Y / 16;
                    int npcTilePositionX = (int)NPC.position.X / 16;
                    int npcTilePositionY = (int)NPC.position.Y / 16;
                    int playerTargetShift = 16;
                    int num90 = 0;


                    for (int s = 0; s < 100; s++)
                    {
                        num90++;
                        int nearPlayerX = Main.rand.Next(playerTilePositionX - playerTargetShift, playerTilePositionX + playerTargetShift);
                        int nearPlayerY = Main.rand.Next(playerTilePositionY - playerTargetShift, playerTilePositionY + playerTargetShift);
                        for (int num93 = nearPlayerY; num93 < playerTilePositionY + playerTargetShift; num93++)
                        {
                            if ((nearPlayerX < playerTilePositionX - 12 || nearPlayerX > playerTilePositionX + 12) && (num93 < npcTilePositionY - 1 || num93 > npcTilePositionY + 1 || nearPlayerX < npcTilePositionX - 1 || nearPlayerX > npcTilePositionX + 1) && Main.tile[nearPlayerX, num93].IsActiveUnactuated)
                            {
                                bool flag5 = true;
                                if (Main.tile[nearPlayerX, num93 - 1].LiquidType == LiquidID.Lava)
                                {
                                    flag5 = false;
                                }
                                if (flag5 && Main.tileSolid[Main.tile[nearPlayerX, num93].type] && !Collision.SolidTiles(nearPlayerX - 1, nearPlayerX + 1, num93 - 4, num93 - 1))
                                {
                                    NPC.ai[1] = 20f;
                                    NPC.ai[2] = nearPlayerX;
                                    NPC.ai[3] = (float)num93 - 1;

                                    break;
                                }
                            }
                        }
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[1] > 0f)
                    {
                        NPC.ai[1] -= 1f;
                    }
                }
                if (teleportTimer >= 64 && teleportTimer <= 124)
                {
                    appearing = true;
                    disappearing = false;
                }
                if (teleportTimer > 124)
                {
                    appearing = false;
                    disappearing = false;
                    attacking = false;
                    teleport = false;
                    teleportTimer = 0;
                    if (customAI[3] != 0)
                    {
                        customAI[1] = 2;
                    }
                    else
                    {
                        customAI[1] = 1;
                    }
                    NPC.netUpdate = true;
                    attackFrame = 0;
                    appearFrame = 0;
                    disappearFrame = 0;
                    disappearCounter = 0;
                    appearCounter = 0;
                    attackCounter = 0;
                    NPC.netUpdate = true;
                }
            }
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (appearing || disappearing)
            {
                damage *= 0.4;
            }
            return true;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !appearing && !disappearing;
        }
        public static Texture2D glowTex = null;
        private bool beginFight;
        private bool teleport;
        private int teleportTimer;
        //private bool thorns;
        private bool attacking;
        private bool appearing;
        private bool disappearing;
        private int attackFrame;
        private int appearFrame;
        private int disappearFrame;
        //private bool whipVine;
        //private bool poison;
        //private bool lifeDrain;
        //private bool manaBarrier;
        //private bool cleave;
        private int attackCounter;
        private int appearCounter;
        private int disappearCounter;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type];
            Texture2D attackAni = mod.GetTexture("NPCs/Bosses/Thorn/ThornAttack1");
            Texture2D appearAni = mod.GetTexture("NPCs/Bosses/Thorn/ThornAppear");
            Texture2D disappearAni = mod.GetTexture("NPCs/Bosses/Thorn/ThornDisappear");
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (!attacking && !appearing && !disappearing)
            {
                spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            if (attacking && !appearing && !disappearing)
            {
                Vector2 drawCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                int num214 = attackAni.Height / 4;
                int y6 = num214 * attackFrame;
                Main.spriteBatch.Draw(attackAni, drawCenter - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, attackAni.Width, num214)), drawColor, NPC.rotation, new Vector2(attackAni.Width / 2f, num214 / 2f), NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            if (appearing && !disappearing)
            {
                Vector2 drawCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                int num214 = appearAni.Height / 12;
                int y6 = num214 * appearFrame;
                Main.spriteBatch.Draw(appearAni, drawCenter - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, appearAni.Width, num214)), drawColor, NPC.rotation, new Vector2(appearAni.Width / 2f, num214 / 2f), NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            if (!appearing && disappearing)
            {
                Vector2 drawCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                int num214 = disappearAni.Height / 11;
                int y6 = num214 * disappearFrame;
                Main.spriteBatch.Draw(disappearAni, drawCenter - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, disappearAni.Width, num214)), drawColor, NPC.rotation, new Vector2(disappearAni.Width / 2f, num214 / 2f), NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            return false;
        }

        private void Target()
        {
            player = Main.player[NPC.target]; // This will get the player target.
        }
        private void DespawnHandler()
        {
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.velocity = new Vector2(0f, -10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
            }
        }

    }
}
