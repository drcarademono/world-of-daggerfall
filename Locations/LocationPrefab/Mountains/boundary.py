import os
import xml.etree.ElementTree as ET

def adjust_dimensions(xml_file):
    # Parse the XML file
    tree = ET.parse(xml_file)
    root = tree.getroot()

    # Find the height and width elements and subtract 100 from their values
    height = root.find('.//height')
    width = root.find('.//width')

    if height is not None and height.text.isdigit():
        height.text = str(int(height.text) - 100)

    if width is not None and width.text.isdigit():
        width.text = str(int(width.text) - 100)

    # Save the modified XML back to the file
    tree.write(xml_file)

def process_folder(folder_path):
    for filename in os.listdir(folder_path):
        if filename.endswith(".txt"):  # Check if the file is a .txt file
            full_path = os.path.join(folder_path, filename)
            adjust_dimensions(full_path)
            print(f"Processed {filename}")

# Adjust the path to your folder if necessary
current_folder = "."  # Current folder
process_folder(current_folder)

