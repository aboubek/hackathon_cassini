import os
import numpy as np
from PIL import Image

from ultralytics import YOLO
import torch
torch.cuda.device_count()

model_path = r"\CassiniHackathon\CassiniHackathon.Data\runs\obb\train2\weights\best.pt"
images_path = r"\CassiniHackathon\datasets\test_data_final"
sel_image = "antakya_photo_before_zoomed.png"

def split_image(image, split_size=640):
    im = np.array(image)
    tiles = [im[x:x+split_size,y:y+split_size]
             for x in range(0,im.shape[0], split_size)
             for y in range(0,im.shape[1], split_size)]
    return tiles
    

if __name__ == '__main__':
    # Image load:
    image = os.path.join(images_path, sel_image)
    image_py = Image.open(image)
    image_py = image_py.convert('RGB')
    print(f"Original image shape: {np.array(image_py).shape}")

    # Image pre-processing:
    image_tiles = split_image(image_py)

    # Load model and predict:
    model = YOLO(model_path)
    results = model(image_tiles, save=False)  # predict on an image
    for i in range(len(results)):
        annotated_img = results[i].plot(probs=False, show=True, conf=False, font_size=3, masks=True, save=True)
