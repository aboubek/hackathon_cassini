// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {



    // Initialize the map after the Google Maps API has loaded
    initMap();


});

let tileOverlay;

function initMap() {
    const layers = {
        2018: "13161",
        2019: "645",
        2020: "11135",
        2021: "26120",
        2022: "44988",
        2023: "56102",
        2024: "49849",
    };

    const center = { lat: 36.2066947, lng: 36.1508677 };

    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 16,
        center: center,
        mapTypeControl: false, // Disable the map type control
        streetViewControl: false, // Disable the Street View control
        fullscreenControl: false, // Disable the Full-Screen control
    });

    // Load GeoJSON data and add to heatmap
    fetch('/api/data')
        .then((response) => response.json())
        .then((geoJsonData) => {
            // Parse GeoJSON and create heatmap data
            const heatmapData = geoJsonData.features.map((feature) => {
                const [lng, lat] = feature.geometry.coordinates; // Extract lat, lng
                const weight = feature.properties.z; // Use 'z' property as weight
                return {
                    location: new google.maps.LatLng(lat, lng),
                    weight: 5,
                };
            });

            // Add heatmap layer to map
            addHeatmapLayer(map, heatmapData);
        })
        .catch((error) => console.error("Error loading GeoJSON:", error));

    function addHeatmapLayer(map, heatmapData) {
        const heatmap = new google.maps.visualization.HeatmapLayer({
            data: heatmapData,
            map: map,
            radius: 30,
            opacity: 0.6,
            gradient: [
                "rgba(255, 255, 0, 0)",   // Transparent
                "rgba(255, 255, 0, 0.4)", // Light yellow
                "rgba(255, 204, 0, 0.7)", // Medium yellow
                "rgba(255, 153, 0, 0.8)", // Orange
                "rgba(255, 102, 0, 0.9)", // Dark orange
                "rgba(255, 51, 0, 1)",    // Red-orange
                "rgba(255, 0, 0, 1)",     // Bright red (maximum intensity)
            ],
        });
    }

    // Add tile overlay for WMTS
    addTileOverlay("8249", 15);

    function addTileOverlay(layerId, zoom = 15) {

        if (tileOverlay) {
            map.overlayMapTypes.clear();
        }

        tileOverlay = new google.maps.ImageMapType({
            getTileUrl: function (coord, zoomLevel) {
                const tileRow = coord.y;
                const tileCol = coord.x;
                return `https://wayback.maptiles.arcgis.com/arcgis/rest/services/World_Imagery/WMTS/1.0.0/default028mm/MapServer/tile/${layerId}/${zoomLevel}/${tileRow}/${tileCol}`;
            },
            tileSize: new google.maps.Size(256, 256),
            opacity: 1.0,
            referrerPolicy: "no-referrer",
        });

        map.overlayMapTypes.insertAt(0, tileOverlay);
    }

    // Disable default marker rendering in map.data
    map.data.setStyle({ visible: false }); // Hide all default markers

    // Slider event for year selection
    document.getElementById("timeSlider").addEventListener("input", (event) => {
        const year = event.target.value;
        const layerId = layers[year];
        if (layerId) {
            addTileOverlay(layerId, 15);
            document.getElementById("dateLabel").textContent = `Year: ${year}`;
        } else {
            console.error(`No layer ID found for year: ${year}`);
        }
    });
}