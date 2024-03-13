import os
import pandas as pd
from pathlib import Path

# Define the path to start searching from (current directory)
start_path = '.'

# Function to process the CSV files
def process_csv(file_path):
    # Read the CSV file into a DataFrame
    df = pd.read_csv(file_path)
    # Filter out rows where altitude >= 8 and climate == 4
    filtered_df = df[(df['altitude'] < 8) | (df['climate'] != 4) | (df['altitude'].isna())]
    # Write the filtered DataFrame back to CSV
    filtered_df.to_csv(file_path, index=False)
    return file_path

# List to keep track of processed files
processed_files = []

# Walk through the directory
for root, dirs, files in os.walk(start_path):
    for file in files:
        # Check if the file name matches the required pattern
        if file.endswith('Rocks.csv'):
            # Process the file
            processed_file = process_csv(os.path.join(root, file))
            # Add the processed file path to the list
            processed_files.append(processed_file)

# Return the list of processed files
processed_files

