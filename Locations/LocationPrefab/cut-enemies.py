import os
import random
import xml.etree.ElementTree as ET

def process_xml_file(file_path):
    # Parse the XML file
    tree = ET.parse(file_path)
    root = tree.getroot()

    # Find all objects with EnemyId in extraData
    objects_with_enemyid = []
    for obj in root.findall('object'):
        extra_data = obj.find('extraData')
        if extra_data is not None and '"EnemyId":' in extra_data.text:
            objects_with_enemyid.append(obj)

    # Only proceed if there are 3 or more EnemyId objects
    if len(objects_with_enemyid) >= 3:
        # Calculate the number of objects to remove
        num_to_remove = len(objects_with_enemyid) // 2

        # Randomly select objects to remove
        objects_to_remove = random.sample(objects_with_enemyid, num_to_remove)

        # Remove the selected objects from the XML tree
        for obj in objects_to_remove:
            root.remove(obj)

        # Save the modified XML back to the file
        tree.write(file_path)

def process_folder(folder_path):
    # Iterate through all files in the folder
    for file_name in os.listdir(folder_path):
        if file_name.endswith('.txt'):  # Assuming the XML files have a .txt extension
            file_path = os.path.join(folder_path, file_name)
            process_xml_file(file_path)

# Path to the folder containing the XML files
folder_path = '.'

# Process all XML files in the folder
process_folder(folder_path)

print("Processing complete!")

