import asyncio
import geotiler
from geotiler.provider import MapProvider

def log_error(tile):
    if tile.error:
        print('tile {} error: {}'.format(tile.url, tile.error))
    return tile

async def fetch(mm):
    tiles = geotiler.fetch_tiles(mm)
    # process tile in error and then render all the tiles
    tiles = (log_error(t) async for t in tiles)
    img = await geotiler.render_map_async(mm, tiles=tiles)
    return img

def save_map_to_png(south, west, east, north, year, output_file):
    # Define the layer mapping
    layers = {
        2018: "13161",
        2019: "645",
        2020: "11135",
        2021: "26120",
        2022: "44988",
        2023: "56102",
        2024: "49849",
    }

    # Validate the year
    if year not in layers:
        raise ValueError(f"Year {year} is not supported. Supported years: {list(layers.keys())}")

    layer_id = layers[year]
    
    # URL for tiles, from Living Atlas Wayback Machine
    # Keep in mind that "Y" coordinate (latitude) goes first
    # That can be reason for NOT FOUND error
    # Also keep in mind that YEAR (layer id) is being hardcoded here using python
    # So -- if you want to fetch for multiple years,
    # you have to create new URL and new MapProvider when switching years
    tile_url = f"https://wayback.maptiles.arcgis.com/arcgis/rest/services/World_Imagery/WMTS/1.0.0/default028mm/MapServer/tile/{layer_id}/{{z}}/{{y}}/{{x}}"
    # tile_url = f"https://wayback.maptiles.arcgis.com/arcgis/rest/services/World_Imagery/WMTS/1.0.0/default028mm/MapServer/tile/{layer_id}/0/0/0"

    mp = MapProvider({"url": tile_url})

    # Bounding box for Mariupol (same as the data, but too large area probably) 
    # Maybe too small resolution for machine learning, caused the library we use to fetch
    # bbox = 37.464397295697694, 47.05992135191326, 37.65127304351881, 47.142010185603084

    # Bounding box for Antakya, smaller area, probably good resolution already
    bbox = 36.148692878491327, 36.202254495488305, 36.15976831740696, 36.20851804388457

    # Play with the zoom! It can be the reason for NOT FOUND error on the tiling server (Atlas)
    mm = geotiler.Map(extent=bbox, zoom=18, provider=mp)
    # mm = geotiler.Map(extent=bbox, zoom=16) # use this to debug that the correct area is fetched - normal map, not photo

    loop = asyncio.get_event_loop()

    img = loop.run_until_complete(fetch(mm))
    img.save(f"./downloads/{output_file}", 'png')

# Example usage
south = 47.0
west = 37.0
east = 47.2
north = 37.2
year = 2023
output_file = "antakya_photo.png"

save_map_to_png(south, west, east, north, year, output_file)