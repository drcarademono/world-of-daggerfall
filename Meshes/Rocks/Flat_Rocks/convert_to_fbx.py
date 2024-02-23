import bpy
import os

def apply_scale_and_export_fbx(blend_file_path, export_dir):
    # Open the blend file
    bpy.ops.wm.open_mainfile(filepath=blend_file_path)

    # Apply scale to all objects
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    bpy.ops.object.select_all(action='DESELECT')

    # Define the export path
    base_name = os.path.basename(blend_file_path)
    name_without_ext = os.path.splitext(base_name)[0]
    fbx_file_path = os.path.join(export_dir, name_without_ext + '.fbx')

    # Export to FBX
    bpy.ops.export_scene.fbx(filepath=fbx_file_path, use_selection=False)

    # Corrected print statement to use within function scope
    print(f'Exported to FBX: {os.path.join("exported_fbx", name_without_ext + ".fbx")}')

if __name__ == "__main__":
    # Use the current directory for .blend files
    blend_files_dir = os.getcwd()
    # Create a subdirectory in the current directory for the exported .fbx files
    export_dir = os.path.join(os.getcwd(), 'exported_fbx')

    # Make sure the export directory exists
    os.makedirs(export_dir, exist_ok=True)

    # Process each .blend file in the directory
    for file in os.listdir(blend_files_dir):
        if file.endswith('.blend'):
            blend_file_path = os.path.join(blend_files_dir, file)
            print(f'Processing: {blend_file_path}')
            apply_scale_and_export_fbx(blend_file_path, export_dir)

