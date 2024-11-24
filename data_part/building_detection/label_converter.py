import os  
import pandas as pd
from tqdm import tqdm

# Note: Run this script only once, otherwise the label files will be changed in unintended ways.
files_dir = r"C:\Users\carsb\CassiniHackathon\datasets\Buildings\labels"
names = ["x1", "y1", "x2", "y2", "x3", "y3", "x4", "y4", "class", "complex"]

def convert(files_dir):
    subdir = os.listdir(files_dir)
    for dir in subdir:
        label_files = os.listdir(os.path.join(files_dir, dir))
        for file in tqdm(label_files):
            extension = os.path.splitext(file)[1]
            if extension =='.txt':
                filename = os.path.join(files_dir, dir, file)
                df = pd.read_csv(filename, sep=' ', names=names)
                df.drop('complex', inplace=True, axis=1)
                cols = df.columns.tolist()
                cols = cols[-1:] + cols[:-1]  # change the ordering of the columns to math the YOLO OBB format
                df = df[cols]
                df.replace("Building", 0, inplace=True)  # replace class name with its index
                df.clip(lower=0, upper=639, inplace=True)  # clip the coordinates that are outside of the image
                df=df/640  # normalize
                df['class'] = df['class'].astype(int)
                new_filename = os.path.splitext(filename)[0] + '.txt'
                df.to_csv(new_filename, sep=' ', index=False, header=False)

if __name__ == '__main__':
    convert(files_dir)
