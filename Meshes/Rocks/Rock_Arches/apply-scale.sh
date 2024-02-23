#!/bin/bash

# Path to the Blender executable; adjust as needed
BLENDER_PATH="blender"

# Path to your Python script
SCRIPT_PATH="apply-scale.py"

# Find all blend files in the current directory and process them
for blendfile in *.blend; do
    echo "Processing $blendfile..."
    $BLENDER_PATH -b "$blendfile" -P "$SCRIPT_PATH"
    echo "$blendfile processed."
done

echo "All files processed."

