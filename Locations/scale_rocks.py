#!/usr/bin/env python3
import os
import pandas as pd
import fnmatch

def main():
    # Walk through current directory and subdirectories
    for root, dirs, files in os.walk('.'):
        for filename in files:
            # Match files ending with _Rocks.csv
            if fnmatch.fnmatch(filename, '*_Rocks.csv'):
                filepath = os.path.join(root, filename)
                try:
                    df = pd.read_csv(filepath)
                except Exception as e:
                    print(f"Failed to read {filepath}: {e}")
                    continue
                if 'scale' in df.columns:
                    # Multiply scale column by 0.5
                    df['scale'] = df['scale'] * 0.5
                    df.to_csv(filepath, index=False)
                    print(f"Processed {filepath}")
                else:
                    print(f"No 'scale' column in {filepath}, skipping")

if __name__ == '__main__':
    main()
