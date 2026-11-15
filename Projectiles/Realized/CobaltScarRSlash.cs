using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace LobotomyCorp.Projectiles.Realized
{
    public class CobaltScarRSlash: ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Slash2A";

        public override void SetDefaults()
        {
            Projectile.height = 18;
            Projectile.width = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 8;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;

            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.Next(48, 96);
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            }
            if (Projectile.localAI[1] == 0)
            {
                 Projectile.localAI[1] = Main.rand.Next(1,5); //Determines a number between 1 and 4. Get 4 and it plays the sound. It gets too spammy otherwise.
            }
           
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            
            if(target.HasBuff(BuffID.Bleeding))
            {
                LobotomyGlobalNPC.CobaltScarApplyScar(target, 1, 10, 240, 480);

            }

            if(!target.HasBuff(BuffID.Bleeding))
            {
            LobotomyGlobalNPC.CobaltScarApplyScar(target, 1, 5, 120, 240); 
            }

            //Add a condition that checks if player has "CobaltScarWolf" ModBuff. If yes, apply bleeding.

            if(player.HasBuff<Buffs.CobaltScarWolf>()){
                target.AddBuff(BuffID.Bleeding, 240); //4 secs of bleeding

            }

            if (Projectile.localAI[1] >= 4)
            {
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Language/Wolf_Scratch") with { Volume = 1f }, Projectile.Center);
            }
            

        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = SpearExtender.SpearTrail;
            Rectangle frame = tex.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, frame.Height / 2);
            float prog = Projectile.timeLeft / 15f;
            Vector2 position = Projectile.Center - new Vector2((Projectile.localAI[0] * (1f - (float)Math.Sin(1.57f - (1f - prog) * 1.57f))) - (Projectile.localAI[0] / 2), 0).RotatedBy(Projectile.rotation);
            prog = (float)Math.Sin(prog * 3.14f);
            if (prog > 0.9f)
                prog = 0.9f;
            Vector2 scale = new Vector2(Projectile.localAI[0] * prog * 1.4f / frame.Width, 0.1f);
            for (int i = -1; i <= 1; i++)
            {
                Vector2 offset = new Vector2(0, 14 * i).RotatedBy(Projectile.rotation);
                Main.EntitySpriteDraw(tex, position + offset - Main.screenPosition, frame, Color.Red, Projectile.rotation - 3.14f, origin, scale, 0);
            }
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
    }
}