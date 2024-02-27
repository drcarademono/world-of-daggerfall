import os
import pandas as pd

def process_csv_file(file_path):
    # Load the CSV file
    df = pd.read_csv(file_path)
    
    # Ensure both 'climate' and 'prefab' columns exist
    if 'climate' in df.columns and 'prefab' in df.columns:
        # Convert 'climate' to float for comparison and 'prefab' to string for manipulation
        df['climate'] = df['climate'].astype(float)
        df['prefab'] = df['prefab'].astype(str)
        
        # Define the condition where 'climate' is 4.0 and 'prefab' contains 'WOD_Rocks_Round'
        condition = (df['climate'] == 2) & (df['prefab'].str.contains('WOD_Rocks_Round'))
        
        # Replace 'WOD_Rocks_Round' with 'WOD_Rocks_RoundSpiky' in 'prefab' where the condition is True
        df.loc[condition, 'prefab'] = df.loc[condition, 'prefab'].str.replace('WOD_Rocks_Round', 'WOD_Rocks_RoundSpiky')
        
        # Save the modified DataFrame back to the same CSV file
        df.to_csv(file_path, index=False)

def process_directory(directory):
    # Walk through the directory and all its subdirectories
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith('.csv'):
                file_path = os.path.join(root, file)
                process_csv_file(file_path)
                print(f"Processed {file_path}")

# Start processing from the current directory (or specify any directory you want to start with)
process_directory('.')

print("All CSV files have been processed.")

