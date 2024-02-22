for file in *.fbx; do
    blender --background --python-expr "
import bpy
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.fbx(filepath='${file}')
bpy.ops.wm.save_as_mainfile(filepath='${file%.fbx}.blend')
bpy.ops.wm.quit()"
done
