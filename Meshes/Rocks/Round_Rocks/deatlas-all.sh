#!/bin/bash

# Loop through all FBX files in the current directory
for fbx_file in *.fbx; do
    echo "Processing ${fbx_file}..."

    # Construct the output filename
    # This example adds "_modified" before the file extension. Adjust as needed.
    output_file="${fbx_file%.fbx}.fbx"

    # Run Blender in background mode, execute the Python script with the current FBX file
    blender -b -P deatlas-fbx.py -- "${fbx_file}" "${output_file}"
done

echo "All files processed."

