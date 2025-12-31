//contains scratch functions used before

//**Works and gorgeous! 
// vec4 effect(vec4 colour, Image texture, vec2 texture_coords, vec2 screen_coords) {
//     vec4 texColor = Texel(texture, texture_coords);
//     vec2 uv = texture_coords * image_details.xy / image_details.y;
// 
//     if (texColor.a < 0.01) {
//         return dissolve_mask(texColor, texture_coords, texture_coords);
//     }

//     float whiteness = texColor.r * texColor.g * texColor.b;
//     // bool is_white = whiteness > 0.55;

//     // //vec4 whiteBlend = 
//     //  if (is_white) {
//     //     float smoke_time = time * 0.3; 
//     //     vec2 smoke_uv = uv * 3.0;
        
//     //     //     //     float smoke1 = sin(smoke_uv.x * 3.0 + smoke_time) * 0.5;
//     //     float smoke2 = cos(smoke_uv.y * 4.0 - smoke_time * 1.2) * 0.5;
//     //     float smoke3 = sin(smoke_uv.x * 1.5 + smoke_uv.y * 1.2 + smoke_time * 0.8) * 0.5;
//     //     
//     //     float smoke_pattern = (smoke1 + smoke2 + smoke3) * 0.3 + 0.5;
//     //     return vec4(vec3(1.0 - smoke_pattern * 0.3), 1.0); // Soft black smoke over white
//     // }    

//   
//     float density = 350.0;  // Was 40.0 

//     vec2 gridPos = floor(uv * density);
//     vec2 randOffset = hash(gridPos);
//     vec2 dustUV = uv - (gridPos + randOffset) / density;

//     float dustMote = smoothstep(0.8, 0.05, length(dustUV) * density); // Increase smoothstep range

//     //float sparkle = dustMote * (sin(time * 1.3 + dot(gridPos, vec2(3.1, 4.2))) * 0.5 + 0.5);

//     float highlight = abs(sin(time * 3.0 + uv.x * 15.0)); 
//     float specular = pow(highlight, 12.0) * 0.5; // Sharper highlight for "glinting" effect

//     float sparkle = dustMote * (sin(time * 1.3 + dot(gridPos, vec2(3.1, 4.2))) * 0.5 + 0.5) + specular;

//     // **Ensure Motes Appear Even on Bright Backgrounds**
//     float minBrightness = 0.05;
//     float hue = fract(dot(gridPos, vec2(0.7, 0.5)) + time * 0.5);
//     vec3 sparkleColor = hueToRGB(hue) * sparkle;
//     sparkleColor = mix(vec3(minBrightness), sparkleColor, sparkle);

//     vec4 finalColor = texColor;
//     finalColor.rgb += sparkleColor;
//     finalColor.a = max(sparkle, texColor.a);

//     finalColor.rgb += secretRare.x * 0.0;

//     return dissolve_mask(finalColor * colour, texture_coords, texture_coords);
// }




// //works!  but want to  to try and improve it by adding an effect to the white area
// vec4 effect(vec4 colour, Image texture, vec2 texture_coords, vec2 screen_coords)
// {
//     vec4 tex = Texel(texture, texture_coords);
// 	vec2 uv = (((texture_coords)*(image_details)) - texture_details.xy*texture_details.ba)/texture_details.ba;

//     // exit earlier for white
//     float whiteness = tex.r * tex.g * tex.b;  
//     if (whiteness > 0.85) {  
//         return tex * colour;  
//     }


// 	number low = min(tex.r, min(tex.g, tex.b));
//     number high = max(tex.r, max(tex.g, tex.b));
// 	number delta = high - low;
// 	number saturation_fac = 1. - max(0., 0.05*(1.1-delta));
// 	vec4 hsl = HSL(vec4(tex.r * saturation_fac, tex.g * saturation_fac, tex.b, tex.a));

// 	float t = glimmer.y * 2.221 + time;
// 	vec2 floored_uv = (floor((uv * texture_details.ba))) / texture_details.ba;
//     vec2 uv_scaled_centered = (floored_uv - 0.5) * 50.;

// 	vec2 field_part1 = uv_scaled_centered + 50. * vec2(sin(-t / 143.6340), cos(-t / 99.4324));
// 	vec2 field_part2 = uv_scaled_centered + 50. * vec2(cos(t / 53.1532), cos(t / 61.4532));
// 	vec2 field_part3 = uv_scaled_centered + 50. * vec2(sin(-t / 87.53218), sin(-t / 49.0000));

//     float field = (1.+ (
//         cos(length(field_part1) / 19.483) + sin(length(field_part2) / 33.155) * cos(field_part2.y / 15.73) +
//         cos(length(field_part3) / 27.193) * sin(field_part3.x / 21.92) ))/2.;

//     float res = (.5 + .5* cos((glimmer.x) * 2.612 + (field + -.5) * 3.14));

//     // made it animate but was rgb again, lol!
//     // float golden_shift = 0.08 + 0.07 * sin(time * 0.5 + field * 6.283);
//     // hsl.x = hsl.x + golden_shift + glimmer.y*0.04;

//     //new approach
//     float golden_shift = 0.08 + 0.07 * sin(time * 0.5 + field * 6.283);
//     hsl.x = mix(0.08, 0.15, fract(hsl.x + golden_shift + glimmer.y * 0.04));

// 	hsl.y = min(0.6, hsl.y + 0.5);  // Slightly boost saturation
    
//     tex.rgb = RGB(hsl).rgb;

    
// 	if (tex[3] < 0.7)
// 		tex[3] = tex[3] / 3.;

//     return dissolve_mask(tex * colour, texture_coords, uv);
// }
