#if defined(VERTEX) || __VERSION__ > 100 || defined(GL_FRAGMENT_PRECISION_HIGH)
	#define MY_HIGHP_OR_MEDIUMP highp
#else
	#define MY_HIGHP_OR_MEDIUMP mediump
#endif

//watch shader Mods/FoxMods/assets/shaders/old/blueRare.fs
extern MY_HIGHP_OR_MEDIUMP vec2 spectralShift; // Uses x or y to prevent optimization
extern MY_HIGHP_OR_MEDIUMP number time;
extern MY_HIGHP_OR_MEDIUMP vec4 texture_details;
extern MY_HIGHP_OR_MEDIUMP vec2 image_details;

extern MY_HIGHP_OR_MEDIUMP number dissolve;
extern MY_HIGHP_OR_MEDIUMP vec4 burn_colour_1;
extern MY_HIGHP_OR_MEDIUMP vec4 burn_colour_2;
extern bool shadow;


vec4 dissolve_mask(vec4 tex, vec2 texture_coords, vec2 uv)
{
    if (dissolve < 0.001) {
        return vec4(shadow ? vec3(0.,0.,0.) : tex.xyz, shadow ? tex.a*0.3: tex.a);
    }

    float adjusted_dissolve = (dissolve*dissolve*(3.-2.*dissolve))*1.02 - 0.01; //Adjusting 0.0-1.0 to fall to -0.1 - 1.1 scale so the mask does not pause at extreme values

	float t = time * 10.0 + 2003.;
	vec2 floored_uv = (floor((uv*texture_details.ba)))/max(texture_details.b, texture_details.a);
    vec2 uv_scaled_centered = (floored_uv - 0.5) * 2.3 * max(texture_details.b, texture_details.a);

	vec2 field_part1 = uv_scaled_centered + 50.*vec2(sin(-t / 143.6340), cos(-t / 99.4324));
	vec2 field_part2 = uv_scaled_centered + 50.*vec2(cos( t / 53.1532),  cos( t / 61.4532));
	vec2 field_part3 = uv_scaled_centered + 50.*vec2(sin(-t / 87.53218), sin(-t / 49.0000));

    float field = (1.+ (
        cos(length(field_part1) / 19.483) + sin(length(field_part2) / 33.155) * cos(field_part2.y / 15.73) +
        cos(length(field_part3) / 27.193) * sin(field_part3.x / 21.92) ))/2.;
    vec2 borders = vec2(0.2, 0.8);

    float res = (.5 + .5* cos( (adjusted_dissolve) / 82.612 + ( field + -.5 ) *3.14))
    - (floored_uv.x > borders.y ? (floored_uv.x - borders.y)*(5. + 5.*dissolve) : 0.)*(dissolve)
    - (floored_uv.y > borders.y ? (floored_uv.y - borders.y)*(5. + 5.*dissolve) : 0.)*(dissolve)
    - (floored_uv.x < borders.x ? (borders.x - floored_uv.x)*(5. + 5.*dissolve) : 0.)*(dissolve)
    - (floored_uv.y < borders.x ? (borders.x - floored_uv.y)*(5. + 5.*dissolve) : 0.)*(dissolve);

    if (tex.a > 0.01 && burn_colour_1.a > 0.01 && !shadow && res < adjusted_dissolve + 0.8*(0.5-abs(adjusted_dissolve-0.5)) && res > adjusted_dissolve) {
        if (!shadow && res < adjusted_dissolve + 0.5*(0.5-abs(adjusted_dissolve-0.5)) && res > adjusted_dissolve) {
            tex.rgba = burn_colour_1.rgba;
        } else if (burn_colour_2.a > 0.01) {
            tex.rgba = burn_colour_2.rgba;
        }
    }

    return vec4(shadow ? vec3(0.,0.,0.) : tex.xyz, res > adjusted_dissolve ? (shadow ? tex.a*0.3: tex.a) : .0);
}

// **Convert RGB to HSL**
vec4 RGBtoHSL(vec4 color) {
    float minC = min(color.r, min(color.g, color.b));
    float maxC = max(color.r, max(color.g, color.b));
    float delta = maxC - minC;

    vec4 hsl = vec4(0.6, 0.0, (maxC + minC) * 0.5, color.a); // Default hue = blueish 0.6

    if (delta == 0.0) return hsl; // No chroma, return default blue hue

    hsl.y = (hsl.z < 0.5) ? (delta / (maxC + minC)) : (delta / (2.0 - maxC - minC));

    if (maxC == color.r) {
        hsl.x = (color.g - color.b) / delta + (color.g < color.b ? 6.0 : 0.0);
    } else if (maxC == color.g) {
        hsl.x = (color.b - color.r) / delta + 2.0;
    } else {
        hsl.x = (color.r - color.g) / delta + 4.0;
    }

    hsl.x /= 6.0;
    return hsl;
}

  float hueToRGB(float p, float q, float t) {
        if (t < 0.0) t += 1.0;
        if (t > 1.0) t -= 1.0;
        if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
        return p;
    }

// **Convert HSL back to RGB**
vec4 HSLtoRGB(vec4 hsl) {
    float q = (hsl.z < 0.5) ? hsl.z * (1.0 + hsl.y) : hsl.z + hsl.y - hsl.z * hsl.y;
    float p = 2.0 * hsl.z - q;



    return vec4(
        hueToRGB(p, q, hsl.x + 1.0 / 3.0),
        hueToRGB(p, q, hsl.x),
        hueToRGB(p, q, hsl.x - 1.0 / 3.0),
        hsl.a
    );
}

// **Noise function for subtle foggy glow**
float noise(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}


// Water parameters
const float waterLevel = 2.0 / 3.0; // Lower third
const float wave_speed = 0.05;
const float wave_distortion = 0.2;
const int wave_multiplier = 7;
const vec4 water_albedo = vec4(0.26, 0.23, 0.73, 1.0);
const float water_opacity = 0.35;
const float reflection_X_offset = 0.0;
const float reflection_Y_offset = 0.0;

// **Mysterious fog effect**
vec4 generateFog(vec2 uv) {
    float t = time * 0.01;
    float fogPattern = noise(uv * 20.0 + vec2(t, t)) * 0.5;
    fogPattern += noise(uv * 40.0 - vec2(t, -t)) * 0.3;
    fogPattern = pow(fogPattern, 1.7); // Adjust softness

    return vec4(vec3(0.5, 0.6, 0.8) * fogPattern, fogPattern * 0.7); // Soft blue-gray fog
}

bool isLowerThird(vec2 texture_coords) {
    return texture_coords.y >= (2.0 / 3.0); // If Y is in the bottom 1/3 of the texture
}


// **Main Shader Effect**
vec4 effect(vec4 colour, Image texture, vec2 texture_coords, vec2 screen_coords) {
    
    vec4 texColor = Texel(texture, texture_coords);    
         vec2 uv = texture_coords * image_details.xy / image_details.y;

    if (texColor.a < 0.01) {
        return dissolve_mask(texColor, texture_coords, texture_coords);
    }
    // vec4 texColor = Texel(texture, texture_coords); // Get original pixel color

    // **Get texture size in pixels (Love2D always provides this)**
    vec2 texSize = image_details.xy; // Texture width and height
    float borderSize = 5.0; // Change this to adjust border thickness
    borderSize += spectralShift.x * 0.02;
    // **Detect if we are near an edge (within `borderSize` pixels)**
    bool isBorder = texture_coords.x * texSize.x < borderSize ||
                    texture_coords.y * texSize.y < borderSize ||
                    ((1.0 - texture_coords.x) * texSize.x > texSize.x - borderSize) ||
                    ((1.0 - texture_coords.y) * texSize.y > texSize.y - borderSize);

    if (isBorder){
        vec4 finalColor = isBorder ? vec4(1.0, 1.0, 0.0, texColor.a) : texColor;

        return finalColor * colour; // Output final color
        return dissolve_mask(finalColor * colour, texture_coords, texture_coords);
    }

        // **Convert to HSL for color shifting**
    vec4 hsl = RGBtoHSL(texColor);

    if (hsl.y > 0.1) {
        // **Shift towards deep blues and dilute the colors**
        hsl.x = mix(hsl.x, 0.6, 0.5); // Move hues toward blue (0.6 in HSL)
        hsl.y *= 0.2; // Lower saturation to "wash out" colors
        hsl.z = mix(hsl.z, 0.2, 0.4); // Darken slightly for a moody effect
        texColor = HSLtoRGB(hsl);
    } else {
        // **Replace whites with subtle fog**
        texColor = generateFog(texture_coords * image_details.xy);
    }

    

   
    // **Check if we are in the lower third**
    if (texture_coords.y >= waterLevel) {
        // **Wave distortion effect**
        vec4 noise_texture = generateFog(texture_coords * image_details.xy);

        vec2 water_uv = vec2(uv.x, uv.y * float(wave_multiplier));
        float noise = texture(noise_texture, vec2(water_uv.x + time * wave_speed, water_uv.y)).x * wave_distortion;
        noise -= (0.5 * wave_distortion); // Center it around 0

        // **Apply water texture**
        float water_texture_limit = 0.35;
        vec4 water_texture = texture(noise_texture2, uv * vec2(0.5, 4.0) + vec2(noise, 0.0));
        float water_texture_value = (water_texture.x < water_texture_limit) ? 1.0 : 0.0;

        // **Distorted reflection**
        vec2 reflectionUV = vec2(texture_coords.x + noise + reflection_X_offset, 1.0 - texture_coords.y - (waterLevel - 0.5) * 2.0 + reflection_Y_offset);
        vec4 reflectionColor = Texel(texture, reflectionUV);

        // **Blend the water texture & reflection into the original**
        vec4 waterEffect = mix(vec4(vec3(water_texture_value), 1.0), reflectionColor, 0.5);
        texColor = mix(texColor, waterEffect, water_opacity);
    }

    // **Prevent Optimization Removal**
    texColor.rgb += spectralShift.x * 0.0;

    return dissolve_mask(texColor * colour, texture_coords, texture_coords);


    // return finalColor * colour; // Output final color
    //     return dissolve_mask(finalColor * colour, texture_coords, texture_coords);

}

//     // **Convert to HSL for color shifting**
//     vec4 hsl = RGBtoHSL(texColor);

//     if (hsl.y > 0.1) {
//         // **Shift towards deep blues and dilute the colors**
//         hsl.x = mix(hsl.x, 0.6, 0.5); // Move hues toward blue (0.6 in HSL)
//         hsl.y *= 0.2; // Lower saturation to "wash out" colors
//         hsl.z = mix(hsl.z, 0.2, 0.4); // Darken slightly for a moody effect
//         texColor = HSLtoRGB(hsl);
//     } else {
//         // **Replace whites with subtle fog**
//         texColor = generateFog(texture_coords * image_details.xy);
//     }

//     // **Prevent Optimization Removal**
//     texColor.rgb += spectralShift.x * 0.0;

//     return dissolve_mask(texColor * colour, texture_coords, texture_coords);
// }





extern MY_HIGHP_OR_MEDIUMP vec2 mouse_screen_pos;
extern MY_HIGHP_OR_MEDIUMP float hovering;
extern MY_HIGHP_OR_MEDIUMP float screen_scale;

#ifdef VERTEX
vec4 position( mat4 transform_projection, vec4 vertex_position )
{
    if (hovering <= 0.){
        return transform_projection * vertex_position;
    }
    float mid_dist = length(vertex_position.xy - 0.5*love_ScreenSize.xy)/length(love_ScreenSize.xy);
    vec2 mouse_offset = (vertex_position.xy - mouse_screen_pos.xy)/screen_scale;
    float scale = 0.2*(-0.03 - 0.3*max(0., 0.3-mid_dist))
                *hovering*(length(mouse_offset)*length(mouse_offset))/(2. -mid_dist);

    return transform_projection * vertex_position + vec4(0,0,0,scale);
}
#endif