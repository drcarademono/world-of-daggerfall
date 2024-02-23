import bpy
import os

def clear_scene():
    # Clear existing objects in the scene
    bpy.ops.wm.read_factory_settings(use_empty=True)

# Function to rescale UVs of all mesh objects in the current scene
def rescale_uvs(scale_factor=64):
    for obj in bpy.context.scene.objects:
        if obj.type == 'MESH':
            bpy.context.view_layer.objects.active = obj
            bpy.ops.object.select_all(action='DESELECT')
            obj.select_set(True)
            bpy.ops.object.mode_set(mode='EDIT')
            for uvmap in obj.data.uv_layers:
                for data in uvmap.data:
                    data.uv[0] *= scale_factor  # Scale U coordinate
                    data.uv[1] *= scale_factor  # Scale V coordinate
            bpy.ops.object.mode_set(mode='OBJECT')

def import_and_reexport_fbx(fbx_file_path, scale_factor=64):
    # Clear the current scene
    clear_scene()
    
    # Import the FBX file
    bpy.ops.import_scene.fbx(filepath=fbx_file_path)

    # Rescale UVs
    rescale_uvs(scale_factor=scale_factor)

    # Export the scene back to FBX, overwriting the original file
    bpy.ops.export_scene.fbx(filepath=fbx_file_path, use_selection=False)

if __name__ == "__main__":
    # Directory where the script is run, to find FBX files
    current_dir = os.getcwd()
    
    # List all FBX files in the current directory
    fbx_files = [f for f in os.listdir(current_dir) if f.endswith('.fbx')]
    
    # Process each FBX file
    for fbx_file in fbx_files:
        fbx_file_path = os.path.join(current_dir, fbx_file)
        print(f'Processing: {fbx_file_path}')
        import_and_reexport_fbx(fbx_file_path)
        print(f'Re-exported: {fbx_file}')

