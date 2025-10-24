# Flutter Integration Guide for Image Uploads

This guide demonstrates how to upload images from your Flutter application to the Educational Platform API.

## Overview

The CloudinaryService has been optimized to handle image uploads from Flutter with different transformations based on usage:
- **Profile Pictures**: 400x400 square crop with face detection, circular border
- **Video Thumbnails**: 1280x720 (16:9) optimized for video previews

## API Endpoints

### 1. Upload Profile Picture
```
POST /api/MediaUpload/profile-picture
Content-Type: multipart/form-data
Authorization: Bearer {token}
```

### 2. Upload Video Thumbnail
```
POST /api/MediaUpload/video-thumbnail
Content-Type: multipart/form-data
Authorization: Bearer {token}
```

### 3. Update Profile Picture
```
PUT /api/MediaUpload/profile-picture/{publicId}
Content-Type: multipart/form-data
Authorization: Bearer {token}
```

### 4. Delete Media
```
DELETE /api/MediaUpload/{publicId}
Authorization: Bearer {token}
```

## Flutter Implementation

### 1. Add Dependencies

Add these to your `pubspec.yaml`:

```yaml
dependencies:
  http: ^1.1.0
  image_picker: ^1.0.4
  dio: ^5.3.3  # Alternative to http, recommended for file uploads
```

### 2. Upload Profile Picture (using http package)

```dart
import 'dart:io';
import 'package:http/http.dart' as http;
import 'package:image_picker/image_picker.dart';

class CloudinaryService {
  final String baseUrl = 'https://your-api-url.com/api';
  final String authToken;

  CloudinaryService({required this.authToken});

  Future<String?> uploadProfilePicture(File imageFile) async {
    try {
      var uri = Uri.parse('$baseUrl/MediaUpload/profile-picture');
      var request = http.MultipartRequest('POST', uri);
      
      // Add authorization header
      request.headers['Authorization'] = 'Bearer $authToken';
      
      // Add file
      request.files.add(
        await http.MultipartFile.fromPath(
          'file',  // Must match the parameter name in the API
          imageFile.path,
        ),
      );

      // Send request
      var response = await request.send();
      var responseData = await response.stream.bytesToString();

      if (response.statusCode == 200) {
        // Parse JSON response
        final jsonResponse = json.decode(responseData);
        return jsonResponse['imageUrl'] as String;
      } else {
        throw Exception('Failed to upload image: $responseData');
      }
    } catch (e) {
      print('Error uploading profile picture: $e');
      return null;
    }
  }

  Future<String?> uploadVideoThumbnail(File imageFile) async {
    try {
      var uri = Uri.parse('$baseUrl/MediaUpload/video-thumbnail');
      var request = http.MultipartRequest('POST', uri);
      
      request.headers['Authorization'] = 'Bearer $authToken';
      
      request.files.add(
        await http.MultipartFile.fromPath('file', imageFile.path),
      );

      var response = await request.send();
      var responseData = await response.stream.bytesToString();

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(responseData);
        return jsonResponse['imageUrl'] as String;
      } else {
        throw Exception('Failed to upload thumbnail: $responseData');
      }
    } catch (e) {
      print('Error uploading thumbnail: $e');
      return null;
    }
  }
}
```

### 3. Upload Profile Picture (using Dio - Recommended)

```dart
import 'dart:io';
import 'package:dio/dio.dart';
import 'package:image_picker/image_picker.dart';

class CloudinaryService {
  final Dio _dio;
  final String baseUrl = 'https://your-api-url.com/api';

  CloudinaryService(String authToken) 
      : _dio = Dio(BaseOptions(
          baseUrl: 'https://your-api-url.com/api',
          headers: {'Authorization': 'Bearer $authToken'},
          connectTimeout: const Duration(seconds: 30),
          receiveTimeout: const Duration(seconds: 30),
        ));

  Future<String?> uploadProfilePicture(File imageFile) async {
    try {
      String fileName = imageFile.path.split('/').last;
      FormData formData = FormData.fromMap({
        'file': await MultipartFile.fromFile(
          imageFile.path,
          filename: fileName,
        ),
      });

      final response = await _dio.post(
        '/MediaUpload/profile-picture',
        data: formData,
        options: Options(
          contentType: 'multipart/form-data',
        ),
      );

      if (response.statusCode == 200) {
        return response.data['imageUrl'] as String;
      }
      return null;
    } on DioException catch (e) {
      print('Dio error uploading profile picture: ${e.message}');
      if (e.response != null) {
        print('Response data: ${e.response?.data}');
      }
      return null;
    } catch (e) {
      print('Error uploading profile picture: $e');
      return null;
    }
  }

  Future<String?> uploadVideoThumbnail(File imageFile) async {
    try {
      String fileName = imageFile.path.split('/').last;
      FormData formData = FormData.fromMap({
        'file': await MultipartFile.fromFile(
          imageFile.path,
          filename: fileName,
        ),
      });

      final response = await _dio.post(
        '/MediaUpload/video-thumbnail',
        data: formData,
        options: Options(
          contentType: 'multipart/form-data',
        ),
      );

      if (response.statusCode == 200) {
        return response.data['imageUrl'] as String;
      }
      return null;
    } on DioException catch (e) {
      print('Error uploading thumbnail: ${e.message}');
      return null;
    }
  }

  Future<String?> updateProfilePicture(String publicId, File imageFile) async {
    try {
      String fileName = imageFile.path.split('/').last;
      FormData formData = FormData.fromMap({
        'file': await MultipartFile.fromFile(
          imageFile.path,
          filename: fileName,
        ),
      });

      final response = await _dio.put(
        '/MediaUpload/profile-picture/$publicId',
        data: formData,
        options: Options(
          contentType: 'multipart/form-data',
        ),
      );

      if (response.statusCode == 200) {
        return response.data['imageUrl'] as String;
      }
      return null;
    } on DioException catch (e) {
      print('Error updating profile picture: ${e.message}');
      return null;
    }
  }

  Future<bool> deleteMedia(String publicId) async {
    try {
      final response = await _dio.delete('/MediaUpload/$publicId');
      return response.statusCode == 200;
    } on DioException catch (e) {
      print('Error deleting media: ${e.message}');
      return false;
    }
  }
}
```

### 4. Complete Flutter Widget Example

```dart
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

class ProfilePictureUploader extends StatefulWidget {
  const ProfilePictureUploader({Key? key}) : super(key: key);

  @override
  State<ProfilePictureUploader> createState() => _ProfilePictureUploaderState();
}

class _ProfilePictureUploaderState extends State<ProfilePictureUploader> {
  final ImagePicker _picker = ImagePicker();
  final CloudinaryService _cloudinaryService = CloudinaryService(authToken: 'YOUR_AUTH_TOKEN');
  
  File? _selectedImage;
  String? _uploadedImageUrl;
  bool _isUploading = false;

  Future<void> _pickAndUploadImage() async {
    try {
      // Pick image from gallery
      final XFile? image = await _picker.pickImage(
        source: ImageSource.gallery,
        maxWidth: 1920,
        maxHeight: 1920,
        imageQuality: 85,
      );

      if (image == null) return;

      setState(() {
        _selectedImage = File(image.path);
        _isUploading = true;
      });

      // Upload to Cloudinary
      final imageUrl = await _cloudinaryService.uploadProfilePicture(_selectedImage!);

      setState(() {
        _uploadedImageUrl = imageUrl;
        _isUploading = false;
      });

      if (imageUrl != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Profile picture uploaded successfully!')),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Failed to upload image')),
        );
      }
    } catch (e) {
      setState(() => _isUploading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error: $e')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Upload Profile Picture')),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            if (_selectedImage != null)
              ClipOval(
                child: Image.file(
                  _selectedImage!,
                  width: 200,
                  height: 200,
                  fit: BoxFit.cover,
                ),
              )
            else if (_uploadedImageUrl != null)
              ClipOval(
                child: Image.network(
                  _uploadedImageUrl!,
                  width: 200,
                  height: 200,
                  fit: BoxFit.cover,
                ),
              )
            else
              Container(
                width: 200,
                height: 200,
                decoration: BoxDecoration(
                  color: Colors.grey[300],
                  shape: BoxShape.circle,
                ),
                child: const Icon(Icons.person, size: 100),
              ),
            
            const SizedBox(height: 20),
            
            if (_isUploading)
              const CircularProgressIndicator()
            else
              ElevatedButton.icon(
                onPressed: _pickAndUploadImage,
                icon: const Icon(Icons.upload),
                label: const Text('Pick & Upload Image'),
              ),

            if (_uploadedImageUrl != null) ...[
              const SizedBox(height: 20),
              Text('Uploaded URL:', style: Theme.of(context).textTheme.titleSmall),
              SelectableText(_uploadedImageUrl!),
            ],
          ],
        ),
      ),
    );
  }
}
```

## File Size and Type Restrictions

- **Maximum file size**: 10 MB
- **Allowed formats**: JPG, JPEG, PNG, GIF, WEBP, BMP
- **Automatic optimizations**:
  - Profile pictures: 400x400, face detection, circular crop
  - Video thumbnails: 1280x720, 16:9 ratio, sharpened
  - Auto quality and format optimization (WebP when supported)

## Error Handling

Common errors and solutions:

| Error | Cause | Solution |
|-------|-------|----------|
| 400 Bad Request | Invalid file type or size | Check file format and size |
| 401 Unauthorized | Missing/invalid token | Ensure valid JWT token is provided |
| 413 Payload Too Large | File exceeds 10MB | Compress image before upload |
| 500 Internal Server Error | Server/Cloudinary error | Check logs, verify Cloudinary config |

## Best Practices

1. **Compress images before upload** using Flutter packages like `flutter_image_compress`
2. **Show upload progress** using Dio's `onSendProgress` callback
3. **Cache uploaded URLs** to avoid re-uploads
4. **Validate file types** on client side before upload
5. **Handle network errors** gracefully with retry logic
6. **Use loading indicators** during upload operations

## Example with Image Compression

```dart
import 'package:flutter_image_compress/flutter_image_compress.dart';

Future<File?> compressImage(File file) async {
  final result = await FlutterImageCompress.compressAndGetFile(
    file.absolute.path,
    '${file.parent.path}/compressed_${file.path.split('/').last}',
    quality: 85,
    minWidth: 1920,
    minHeight: 1920,
  );
  
  return result != null ? File(result.path) : null;
}

// Use in upload function
Future<String?> uploadWithCompression(File imageFile) async {
  final compressed = await compressImage(imageFile);
  if (compressed == null) return null;
  
  return await _cloudinaryService.uploadProfilePicture(compressed);
}
```

## Notes

- All uploads are authenticated - ensure valid JWT token
- Images are automatically optimized by Cloudinary
- Profile pictures get face detection and circular crop
- Video thumbnails are optimized for 16:9 display
- Uploaded files are organized in folders by category
