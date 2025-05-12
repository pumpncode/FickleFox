#if defined(VERTEX) || __VERSION__ > 100 || defined(GL_FRAGMENT_PRECISION_HIGH)
	#define MY_HIGHP_OR_MEDIUMP highp
#else
	#define MY_HIGHP_OR_MEDIUMP mediump
#endif

//watch shader Mods/FickleFox/assets/shaders/secretRare_4.fs
extern MY_HIGHP_OR_MEDIUMP vec2 secretRare;

extern MY_HIGHP_OR_MEDIUMP number dissolve;
extern MY_HIGHP_OR_MEDIUMP number time;
extern MY_HIGHP_OR_MEDIUMP vec4 texture_details;
extern MY_HIGHP_OR_MEDIUMP vec2 image_details;
extern bool shadow;
extern MY_HIGHP_OR_MEDIUMP vec4 burn_colour_1;
extern MY_HIGHP_OR_MEDIUMP vec4 burn_colour_2;

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

number hue(number s, number t, number h)
{
	number hs = mod(h, 1.)*6.;
	if (hs < 1.) return (t-s) * hs + s;
	if (hs < 3.) return t;
	if (hs < 4.) return (t-s) * (4.-hs) + s;
	return s;
}

vec4 RGB(vec4 c)
{
	if (c.y == 0.)
		return vec4(vec3(c.z), c.a);

	number t = (c.z < .5) ? c.y*c.z + c.z : -c.y*c.z + (c.y+c.z);
	number s = 2.0 * c.z - t;
	return vec4(hue(s,t,c.x + 1./3.), hue(s,t,c.x), hue(s,t,c.x - 1./3.), c.w);
}

vec4 HSL(vec4 c)
{
	number low = min(c.r, min(c.g, c.b));
	number high = max(c.r, max(c.g, c.b));
	number delta = high - low;
	number sum = high+low;

	vec4 hsl = vec4(.0, .0, .5 * sum, c.a);
	if (delta == .0)
		return hsl;

	hsl.y = (hsl.z < .5) ? delta / sum : delta / (2.0 - sum);

	if (high == c.r)
		hsl.x = (c.g - c.b) / delta;
	else if (high == c.g)
		hsl.x = (c.b - c.r) / delta + 2.0;
	else
		hsl.x = (c.r - c.g) / delta + 4.0;

	hsl.x = mod(hsl.x / 6., 1.);
	return hsl;
}



// **Hash function for procedural randomness**
vec2 hash(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)),
             dot(p, vec2(269.5, 183.3)));
    return fract(sin(p) * 43758.5453123);
}


vec3 hueToRGB(float h) {
    h = fract(h); // Keep hue within 0-1 range
    vec3 col = clamp(abs(mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    return col * col * (3.0 - 2.0 * col); // Slight contrast enhancement
}


//**Works and gorgeous! 
vec4 effect(vec4 colour, Image texture, vec2 texture_coords, vec2 screen_coords) {
    vec4 texColor = Texel(texture, texture_coords);
    vec2 uv = texture_coords * image_details.xy / image_details.y;

    texColor.rgb *= 0.7;
    if (texColor.a < 0.01) {
        return dissolve_mask(texColor, texture_coords, texture_coords);
    }

    float whiteness = texColor.r * texColor.g * texColor.b;
    // bool is_white = whiteness > 0.55;

    // //vec4 whiteBlend = 
    //  if (is_white) {
    //     float smoke_time = time * 0.3; // Slows the movement for a natural drift
    //     vec2 smoke_uv = uv * 3.0; // Scale noise effect
        
    //     // layered turbulence for smoky effect**
    //     float smoke1 = sin(smoke_uv.x * 3.0 + smoke_time) * 0.5;
    //     float smoke2 = cos(smoke_uv.y * 4.0 - smoke_time * 1.2) * 0.5;
    //     float smoke3 = sin(smoke_uv.x * 1.5 + smoke_uv.y * 1.2 + smoke_time * 0.8) * 0.5;

    //     float smoke_pattern = (smoke1 + smoke2 + smoke3) * 0.3 + 0.5;
    
    //     return vec4(vec3(1.0 - smoke_pattern * 0.3), 1.0); // Soft black smoke over white
    // }    

    float density = 350.0;  

    vec2 gridPos = floor(uv * density);
    vec2 randOffset = hash(gridPos);
    vec2 dustUV = uv - (gridPos + randOffset) / density;

    // Increase Mote Size in Small Areas
    float dustMote = smoothstep(0.8, 0.05, length(dustUV) * density); // Increase smoothstep range

    // Flicker - does not work
    //float sparkle = dustMote * (sin(time * 1.3 + dot(gridPos, vec2(3.1, 4.2))) * 0.5 + 0.5);

    float highlight = abs(sin(time * 3.0 + uv.x * 15.0)); 
    float specular = pow(highlight, 12.0) * 0.5; // gold seal approach?

    float sparkle = dustMote * (sin(time * 1.3 + dot(gridPos, vec2(3.1, 4.2))) * 0.5 + 0.5) + specular;

    float minBrightness = 0.05;
    float hue = fract(dot(gridPos, vec2(0.7, 0.5)) + time * 0.5);
    vec3 sparkleColor = hueToRGB(hue) * sparkle;
    sparkleColor = mix(vec3(minBrightness), sparkleColor, sparkle);

    vec4 finalColor = texColor;
    finalColor.rgb += sparkleColor;
    finalColor.a = max(sparkle, texColor.a);

    // Do not remove!  some weird shader optimization happens here
    finalColor.rgb += secretRare.x * 0.0;

    return dissolve_mask(finalColor * colour, texture_coords, texture_coords);
}


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