import bpy
import os

def apply_scale_and_reexport_fbx(fbx_file_path):
    # Clear the current scene
    bpy.ops.wm.read_factory_settings(use_empty=True)
    
    # Import FBX file
    bpy.ops.import_scene.fbx(filepath=fbx_file_path)
    
    # Apply the scale to all objects
    for obj in bpy.context.selected_objects:
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    
    # Export to FBX with the same filename
    bpy.ops.export_scene.fbx(filepath=fbx_file_path, use_selection=False)

if __name__ == "__main__":
    # Use the current directory for FBX files
    current_dir = os.getcwd()
    fbx_files = [f for f in os.listdir(current_dir) if f.endswith('.fbx')]
    
    # Process each FBX file in the directory
    for fbx_file in fbx_files:
        fbx_file_path = os.path.join(current_dir, fbx_file)
        print(f'Processing: {fbx_file_path}')
        apply_scale_and_reexport_fbx(fbx_file_path)
        print(f'Re-exported: {fbx_file}')

