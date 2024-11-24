import os
from ultralytics import YOLO
import torch
torch.cuda.device_count()

ds_path = r"\CassiniHackathon\datasets\Buildings"
if __name__ == '__main__':
    model = YOLO("yolo11n-obb.yaml")
    ds_config = os.path.join(ds_path, "data.yaml")
    # Train the model on the DOTAv1 dataset
    # results = model.train(data="DOTAv1.yaml", epochs=100, imgsz=1024, device=[1], batch=4)
    results = model.train(data=ds_config, epochs=30, imgsz=640, device='cuda', batch=32)
    print(f"Training results {results}")
