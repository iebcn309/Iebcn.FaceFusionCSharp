# Iebcn.FaceFusionCSharp
C#版Facefusion一共有如下5个步骤：

1、使用yoloface_8n.onnx进行人脸检测

2、使用2dfan4.onnx获取人脸关键点

3、使用arcface_w600k_r50.onnx获取人脸特征值

4、使用inswapper_128.onnx进行人脸替换

5、使用gfpgan_1.4.onnx进行人脸增强