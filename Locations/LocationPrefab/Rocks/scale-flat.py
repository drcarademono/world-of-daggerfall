import os
import xml.etree.ElementTree as ET

# Use '.' to represent the current directory
directory_path = '.'

def process_file(file_path):
    # Parse the XML file
    tree = ET.parse(file_path)
    root = tree.getroot()

    # Iterate over each object element
    for obj in root.findall('.//object'):
        name = obj.find('name').text if obj.find('name') is not None else ''
        scaleX = obj.find('scaleX')
        scaleY = obj.find('scaleY')
        scaleZ = obj.find('scaleZ')

        # Check if this is the specific object we want to modify
        if name == 'WOD_Rock_Flat_l_2C_05':
            # Multiply the scale values by 50, if they exist
            if scaleX is not None:
                scaleX.text = str(float(scaleX.text) / 50)
            if scaleY is not None:
                scaleY.text = str(float(scaleY.text) / 50)
            if scaleZ is not None:
                scaleZ.text = str(float(scaleZ.text) / 50)

    # Save the modified XML back to the file
    tree.write(file_path)

# Process each file in the directory
for filename in os.listdir(directory_path):
    if filename.endswith('.txt'):
        file_path = os.path.join(directory_path, filename)
        process_file(file_path)
        print(f'Processed {filename}')

