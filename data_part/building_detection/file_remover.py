import os
from tqdm import tqdm

files_dir = r"C:\Users\carsb\CassiniHackathon\CassiniHackathon.Data\datasets\Buildings\labels"


subdir = os.listdir(files_dir)
for sdir in subdir:
    label_files = os.listdir(os.path.join(files_dir, sdir))
    for file in tqdm(label_files):
        extension = os.path.splitext(file)[1]
        if extension =='.txt':
                filename = os.path.join(files_dir, sdir, file)
                os.remove(filename)
