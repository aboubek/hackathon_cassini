import rasterio
import rasterio.plot
import matplotlib
import numpy as np

data_name = r"\\CassiniHackathon\\CassiniHackathon.Data\\copernicus_s1.tif"
tiff = rasterio.open(data_name)
print(tiff)
rasterio.plot.show(tiff)
band_arr = tiff.read(2)
print(band_arr)