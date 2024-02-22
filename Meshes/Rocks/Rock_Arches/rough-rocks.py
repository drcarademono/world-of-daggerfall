import bpy
import sys

def clear_scene():
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete()

def import_fbx(file_path):
    bpy.ops.import_scene.fbx(filepath=file_path)

def apply_displacement(displacement_strength=0.05):
    for obj in bpy.context.scene.objects:
        if obj.type == 'MESH':
            bpy.context.view_layer.objects.active = obj
            bpy.ops.object.modifier_add(type='DISPLACE')
            modifier = obj.modifiers['Displace']
            
            # Create a new texture for the displacement modifier
            texture = bpy.data.textures.new(name="DisplaceTex", type='CLOUDS')
            modifier.texture = texture
            modifier.strength = displacement_strength
            
            # Apply the modifier
            bpy.ops.object.modifier_apply(modifier='Displace')

def export_fbx(file_path):
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.export_scene.fbx(filepath=file_path, use_selection=True)

if __name__ == "__main__":
    argv = sys.argv
    argv = argv[argv.index("--") + 1:] if "--" in argv else argv
    if len(argv) != 2:
        print("Usage: blender -b -P script.py -- <input_fbx_file> <output_fbx_file>")
        sys.exit(1)

    input_file, output_file = argv
    clear_scene()
    import_fbx(input_file)
    apply_displacement(0.05)  # Adjust the displacement strength as needed
    export_fbx(output_file)

