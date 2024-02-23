import bpy
import os

def clear_scene():
    bpy.ops.wm.read_factory_settings(use_empty=True)

def import_fbx(fbx_file_path):
    bpy.ops.import_scene.fbx(filepath=fbx_file_path)

def save_blend_file(blend_file_path):
    bpy.ops.wm.save_as_mainfile(filepath=blend_file_path)

def convert_fbx_to_blend():
    current_working_directory = os.getcwd()  # Use the current working directory for both source and destination
    for filename in os.listdir(current_working_directory):
        if filename.lower().endswith('.fbx'):
            fbx_file_path = os.path.join(current_working_directory, filename)
            blend_file_name = os.path.splitext(filename)[0] + '.blend'
            blend_file_path = os.path.join(current_working_directory, blend_file_name)
            
            clear_scene()  # Start with a fresh scene
            import_fbx(fbx_file_path)  # Import the FBX file
            save_blend_file(blend_file_path)  # Save the scene as a .blend file
            print(f"Converted {fbx_file_path} to {blend_file_path}")

# Call the function without needing to set a path
convert_fbx_to_blend()

