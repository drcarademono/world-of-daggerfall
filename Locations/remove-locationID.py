import os
import pandas as pd

def remove_column_from_csv(directory, column_name):
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith(".csv"):
                file_path = os.path.join(root, file)
                try:
                    df = pd.read_csv(file_path)
                    # Check if the column exists in the DataFrame
                    if column_name in df.columns:
                        # Remove the column
                        df.drop(column_name, axis=1, inplace=True)
                        # Save the modified DataFrame back to the CSV, overwriting the original file
                        df.to_csv(file_path, index=False)
                        print(f"Removed '{column_name}' from {file_path}")
                    else:
                        print(f"'{column_name}' column not found in {file_path}")
                except Exception as e:
                    print(f"Error processing {file_path}: {e}")

# Replace 'your_directory_path' with the path to the directory you want to start from.
# Use '.' to represent the current directory.
your_directory_path = '.'
remove_column_from_csv(your_directory_path, 'locationID')

