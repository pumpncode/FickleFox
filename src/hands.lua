sendInfoMessage("Processing hands", "hands.lua")
FoxMod.PokerHandDummies = {}

FoxMod.PokerHandDummy = SMODS.Center:extend {
    set = 'FoxPokerHand',
    obj_buffer = {},
    obj_table = FoxMod.PokerHandDummies,
    class_prefix = 'phd',
    required_params = {
        'key',
    },
    pre_inject_class = function(self)
        G.P_CENTER_POOLS[self.set] = {}
    end,
    inject = function(self)
        SMODS.Center.inject(self)
    end,
    get_obj = function(self, key)
        if key == nil then
            return nil
        end
        return self.obj_table[key]
    end
}

SMODS.PokerHand({
    key = "cosmocanyon",
    chips = 40,
    mult = 5,
    l_chips = 15,
    l_mult = 2,
    visible = false,
    example = {        
        { 'S_3', true},
        { 'H_8', true},
        { 'C_2', true},
    },
    loc_txt = {
        name = "Cosmo Canyon",
        text = {
            "3 or more cards divisble by {C:attention}XIII{}",
        },
        description = {
            "3 or more cards divisble by XIII",
            "The ancestral home of Nanaki and those like him"
        }
    },
    evaluate = function(parts, hand)
        if #hand > 2 then -- TODO - find redxiii joker to trigger bonus and next(SMODS.find_card("fox_redxiii")) then         
        local counted = 0
            for _, card in ipairs(hand) do
                counted = counted + card.base.id
            end
            sendInfoMessage("Hand was played, checking value " .. counted, "cosmo")
            if counted % 13 == 0 then
                return { hand }
            end
        end
        return {}
    end
})

FoxMod.PokerHandDummy {
    key = "cosmocanyon",
    generate_ui = function(self, info_queue, card, desc_nodes, specific_vars, full_UI_table)
        SMODS.Center.generate_ui(self, info_queue, card, desc_nodes, specific_vars, full_UI_table)
        if desc_nodes ~= full_UI_table.main then
            local cards = {}
            local card_w = G.CARD_W * 0.6
            local card_h = G.CARD_H * 0.6            
            table.insert(cards, Card(0, 0, card_w, card_h, G.P_CARDS['S_3'], G.P_CENTERS.m_gold))
            table.insert(cards, Card(0, 0, card_w, card_h, G.P_CARDS['H_8'], G.P_CENTERS.m_steel))
            table.insert(cards, Card(0, 0, card_w, card_h, G.P_CARDS['C_2']))
            G.joy_dummy_area = CardArea(
                0, 0,
                4.25 * card_w,
                0.95 * card_h,
                { card_limit = 5, type = 'title', highlight_limit = 0, collection = true }
            )

            for i, p_card in ipairs(cards) do
                G.joy_dummy_area:emplace(p_card)
            end

            desc_nodes[#desc_nodes + 1] = {
                {
                    n = G.UIT.B,
                    config = { w = 0, h = 0.1 },
                },
            }

            desc_nodes[#desc_nodes + 1] = {
                {
                    n = G.UIT.R,
                    config = { align = "cm", padding = 0.07, no_fill = true },
                    nodes = {
                        { n = G.UIT.O, config = { object = G.joy_dummy_area } }
                    }
                },
            }
        end
    end,
}

sendInfoMessage("Completed processing hands", "hands.lua")