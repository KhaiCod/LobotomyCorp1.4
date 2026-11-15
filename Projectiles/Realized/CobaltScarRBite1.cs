using LobotomyCorp.Items;
using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using static LobotomyCorp.LobotomyCorp;
using static Terraria.Player;
using LobotomyCorp.Misc.Dusts;

namespace LobotomyCorp.Projectiles.Realized
{
    public class CobaltScarRBite1: ModProjectile  //or the windup that exists just to do nothing on it's own other then summon 2 projectiles, with one actually doing something.
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/CobaltScarRBiteSpriteSheet"; 
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13; //13 different frames. 0-12, 10-12 is when you hit.
        }
        public override void SetDefaults()
        {
            Projectile.height = 200;//Hitbox is 48 by 12*16, or 192. The image is a lot larger then the planned hitbox.
            Projectile.width = 240;//back to the drawing board..........
            Projectile.aiStyle = -1; //I want this to be anchored to the player. Flip the image every time it switches between pointing left and right.
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 24;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;

        
        }

        public override void AI()
        {
            
            Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;
            Projectile.alpha = Projectile.alpha;
            float rotation = Projectile.velocity.ToRotation();
            float radius = 7f*16f+((float)Projectile.localAI[0]*1.05f);//take horizonal size of the sheet, add about 28 pixels, then divide by 32 to get 8.65f.
            //float bitedustradius = 11f*16f+((float)Projectile.localAI[0]*1.05f);//jaw center is about 3 blocks out.
            //This is kinda longer then i planned, but whatever. 16 tiles.
            

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] == 1)//Bite sfx
            {
               SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Language/Wolf_Bite") with { Volume = 1f }, Projectile.Center);

            }



            if (Projectile.localAI[0] == 3)//Bite starts at tick 3-4
            {
               Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CobaltScarRBiteHitbox>(), 1, Projectile.knockBack, Projectile.owner, 0, 0, 0);
            }

            if (Projectile.localAI[0] == 5)
            {
                 Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CobaltScarRChompHitbox>(), Projectile.damage * 8, Projectile.knockBack, Projectile.owner, 0, 0, 0);

            }

            if (Projectile.alpha < 0) {
				Projectile.alpha = 0;
			}
            if (Projectile.alpha > 255) {
				Projectile.alpha = 255;
			}

            if (Main.myPlayer == Projectile.owner)
                {
                    float targetRotation = (Main.MouseWorld - owner.Center).ToRotation(); 
                    rotation = Terraria.Utils.AngleLerp(rotation, targetRotation, .2f);
                    Projectile.velocity = new Vector2(1, 0).RotatedBy(rotation);
                }

            Projectile.Center = owner.Center + Projectile.velocity * radius;
           
            LobCorpLight.LobItemFrame(owner, rotation, Projectile.spriteDirection);
            Projectile.rotation = rotation;
            Projectile.spriteDirection = owner.direction; 

            Projectile.frameCounter++;
            
            

            if (Projectile.frame < 2)//skip to the bite
            {
                Projectile.frameCounter = 0;
                Projectile.frame = 2;
                Projectile.alpha = 155;

            }
            if ((Projectile.frame == 7 || Projectile.frame == 8 || Projectile.frame == 10 || Projectile.frame == 11 ) ) //The ending sequences get 4 frames each so it's actually kinda easy to see expressions.
            {
                if(Projectile.frameCounter >= 4)
                {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 13; //8
                }

                if(Projectile.frame == 10||Projectile.frame == 11)
                {
                    if(Projectile.alpha >= 102)
                    {
                        Projectile.alpha = 102;
                    }

                    
                }
                else
                {
                    Projectile.alpha += 13;
                }
                Projectile.alpha += 6;
            }

          if (Projectile.frame == 3 || Projectile.frame == 4 || Projectile.frame ==5 ||  Projectile.frame ==6 ){ //make some frames longer or shorter. 0,9 and 12 are longer while 1 is shorter.
             if (Projectile.frameCounter >= 2) { //how long a normal frame plays. The bite (of 87) is especially fast to indicate speed.
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 13; //8

                for (int i = 0; i < 7; i++)
                {
                    float DustTargetRotation = (Main.MouseWorld - owner.Center).ToRotation(); 
                    rotation = Terraria.Utils.AngleLerp(rotation, DustTargetRotation, .2f);
                    Vector2 vel2 = new Vector2(1, 0).RotatedBy(rotation);

                    
                    Vector2 velrandom = new Vector2(Main.rand.Next(6,125), Main.rand.Next(-33,33));
                    Vector2 dustRotation =  velrandom.RotatedBy(Projectile.rotation);

					//Vector2 vel = owner.Center + vel2 * bitedustradius;
					Dust d = Dust.NewDustPerfect((Projectile.Center + dustRotation), 96);

					d.noGravity = true;

					d.fadeIn = 1f;

                    d.velocity = vel2;

                }
             } 
             Projectile.alpha -= 20;
            }

            if (owner.HasBuff<Buffs.CobaltScarConsuming>() ||owner.HasBuff<Buffs.CobaltScarRecentBite>() && Projectile.frame == 7) //On the 8th frame, check for this shit.
            {
                Projectile.frame = Projectile.frame + 3; //move to 10-12 if true.
            }

            if (Projectile.frame == 9 )
            {
                Projectile.alpha += 20;
            }

            if (Projectile.frame == 12 )
            {
                Projectile.alpha += 13; 
                //it will not loop. 5 frames long, the image might wind up being still.

            }


            if (Projectile.frame == 2 ) {
                Projectile.alpha -= 20;
                if(Projectile.frameCounter >= 3)
                {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 13; //3 } 
                // frame 2(or the 3rd frame)
             } 
             }


           

        
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false; //Purely Visual. It will NOT hit NPCs.
        }


public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            float rotation = Projectile.rotation;
            SpriteEffects sp = 0;
            Color color = Projectile.GetAlpha(lightColor);
            
            if (Projectile.spriteDirection == -1)
            {
                sp = SpriteEffects.FlipHorizontally;
                rotation += 3.14f;
            }
            
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(1, Main.projFrames[Type], 0, Projectile.frame), color, rotation, frame.Size() / 2f, Projectile.scale, sp); 

//tex.Frame(horizontal, vertical, Starting horizontal frame, Starting vertical frame)
            return false;
        }


        






    }
}


//A lot of this code needs like, shit for me to figure out.