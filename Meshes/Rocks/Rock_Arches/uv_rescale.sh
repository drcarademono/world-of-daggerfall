#!/bin/bash

for file in *.blend; do
    blender -b "$file" -P uv_rescale.py
done

