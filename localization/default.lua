
sendInfoMessage("Registered localization info, checking value ", "default.luam")

return {
	descriptions = {		
		Joker = {						
			--#endregion
		},
		FoxPokerHand = {
			phd_fox_cosmocanyon = {
				name = "Cosmo Canyon",
				text = {
					"3 or more cards divisble by {C:attention}XIII{}",
				},
			},
			phd_fox_fullmoon = {
				name = "Full Moon",
				text = {
					"3 or more cards divisble by {C:attention}XVI{}",
				},
			},
		}
	},
	misc = {
		poker_hands = {
			joy_eldlixir = "Eldlixir",
			fox_cosmocanyon = "Cosmo Canyon",
			fox_fullmoon = "Full Moon",
			fox_shungoku = "Shun Goku Satsu"
		},
		poker_hand_descriptions = {
			["Cosmo Canyon"] =  {
				"3 or more cards divisble by {C:attention}XIII{}",
			},
			["Full Moon"] =  {
				"3 or more cards divisble by {C:attention}XVI{}",
			},
			["Shun Goku Satsu"] =  {
				"4 or more cards of rank 10",
			}
		},
		dictionary = {
			RarityCollection = "Spice up your game with special editions",
			RarityMegaCollection = "Two is better than one",
			FickleFoxBooster = "Try some FickleFox exclusive jokers!",
			NegationPack = "After all, why not take one",
			StandardRarity = "Spiffed up standard cards!",
			hologramPACK = "Oops all shineys",
		},
	}
}
