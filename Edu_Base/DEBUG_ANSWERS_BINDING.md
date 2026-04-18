# Debugging Answers Binding Issue

## Test the Model Binder

To debug the JSON binding issue, you can test with these example values:

### 1. Test with Valid JSON
```dart
final formData = FormData.fromMap({
  'QuestionId': '550e8400-e29b-41d4-a716-446655440000',
  'QuestionString': 'Test Question',
  'Answers': '[{"id": null, "answerText": "Test Answer", "isCorrect": true, "explanation": null}]',
});
```

### 2. Test with Empty Array
```dart
final formData = FormData.fromMap({
  'QuestionId': '550e8400-e29b-41d4-a716-446655440000',
  'QuestionString': 'Test Question',
  'Answers': '[]',
});
```

### 3. Test with Multiple Answers
```dart
final formData = FormData.fromMap({
  'QuestionId': '550e8400-e29b-41d4-a716-446655440000',
  'QuestionString': 'Test Question',
  'Answers: '[{"id": null, "answerText": "Answer 1", "isCorrect": true}, {"id": null, "answerText": "Answer 2", "isCorrect": false}]',
});
```

## Common JSON Format Issues

### Issue: Extra Quotes Around JSON
**WRONG**: `'"[{"id": null, "answerText": "Test", "isCorrect": true}]"'`
**RIGHT**: `'[{"id": null, "answerText": "Test", "isCorrect": true}]'`

### Issue: Single Object Instead of Array
**WRONG**: `'{"id": null, "answerText": "Test", "isCorrect": true}'`
**RIGHT**: `'[{"id": null, "answerText": "Test", "isCorrect": true}]'`

### Issue: Capitalized Boolean
**WRONG**: `'[{"id": null, "answerText": "Test", "isCorrect": True}]'`
**RIGHT**: `'[{"id": null, "answerText": "Test", "isCorrect": true}]'`

## Debugging Steps

1. **Print the JSON before sending**:
   ```dart
   final answersJson = jsonEncode([...]);
   print('Answers JSON: $answersJson'); // This should show the exact JSON being sent
   ```

2. **Check the error message**: The improved model binder now shows the received value in the error message

3. **Test with Postman/Insomnia**: Use a REST client to test the exact JSON format

4. **Verify form data structure**: Ensure the `Answers` field is sent as a string, not as nested form fields

## Model Binder Improvements

The updated model binder now:
- Handles empty arrays and null values
- Removes extra quotes automatically
- Provides detailed error messages with the received value
- Handles edge cases like `[]` and `null` strings
- **Auto-wraps JSON objects in array brackets** - Fixes the most common issue!

## Recent Fix: Array Wrapping

**Issue**: JSON was sent as objects without array brackets:
```json
{"Id":"...","AnswerText":"3","IsCorrect":false},{"Id":"...","AnswerText":"1","IsCorrect":false}
```

**Solution**: Model binder now automatically wraps in brackets:
```json
[{"Id":"...","AnswerText":"3","IsCorrect":false},{"Id":"...","AnswerText":"1","IsCorrect":false}]
```

This means both formats now work:
```dart
// Both of these work now:
final answersJson1 = '[{"id": null, "answerText": "Test", "isCorrect": true}]'; // With brackets
final answersJson2 = '{"id": null, "answerText": "Test", "isCorrect": true}';   // Without brackets
```

If you're still getting errors, the error message will now show exactly what JSON was received, making it easier to debug.
