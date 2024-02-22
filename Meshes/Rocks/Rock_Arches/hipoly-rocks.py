import bpy
import sys

def clear_scene():
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete()

def import_fbx(file_path):
    bpy.ops.import_scene.fbx(filepath=file_path)

def apply_subdivision(subdivision_levels=1):
    for obj in bpy.context.scene.objects:
        if obj.type == 'MESH':
            bpy.context.view_layer.objects.active = obj
            bpy.ops.object.modifier_add(type='SUBSURF')
            modifier = obj.modifiers['Subdivision']
            modifier.levels = subdivision_levels
            bpy.ops.object.modifier_apply(modifier='Subdivision')

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
    apply_subdivision(1)  # Feel free to adjust the subdivision level
    export_fbx(output_file)

