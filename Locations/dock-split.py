import pandas as pd

# Load the CSV file
csv_file = 'WOD_Docks.csv'  # Replace with your actual CSV filename if different
docks_df = pd.read_csv(csv_file)

# Group by 'LocationType' and then export each group to a new CSV
for location_type, group in docks_df.groupby('LocationType'):
    # Replace any characters not suitable for filenames
    filename_safe_location_type = str(location_type).replace('/', '_').replace(' ', '_')
    group.to_csv(f'WOD_Docks_{filename_safe_location_type}.csv', index=False)
    print(f'Saved WOD_Docks_{filename_safe_location_type}.csv')

print("All files have been saved.")

