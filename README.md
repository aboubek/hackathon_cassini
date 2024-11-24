# hackathon_cassini

In case of any accidents/disasters, time is crucial, therefore we developed application, that is near real-time monitoring impact of such incidents and help with work coordination.

## Structure
There are two basic parts:
- Visualization part
- Data part

### Data part
Data part is combining object detection from Sentinel2, to identify structures such as buildings etc. and anomaly detection from Sentinel1 using SAR. 

Any input can be replaced by different sources. In general it combines image and radar to identify changes and timeline. 

### Visualization part
Web application is focusing on data visualization. As we get streams of data, its visualization is crucial part in order to provide related data to incidents.

## Technical specification
### Data part
We are using object detection alghoritms such as YOLO or MaskRcnn to detected structed and create timeline of detections. Changes are not handled here, but in visualization part. 
All scripts are written in python. 

Detection is purely extracting data from images. So far images from Sentinel2, but can be replaced by any source. From reason of image rescaling, we decided to use convolutional
neural networks rather than transorfmers that sufferers from images rescaling and resizing. This makes image object detection more robust. 

Anomaly detection is implemented in visualization part. We are preprocessing data and assuring data quality for later use. 

### Visualization part
This part written in C# and web technologies (Javascript, html, css etc.). Its written to aggregate streams, put layer on top of each other and visualize crucial parts. 

## Team
Matěj Mužátko - https://github.com/mutje/ </br>
Georgi S. Georgiev - https://github.com/GeorgiSGeorgiev </br>
Mikoláš Belec
