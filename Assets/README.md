# PhotoUtilityWin32 Assets

Drop a local ONNX image classification model into this folder and update in `MainForm.cs`.

The application will attempt to classify the first photo in each event group and use that label for the suggested group name.

The built-in label file contains example categories. For best results, replace `imagenet_classes.txt` with a label file matching your ONNX model's output ordering.
