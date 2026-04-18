# Update Question API Format

## Endpoint
```
PATCH /api/Question/UpdateQuestion
Content-Type: multipart/form-data
```

## Form Data Fields

### Basic Question Fields
- `QuestionId`: Guid (required)
- `QuestionString`: string (required)
- `QuestionImageUrl`: string (optional)
- `Mark`: decimal (optional)
- `PictureFile`: IFormFile (optional)

### Answers Field (Clean JSON Array)
- `Answers`: JSON string array of UpdateAnswerDto objects

#### UpdateAnswerDto Structure:
```json
{
  "id": "guid-or-null",           // null for new answers, existing GUID for updates
  "answerText": "string",         // required
  "isCorrect": boolean,           // required
  "explanation": "string-or-null" // optional
}
```

## Example Form Data

### Updating existing answers:
```
QuestionId: 550e8400-e29b-41d4-a716-446655440000
QuestionString: "What is 2 + 2?"
Answers: [
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "answerText": "4",
    "isCorrect": true,
    "explanation": "2 + 2 = 4"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "answerText": "5",
    "isCorrect": false,
    "explanation": "This is incorrect"
  }
]
```

### Adding new answers (null IDs):
```
QuestionId: 550e8400-e29b-41d4-a716-446655440000
QuestionString: "What is the capital of France?"
Answers: [
  {
    "id": null,
    "answerText": "Paris",
    "isCorrect": true,
    "explanation": null
  },
  {
    "id": null,
    "answerText": "London",
    "isCorrect": false,
    "explanation": null
  }
]
```

### Mixed (update existing + add new):
```
QuestionId: 550e8400-e29b-41d4-a716-446655440000
QuestionString: "What color is the sky?"
Answers: [
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "answerText": "Blue",
    "isCorrect": true,
    "explanation": "The sky appears blue due to Rayleigh scattering"
  },
  {
    "id": null,
    "answerText": "Green",
    "isCorrect": false,
    "explanation": null
  }
]
```

## Important Notes

1. **JSON Format**: The `Answers` field must be a valid JSON array string
2. **New Answers**: Use `null` for the `id` field when adding new answers
3. **Existing Answers**: Provide the actual GUID string for existing answers
4. **Required Fields**: `answerText` and `isCorrect` are required for each answer
5. **Optional Fields**: `explanation` can be omitted or set to `null`
6. **Custom Model Binder**: Uses a custom model binder to properly deserialize JSON from form data

## Common Issues & Solutions

### Issue: "Invalid JSON format" Error
**Cause**: The JSON string is malformed or contains extra quotes

**Solution**: Ensure the JSON is properly formatted:
```dart
// CORRECT - No extra quotes around the entire JSON
final answersJson = jsonEncode([
  {"id": null, "answerText": "Paris", "isCorrect": true}
]);

// INCORRECT - Extra quotes
final answersJson = '"[{"id": null, "answerText": "Paris", "isCorrect": true}]"';
```

### Issue: Empty Answers Array
**Solution**: Send an empty JSON array:
```dart
final answersJson = '[]';
```

### Issue: Single Answer (Not Array)
**Solution**: Always send as an array, even for one answer:
```dart
// CORRECT
final answersJson = jsonEncode([{"id": null, "answerText": "Paris", "isCorrect": true}]);

// INCORRECT
final answersJson = jsonEncode({"id": null, "answerText": "Paris", "isCorrect": true});
```

## Debugging Tips

1. **Check the raw JSON**: Print the JSON string before sending
2. **Validate JSON**: Use a JSON validator to ensure proper format
3. **Empty arrays**: Use `[]` for no answers, not `null` or empty string
4. **Boolean values**: Use `true`/`false` (lowercase), not `True`/`False`

## Flutter/Dio Example

```dart
final answersJson = jsonEncode([
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "answerText": "4",
    "isCorrect": true,
    "explanation": "2 + 2 = 4"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "answerText": "5",
    "isCorrect": false,
    "explanation": "This is incorrect"
  }
]);

final formData = FormData.fromMap({
  'QuestionId': '550e8400-e29b-41d4-a716-446655440000',
  'QuestionString': 'What is 2 + 2?',
  'Answers': answersJson,
});

if (imageFile != null) {
  formData.files.add(MapEntry(
    'PictureFile',
    await MultipartFile.fromFile(imageFile.path),
  ));
}

final response = await dio.patch(
  '/api/Question/UpdateQuestion',
  data: formData,
);
```

## Architecture Benefits

1. **Clean DTO Structure**: Maintains the original `List<UpdateAnswerDto>` structure
2. **Type Safety**: Full type safety and IntelliSense support
3. **Custom Model Binder**: Handles JSON deserialization from multipart/form-data seamlessly
4. **Backward Compatible**: No changes needed to existing service layer code
5. **Maintainable**: Single source of truth for answer structure
