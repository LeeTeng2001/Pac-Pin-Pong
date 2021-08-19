shader_type canvas_item;

uniform vec4 flashColor: hint_color = vec4(1.0);
uniform float flashModifier: hint_range(0.0, 1.0) = 0.6;

void fragment() {
	vec4 color = texture(TEXTURE, UV);
	color.rgb = mix(color.rgb, flashColor.rgb, flashModifier);
	COLOR = color;
}