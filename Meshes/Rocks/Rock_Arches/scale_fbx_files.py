import bpy
import os

def scale_and_export_fbx(fbx_file_path):
    # Clear the current scene
    bpy.ops.wm.read_factory_settings(use_empty=True)
    
    # Import FBX file
    bpy.ops.import_scene.fbx(filepath=fbx_file_path)
    
    # Scale all objects by 100x
    for obj in bpy.context.scene.objects:
        obj.scale *= 100.0
        obj.select_set(True)
        bpy.context.view_layer.objects.active = obj
    
    # Apply the scale
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
        print(f'Processing and scaling: {fbx_file_path}')
        scale_and_export_fbx(fbx_file_path)
        print(f'Scaled and re-exported: {fbx_file}')

