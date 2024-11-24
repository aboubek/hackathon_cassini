import numpy as np
import ee
import geemap

print("sfda")
print(np.__version__)
ee.Authenticate()

# Connect to my project number
ee.Initialize(project='34567597292')

# STUDY AREA
# Define study area polygon in Rakovnik and surroundings
study_area = ee.Geometry.Polygon(
    [[[13.560454970581674, 50.16692861843212],
      [13.560454970581674, 50.02244563472271],
      [13.958709365112924, 50.02244563472271],
      [13.958709365112924, 50.16692861843212]]])

# DATA
# Select Sentinel-1/2 and Landsat 8/9 data
sentinel_1 = ee.ImageCollection('COPERNICUS/S1_GRD').filterBounds(study_area).filterDate('2020-01-01', '2020-02-01').first()

# print(sentinel_1)
Map = geemap.Map()
Map.addLayer(sentinel_1)
Map.centerObject(sentinel_1)
geemap.download_ee_image(sentinel_1, "copernicus_s1.tif", scale=100)
