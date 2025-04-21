#if defined(VERTEX) || __VERSION__ > 100 || defined(GL_FRAGMENT_PRECISION_HIGH)
	#define MY_HIGHP_OR_MEDIUMP highp
#else
	#define MY_HIGHP_OR_MEDIUMP mediump
#endif

//watch shader Mods/FoxMods/assets/shaders/ghostRare.fs
extern MY_HIGHP_OR_MEDIUMP vec2 ghostRare;
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
	if (c.y < 0.0001)
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

vec4 effect(vec4 colour, Image texture, vec2 texture_coords, vec2 screen_coords)
{
    vec4 tex = Texel(texture, texture_coords);
	vec2 uv = (((texture_coords) * image_details) - texture_details.xy * texture_details.ba) / texture_details.ba;

    float whiteness = tex.r * tex.g * tex.b;
    bool is_white = whiteness > 0.85;

    number low = min(tex.r, min(tex.g, tex.b));
    number high = max(tex.r, max(tex.g, tex.b));
	number delta = high - low - 0.1;

    // Voronoi-style smoke for white regions
    if (is_white) {
        vec2 smoke_uv = uv * 20.0;
        float t = time * 12.5;

        float noise = (
            sin(smoke_uv.x + t) +
            cos(smoke_uv.y * 1.3 - t * 0.6) +
            sin(smoke_uv.x * 0.8 + smoke_uv.y * 0.6 + t * 0.7) +
            cos((smoke_uv.x + smoke_uv.y) * 1.5 - t * 0.9)
        ) * 0.25 + 0.5;

        float softness = smoothstep(0.1, 0.9, noise);
        return dissolve_mask(vec4(vec3(softness * 0.6), 1.0), texture_coords, uv);
    }

    // Faded color logic
    number saturation_fac = 1.0 - max(0.0, 0.05 * (1.1 - delta));
    vec4 hsl = HSL(vec4(tex.r * saturation_fac, tex.g * saturation_fac, tex.b, tex.a));

    float t = ghostRare.y * 2.221 + time;
    vec2 floored_uv = (floor((uv * texture_details.ba))) / texture_details.ba;
    vec2 uv_scaled_centered = (floored_uv - 0.5) * 50.0;

    vec2 field_part1 = uv_scaled_centered + 50.0 * vec2(sin(-t / 143.63), cos(-t / 99.43));
    vec2 field_part2 = uv_scaled_centered + 50.0 * vec2(cos(t / 53.15), cos(t / 61.45));
    vec2 field_part3 = uv_scaled_centered + 50.0 * vec2(sin(-t / 87.53), sin(-t / 49.00));

    float field = (1.0 + (
        cos(length(field_part1) / 19.48) +
        sin(length(field_part2) / 33.15) * cos(field_part2.y / 15.73) +
        cos(length(field_part3) / 27.19) * sin(field_part3.x / 21.92)
    )) / 2.0;

    float res = 0.5 + 0.5 * cos((ghostRare.x) * 2.612 + (field - 0.5) * 3.14);

    // hue toward bluish greys
    float hueShift = 0.58 + 0.02 * sin(time * 0.5 + field * 6.283); 
    hsl.x = mix(hsl.x, hueShift, 0.9); 

    // Shimmering Shine Boost from negative shine
    float fac = 0.8 + 0.9 * sin(11. * uv.x + 4.32 * uv.y + ghostRare.r * 12. + cos(ghostRare.r * 5.3 + uv.y * 4.2 - uv.x * 4.));
    float fac2 = 0.5 + 0.5 * sin(8. * uv.x + 2.32 * uv.y + ghostRare.r * 5. - cos(ghostRare.r * 2.3 + uv.x * 8.2));
    float fac3 = 0.5 + 0.5 * sin(10. * uv.x + 5.32 * uv.y + ghostRare.r * 6.111 + sin(ghostRare.r * 5.3 + uv.y * 3.2));
    float fac4 = 0.5 + 0.5 * sin(3. * uv.x + 2.32 * uv.y + ghostRare.r * 8.111 + sin(ghostRare.r * 1.3 + uv.y * 11.2));
    float fac5 = sin(0.9 * 16. * uv.x + 5.32 * uv.y + ghostRare.r * 12. + cos(ghostRare.r * 5.3 + uv.y * 4.2 - uv.x * 4.));

    float maxfac = 0.7 * max(max(fac, max(fac2, max(fac3, 0.0))) + (fac + fac2 + fac3 * fac4), 0.);

    // Option A: if you're still working in HSL, boost the lightness
    hsl.z = clamp(hsl.z + maxfac * 0.25, 0.3, 1.0);

    // Option B: if already in RGB
    // tex.rgb += maxfac * vec3(0.2, 0.25, 0.3); // adds a light shimmer tint

    
    hsl.y = min(0.25, hsl.y * 0.4);  // very faint saturation
    hsl.z = mix(0.6, 0.85, pow(abs(sin(time + uv.x * 2.0)), 3.0));

    
    float specular = pow(abs(sin(time * 1.5 + uv.y * 5.0)), 8.0) * 0.2;
    hsl.z = clamp(hsl.z + specular, 0.3, 1.0);

    tex.rgb = RGB(hsl).rgb;

    if (tex.a < 0.7) tex.a = tex.a / 3.0;

    return dissolve_mask(tex * colour, texture_coords, uv);
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