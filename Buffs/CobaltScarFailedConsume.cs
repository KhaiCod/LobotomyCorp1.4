using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Buffs
{//While having this debuff set WolfSated to 0. 
//Could make the player endurance be set player.endurance -= 0.15f
	public class CobaltScarFailedConsume : ModBuff
	{

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Role of The Wolf:Disabled");
			// Description.SetDefault("Ouch, I deserved that.");
			Main.debuff[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			player.endurance -= 0.15f; //Adjust this as needed.
				for (int i = 0; i < 6; i++)
				{
					Vector2 vel = new Vector2((8*(float)player.direction), 0).RotatedBy(Main.rand.NextFloat(-0.39f,0.39f));
					Dust d = Dust.NewDustPerfect(player.Center, DustID.t_Flesh, vel); //replace it with brighter red.
					d.noGravity = false;
					//d.fadeIn = 1f;
				}
		}
    }

}