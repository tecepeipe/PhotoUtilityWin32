# PhotoUtilityWin32

A WinForms .NET Framework 4.8 photo utility for intelligent grouping, temporal event detection, and duplicate removal.

## Features
- Fast EXIF metadata scan after selecting a folder.
- Temporal clustering into events based on a 1-hour threshold.
- In-memory 256×256 thumbnail generation for perceptual hashing.
- pHash similarity grouping with Hamming distance ≤ 5.
- Local ONNX model integration for group labeling (Open Neural Network Exchange).
- Manual rename and merge support for event groups.
- Delete duplicate candidates while keeping the highest-quality image.

## Setup
1. Open `PhotoUtilityWin32.sln` in Visual Studio 2022 or newer.
2. Place a compatible ONNX model file at `Assets\mobilenetv2.onnx`.
3. Update `Assets\imagenet_labels.txt` to match your model's output labels if needed.
4. Build and run the solution.

## Notes
- The project is configured for `.NET Framework 4.8`.
- `Assets\yolo26n-cls.onnx` is the local model; replace it with a working model for classification (like old `mobilenetv2.onnx`).
- The UI uses a `TreeView` to show grouped events and photo items.
