import bpy
import os

def clear_scene():
    bpy.ops.wm.read_factory_settings(use_empty=True)

def swap_materials(obj, mat_index_a, mat_index_b):
    if len(obj.material_slots) < max(mat_index_a, mat_index_b) + 1:
        return  # Not enough materials to swap
    mat_a = obj.material_slots[mat_index_a].material
    mat_b = obj.material_slots[mat_index_b].material
    obj.material_slots[mat_index_a].material = mat_b
    obj.material_slots[mat_index_b].material = mat_a

def import_and_process_fbx(fbx_file_path):
    clear_scene()
    bpy.ops.import_scene.fbx(filepath=fbx_file_path)
    
    for obj in bpy.context.scene.objects:
        if obj.type == 'MESH' and len(obj.material_slots) >= 2:
            swap_materials(obj, 0, 1)  # Swap the first two materials

    bpy.ops.export_scene.fbx(filepath=fbx_file_path, use_selection=False)

if __name__ == "__main__":
    current_dir = os.getcwd()
    fbx_files = [f for f in os.listdir(current_dir) if f.endswith('.fbx')]
    
    for fbx_file in fbx_files:
        fbx_file_path = os.path.join(current_dir, fbx_file)
        print(f'Processing: {fbx_file_path}')
        import_and_process_fbx(fbx_file_path)
        print(f'Finished processing: {fbx_file}')

