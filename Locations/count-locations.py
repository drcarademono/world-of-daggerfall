import os
import csv

def count_csv_rows_in_directory():
    total_rows = 0

    for root, _, files in os.walk(os.getcwd()):
        for file in files:
            if file.endswith('.csv'):
                file_path = os.path.join(root, file)
                try:
                    with open(file_path, mode='r', encoding='utf-8') as f:
                        reader = csv.reader(f)
                        row_count = sum(1 for row in reader)
                        total_rows += row_count
                        print(f"{file}: {row_count} rows")
                except Exception as e:
                    print(f"Error reading {file}: {e}")

    print(f"Total rows across all CSVs: {total_rows}")

if __name__ == "__main__":
    count_csv_rows_in_directory()
